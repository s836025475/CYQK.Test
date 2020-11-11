using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CYQK.Test.Dto.ExamineDto;
using CYQK.Test.Model;
using CYQK.Test.Model.Examine;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Nancy.Json;
using CYQK.Test.Dto;
using System.Security.Cryptography;
using System.Text;
using CYQK.Test.Util;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Net;
using System.Threading;
using Hangfire;

namespace CYQK.Test.Controllers
{
    [Route("cyqk/[controller]/[Action]")]
    [ApiController]
    public class ExamineController : ControllerBase
    {
        //private readonly TestContext _testContext;
        //public ExamineController(
        //    TestContext testContext)
        //{
        //    _testContext = testContext;
        //}
        [HttpGet]
        public object Get(int id)
        {
            try
            {
                using (var db = new TestContext())
                {
                    var query = db.CGSqlist.ToList();
                    //BackgroundJob.Schedule(() => new BackJob().Job(1), DateTime.Now.AddSeconds(5));
                    return "ok";
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }
            //new TimeJob().DoTask();
            
        }
        /// <summary>
        /// 获取遗漏表单
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<object> TakeMissingLog([FromBody] ParamInput input)
        {
            try
            {
                using (var db = new TestContext())
                {
                    //获取AccessToken
                    string accessToken = CallExternal.GetAccessToken();
                    //获取外部接口日志
                    long startTime = input.StartTime;//开始时间
                    long endTime = input.EndTime;//结束时间
                    string pageId = null;//页码Id 上一页则传当前最小记录id，下一页则传当前最大id，首页或者最后页传null
                    string pageType = "first";//页码类型 prev=上一页，next=下一页，first=第一页，last=最后一页
                    var pushLogs = GetExternalLog(accessToken, startTime, endTime, pageId, pageType);
                    //获取实例
                    pushLogs.ForEach(p =>
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
                            var query = db.CGSqlist.Where(c => c.FormInstId.Equals(cg.FormInstId))
                                        .Where(c => c.FormCodeId.Equals(cg.FormCodeId))
                                        .AsNoTracking().ToList();
                            if (query.Count == 0)
                            {
                                //获取CgsqListentry
                                List<CgsqListentry> cleList = PE.GetCgsqListentry(jObject, cg.Fbillid);
                                List<Reqlist> rlList = PE.GetReqlist(JObject.Parse(GetFlowRecord(p.FormInstId, p.FormCodeId)), cg.Fbillid);
                                db.CGSqlist.Add(cg);
                                db.CgsqListentry.AddRange(cleList);
                                db.Reqlist.AddRange(rlList);
                                db.SaveChanges();
                            }
                            
                        }
                    });
                }
                return new ReturnMessage { Success = true, Data = "ok" };
            }
            catch (Exception e)
            {
                return new ReturnMessage { Success = false, Data = e.Message };
            }
            
        }
        /// <summary>
        /// 获取外部日志
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="pageId"></param>
        /// <param name="pageType"></param>
        /// <returns></returns>
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
            postParam.Add("pushType", "all");
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
        /// <summary>
        /// 获取实例
        /// </summary>
        /// <param name="formInstId"></param>
        /// <param name="formCodeId"></param>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        private string GetInstance(string formInstId, string formCodeId, string accessToken)
        {
            string url = "https://yunzhijia.com/gateway/workflow/form/thirdpart/viewFormInst?accessToken=" + accessToken;
            JObject param = new JObject();
            param.Add("formInstId", formInstId);
            param.Add("formCodeId", formCodeId);
            string response = CallExternal.PostUrl(url, param.ToString(), "application/json");
            return response;
        }
        [HttpPost]
        public async Task<object> SendMessage()
        {
            try
            {
                using (var _testContext = new TestContext())
                {
                    //读取请求信息
                    Stream reqStream = Request.Body;
                    string key = "mYOxAyHTFNCoFg3c";
                    string text = "";
                    using (StreamReader reader = new StreamReader(reqStream))
                    {
                        text = await reader.ReadToEndAsync();
                    }
                    //获取解密后的字符串
                    string decrypt = AesDecrypt(text, key);
                    JObject json = JObject.Parse(decrypt);
                    ParseEntity PE = new ParseEntity();
                    //获取CGSqlist
                    CGSqlist cg = PE.GetCGSQlist(json);
                    //获取CgsqListentry
                    List<CgsqListentry> cleList = PE.GetCgsqListentry(json, cg.Fbillid);
                    //获取审批痕迹
                    List<Reqlist> rlList = PE.GetReqlist(JObject.Parse(GetFlowRecord(cg.FormInstId, cg.FormCodeId)), cg.Fbillid);
                    //存入数据库
                    _testContext.CGSqlist.Add(cg);
                    _testContext.CgsqListentry.AddRange(cleList);
                    _testContext.Reqlist.AddRange(rlList);
                    //_testContext.Reqlist.Add(rl);
                    //数据添加日志
                    var log = new TestLog
                    {
                        Id = Guid.NewGuid(),
                        Input = text,
                        CreationTime = DateTime.Now
                    };
                    _testContext.TestLog.Add(log);
                    await _testContext.SaveChangesAsync();
                    //创建后台任务读取审批状态
                    //BackgroundJob.Schedule(() => new BackJob().GetProcess(JObject.Parse(GetFlowRecord(cg.FormInstId, cg.FormCodeId)), cg.Fbillid), DateTime.Now.AddMinutes(3));
                    //返回值
                    return new ReturnMessage { Success = true };
                }
            }
            catch (Exception ex)
            {
                using (var _testContext = new TestContext())
                {
                    var log = new TestLog
                    {
                        Id = Guid.NewGuid(),
                        Output = ex.Message,
                        CreationTime = DateTime.Now
                    };
                    _testContext.TestLog.Add(log);
                    await _testContext.SaveChangesAsync();
                    return new ReturnMessage { Success = false, Data = ex.Message.ToString() };
                }
            }
            
        }
        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="str"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        private string AesDecrypt(string str, string key)
        {
            if (string.IsNullOrEmpty(str)) return null;
            Byte[] toEncryptArray = Convert.FromBase64String(str);

            RijndaelManaged rm = new RijndaelManaged
            {
                Key = Encoding.UTF8.GetBytes(key),
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };

            ICryptoTransform cTransform = rm.CreateDecryptor();
            Byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return Encoding.UTF8.GetString(resultArray);
        }
        /// <summary>
        /// 获取审批痕迹
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="formInstId"></param>
        /// <param name="formCodeId"></param>
        /// <returns></returns>
        private string GetFlowRecord(string formInstId, string formCodeId)
        {
            string url = "https://yunzhijia.com/gateway/workflow/form/thirdpart/getFlowRecord?accessToken=" + CallExternal.GetAccessToken();
            JObject param = new JObject();
            param.Add("formInstId", formInstId);
            param.Add("formCodeId", formCodeId);
            string response = CallExternal.PostUrl(url, param.ToString(), "application/json");
            return response;
        }
        
    }
}
