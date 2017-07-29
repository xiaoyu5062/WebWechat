using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Wechat.Web
{
    public class Result
    {
        public Result()
        {
            code = 200;
            msg = "succ";
        }
        public int code { get; set; }

        public string msg { get; set; }

        public object data { get; set; }
    }



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

}