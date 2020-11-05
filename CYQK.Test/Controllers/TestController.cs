using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CYQK.Test.Dto;
using CYQK.Test.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CYQK.Test.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly TestContext _testContext;
        private readonly IMapper _mapper;

        public TestController(
            TestContext testContext,
            IMapper mapper)
        {
            _testContext = testContext;
            _mapper = mapper;
        }
        // GET: api/<TestController>
        [HttpGet]
        public List<TestEntity> GetAll(int input)
        {
            int skip = -1;
            skip = (skip + input) * 5;
            var result = _testContext.TestEntity.Skip(skip).Take(5).ToList();
            return result;
        }

        // GET api/<TestController>/5
        [HttpGet]
        public string Get(int id)
        {
            return "ok";
        }
        [HttpPost]
        //[Consumes("application/x-www-form-urlencoded")]
        //测试x-www-form-urlencoded请求
        public object FormPost([FromForm] Student stu)
        {
            //获取请求url里的参数信息
            string id = Request.Query["id"];
            //测试AutoMap
            var entity = _mapper.Map<TestEntity>(stu);
            var student = _mapper.Map<Student>(entity);
            return "ok";
        }
        [HttpPost]
        public async Task<object> Post()
        {
            try
            {
                //读取请求信息
                Stream reqStream = Request.Body;
                string text = "";
                using (StreamReader reader = new StreamReader(reqStream))
                {
                    text = await reader.ReadToEndAsync();
                }
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
                return "success";
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
                return new { ret = -1, msg = ex.Message };
            }
        }
    }
}
