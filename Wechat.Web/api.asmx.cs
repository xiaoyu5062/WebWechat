using System;
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
     [System.Web.Script.Services.ScriptService]
    public class api : System.Web.Services.WebService
    {
        public class BaseInfo
        {
            public string skey { get; set; }
            public string sid { get; set; }
            public string uin { get; set; }
            public string pass_ticket { get; set; }
        }
        public class Param
        {
            public int u { get; set; }
            public int p { get; set; }
            public int m { get; set; }
        }

        const long TIME = 1498943586000;//r参数和当前时间戳的差值
        public static Dictionary<string, CookieContainer> UserCookies = new Dictionary<string, CookieContainer>();
        public static Dictionary<string, JToken> ScanUser = new Dictionary<string, JToken>();//用户状态
        public static Dictionary<string, BaseInfo> ScanBaseInfo = new Dictionary<string, BaseInfo>();//用户发消息信息
        public static Dictionary<string, Param> ScanParms = new Dictionary<string, Param>();//扫码参数
        public static Dictionary<string, BaseRequest> SendMsgRequest = new Dictionary<string, BaseRequest>();//发送消息凭证
        [WebMethod]
        public string test()
        {
            var now = ZFY.FYCommon.GetTimeStamp(null);
            var r = long.Parse(now) - TIME;
            var fan = ~long.Parse(now);
            return "a=" + r + "   b=" + fan;
        }
        /// <summary>
        /// 获取登录二维码
        /// </summary>
        [WebMethod]
        public void GetQRCode()
        {
            try
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
                    JObject obj = new JObject();
                    obj.Add("uuid", uuid);
                    obj.Add("qrcode", img_url);
                    result.data = obj;
                }
                else
                {
                    result.code = 500;
                    result.msg = "请重新获取二维码";
                }
                ResponseResult(result);
            }
            catch (Exception e){
                ResponseResult( new Result() { code = 500, msg = e.Message, data = e.StackTrace });
            }
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
                ResponseResult(result);
                //break;
            }
            else if (html.Contains("200"))
            {
                result.code = 200;

                string redirect = html.Split(';')[1].Split('\"')[1].Replace("\"", "");
                //Response.Write("</br>已确认登录。");
                if (ScanParms.ContainsKey(uuid))
                {
                    ScanParms.Remove(uuid);
                }
                Param p = new Web.api.Param()
                {
                    u = int.Parse(Context.Request["u"]),
                    m = int.Parse(Context.Request["m"]),
                    p = int.Parse(Context.Request["p"])
                };
                ScanParms.Add(uuid, p);
                Login(redirect, uuid);
                //  break;
            }
            else if (html.Contains("408"))
            {
                result.code = 408; ResponseResult(result);
            }

            //r += 25045;
            //ts += 1;
            // }

        }

        //CookieContainer cookies;

        void Login(string redirect, string uuid)
        {
            CookieContainer cookie = new CookieContainer();
            if (UserCookies.ContainsKey(uuid))
            {
                UserCookies.Remove(uuid);
            }
            UserCookies.Add(uuid, cookie);
            var html = ZFY.FYHttpHelper.GetUrltoHtml(redirect + "&fun=new&version=v2", cookie: cookie);
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.LoadXml(html);
            InitWX(xmldoc, uuid);
        }
        void InitWX(XmlDocument xml, string uuid)
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

            if (SendMsgRequest.ContainsKey(uuid))

            {
                SendMsgRequest.Remove(uuid);
            }
            SendMsgRequest.Add(uuid, payload.BaseRequest);
            var payloadStr = JsonConvert.SerializeObject(payload);

            var html = ZFY.FYHttpHelper.PostUrltoHtml(url, payloadStr);
            JObject json = JObject.Parse(html);
            if (json["BaseResponse"]["Ret"].ToString() == "0")
            {
                //成功
                var cur_user = json["User"];//当前扫码用户
                                            // SyncCheck(json["SyncKey"], baseRequest);
                                            //直接发送消息了
                if (ScanUser.ContainsKey(uuid))
                {
                    ScanUser.Remove(uuid);
                }
                ScanUser.Add(uuid, cur_user);

                BaseInfo info = new Web.api.BaseInfo()
                {
                    pass_ticket = pass_ticket,
                    sid = sid,
                    skey = skey,
                    uin = uin
                };
                if (ScanBaseInfo.ContainsKey(uuid))
                {
                    ScanBaseInfo.Remove(uuid);
                }
                ScanBaseInfo.Add(uuid, info);
                //GetFriends(uuid);
                Result rs = new Result() { code = 200 };
                ResponseResult(rs);
            }
            else
            {
                //失败  重新扫
                Result result = new Result();
                result.code = 400;//初始化失败
                result.msg = "初始化失败，重新扫码";
                ResponseResult(result);
            }

            //  Response.Write("</br>初始化完成。</br>用户信息:</br>UserName:" + cur_user["UserName"] + "</br>NickName:" + cur_user["NickName"]);

            //GetFriends();
        }

        [WebMethod]
        public void Send(string uuid)
        {
            BaseInfo cur_user = ScanBaseInfo[uuid];
            string ts = ZFY.FYCommon.GetTimeStamp(DateTime.Now);
            string url = string.Format("https://wx.qq.com/cgi-bin/mmwebwx-bin/webwxgetcontact?lang=zh_CN&pass_ticket={0}&r={1}&seq=0&skey={2}", cur_user.pass_ticket, ts, cur_user.skey);

            HttpWebRequest wReq = (HttpWebRequest)System.Net.WebRequest.Create(url);
            wReq.CookieContainer = UserCookies[uuid];
            wReq.Method = "GET";
            wReq.ContentType = "application/json;charset=UTF-8";
            var wRes = wReq.GetResponse();
            StreamReader sr = new StreamReader(wRes.GetResponseStream());
            var html = sr.ReadToEnd();
            // Response.Write("</br>-------------");
            //  Response.Write("</br>好友列表：</br>");
            JObject result = JObject.Parse(html);
            var Ret = result["BaseResponse"]["Ret"];
            //  Response.Write("</br>Ret:" + Ret);
            var MemberCount = result["MemberCount"];
            // Response.Write("</br>好友数量：" + MemberCount);
            var list = result["MemberList"];
            int ready_count = 0;
            //查询消息内容
            string sql = "select content from bk_msg where id=" + ScanParms[uuid].m;
            string msg = FXH.DbUtility.AosyMySql.ExecuteScalar(sql).ToString();
            foreach (var item in list.Children())
            {
                var UserName = item["UserName"];
                var NickName = item["NickName"];
                var RemarkName = item["RemarkName"];
                //"ContactFlag": 1 - 好友， 2 - 群组， 3 - 公众号
                int ContactFlag = int.Parse(item["ContactFlag"].ToString());
                string friendType = ContactFlag == 1 ? "好友" : ContactFlag == 2 ? "群组" : ContactFlag == 3 ? "公众号" : "未知" + ContactFlag;
                //  Response.Write("</br>--------------</br>UserName:" + UserName + "</br>NickName:" + NickName + "(" + friendType + ")</br>RemarkName:" + RemarkName);
                //  Response.Flush();

                if (UserName.ToString()=="weixin")//过滤微信官方
                {
                    continue;
                }

                //  if (NickName.ToString() == "vic"||NickName.ToString()=="Mr.Zhang")
                //  {
                SendMsg(uuid, msg, UserName.ToString());
                ready_count++;
                //  }
            }
            Result r = new Web.Result();
            r.code = ready_count > 0 ? 1 : 0;
            r.data = ready_count;
            //insert send log
            var nickname = ScanUser[uuid]["NickName"];
            var info = ScanParms[uuid];
            string sql_log = "insert into bk_msg_log (msg_id,uid,uid_parent,wx_nickname,wx_send_count) values (@msg_id,@uid,@uid_parent,@wx_nickname,@wx_send_count)";
            FXH.DbUtility.AosyMySql.ExecuteforBool(sql_log, System.Data.CommandType.Text, new MySql.Data.MySqlClient.MySqlParameter[]{
                new MySql.Data.MySqlClient.MySqlParameter("@msg_id",info.m),
                new MySql.Data.MySqlClient.MySqlParameter("@uid",info.u),
                 new MySql.Data.MySqlClient.MySqlParameter("@uid_parent",info.p),
                 new MySql.Data.MySqlClient.MySqlParameter("@wx_nickname",nickname.ToString()),
                 new MySql.Data.MySqlClient.MySqlParameter("@wx_send_count",ready_count)
            });
            Exit(uuid);
            ResponseResult(r);
            //Response.Write(html);
            //Response.Flush();
            /*
             "Uin": 0,
"UserName": 用户名称，一个"@"为好友，两个"@"为群组
"NickName": 昵称
"HeadImgUrl":头像图片链接地址
"ContactFlag": 1-好友， 2-群组， 3-公众号
"MemberCount": 成员数量，只有在群组信息中才有效,
"MemberList": 成员列表,
"RemarkName": 备注名称
"HideInputBarFlag": 0,
"Sex": 性别，0-未设置（公众号、保密），1-男，2-女
"Signature": 公众号的功能介绍 or 好友的个性签名
"VerifyFlag": 0,
"OwnerUin": 0,
"PYInitial": 用户名拼音缩写
"PYQuanPin": 用户名拼音全拼
"RemarkPYInitial":备注拼音缩写
"RemarkPYQuanPin": 备注拼音全拼
"StarFriend": 是否为星标朋友  0-否  1-是
"AppAccountFlag": 0,
"Statues": 0,
"AttrStatus": 119911,
"Province": 省
"City": 市
"Alias": 
"SnsFlag": 17,
"UniFriend": 0,
"DisplayName": "",
"ChatRoomId": 0,
"KeyWord": 
"EncryChatRoomId": ""
             */
            //@9e3bf447167bb69b9a40a7e084d1cd8ded6ab2826093dbaaf7cbbc4087cda0cd  秋秋的
        }


        void Exit(string uuid)
        {
            //https://wx.qq.com/cgi-bin/mmwebwx-bin/webwxlogout?redirect=1&type=0&skey=%40crypt_fa8c7d22_1d9c75c3c227d5e2bde98bb4f1f28a0d
            string url = string.Format("https://wx.qq.com/cgi-bin/mmwebwx-bin/webwxlogout?redirect=1&type=0&skey={0}", ScanBaseInfo[uuid].skey);
            string postdata = string.Format("sid={0}&uin={1}", ScanBaseInfo[uuid].sid, ScanBaseInfo[uuid].uin);

            var html = ZFY.FYHttpHelper.PostUrltoHtml(url, postdata, UserCookies[uuid]);
            Debug.Write(html);
            //clear
            ScanBaseInfo.Remove(uuid);
            UserCookies.Remove(uuid);
            ScanParms.Remove(uuid);
            ScanUser.Remove(uuid);
            SendMsgRequest.Remove(uuid);
        }

        //void SyncCheck(JToken syncKey, BaseRequest baseRequest)
        //{
        //    string synckey = string.Empty;
        //    foreach (var item in syncKey["List"].Children())
        //    {
        //        synckey += item["Key"] + "_" + item["Val"] + "|";
        //    }
        //    synckey = synckey.Substring(0, synckey.Length - 1);
        //    System.Threading.Thread thread = new System.Threading.Thread(() =>
        //    {
        //        System.Threading.Thread.Sleep(25000);
        //        string ts = ZFY.FYCommon.GetTimeStamp(DateTime.Now);
        //        long r = long.Parse(ts) - TIME;
        //        while (true)
        //        {
        //            string url = string.Format("https://webpush.wx.qq.com/cgi-bin/mmwebwx-bin/synccheck?r={0}&skey={1}&sid={2}&uin={3}&deviceid={4}&synckey={5}", ts, baseRequest.Skey, baseRequest.Sid, baseRequest.Uin, baseRequest.DeviceID, synckey);
        //            HttpWebRequest wReq = (HttpWebRequest)System.Net.WebRequest.Create(url);
        //            wReq.CookieContainer = cookies;
        //            wReq.Method = "GET";
        //            var wRes = wReq.GetResponse();
        //            StreamReader sr = new StreamReader(wRes.GetResponseStream());
        //            var html = sr.ReadToEnd();
        //            Debug.WriteLine("SyncCheck:" + html);
        //            System.Threading.Thread.Sleep(25000);
        //        }

        //    });
        //    thread.IsBackground = true;
        //    thread.Start();
        //}


        void SendMsg(string uuid, string content, string toUserName)
        {
            /*POST:
             * https://wx.qq.com/cgi-bin/mmwebwx-bin/webwxsendmsg?lang=zh_CN&pass_ticket=xxx            
             BaseRequest:
{
    DeviceID:”xxx”
    Sid:”xxx”
    Skey:”xxx”
    Uin:xxx
}
Msg:
{
    ClientMsgId:”14672041846800613”
    Content:”hello, myself.”
    FromUserName:”xxx”
    LocalID:”14672041846800613”
    ToUserName:”filehelper”
    Type:1
}
Scene:0

            说明：
            Type: 1 文字消息，3 图片消息（先把图片上传得到MediaId再调用webwxsendmsg发送），其他消息类型没试。
Content: 要发送的消息（发送图片消息时该字段为MediaId）
FromUserName: 自己的ID
ToUserName: 好友的ID
ClientMsgId: 时间戳左移4位随后补上4位随机数 
LocalID: 与clientMsgId相同
             */

            //string ts = GetTimeStamp(DateTime.Now);
            //long r =0-( long.Parse(ts) - time);


            //---------------------------------------------------

            JObject payload = new JObject();
            payload.Add("BaseRequest", JToken.FromObject(SendMsgRequest[uuid]));
            MsgRequest msg = new MsgRequest()
            {
                Content = content.Replace("\r\n", "\n"),
                FromUserName = ScanUser[uuid]["UserName"].ToString(),
                ToUserName = toUserName,
                //"@08e85ac96adb82a67a80c72e6403a049e15f759352c51fdf569e4ce2bf62019e"
                Type = 1
            };
            payload.Add("Msg", JToken.FromObject(msg));
            payload.Add("Scene", 0);
            string url = string.Format("https://wx.qq.com/cgi-bin/mmwebwx-bin/webwxsendmsg?lang=zh_CN&pass_ticket={0}", ScanBaseInfo[uuid].pass_ticket);
            HttpWebRequest wReq = (HttpWebRequest)System.Net.WebRequest.Create(url);
            wReq.Accept = "application/json, text/plain, */*";
            wReq.ContentType = "application/json;charset=UTF-8";
            wReq.CookieContainer = UserCookies[uuid];
            wReq.Method = "POST";

            // Response.Write("</br>消息参数:" + payload.ToString().Replace("\r\n", ""));
            byte[] data = System.Text.Encoding.UTF8.GetBytes(payload.ToString().Replace("\r\n", ""));
            using (Stream stream = wReq.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }
            var wRes = wReq.GetResponse();
            StreamReader sr = new StreamReader(wRes.GetResponseStream());
            var html = sr.ReadToEnd();
            // Response.Write(html);
        }

        void UploadImg()
        {
            //https://file.wx.qq.com/cgi-bin/mmwebwx-bin/webwxuploadmedia?f=json
            //Content-Type:multipart/form-data;
        }

        void ResponseResult(Result result)
        {
            Context.Response.AddHeader("content-type", "application/json");
            Context.Response.Write(JsonConvert.SerializeObject(result));
        }
    }
}
