using System.Net;
using System.Text;
using System;
using System.IO;
//using System.Net.Http;
using System.Configuration;
namespace ZFY
{

    static class FYHttpHelper
    {
        public static string GetUrltoHtml(string Url,  string type="utf-8",CookieContainer cookie=null)
        {
            try
            {
                HttpWebRequest wReq = (HttpWebRequest)System.Net.WebRequest.Create(Url);
                //    Uri uUri=new Uri("http://61.55.135.192:82");
                //    if(ProxyUtil.ValidateProxy("61.55.135.19",82)){
                //         wReq.Proxy= new UseSpecifiedUriWebProxy(uUri);
                //           FYCommon.Log(Url+"[使用代理]"+wReq.Proxy.ToString());
                //    }
                //    else
                //    {
                //        FYCommon.Log(Url+"[未使用代理]");
                //    }

                if (cookie != null)
                {
                    wReq.CookieContainer = cookie;
                }
                var wResp = wReq.GetResponse();
                //System.Net.WebResponse wResp = wReq.GetResponse();
              
                System.IO.Stream respStream = wResp.GetResponseStream();
                using (System.IO.StreamReader reader = new System.IO.StreamReader(respStream, Encoding.GetEncoding(type)))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (System.Exception ex)
            {
                FYCommon.ErroLog("GetUrltoHtml:" + Url + "\r\n" + ex.Message);
                // System.Console.WriteLine(ex);
                return "";
            }
        }
        /// <summary>
        /// POST
        /// </summary>
        /// <param name="url">Url</param>
        /// <param name="param">参数</param>
        /// <returns></returns>
        public static string PostUrltoHtml(string url, string param)
        {
            try
            {
                HttpWebRequest wReq = (HttpWebRequest)HttpWebRequest.Create(url);
                Encoding encoding = Encoding.UTF8;
                wReq.Method = "POST";
                byte[] bs = Encoding.ASCII.GetBytes(param);
                string responseData = string.Empty;
                wReq.ContentType = "application/x-www-form-urlencoded";
                //using (Stream rs = wReq.GetRequestStreamAsync().Result)
                using (Stream rs = wReq.GetResponse().GetResponseStream())
                {
                    rs.Write(bs, 0, bs.Length);
                }
                using (HttpWebResponse rResponse = (HttpWebResponse)wReq.GetResponse())
                {
                    using (StreamReader sReader = new StreamReader(rResponse.GetResponseStream(), encoding))
                    {
                        responseData = sReader.ReadToEnd().ToString();
                    }
                    return responseData;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }

    static class FYCommon
    {
        #region 文件读写
        public static void WriteFile(string dir,string fileName, string text)
        {
            try
            {
                ///Applications/XAMPP/xamppfiles/htdocs/news
                //var path = string.Format("{0}/{1}", AppContext.BaseDirectory, DateTime.Now.ToString("yyyy/MM/dd"));
                //  var path = "/Applications/XAMPP/xamppfiles/htdocs/news";
                var path = ConfigurationSettings.AppSettings["newsPath"] + "/" + dir;

                var filePath = path + "/" + fileName + ".html";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    using (StreamWriter sw = new StreamWriter(fs, Encoding.UTF8))
                    {
                        sw.Write(text);
                    }
                }
            }catch(Exception e)
            {
                Console.WriteLine("dir:"+dir+"\nfileName:"+fileName+"\n");
                throw e;
            }
        }

        /// <summary>
        /// 获取模板html
        /// </summary>
        /// <param name="type">0新闻 1段子</param>
        /// <returns></returns>
        public static string GetTempHtml(int type)
        {
            // var path = "/Applications/XAMPP/xamppfiles/htdocs/news";
            var path = ConfigurationSettings.AppSettings["tmpPath"];
            var tempName = string.Empty;
            if (type == 0)
            {
                tempName = "new.html";
            }
            else if (type == 1)
            {
                tempName = "Satin.html";
            }
            var filePath = path + "/" + tempName;
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                using (StreamReader sr = new StreamReader(fs, Encoding.UTF8))
                {
                    return sr.ReadToEnd();
                }
            }

        }
        #endregion

        #region 日志

        public static void Log(string msg)
        {
            System.Console.WriteLine("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "]" + msg);
        }

        public static void Log(string site, string category, string msg)
        {
            Log(string.Format("[{0}][{1}]{2}", site, category, msg));
        }

        public static void ErroLog(string msg)
        {
            Console.ForegroundColor = System.ConsoleColor.Red;
            Log(msg);
            Console.ForegroundColor = System.ConsoleColor.White;
        }
        #endregion

        #region 时间戳转换
        /// <summary>
        /// 将Unix时间戳转换为DateTime类型时间
        /// </summary>
        /// <param name="d">double 型数字</param>
        /// <returns>DateTime</returns>
        public static System.DateTime ConvertIntDateTime(double d)
        {

            System.DateTime time = System.DateTime.MinValue;
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            time = startTime.AddMilliseconds(d);
            return time;
        }

        /// <summary>
        /// 将c# DateTime时间格式转换为Unix时间戳格式
        /// </summary>
        /// <param name="time">时间</param>
        /// <returns>long</returns>
        public static long ConvertDateTimeInt(System.DateTime time)
        {
            //double intResult = 0;
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
            //intResult = (time- startTime).TotalMilliseconds;
            long t = (time.Ticks - startTime.Ticks) / 10000;            //除10000调整为13位
            return t;
        }

        #endregion
    }

}
