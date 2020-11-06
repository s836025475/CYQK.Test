using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CYQK.Test.Model.Migrations
{
    public partial class ExamineTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CgsqListentry",
                columns: table => new
                {
                    Guid = table.Column<Guid>(nullable: false),
                    Id = table.Column<string>(nullable: true),
                    Fcosttype = table.Column<string>(nullable: true),
                    Fcostamount = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CgsqListentry", x => x.Guid);
                });

            migrationBuilder.CreateTable(
                name: "t_CGSqlist",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Fbillid = table.Column<string>(nullable: true),
                    Freqamount = table.Column<decimal>(nullable: false),
                    Fuseman = table.Column<string>(nullable: true),
                    FmarkertOrea = table.Column<string>(nullable: true),
                    FrequestContext = table.Column<string>(nullable: true),
                    Fdepartment = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_CGSqlist", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "t_reqlist",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Fbilltype = table.Column<string>(nullable: true),
                    Fbillno = table.Column<string>(nullable: true),
                    Fbillid = table.Column<string>(nullable: true),
                    Fcheckerman = table.Column<string>(nullable: true),
                    Fcheckstep = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_reqlist", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CgsqListentry");

            migrationBuilder.DropTable(
                name: "t_CGSqlist");

            migrationBuilder.DropTable(
                name: "t_reqlist");
        }
    }
}
