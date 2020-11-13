using CYQK.Test.Model.Examine;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CYQK.Test.Model
{
    public class TestContext : DbContext
    {
        public virtual DbSet<CGSqlist> CGSqlist { get; set; }
        public virtual DbSet<CgsqListentry> CgsqListentry { get; set; }
        public virtual DbSet<Reqlist> Reqlist { get; set; }
        public virtual DbSet<ExternalLog> ExternalLog { get; set; }


        //public TestContext(DbContextOptions<TestContext> options)
        //    : base(options)
        //{

        //}
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json");

            var configuration = builder.Build();

            var conn = configuration.GetConnectionString("DbTest");
            optionsBuilder.UseSqlServer(conn);
        }
    }
}
