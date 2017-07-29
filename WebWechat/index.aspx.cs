using System;
using System.IO;
using System.Net;
using System.Xml;
using Newtonsoft.Json;
using System.Diagnostics;
using Newtonsoft.Json.Linq;

namespace WebWechat
{
    public partial class index : System.Web.UI.Page
    {
        #region Fileds
        long time = 1498943586000;//r参数和当前时间戳的差值
        CookieContainer cookies;
        string skey = string.Empty;
        string sid = string.Empty;
        string uin = string.Empty;
        string deviceID = string.Empty;
        string pass_ticket = string.Empty;
        string payloadStr = string.Empty;
        BaseRequest VAL_baseRequest = null;
        JObject VAL_SyncKey = null;
        JToken VAL_Self = null;
       
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Response.HeaderEncoding = System.Text.Encoding.UTF8;
                Response.Charset = "UTF-8";
                getQRCode();
            }
        }
       
        void getQRCode()
        {
            var url = "https://login.wx.qq.com/jslogin?appid=wx782c26e4c19acffb&redirect_uri=https%3A%2F%2Fwx.qq.com%2Fcgi-bin%2Fmmwebwx-bin%2Fwebwxnewloginpage&fun=new&lang=zh_CN&_=" + ZFY.FYCommon.GetTimeStamp(DateTime.Now);
            var html = ZFY.FYHttpHelper.GetUrltoHtml(url);
            Response.Write(html);
            Response.Flush();
            if (html.Contains("200"))
            {
                //window.QRLogin.code = 200; window.QRLogin.uuid = "AcuVEl52KQ==";
                //获取成功
                var uuid = html.Split(';')[1].Split('\"')[1].Replace("\"", "");
                Response.Write("<img src='https://login.weixin.qq.com/qrcode/" + uuid + " ' alt=\'二维码\'></img></br>");
                Response.Flush();
                WaitScan(uuid);
            }
        }

        void WaitScan(string uuid)
        {
            string ts = ZFY.FYCommon.GetTimeStamp(DateTime.Now);
            long r = long.Parse(ts) - time;
            while (true)
            {
                string url = "https://login.wx.qq.com/cgi-bin/mmwebwx-bin/login?loginicon=true&uuid=" + uuid + "&tip=1&r=" + r + "&_=" + ts;
                var html = ZFY.FYHttpHelper.GetUrltoHtml(url);
                Response.Write(html);
                Response.Flush();
                if (html.Contains("201"))
                {
                    Response.Write("</br>已扫码，等待确认。");
                    Response.Flush();
                    System.Threading.Thread.Sleep(2000);
                }
                else if (html.Contains("200"))
                {
                    string redirect = html.Split(';')[1].Split('\"')[1].Replace("\"", "");
                    Response.Write("</br>已确认登录。");
                    Response.Write("</br>redirect_uri=" + redirect);
                    Response.Flush();
                    Login(redirect);
                    break;
                }
                r += 25045;
                ts += 1;
            }
        }

        void Login(string redirect)
        {
            Response.Write("</br>----------------------");
            Response.Write("</br>开始登录...</br>");
            Response.Flush();
            cookies = new CookieContainer();
            var html = ZFY.FYHttpHelper.GetUrltoHtml(redirect + "&fun=new&version=v2", cookie: cookies);
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.LoadXml(html);
            Response.Write(html);
            Response.Flush();
            InitWX(xmldoc);
        }
      
        void InitWX(XmlDocument xml)
        {
            string ts = ZFY.FYCommon.GetTimeStamp(DateTime.Now);
            long r = long.Parse(ts) - time;
            /*
             * 
             *  /*
             <error>
               0 <ret>0</ret>
               1 <message></message>
               2 <skey>@crypt_fa8c7d22_0a321a36eb4fa6e5be6b973a378cbbe3</skey>
               3 <wxsid>lmy2pOXnB89DpM9Q</wxsid>
               4 <wxuin>752194140</wxuin>
               5 <pass_ticket>JLyFjOZFx1eX2FGMtPXx7JsaBLj%2BchThEUpraGCbM3tJN%2Ff4Jud7%2FkAT8RMI7NPt</pass_ticket>
                 <isgrayscale>1</isgrayscale>
             </error>
               

            https://wx.qq.com/cgi-bin/mmwebwx-bin/webwxinit?r=-2094998670&pass_ticket=JLyFjOZFx1eX2FGMtPXx7JsaBLj%252BchThEUpraGCbM3tJN%252Ff4Jud7%252FkAT8RMI7NPt

            Payload{"BaseRequest":{"Uin":"752194140","Sid":"lmy2pOXnB89DpM9Q","Skey":"@crypt_fa8c7d22_0a321a36eb4fa6e5be6b973a378cbbe3","DeviceID":"e000373749172002"}}
             */
            skey = xml.FirstChild.ChildNodes[2].InnerText;
            sid = xml.FirstChild.ChildNodes[3].InnerText;
            uin = xml.FirstChild.ChildNodes[4].InnerText;
            pass_ticket = xml.FirstChild.ChildNodes[5].InnerText;
            var url = string.Format("https://wx.qq.com/cgi-bin/mmwebwx-bin/webwxinit?r=-{0}&pass_ticket={1}", r, pass_ticket);

            //HttpWebRequest wReq = (HttpWebRequest)System.Net.WebRequest.Create(url);
            //wReq.Accept = "application/json, text/plain, */*";
            //wReq.ContentType = "application/json;charset=UTF-8";
            //wReq.CookieContainer = cookies;
            //wReq.Method = "POST";
            InitBaseRequest payload = new InitBaseRequest();
            payload.BaseRequest = new BaseRequest()
            {
                Sid = sid,
                Skey = skey,
                Uin = uin
            };
            VAL_baseRequest = payload.BaseRequest;
            deviceID = payload.BaseRequest.DeviceID;
            payloadStr = JsonConvert.SerializeObject(payload);
            //byte[] data = System.Text.Encoding.UTF8.GetBytes(payloadStr);
            //using (Stream stream = wReq.GetRequestStream())
            //{
            //    stream.Write(data, 0, data.Length);
            //}
            //var wRes = wReq.GetResponse();
            //StreamReader sr = new StreamReader(wRes.GetResponseStream());
            //var html = sr.ReadToEnd();
            var html = ZFY.FYHttpHelper.PostUrltoHtml(url, payloadStr, cookies);
            JObject json = JObject.Parse(html);
            /*BaseResponse": { "Ret": 0, "ErrMsg": "" } */
            //if (html.Contains("\"BaseResponse\":{\"Ret\":0,\"ErrMsg\":\"\"}"))
            //{
            //    Response.Write("</br>初始化成功...可以胡作非为了。");
            //}
            //else
            //{
            //    Response.Write("</br>初始化失败喽，重新走一遍试试");
            //}
            // Response.Flush();
            VAL_Self = json["User"];
            Response.Write("</br>初始化完成。</br>用户信息:</br>UserName:" + VAL_Self["UserName"] + "</br>NickName:" + VAL_Self["NickName"]);
            SyncCheck((JObject)json["SyncKey"]);
            GetFriends();
            // SendMsg();
        }

        void GetFriends()
        {
            string ts = ZFY.FYCommon.GetTimeStamp(DateTime.Now);
            string url = string.Format("https://wx.qq.com/cgi-bin/mmwebwx-bin/webwxgetcontact?lang=zh_CN&pass_ticket={0}&r={1}&seq=0&skey={2}", pass_ticket, ts, skey);

            HttpWebRequest wReq = (HttpWebRequest)System.Net.WebRequest.Create(url);
            wReq.CookieContainer = cookies;
            wReq.Method = "GET";
            wReq.ContentType = "application/json;charset=UTF-8";
            var wRes = wReq.GetResponse();
            StreamReader sr = new StreamReader(wRes.GetResponseStream());
            var html = sr.ReadToEnd();
            Response.Write("</br>-------------");
            Response.Write("</br>好友列表：</br>");
            JObject result = JObject.Parse(html);
            var Ret = result["BaseResponse"]["Ret"];
            Response.Write("</br>Ret:" + Ret);
            var MemberCount = result["MemberCount"];
            Response.Write("</br>好友数量：" + MemberCount);
            var list = result["MemberList"];
            foreach (var item in list.Children())
            {
                var UserName = item["UserName"];
                var NickName = item["NickName"];
                var RemarkName = item["RemarkName"];
                //"ContactFlag": 1 - 好友， 2 - 群组， 3 - 公众号
                int ContactFlag = int.Parse(item["ContactFlag"].ToString());
                string friendType = ContactFlag == 1 ? "好友" : ContactFlag == 2 ? "群组" : ContactFlag == 3 ? "公众号" : "未知" + ContactFlag;
                Response.Write("</br>--------------</br>UserName:" + UserName + "</br>NickName:" + NickName + "(" + friendType + ")</br>RemarkName:" + RemarkName);
                Response.Flush();
                if (NickName.ToString() == "vic")
                {
                    SendMsg("我是自动发送的", UserName.ToString());
                }
            }
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

        void SendMsg(string content, string toUserName)
        {
            Response.Write("</br>-------------</br>发送[" + content + "]给[" + toUserName + "]");
            Response.Flush();
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

            JObject payload = new JObject();
            payload.Add("BaseRequest", JToken.FromObject(VAL_baseRequest));
            MsgRequest msg = new MsgRequest()
            {
                Content = content,
                FromUserName = VAL_Self["UserName"].ToString(),
                ToUserName = toUserName,
                //"@08e85ac96adb82a67a80c72e6403a049e15f759352c51fdf569e4ce2bf62019e"
                Type = 1
            };
            payload.Add("Msg", JToken.FromObject(msg));
            payload.Add("Scene", 0);
            string url = string.Format("https://wx.qq.com/cgi-bin/mmwebwx-bin/webwxsendmsg?lang=zh_CN&pass_ticket={0}", pass_ticket);
            HttpWebRequest wReq = (HttpWebRequest)System.Net.WebRequest.Create(url);
            wReq.Accept = "application/json, text/plain, */*";
            wReq.ContentType = "application/json;charset=UTF-8";
            wReq.CookieContainer = cookies;
            wReq.Method = "POST";

            Response.Write("</br>消息参数:" + payload.ToString().Replace("\r\n", ""));
            byte[] data = System.Text.Encoding.UTF8.GetBytes(payload.ToString().Replace("\r\n", ""));
            using (Stream stream = wReq.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }
            var wRes = wReq.GetResponse();
            StreamReader sr = new StreamReader(wRes.GetResponseStream());
            var html = sr.ReadToEnd();
            Response.Write(html);
        }

        void SyncCheck(JObject syncKey)
        {
            VAL_SyncKey = syncKey;
            string synckey = string.Empty;
            foreach (var item in syncKey["List"].Children())
            {
                synckey += item["Key"] + "_" + item["Val"] + "|";
            }
            synckey = synckey.Substring(0, synckey.Length - 1);
            System.Threading.Thread thread = new System.Threading.Thread(() =>
            {
                //https://webpush.wx.qq.com/cgi-bin/mmwebwx-bin/synccheck?r=1501042097300&skey=%40crypt_fa8c7d22_a72311e1a87a045893d475b18968c70c&sid=T1VVW9p0%2BFwu0%2B%2BB&uin=752194140&deviceid=e133973973929944&synckey=1_675439007%7C2_675439150%7C3_675439136%7C11_675439098%7C13_675340098%7C201_1501042054%7C1000_1501030261%7C1001_1501030292%7C1002_1500889686&_=1501041063253
                System.Threading.Thread.Sleep(25000);
                string ts = ZFY.FYCommon.GetTimeStamp(DateTime.Now);
                long r = long.Parse(ts) - time;
                while (true)
                {
                    string url = string.Format("https://webpush.wx.qq.com/cgi-bin/mmwebwx-bin/synccheck?r={0}&skey={1}&sid={2}&uin={3}&deviceid={4}&synckey={5}", ts, skey, sid, uin, deviceID, synckey);
                    //  var html = ZFY.FYHttpHelper.GetUrltoHtml(url,cookie:cookies);
                    HttpWebRequest wReq = (HttpWebRequest)System.Net.WebRequest.Create(url);
                    wReq.CookieContainer = cookies;
                    wReq.Method = "GET";
                    //byte[] data = System.Text.Encoding.UTF8.GetBytes(payloadStr);
                    //using (Stream stream = wReq.GetRequestStream())
                    //{
                    //    stream.Write(data, 0, data.Length);
                    //}
                    var wRes = wReq.GetResponse();
                    StreamReader sr = new StreamReader(wRes.GetResponseStream());
                    var html = sr.ReadToEnd();
                    //Response.Write("</br>heart.." + html);
                    //Response.Flush();
                    Debug.WriteLine(html);
                    System.Threading.Thread.Sleep(25000);
                }

            });
            thread.IsBackground = true;
            thread.Start();
        }
    }
}