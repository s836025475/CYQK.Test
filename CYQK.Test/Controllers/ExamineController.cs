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

namespace CYQK.Test.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class ExamineController : ControllerBase
    {
        private readonly TestContext _testContext;
        public ExamineController(
            TestContext testContext)
        {
            _testContext = testContext;
        }

        [HttpPost]
        public async Task<object> Post()
        {
            //读取json信息
            var fileName = @"D:\zjc\new 25.txt";
            string data = JsonStr(fileName);
            JObject Json = JObject.Parse(data);
            //获取CGSqlist
            CGSqlist cg = GetCGSQlist(Json);
            //获取CgsqListentry
            CgsqListentry cle = GetCgsqListentry(Json, cg.Fbillid);
            //存入数据库
            _testContext.CGSqlist.Add(cg);
            _testContext.CgsqListentry.Add(cle);
            await _testContext.SaveChangesAsync();
            return "ok";
        }
        private string JsonStr(string fileName)
        {
            StreamReader sr = new StreamReader(fileName);
            String data = sr.ReadToEnd();
            data = data.Replace("\r", string.Empty).Replace("\n", string.Empty).Replace("\t", string.Empty);//去除空格
            return data;
        }
        private CGSqlist GetCGSQlist(JObject Json)
        {
            //添加信息
            //申请金额
            decimal applyAmount = decimal.Parse(Json["data"]["formInfo"]["widgetMap"]["Mo_1"]["value"].ToString());
            //执行人
            List<PersonInfo> persons = GetPerson(Json["data"]["formInfo"]["widgetMap"]["Ps_0"]["personInfo"].ToString());
            string executor = "";//内容
            persons.ForEach(p =>
            {
                executor += p.Name.ToString() + ", ";
            });
            //市场区域
            List<MarketArea> markets = GetMarket(Json["data"]["formInfo"]["widgetMap"]["Ra_0"]["options"].ToString());
            string marketKey = Json["data"]["formInfo"]["widgetMap"]["Ra_0"]["value"].ToString();
            string marketArea = "";//内容
            foreach (var item in markets)
            {
                if (item.Key.Equals(marketKey))
                    marketArea = item.Value;
            }
            //提交人
            List<PersonInfo> subpersons = GetPerson(Json["data"]["formInfo"]["widgetMap"]["_S_APPLY"]["personInfo"].ToString());
            string submitter = "";//内容
            persons.ForEach(p =>
            {
                submitter += p.Name.ToString() + ", ";
            });
            //申请内容
            string content = Json["data"]["formInfo"]["widgetMap"]["Ta_0"]["value"].ToString();
            //所属部门
            List<DeptInfo> deptInfo = GetDept(Json["data"]["formInfo"]["widgetMap"]["_S_DEPT"]["deptInfo"].ToString());
            string department = "";//内容
            deptInfo.ForEach(d =>
            {
                department += d.Name.ToString() + ", ";
            });
            CGSqlist cg = new CGSqlist()
            {
                Fbillid = "DD" + DateTime.Now.ToString("yyMMddHHmmss") + Util.RandomHelper.RandomString(4),//Fbillid
                Freqamount = applyAmount,//申请金额
                Fuseman = executor,//执行人
                FmarkertOrea = marketArea,//市场区域
                Fsubmitman = submitter,//提交人
                FrequestContext = content,//申请内容
                Fdepartment = department//所属部门
            };
            return cg;
        }
        private CgsqListentry GetCgsqListentry(JObject Json, string fbillid)
        {
            //添加信息
            //费用类型
            List<FeeType> feeTypes = GetFeeType(Json["data"]["formInfo"]["widgetMap"]["Ra_1"]["options"].ToString());
            string feeKey = Json["data"]["formInfo"]["widgetMap"]["Ra_1"]["value"].ToString();//内容
            string feeType = "";
            foreach (var item in feeTypes)
            {
                if (item.Key.Equals(feeKey))
                    feeType = item.Value;
            }
            //申请金额
            string applyAmount = Json["data"]["formInfo"]["widgetMap"]["Mo_1"]["value"].ToString();
            CgsqListentry cle = new CgsqListentry()
            {
                Guid = Guid.NewGuid(),
                Fbillid = fbillid,//id
                Fcosttype = feeType,//费用类型
                Fcostamount = applyAmount//申请金额
            };
            return cle;
        }
        private List<PersonInfo> GetPerson(string personStr)
        {
            List<PersonInfo> persons = new List<PersonInfo>();
            JavaScriptSerializer Serializer = new JavaScriptSerializer();
            persons = Serializer.Deserialize<List<PersonInfo>>(personStr);
            return persons;
        }
        private List<MarketArea> GetMarket(string marketStr)
        {
            List<MarketArea> markets = new List<MarketArea>();
            JavaScriptSerializer Serializer = new JavaScriptSerializer();
            markets = Serializer.Deserialize<List<MarketArea>>(marketStr);
            return markets;
        }
        private List<FeeType> GetFeeType(string feeTypeStr)
        {
            List<FeeType> feeTypes = new List<FeeType>();
            JavaScriptSerializer Serializer = new JavaScriptSerializer();
            feeTypes = Serializer.Deserialize<List<FeeType>>(feeTypeStr);
            return feeTypes;
        }
        private List<DeptInfo> GetDept(string deptStr)
        {
            List<DeptInfo> deptInfo = new List<DeptInfo>();
            JavaScriptSerializer Serializer = new JavaScriptSerializer();
            deptInfo = Serializer.Deserialize<List<DeptInfo>>(deptStr);
            return deptInfo;
        }
    }
}
