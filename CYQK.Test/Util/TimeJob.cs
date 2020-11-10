using CYQK.Test.Dto.ExamineDto;
using CYQK.Test.Model;
using CYQK.Test.Model.Examine;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Nancy.Json;
using Newtonsoft.Json.Linq;
using Pomelo.AspNetCore.TimedJob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CYQK.Test.Util
{
    public class TimeJob : Job
    {
        //private readonly TestContext _testContext;
        //public TimeJob(TestContext testContext)
        //{
        //    _testContext = testContext;
        //}
        [Invoke(Begin = "2020-11-6 18:30", Interval = 1000 * 3600, SkipWhileExecuting = true)]
        public void DoTask()
        {
            //获取AccessToken
            string accessToken = GetAccessToken();
            //获取外部接口日志
            string externalLog = GetExternalLog(accessToken);
            JObject json = JObject.Parse(externalLog);
            List<PushLogs> pushlogs = GetPushLogs(json["data"]["pushLogs"].ToString());
            //获取实例
            List<JObject> instance = new List<JObject>();
            pushlogs.ForEach(p =>
            {
                try
                {
                    string ins = GetInstance(p.FormInstId, p.FormCodeId, accessToken);
                    JObject jObject = JObject.Parse(ins);
                    string success = jObject["success"].ToString();
                    if (jObject["success"].ToString() == "True")
                    {
                        //解析数据存入数据库
                        //获取CGSqlist
                        ParseEntity PE = new ParseEntity();
                        CGSqlist cg = PE.GetCGSQlist(jObject);
                        cg.FirstInput = false;
                        //获取CgsqListentry
                        CgsqListentry cle = PE.GetCgsqListentry(jObject, cg.Fbillid);
                        using (var db = new TestContext())
                        {
                            db.CGSqlist.Add(cg);
                            db.CgsqListentry.Add(cle);
                            db.SaveChanges();
                        }
                    }
                }
                catch (Exception e)
                {
                }
                
            });
            
        }
        public string GetInstance(string formInstId, string formCodeId, string accessToken)
        {
            string url = "https://yunzhijia.com/gateway/workflow/form/thirdpart/viewFormInst?accessToken=" + accessToken;
            JObject param = new JObject();
            param.Add("formInstId", formInstId);
            param.Add("formCodeId", formCodeId);
            string response = PostUrl(url, param.ToString(), "application/json");
            return response;
        }
        private string GetExternalLog(string accessToken)
        {
            string url = "https://yunzhijia.com/gateway/workflow/form/thirdpart/getPushLog?accessToken=" + accessToken;
            //页码信息
            JObject pageable = new JObject();
            pageable.Add("id", null);
            pageable.Add("pageSize", 100);
            pageable.Add("type", "first");
            //请求参数
            JObject postParam = new JObject();
            postParam.Add("pageable", pageable);
            postParam.Add("devType", "user");
            postParam.Add("startTime", TimeFormat.ToUnixTimestampByMilliseconds(DateTime.Now.AddHours(-1))) ;
            postParam.Add("endTime", TimeFormat.ToUnixTimestampByMilliseconds(DateTime.Now));
            postParam.Add("pushType", "failed");
            string jsonRequest = PostUrl(url, postParam.ToString(), "application/json");
            return jsonRequest.ToString();
        }

        private string GetAccessToken()
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
        /// <summary>
        /// Post提交数据
        /// </summary>
        /// <param name="postUrl">URL</param>
        /// <param name="paramData">参数</param>
        /// <returns></returns>
        public string PostUrl(string url, string postData, string contentType)
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
        private List<PushLogs> GetPushLogs(string jsonstr)
        {
            List<PushLogs> pushLogs = new List<PushLogs>();
            JavaScriptSerializer Serializer = new JavaScriptSerializer();
            pushLogs = Serializer.Deserialize<List<PushLogs>>(jsonstr);
            return pushLogs;
        }
    }
}
