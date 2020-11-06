using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CYQK.Test.Model;
using CYQK.Test.Model.Examine;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
            var fileName = @"D:\zjc\new 25.txt";
            string data = JsonStr(fileName);

            //_testContext.CGSqlist.Add(cg);
            await _testContext.SaveChangesAsync();
            return "ok";
        }
        private static string JsonStr(string fileName)
        {
            StreamReader sr = new StreamReader(fileName);
            String data = sr.ReadToEnd();
            data = data.Replace("\r", string.Empty).Replace("\n", string.Empty).Replace("\t", string.Empty);//去除空格
            return data;
        }
    }
}
