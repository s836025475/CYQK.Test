using CYQK.Test.Dto.ExamineDto;
using CYQK.Test.Mail;
using CYQK.Test.Model;
using CYQK.Test.Model.Examine;
using Microsoft.EntityFrameworkCore;
using Nancy.Json;
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
    public class CheckFailJob
    {
        public void DoCheck()
        {
            int count = 0;
            int totalCount = 0;
            string accessToken = GetAccessToken();
            var dbContext = new TestContext();
            //查询失败的日志
            var query = dbContext.ExternalLog.Where(e => e.QueryState == QueryState.失败).ToList();
            //获取外部日志存入数据库
            query.ForEach(q =>
            {
                long startTime = TimeFormat.ToUnixTimestampByMilliseconds(q.StartTime);//开始时间
                long endTime = TimeFormat.ToUnixTimestampByMilliseconds(q.EndTime);//结束时间
                string pageId = null;//页码Id 上一页则传当前最小记录id，下一页则传当前最大id，首页或者最后页传null
                string pageType = "first";//页码类型 prev=上一页，next=下一页，first=第一页，last=最后一页
                var pushLogs = GetExternalLog(accessToken, startTime, endTime, pageId, pageType);
                List<string> insList = new List<string>();
                pushLogs.ForEach(p =>
                {
                    string ins = GetInstance(p.FormInstId, p.FormCodeId, accessToken);
                    insList.Add(ins);
                });
                insList = insList.Where(i => i.Contains("true")).ToList();
                totalCount = insList.Count();
                //获取实例
                try
                {
                    insList.ForEach(p =>
                    {
                        JObject jObject = JObject.Parse(p);
                        if (jObject["success"].ToString() == "True")
                        {
                            count += 1;
                            using (var db = new TestContext())
                            {
                                //解析数据存入数据库
                                //获取CGSqlist
                                ParseEntity PE = new ParseEntity();
                                CGSqlist cg = PE.GetCGSQlist(jObject);
                                cg.FirstInput = false;
                                //审批流
                                List<Reqlist> rlList = PE.GetReqlist(JObject.Parse(GetFlowRecord(cg.FormInstId, cg.FormCodeId)), cg.Fbillid);
                                rlList.ForEach(r =>
                                {
                                    var list = db.Reqlist.AsNoTracking().Where(c => c.CreateTime == r.CreateTime)
                                                                        .Where(c => c.Fbillno == r.Fbillno).ToList();
                                    if (list.Count() == 0)
                                        db.Reqlist.Add(r);
                                });
                                var query = db.CGSqlist.AsNoTracking()
                                                    .Where(c => c.SerialNumber.Equals(cg.SerialNumber))
                                                    .ToList();
                                if (query.Count == 0)
                                {

                                    //获取CgsqListentry
                                    List<CgsqListentry> cleList = PE.GetCgsqListentry(jObject, cg.Fbillid);
                                    db.CGSqlist.Add(cg);
                                    db.CgsqListentry.AddRange(cleList);
                                }
                                db.SaveChanges();
                            }
                        }
                    });
                    q.CatchNum = count;
                    q.TakeStatus = TakeStatus.完全同步;
                    q.QueryState = QueryState.成功;
                    q.TakeTime = DateTime.Now;
                    dbContext.Update(q);
                    dbContext.SaveChanges();
                }
                catch (Exception e)
                {
                    
                    string content = startTime + "——" + endTime + "云之家外部日志接口同步失败";
                    //发送邮件
                    SendMail mailService = new SendMail();
                    bool fmailback = mailService.Send("836025475@qq.com", "s836025475@163.com", "云之家外部日志接口问题", content, "s836025475@163.com", "naduohua", "smtp.163.com", "", "");
                }
            });
        }
        private string GetFlowRecord(string formInstId, string formCodeId)
        {
            string url = "https://yunzhijia.com/gateway/workflow/form/thirdpart/getFlowRecord?accessToken=" + CallExternal.GetAccessToken();
            JObject param = new JObject();
            param.Add("formInstId", formInstId);
            param.Add("formCodeId", formCodeId);
            string response = CallExternal.PostUrl(url, param.ToString(), "application/json");
            return response;
        }
        private List<PushLogs> GetExternalLog(
            string accessToken,
            long startTime,
            long endTime,
            string pageId,
            string pageType)
        {
            string url = "https://yunzhijia.com/gateway/workflow/form/thirdpart/getPushLog?accessToken=" + accessToken;
            //页码信息
            JObject pageable = new JObject();
            pageable.Add("id", pageId);
            pageable.Add("pageSize", 100);
            pageable.Add("type", pageType);
            //请求参数
            JObject postParam = new JObject();
            postParam.Add("pageable", pageable);
            postParam.Add("devType", "user");
            postParam.Add("startTime", startTime);
            postParam.Add("endTime", endTime);
            postParam.Add("pushType", "failed");
            string jsonRequest = CallExternal.PostUrl(url, postParam.ToString(), "application/json");

            JObject json = JObject.Parse(jsonRequest.ToString());
            List<PushLogs> pushlogs = GetPushLogs(json["data"]["pushLogs"].ToString());
            if (pushlogs.Count() == 100)
            {
                pageId = pushlogs.Last().Id;
                pageType = "next";
                var logs = GetExternalLog(accessToken, startTime, endTime, pageId, pageType);
                pushlogs.AddRange(logs);
            }
            return pushlogs;
        }
        private string GetInstance(string formInstId, string formCodeId, string accessToken)
        {
            string url = "https://yunzhijia.com/gateway/workflow/form/thirdpart/viewFormInst?accessToken=" + accessToken;
            JObject param = new JObject();
            param.Add("formInstId", formInstId);
            param.Add("formCodeId", formCodeId);
            string response = CallExternal.PostUrl(url, param.ToString(), "application/json");
            return response;
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
        private string PostUrl(string url, string postData, string contentType)
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
