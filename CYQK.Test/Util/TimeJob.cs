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
    public class TimeJob //: Job
    {
        //[Invoke(Begin = "2020-11-11 00:00", Interval = 1000 * 3600, SkipWhileExecuting = true)]
        public void DoTask()
        {
            int totalCount = 0;
            int count = 0;
            //获取AccessToken
            string accessToken = CallExternal.GetAccessToken();
            //获取外部接口日志
            DateTime time = DateTime.Today.AddDays(-13);//开始时间
            using (var db = new TestContext())
            {
                var query = db.ExternalLog.OrderByDescending(e => e.EndTime).FirstOrDefault();
                if (query != null)
                    time = query.EndTime;
            }
            long startTime = TimeFormat.ToUnixTimestampByMilliseconds(time.AddMinutes(-1));//开始时间
            long endTime = TimeFormat.ToUnixTimestampByMilliseconds(DateTime.Now);//结束时间
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
                using (var DbContext = new TestContext())
                {
                    ExternalLog log = new ExternalLog
                    {
                        StartTime = TimeFormat.ToLocalTimeTime(startTime),
                        EndTime = TimeFormat.ToLocalTimeTime(endTime),
                        QueryState = QueryState.成功,
                        FailTotalNum = totalCount,
                        TakeStatus = TakeStatus.完全同步,
                        CatchNum = count,
                        CreationTime = TimeFormat.ToLocalTimeTime(endTime),
                        TakeTime = TimeFormat.ToLocalTimeTime(endTime)
                    };
                    DbContext.ExternalLog.Add(log);
                    DbContext.SaveChanges();
                }
            }
            catch (Exception e)
            {
                
                using (var DbContext = new TestContext())
                {
                    ExternalLog log = new ExternalLog
                    {
                        StartTime = TimeFormat.ToLocalTimeTime(startTime),
                        EndTime = TimeFormat.ToLocalTimeTime(endTime),
                        QueryState = QueryState.失败,
                        FailTotalNum = totalCount,
                        TakeStatus = TakeStatus.部分同步,
                        CatchNum = count,
                        CreationTime = TimeFormat.ToLocalTimeTime(endTime),
                        TakeTime = TimeFormat.ToLocalTimeTime(endTime)
                    };
                    if (totalCount == 0 || count == 0)
                        log.TakeStatus = TakeStatus.未同步;
                    DbContext.ExternalLog.Add(log);
                    DbContext.SaveChanges();
                }
            }
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
            if (pushlogs.Count() == 100)
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
