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
            using (var _testContext = new TestContext())
            {
                try
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
                    //获取CGSqlist
                    CGSqlist cg = ParseEntity.GetCGSQlist(json);
                    //获取CgsqListentry
                    CgsqListentry cle = ParseEntity.GetCgsqListentry(json, cg.Fbillid);
                    //存入数据库
                    _testContext.CGSqlist.Add(cg);
                    _testContext.CgsqListentry.Add(cle);
                    await _testContext.SaveChangesAsync();
                    //数据添加日志
                    var log = new TestLog
                    {
                        Id = Guid.NewGuid(),
                        Input = text,
                        CreationTime = DateTime.Now
                    };
                    _testContext.TestLog.Add(log);
                    _testContext.SaveChanges();
                    //返回值
                    return new ReturnMessage { Success = true };
                }
                catch (Exception ex)
                {
                    var log = new TestLog
                    {
                        Id = Guid.NewGuid(),
                        Output = ex.StackTrace,
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

    }
}
