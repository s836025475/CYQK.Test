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
            string url = "https://yunzhijia.com/gateway/workflow/form/thirdpart/getFlowRecord?accessToken=" + GetAccessToken();
            JObject param = new JObject();
            param.Add("formInstId", formInstId);
            param.Add("formCodeId", formCodeId);
            string response = PostUrl(url, param.ToString(), "application/json");
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
    }
}
