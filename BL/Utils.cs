using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;

namespace TaxesDemo.BL
{
    public class Utils
    {
        public static string HttpGet(string url)
        {

            try
            {
                Tracer.WriteLine(string.Format("GET {0}", url));

                WebRequest request = WebRequest.Create(url);

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (var reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(response.CharacterSet)))
                    {
                        string s = reader.ReadToEnd();
                        Tracer.WriteLine(string.Format("Response: {0}", s));
                        return s;
                    }
                }
            }
            catch (WebException ex)
            {
                string msg = string.Format("GET {0}", url);
                throw new WebException(msg, ex);
            }
        }

        private static string HttpPost(string url, string postData, string contentType, Dictionary<string, string> headers, out string debug)
        {
            string requstString = "";
            debug = null;

            requstString = postData;

            // Post Information
            WebRequest request = WebRequest.Create(url);

            if (headers != null)
            {
                foreach (KeyValuePair<string, string> k in headers)
                {
                    request.Headers.Add($"{k.Key}:{k.Value}");
                }
            }

            request.Method        = "POST";
            byte[] byteArray      = Encoding.UTF8.GetBytes(requstString);
            request.ContentType   = contentType;
            request.ContentLength = byteArray.Length;

            using (Stream writeStream = request.GetRequestStream())
            {
                writeStream.Write(byteArray, 0, byteArray.Length);
            }

            try
            {
                Tracer.WriteLine(string.Format("POST {0} {1}", url, System.Text.Encoding.UTF8.GetString(byteArray)));
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    string hdrs = "";
                    foreach (var h in request.Headers)
                    {
                        hdrs += $"{h}: { request.Headers.Get(h.ToString())}\r\n";
                    }
                    debug = $"\r\nPOST {url}\r\n{hdrs}\r\n{System.Text.Encoding.UTF8.GetString(byteArray)}\r\n";

                    string charSet = response.CharacterSet;
                    if (string.IsNullOrEmpty(charSet)) { charSet = "UTF-8"; }
                    using (var reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(charSet)))
                    {
                        string s = reader.ReadToEnd();
                        Tracer.WriteLine($"{debug}\r\b{s}");
                        return s;
                    }
                }
            }
            catch (WebException ex)
            {
                string hdrs = "";
                foreach (var h in request.Headers)
                {
                    hdrs += $"{h}: { request.Headers.Get(h.ToString())}\r\n";
                }

                string rsp = "";
                using (var reader = new StreamReader(ex.Response.GetResponseStream(), Encoding.GetEncoding("UTF-8")))
                {
                    rsp = reader.ReadToEnd();
                }
                
                debug = $"\r\nPOST {url}\r\n{hdrs}\r\n{System.Text.Encoding.UTF8.GetString(byteArray)}\r\n==>\r\n{rsp}";
                Tracer.WriteLine(debug);
                throw new WebException(debug, ex);
            }
        }

        public static string HttpPost(string url, Dictionary<string, string> postData, out string debug, Dictionary<string, string> headers = null)
        {
            // Create Vars
            StringBuilder sb = new StringBuilder(1024);

            foreach (KeyValuePair<string, string> kv in postData)
            {
                sb.AppendFormat("{0}={1}&", kv.Key, System.Web.HttpUtility.UrlEncode(kv.Value, Encoding.UTF8));
            }
            sb.Remove(sb.Length - 1, 1); // Remove the last &

            string requstString = sb.ToString();
            return HttpPost(url, requstString, "application/x-www-form-urlencoded", headers, out debug);
        }

        public static string HttpPostJson(string url, object postData, out string debug, Dictionary<string, string> headers = null)
        {
            string requstString = new JavaScriptSerializer().Serialize(postData);

            return HttpPost(url, requstString, "application/json", headers, out debug);
        }


        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dateTime;
        }
    }
}