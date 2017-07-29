﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using Newtonsoft.Json;
using System.Net;
using System.Xml;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace Wechat.Web
{
    /// <summary>
    /// api 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class api : System.Web.Services.WebService
    {
        const long TIME = 1498943586000;//r参数和当前时间戳的差值


        /// <summary>
        /// 获取登录二维码
        /// </summary>
        [WebMethod]
        public void GetQRCode()
        {
            Result result = new Result();
            var url = "https://login.wx.qq.com/jslogin?appid=wx782c26e4c19acffb&redirect_uri=https%3A%2F%2Fwx.qq.com%2Fcgi-bin%2Fmmwebwx-bin%2Fwebwxnewloginpage&fun=new&lang=zh_CN&_=" + ZFY.FYCommon.GetTimeStamp(DateTime.Now);
            var html = ZFY.FYHttpHelper.GetUrltoHtml(url);
            if (html.Contains("200"))
            {
                //window.QRLogin.code = 200; window.QRLogin.uuid = "AcuVEl52KQ==";
                //获取成功 300秒有效期
                var uuid = html.Split(';')[1].Split('\"')[1].Replace("\"", "");
                var img_url = "https://login.weixin.qq.com/qrcode/" + uuid;
                JObject obj=new JObject();
                obj.Add("uuid",uuid);
                obj.Add("qrcode",img_url);
                result.data = obj;
            }
            else
            {
                result.code = 500;
                result.msg = "请重新获取二维码";
            }
            ResponseResult(result);
        }

        [WebMethod]
        public void WaitScan(string uuid)
        {
            Result result = new Result();
            string ts = ZFY.FYCommon.GetTimeStamp(DateTime.Now);
            long r = long.Parse(ts) - TIME;
            //while (true)
            //{
                string url = "https://login.wx.qq.com/cgi-bin/mmwebwx-bin/login?loginicon=true&uuid=" + uuid + "&tip=1&r=" + r + "&_=" + ts;
                var html = ZFY.FYHttpHelper.GetUrltoHtml(url);
                if (html.Contains("201"))
                {
                    //已扫码，等待确认
                    result.code = 201;
                    result.data = "已扫码，等待确认";
                   System.Threading.Thread.Sleep(2000);
                    //break;
                }
                else if (html.Contains("200"))
                {
                    result.code = 200;

                    string redirect = html.Split(';')[1].Split('\"')[1].Replace("\"", "");
                    //Response.Write("</br>已确认登录。");
                    Login(redirect);
                  //  break;
                }
                else if(html.Contains("408")){
                    result.code = 408;
                }
                //r += 25045;
                //ts += 1;
           // }
            ResponseResult(result);
        }

        CookieContainer cookies;
        void Login(string redirect)
        {

            cookies = new CookieContainer();
            var html = ZFY.FYHttpHelper.GetUrltoHtml(redirect + "&fun=new&version=v2", cookie: cookies);
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.LoadXml(html);
            InitWX(xmldoc);
        }
        void InitWX(XmlDocument xml)
        {
            string ts = ZFY.FYCommon.GetTimeStamp(DateTime.Now);
            long r = long.Parse(ts) - TIME;

            var skey = xml.FirstChild.ChildNodes[2].InnerText;
            var sid = xml.FirstChild.ChildNodes[3].InnerText;
            var uin = xml.FirstChild.ChildNodes[4].InnerText;
            var pass_ticket = xml.FirstChild.ChildNodes[5].InnerText;
            var url = string.Format("https://wx.qq.com/cgi-bin/mmwebwx-bin/webwxinit?r=-{0}&pass_ticket={1}", r, pass_ticket);

            InitBaseRequest payload = new InitBaseRequest();
            payload.BaseRequest = new BaseRequest()
            {
                Sid = sid,
                Skey = skey,
                Uin = uin
            };
            BaseRequest baseRequest = payload.BaseRequest;
            var deviceID = payload.BaseRequest.DeviceID;
            var payloadStr = JsonConvert.SerializeObject(payload);

            var html = ZFY.FYHttpHelper.PostUrltoHtml(url, payloadStr);
            JObject json = JObject.Parse(html);
            if (json["BaseResponse"]["Ret"].ToString() == "0")
            {
                //成功
                var cur_user = json["User"];//当前扫码用户
                SyncCheck(json["SyncKey"], baseRequest);
            }
            else
            {
                //失败  重新扫
            }

            //  Response.Write("</br>初始化完成。</br>用户信息:</br>UserName:" + cur_user["UserName"] + "</br>NickName:" + cur_user["NickName"]);

            //GetFriends();
        }

        void SyncCheck(JToken syncKey, BaseRequest baseRequest)
        {
            string synckey = string.Empty;
            foreach (var item in syncKey["List"].Children())
            {
                synckey += item["Key"] + "_" + item["Val"] + "|";
            }
            synckey = synckey.Substring(0, synckey.Length - 1);
            System.Threading.Thread thread = new System.Threading.Thread(() =>
            {
                System.Threading.Thread.Sleep(25000);
                string ts = ZFY.FYCommon.GetTimeStamp(DateTime.Now);
                long r = long.Parse(ts) - TIME;
                while (true)
                {
                    string url = string.Format("https://webpush.wx.qq.com/cgi-bin/mmwebwx-bin/synccheck?r={0}&skey={1}&sid={2}&uin={3}&deviceid={4}&synckey={5}", ts, baseRequest.Skey, baseRequest.Sid, baseRequest.Uin, baseRequest.DeviceID, synckey);
                    HttpWebRequest wReq = (HttpWebRequest)System.Net.WebRequest.Create(url);
                    wReq.CookieContainer = cookies;
                    wReq.Method = "GET";
                    var wRes = wReq.GetResponse();
                    StreamReader sr = new StreamReader(wRes.GetResponseStream());
                    var html = sr.ReadToEnd();
                    Debug.WriteLine("SyncCheck:" + html);
                    System.Threading.Thread.Sleep(25000);
                }

            });
            thread.IsBackground = true;
            thread.Start();
        }


        void ResponseResult(Result result)
        {
            Context.Response.AddHeader("content-type", "application/json");
            Context.Response.Write(JsonConvert.SerializeObject(result));
        }
    }
}
