﻿// <auto-generated />
using System;
using CYQK.Test.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CYQK.Test.Model.Migrations
{
    [DbContext(typeof(TestContext))]
    [Migration("20201106102622_cgtablefix")]
    partial class cgtablefix
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.9")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("CYQK.Test.Model.Examine.CGSqlist", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Fbillid")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Fdepartment")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("FirstInput")
                        .HasColumnType("bit");

                    b.Property<string>("FmarkertOrea")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("Freqamount")
                        .HasColumnType("decimal(18,6)");

                    b.Property<string>("FrequestContext")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Fsubmitman")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Fuseman")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("t_CGSqlist");
                });

            modelBuilder.Entity("CYQK.Test.Model.Examine.CgsqListentry", b =>
                {
                    b.Property<Guid>("Guid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Fbillid")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Fcostamount")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Fcosttype")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Guid");

                    b.ToTable("t_CgsqListentry");
                });

            modelBuilder.Entity("CYQK.Test.Model.Examine.Reqlist", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Fbillid")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Fbillno")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Fbilltype")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Fcheckerman")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Fcheckstep")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("t_reqlist");
                });

            modelBuilder.Entity("CYQK.Test.Model.TestEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("TestEntity");
                });

            modelBuilder.Entity("CYQK.Test.Model.TestLog", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreationTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Input")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Output")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("TestLog");
                });
#pragma warning restore 612, 618
        }
    }
}