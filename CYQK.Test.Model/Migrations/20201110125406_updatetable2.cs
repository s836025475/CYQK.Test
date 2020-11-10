using Microsoft.EntityFrameworkCore.Migrations;

namespace CYQK.Test.Model.Migrations
{
    public partial class updatetable2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Fcostamount",
                table: "t_CgsqListentry");

            migrationBuilder.DropColumn(
                name: "Fcosttype",
                table: "t_CgsqListentry");

            migrationBuilder.AddColumn<string>(
                name: "WineCount",
                table: "t_CgsqListentry",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "WineFee",
                table: "t_CgsqListentry",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "WineName",
                table: "t_CgsqListentry",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FeeType",
                table: "t_CGSqlist",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "TotalCount",
                table: "t_CGSqlist",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalFee",
                table: "t_CGSqlist",
                type: "decimal(18,6)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WineCount",
                table: "t_CgsqListentry");

            migrationBuilder.DropColumn(
                name: "WineFee",
                table: "t_CgsqListentry");

            migrationBuilder.DropColumn(
                name: "WineName",
                table: "t_CgsqListentry");

            migrationBuilder.DropColumn(
                name: "FeeType",
                table: "t_CGSqlist");

            migrationBuilder.DropColumn(
                name: "TotalCount",
                table: "t_CGSqlist");

            migrationBuilder.DropColumn(
                name: "TotalFee",
                table: "t_CGSqlist");

            migrationBuilder.AddColumn<string>(
                name: "Fcostamount",
                table: "t_CgsqListentry",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Fcosttype",
                table: "t_CgsqListentry",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
