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
            //流水号
            string serialNumber = Json["data"]["formInfo"]["widgetMap"]["_S_SERIAL"]["value"].ToString();
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
            executor = executor.Remove(executor.LastIndexOf(','), 1);
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
            submitter = submitter.Remove(submitter.LastIndexOf(','), 1);
            //申请内容
            string content = Json["data"]["formInfo"]["widgetMap"]["Ta_0"]["value"].ToString();
            //所属部门
            List<DeptInfo> deptInfo = GetDept(Json["data"]["formInfo"]["widgetMap"]["_S_DEPT"]["deptInfo"].ToString());
            string department = "";//内容
            deptInfo.ForEach(d =>
            {
                department += d.Name.ToString() + ",";
            });
            department = department.Remove(department.LastIndexOf(','), 1);
            //时间
            DateTime eventTime = TimeFormat.ToLocalTimeTime((long)Json["data"]["basicInfo"]["eventTime"]);
            //费用类型
            List<FeeType> feeTypes = GetFeeType(Json["data"]["formInfo"]["widgetMap"]["Ra_1"]["options"].ToString());
            string feeKey = Json["data"]["formInfo"]["widgetMap"]["Ra_1"]["value"].ToString();//内容
            string feeType = "";
            foreach (var item in feeTypes)
            {
                if (item.Key.Equals(feeKey))
                    feeType = item.Value;
            }
            List<WidgetValue> widgetValues = GetWidgetValue(Json["data"]["formInfo"]["detailMap"]["Dd_0"]["widgetValue"].ToString());
            //总数量
            int totalCount = 0;
            //总金额
            decimal totalFee = 0;
            widgetValues.ForEach(w =>
            {
                totalCount += int.Parse(w.Nu_1);
                totalFee += decimal.Parse(w.Mo_0);
            });
            CGSqlist cg = new CGSqlist()
            {
                FormCodeId = formCodeId,
                FormInstId = formInstId,
                SerialNumber = serialNumber,
                Fbillid = "DD" + DateTime.Now.ToString("yyMMddHHmmss") + RandomHelper.RandomString(4),//Fbillid
                Freqamount = applyAmount,//申请金额
                Fuseman = executor,//执行人
                FmarkertOrea = marketArea,//市场区域
                Fsubmitman = submitter,//提交人
                FrequestContext = content,//申请内容
                Fdepartment = department,//所属部门
                FirstInput = true,
                FeeType = feeType,//费用类型
                TotalCount = totalCount,//总数量
                TotalFee = totalFee,//总金额
                EventTime = eventTime
            };
            return cg;
        }
        public List<CgsqListentry> GetCgsqListentry(JObject Json, string fbillid)
        {
            //添加信息
            List<WidgetValue> widgetValues = GetWidgetValue(Json["data"]["formInfo"]["detailMap"]["Dd_0"]["widgetValue"].ToString());
            //申请金额
            string applyAmount = Json["data"]["formInfo"]["widgetMap"]["Mo_1"]["value"].ToString();
            List<CgsqListentry> cleList = new List<CgsqListentry>();
            foreach (var item in widgetValues)
            {
                CgsqListentry cle = new CgsqListentry()
                {
                    Guid = Guid.NewGuid(),
                    Fbillid = fbillid,//id
                    WineName = item.Te_0,
                    WineCount = int.Parse(item.Nu_1),
                    WineFee = decimal.Parse(item.Mo_0)
                };
                cleList.Add(cle);
            }
            return cleList;
        }
        public List<Reqlist> GetReqlist(JObject Json, string fbillid)
        {
            string jsonData = Json["data"].ToString();
            JArray jo = (JArray)JsonConvert.DeserializeObject(jsonData);
            List<ApprovalProcess> ap = jo.ToObject<List<ApprovalProcess>>();
            //获取最近的一次流程
            List<Reqlist> list = new List<Reqlist>();
            foreach (var item in ap)
            {
                Reqlist reqList = new Reqlist()
                {
                    Fbilltype = item.ActivityType,//单据类型
                    Fbillno = item.FlowInstId,//单据编号
                    Fbillid = fbillid,//单据Id
                    Fcheckerman = item.Name,//审核人
                    Fcheckstep = item.ActivityName//审核级次
                };
                //创建时间
                if (item.CreateTime != null)
                    reqList.CreateTime = TimeFormat.ToLocalTimeTime((long)item.CreateTime);
                //执行时间
                if (item.HandleTime != null)
                    reqList.HandleTime = TimeFormat.ToLocalTimeTime((long)item.HandleTime);
                list.Add(reqList);
            }
            return list;
        }
        /// <summary>
        /// 获取执行人
        /// </summary>
        /// <param name="personStr"></param>
        /// <returns></returns>
        public List<PersonInfo> GetPerson(string personStr)
        {
            List<PersonInfo> persons = new List<PersonInfo>();
            JavaScriptSerializer Serializer = new JavaScriptSerializer();
            persons = Serializer.Deserialize<List<PersonInfo>>(personStr);
            return persons;
        }
        /// <summary>
        /// 获取市场区域
        /// </summary>
        /// <param name="marketStr"></param>
        /// <returns></returns>
        public List<MarketArea> GetMarket(string marketStr)
        {
            List<MarketArea> markets = new List<MarketArea>();
            JavaScriptSerializer Serializer = new JavaScriptSerializer();
            markets = Serializer.Deserialize<List<MarketArea>>(marketStr);
            return markets;
        }
        /// <summary>
        /// 获取费用类型
        /// </summary>
        /// <param name="feeTypeStr"></param>
        /// <returns></returns>
        public List<FeeType> GetFeeType(string feeTypeStr)
        {
            List<FeeType> feeTypes = new List<FeeType>();
            JavaScriptSerializer Serializer = new JavaScriptSerializer();
            feeTypes = Serializer.Deserialize<List<FeeType>>(feeTypeStr);
            return feeTypes;
        }
        /// <summary>
        /// 获取市场
        /// </summary>
        /// <param name="deptStr"></param>
        /// <returns></returns>
        public List<DeptInfo> GetDept(string deptStr)
        {
            List<DeptInfo> deptInfo = new List<DeptInfo>();
            JavaScriptSerializer Serializer = new JavaScriptSerializer();
            deptInfo = Serializer.Deserialize<List<DeptInfo>>(deptStr);
            return deptInfo;
        }
        /// <summary>
        /// 获取用酒
        /// </summary>
        /// <param name="widgetValueStr"></param>
        /// <returns></returns>
        private static List<WidgetValue> GetWidgetValue(string widgetValueStr)
        {
            List<WidgetValue> widgetValues = new List<WidgetValue>();
            JavaScriptSerializer Serializer = new JavaScriptSerializer();
            widgetValues = Serializer.Deserialize<List<WidgetValue>>(widgetValueStr);
            return widgetValues;
        }
    }
}
