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
            string accessToken = CallExternal.GetAccessToken();
            //获取外部接口日志
            long startTime = TimeFormat.ToUnixTimestampByMilliseconds(DateTime.Now.AddHours(-1));//开始时间
            long endTime = TimeFormat.ToUnixTimestampByMilliseconds(DateTime.Now);//结束时间
            string pageId = null;//页码Id 上一页则传当前最小记录id，下一页则传当前最大id，首页或者最后页传null
            string pageType = "first";//页码类型 prev=上一页，next=下一页，first=第一页，last=最后一页
            var pushLogs = GetExternalLog(accessToken, startTime, endTime, pageId, pageType);
            //获取实例
            pushLogs.ForEach(p =>
            {
                try
                {
                    string ins = GetInstance(p.FormInstId, p.FormCodeId, accessToken);
                    JObject jObject = JObject.Parse(ins);
                    if (jObject["success"].ToString() == "True")
                    {
                        //解析数据存入数据库
                        //获取CGSqlist
                        ParseEntity PE = new ParseEntity();
                        CGSqlist cg = PE.GetCGSQlist(jObject);
                        cg.FirstInput = false;
                        //获取CgsqListentry
                        List<CgsqListentry> cleList = PE.GetCgsqListentry(jObject, cg.Fbillid);
                        List<Reqlist> rlList = PE.GetReqlist(JObject.Parse(GetFlowRecord(p.FormInstId, p.FormCodeId)), cg.Fbillid);
                        using (var db = new TestContext())
                        {
                            db.CGSqlist.Add(cg);
                            db.CgsqListentry.AddRange(cleList);
                            db.Reqlist.AddRange(rlList);
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
            string response = CallExternal.PostUrl(url, param.ToString(), "application/json");
            return response;
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
            if (pushlogs.Count() == 5)
            {
                pageId = pushlogs.Last().Id;
                pageType = "next";
                var logs = GetExternalLog(accessToken, startTime, endTime, pageId, pageType);
                pushlogs.AddRange(logs);
            }
            return pushlogs;
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
