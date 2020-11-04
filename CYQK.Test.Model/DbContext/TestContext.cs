﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CYQK.Test.Model
{
    public class TestContext : DbContext
    {
        public virtual DbSet<TestEntity> TestEntity { get; set; }
        public virtual DbSet<DefaultTable> DefaultTable { get; set; }
        public virtual DbSet<TestLog> TestLog { get; set; }

        public TestContext(DbContextOptions<TestContext> options)
            : base(options)
        {

        }
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    var builder = new ConfigurationBuilder()
        //    .SetBasePath(Directory.GetCurrentDirectory())
        //    .AddJsonFile("appsettings.json");

        //    var configuration = builder.Build();

        //    var conn = configuration.GetConnectionString("DbTest");
        //    optionsBuilder.UseSqlServer(conn);
        //}
    }
}