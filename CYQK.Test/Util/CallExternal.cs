using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CYQK.Test.Util
{
    public static class CallExternal
    {
        public static string PostUrl(string url, string postData, string contentType)
        {
            string result = "";
            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                req.Method = "POST";
                req.ContentType = contentType;// "application/x-www-form-urlencoded"
                req.Timeout = 800;//请求超时时间
                byte[] data = Encoding.UTF8.GetBytes(postData);
                req.ContentLength = data.Length;
                using (Stream reqStream = req.GetRequestStream())
                {
                    reqStream.Write(data, 0, data.Length);
                    reqStream.Close();
                }
                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                Stream stream = resp.GetResponseStream();
                //获取响应内容
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    result = reader.ReadToEnd();
                }
            }
            catch (Exception e) { }
            return result;
        }
        public static string GetAccessToken()
        {
            JObject param = new JObject();
            param.Add("appId", "SP15452095");
            param.Add("eid", "15452095");
            param.Add("secret", "dxjGOSdfAtTwCEqmQTDKdAzKfau7bK");
            param.Add("timestamp", TimeFormat.ToUnixTimestampByMilliseconds(DateTime.Now));
            param.Add("scope", "team");
            String url = "https://yunzhijia.com/gateway/oauth2/token/getAccessToken";
            string jsonRequest = PostUrl(url, param.ToString(), "application/json");
            JObject resGroupJson = JObject.Parse(jsonRequest);
            return resGroupJson["data"]["accessToken"].ToString();
        }
    }
}
