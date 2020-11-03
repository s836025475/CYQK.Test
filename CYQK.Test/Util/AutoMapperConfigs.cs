using AutoMapper;
using CYQK.Test.Dto;
using CYQK.Test.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace CYQK.Test.Util
{
    public class AutoMapperConfigs : Profile
    {
        public AutoMapperConfigs()
        {
            CreateMap<Student, TestEntity>();
            CreateMap<TestEntity, Student>();
        }
    }
}
