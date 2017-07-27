using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebWechat
{


    public class InitBaseRequest
    {
        public BaseRequest BaseRequest { get; set; }
    }

    public class BaseRequest
    {
        public string Uin { get; set; }

        public string Sid { get; set; }

        public string Skey { get; set; }

        private string _DeviceID;
        public string DeviceID {
            get {
                if (string.IsNullOrEmpty(_DeviceID))
                {
                    return "e000373749172002";
                }
                else return _DeviceID;
            }
            set {
                _DeviceID = value;
            }
        }
    }

    public class MsgRequest
    {
        /* ClientMsgId:”14672041846800613”
    Content:”hello, myself.”
    FromUserName:”xxx”
    LocalID:”14672041846800613”
    ToUserName:”filehelper”
    Type:1

     说明：
            Type: 1 文字消息，3 图片消息（先把图片上传得到MediaId再调用webwxsendmsg发送），其他消息类型没试。
Content: 要发送的消息（发送图片消息时该字段为MediaId）
FromUserName: 自己的ID
ToUserName: 好友的ID
ClientMsgId: 时间戳左移4位随后补上4位随机数 
LocalID: 与clientMsgId相同
             */
        private string _ClientMsgId;

        public string ClientMsgId {
            get {
                System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
                long t = (DateTime.Now.Ticks - startTime.Ticks) / 10000;   //除10000调整为13位     
                                                                           //  t = t << 4;
                var r = t.ToString() + (new Random().Next(1000, 9999));
                LocalID = r;
                return r;
            }
            set { _ClientMsgId = LocalID = value; }
        }
        public string LocalID { get; set; }

        public string Content { get; set; }
        public int Type { get; set; }

        public string ToUserName { get; set; }
        public string FromUserName { get; set; }
    }



}