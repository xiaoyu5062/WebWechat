using System.Net;
using System.Text;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Data;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace ZFY
{
    static class FYHttpHelper
    {
        public static string GetUrltoHtml(string Url,  string type="utf-8",CookieContainer cookie=null)
        {
            try
            {
                HttpWebRequest wReq = (HttpWebRequest)System.Net.WebRequest.Create(Url);
                if (cookie != null)
                {
                    wReq.CookieContainer = cookie;
                }
                var wResp = wReq.GetResponse();
                System.IO.Stream respStream = wResp.GetResponseStream();
                using (System.IO.StreamReader reader = new System.IO.StreamReader(respStream, Encoding.GetEncoding(type)))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (System.Exception ex)
            {
                FYCommon.ErroLog("GetUrltoHtml:" + Url + "\r\n" + ex.Message);
                return "";
            }
        }
        /// <summary>
        /// POST
        /// </summary>
        /// <param name="url">Url</param>
        /// <param name="param">参数</param>
        /// <returns></returns>
        public static string PostUrltoHtml(string url, string param,CookieContainer cookie=null)
        {
            try
            {
                HttpWebRequest wReq = (HttpWebRequest)HttpWebRequest.Create(url);
                if (cookie!=null)
                {
                    wReq.CookieContainer = cookie;
                }
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
                        responseData = sReader.ReadToEnd();
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

    public static class FYCommon
    {
       

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

        #region 时间戳
        public static string GetTimeStamp(System.DateTime? time)
        {
            long ts = ConvertDateTimeInt(time.HasValue?(DateTime)time:DateTime.Now);
            return ts.ToString();
        }
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

        public static string GetMD5(this string myString, bool isLower = false)
        {
            // Use input string to calculate MD5 hash
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(myString);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            // Convert the byte array to hexadecimal string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                if (!isLower)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                else
                {
                    sb.Append(hashBytes[i].ToString("x2"));
                }

            }
            return sb.ToString();
        }
    }

    public static class DataExtensions
    {
        /// <summary>
        /// DataRow扩展方法：将DataRow类型转化为指定类型的实体
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <returns></returns>
        public static T ToModel<T>(this DataRow dr) where T : class, new()
        {
            return ToModel<T>(dr, true);
        }
        /// <summary>
        /// DataRow扩展方法：将DataRow类型转化为指定类型的实体
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="dateTimeToString">是否需要将日期转换为字符串，默认为转换,值为true</param>
        /// <returns></returns>
        /// <summary>
        public static T ToModel<T>(this DataRow dr, bool dateTimeToString) where T : class, new()
        {
            if (dr != null)
                return ToList<T>(dr.Table, dateTimeToString).First();
            return null;
        }
        /// <summary>
        /// DataTable扩展方法：将DataTable类型转化为指定类型的实体集合
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <returns></returns>
        public static List<T> ToList<T>(this DataTable dt) where T : class, new()
        {
            return ToList<T>(dt, true);
        }
        /// <summary>
        /// DataTable扩展方法：将DataTable类型转化为指定类型的实体集合
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="dateTimeToString">是否需要将日期转换为字符串，默认为转换,值为true</param>
        /// <returns></returns>
        public static List<T> ToList<T>(this DataTable dt, bool dateTimeToString) where T : class, new()
        {
            List<T> list = new List<T>();

            if (dt != null)
            {
                List<PropertyInfo> infos = new List<PropertyInfo>();
                Array.ForEach<PropertyInfo>(typeof(T).GetProperties(), p =>
                {
                    if (dt.Columns.Contains(p.Name) == true)
                    {
                        infos.Add(p);
                    }
                });
                SetList<T>(list, infos, dt, dateTimeToString);
            }
            return list;
        }
        #region 私有方法
        private static void SetList<T>(List<T> list, List<PropertyInfo> infos, DataTable dt, bool dateTimeToString) where T : class, new()
        {
            foreach (DataRow dr in dt.Rows)
            {
                T model = new T();
                infos.ForEach(p =>
                {
                    if (dr[p.Name] != DBNull.Value)
                    {
                        object tempValue = dr[p.Name];
                        if (dr[p.Name].GetType() == typeof(DateTime) && dateTimeToString == true)
                        {
                            tempValue = dr[p.Name].ToString();
                        }
                        try
                        {
                            p.SetValue(model, tempValue, null);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                    }
                });
                list.Add(model);
            }
        }
        #endregion
    }

}
