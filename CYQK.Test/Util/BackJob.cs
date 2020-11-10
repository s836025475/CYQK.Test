using CYQK.Test.Model;
using CYQK.Test.Model.Examine;
using Hangfire;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CYQK.Test.Util
{
    public class BackJob
    {
        public void Job(int times)
        {
            using (var db = new TestContext())
            {
                var log = new TestLog
                {
                    Id = Guid.NewGuid(),
                    Input = "BackJob",
                    CreationTime = DateTime.Now
                };
                db.TestLog.Add(log);
                db.SaveChanges();
                times++;
                if(times < 5 )
                    BackgroundJob.Schedule(() => new BackJob().Job(times), DateTime.Now.AddSeconds(5));
            }
        }
        //后台任务，获取审批流
        //public void GetProcess(JObject Json, string fbillid)
        //{
        //    ParseEntity PE = new ParseEntity();
        //    Reqlist rl = PE.GetReqlist(Json, fbillid);
        //    //判断数据库是否存在
        //    using (var db = new TestContext())
        //    {
        //        var query = db.Reqlist.Where(r => r.Fbillid == fbillid)
        //                                .FirstOrDefault(r => r.Fbilltype == rl.Fbilltype);
        //        if (query != null)
        //            db.Add(rl);
        //        db.SaveChanges();
        //    }
        //    if (!rl.Fcheckstep.Equals("End"))
        //    {
        //        BackgroundJob.Schedule(() => new BackJob().GetProcess(Json, fbillid), DateTime.Now.AddMinutes(3));
        //    }
        //}
    }
}
