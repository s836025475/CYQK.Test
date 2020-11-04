﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
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
        public object Post2()
        {
            return "success";
        }
        // POST api/<TestController>
        [HttpPost]
        public object Post([FromBody] object input)
        {
            try
            {
                //转化请求body
                var data = JsonConvert.DeserializeObject<TestEntity>(input.ToString());
                var test = new TestEntity()
                {
                    Name = data.Name,
                };
                _testContext.TestEntity.Add(test);
                //数据添加日志
                var log = new TestLog
                {
                    Id = Guid.NewGuid(),
                    Input = input.ToString(),
                    Output = "",
                    CreationTime = DateTime.Now
                };
                _testContext.TestLog.Add(log);
                _testContext.SaveChanges();
                //返回值
                Result result = new Result()
                {
                    success = true
                };
                return result;
            }
            catch (Exception ex)
            {
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

        [HttpPost]
        public object DocumentInstance()
        {

            return "ok";
        }

    }
}
