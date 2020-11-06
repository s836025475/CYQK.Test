using Microsoft.EntityFrameworkCore.Migrations;

namespace CYQK.Test.Model.Migrations
{
    public partial class clefbillid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Id",
                table: "t_CgsqListentry");

            migrationBuilder.AddColumn<string>(
                name: "Fbillid",
                table: "t_CgsqListentry",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Fbillid",
                table: "t_CgsqListentry");

            migrationBuilder.AddColumn<string>(
                name: "Id",
                table: "t_CgsqListentry",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
