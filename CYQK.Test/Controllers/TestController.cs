using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CYQK.Test.Dto;
using CYQK.Test.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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
        public string Get()
        {
            return "ok";
        }

        // GET api/<TestController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }
        [HttpPost]
        //[Consumes("application/x-www-form-urlencoded")]
        //测试x-www-form-urlencoded请求
        public object FormPost([FromForm] Student stu)
        {
            //测试AutoMap
            var entity = _mapper.Map<TestEntity>(stu);
            var student = _mapper.Map<Student>(entity);
            return "ok";
        }
        // POST api/<TestController>
        [HttpPost]
        public object Post([FromBody] object input)
        {
            try
            {
                try
                {
                    var data = JsonConvert.DeserializeObject<TestEntity>(input.ToString());
                    //using (var db = new TestContext())
                    //{
                        var test = new TestEntity()
                        {
                            //Id = data.Id,
                            Name = data.Name,
                        };
                    _testContext.TestEntity.Add(test);
                        //添加日志
                        var log = new TestLog
                        {
                            Id = Guid.NewGuid(),
                            Input = input.ToString(),
                            Output = "",
                            CreationTime = DateTime.Now
                        };
                    _testContext.TestLog.Add(log);
                    _testContext.SaveChanges();
                    //}
                    return "OK";
                }
                catch (Exception ex)
                {
                    return new { ret = -1, msg = ex.Message };
                }
            }
            catch (Exception ex)
            {
                //using (var db = new TestContext())
                //{
                //    //报错输出，添加日志
                //    var log = new TestLog
                //    {
                //        Id = Guid.NewGuid(),
                //        Input = input.ToString(),
                //        Output = ex.StackTrace,
                //        CreationTime = DateTime.Now
                //    };
                //    db.TestLog.Add(log);
                //    db.SaveChanges();
                //}
                var log = new TestLog
                {
                    Id = Guid.NewGuid(),
                    Input = input.ToString(),
                    Output = ex.StackTrace,
                    CreationTime = DateTime.Now
                };
                _testContext.TestLog.Add(log);
                _testContext.SaveChangesAsync();
                return new { ret = -1, msg = ex.Message };
            }

        }

    }
}
