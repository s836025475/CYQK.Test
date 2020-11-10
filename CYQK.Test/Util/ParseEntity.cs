using CYQK.Test.Dto.ExamineDto;
using CYQK.Test.Model.Examine;
using Microsoft.AspNetCore.Razor.Language;
using Nancy.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace CYQK.Test.Util
{
    public class ParseEntity
    {
        public CGSqlist GetCGSQlist(JObject Json)
        {
            //添加信息
            //FormCodeId
            string formCodeId = Json["data"]["basicInfo"]["formCodeId"].ToString();
            //FormInstId
            string formInstId = Json["data"]["basicInfo"]["formInstId"].ToString();
            //申请金额
            decimal applyAmount = decimal.Parse(Json["data"]["formInfo"]["widgetMap"]["Mo_1"]["value"].ToString());
            //执行人
            List<PersonInfo> persons = GetPerson(Json["data"]["formInfo"]["widgetMap"]["Ps_0"]["personInfo"].ToString());
            string executor = "";//内容
            persons.ForEach(p =>
            {
                executor += p.Name.ToString() + ",";
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
                submitter += p.Name.ToString() + ",";
            });
            //申请内容
            string content = Json["data"]["formInfo"]["widgetMap"]["Ta_0"]["value"].ToString();
            //所属部门
            List<DeptInfo> deptInfo = GetDept(Json["data"]["formInfo"]["widgetMap"]["_S_DEPT"]["deptInfo"].ToString());
            string department = "";//内容
            deptInfo.ForEach(d =>
            {
                department += d.Name.ToString() + ",";
            });
            //时间
            DateTime eventTime = TimeFormat.ToLocalTimeTime((long)Json["data"]["basicInfo"]["eventTime"]); 
            CGSqlist cg = new CGSqlist()
            {
                FormCodeId = formCodeId,
                FormInstId = formInstId,
                Fbillid = "DD" + DateTime.Now.ToString("yyMMddHHmmss") + RandomHelper.RandomString(4),//Fbillid
                Freqamount = applyAmount,//申请金额
                Fuseman = executor,//执行人
                FmarkertOrea = marketArea,//市场区域
                Fsubmitman = submitter,//提交人
                FrequestContext = content,//申请内容
                Fdepartment = department,//所属部门
                FirstInput = true,
                EventTime = eventTime
            };
            return cg;
        }
        public CgsqListentry GetCgsqListentry(JObject Json, string fbillid)
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
        public Reqlist GetReqlist(JObject Json, string fbillid)
        {
            string jsonData = Json["data"].ToString();
            JArray jo = (JArray)JsonConvert.DeserializeObject(jsonData);
            List<ApprovalProcess> ap = jo.ToObject<List<ApprovalProcess>>();
            var item = ap.Last();
            //放入List
            Reqlist reqList = new Reqlist()
            {
                Fbilltype = item.ActivityType,//单据类型
                Fbillno = item.FlowInstId,//单据编号
                Fbillid = fbillid,//单据Id
                Fcheckerman = item.Name,//审核人
                Fcheckstep = item.ActivityName//审核级次
            };
            return reqList;
        }
        public List<PersonInfo> GetPerson(string personStr)
        {
            List<PersonInfo> persons = new List<PersonInfo>();
            JavaScriptSerializer Serializer = new JavaScriptSerializer();
            persons = Serializer.Deserialize<List<PersonInfo>>(personStr);
            return persons;
        }
        public List<MarketArea> GetMarket(string marketStr)
        {
            List<MarketArea> markets = new List<MarketArea>();
            JavaScriptSerializer Serializer = new JavaScriptSerializer();
            markets = Serializer.Deserialize<List<MarketArea>>(marketStr);
            return markets;
        }
        public List<FeeType> GetFeeType(string feeTypeStr)
        {
            List<FeeType> feeTypes = new List<FeeType>();
            JavaScriptSerializer Serializer = new JavaScriptSerializer();
            feeTypes = Serializer.Deserialize<List<FeeType>>(feeTypeStr);
            return feeTypes;
        }
        public List<DeptInfo> GetDept(string deptStr)
        {
            List<DeptInfo> deptInfo = new List<DeptInfo>();
            JavaScriptSerializer Serializer = new JavaScriptSerializer();
            deptInfo = Serializer.Deserialize<List<DeptInfo>>(deptStr);
            return deptInfo;
        }
    }
}
