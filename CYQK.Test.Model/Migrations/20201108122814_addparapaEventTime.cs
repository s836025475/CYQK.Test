using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CYQK.Test.Model.Migrations
{
    public partial class addparapaEventTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "EventTime",
                table: "t_CGSqlist",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "FormCodeId",
                table: "t_CGSqlist",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FormInstId",
                table: "t_CGSqlist",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EventTime",
                table: "t_CGSqlist");

            migrationBuilder.DropColumn(
                name: "FormCodeId",
                table: "t_CGSqlist");

            migrationBuilder.DropColumn(
                name: "FormInstId",
                table: "t_CGSqlist");
        }
    }
}
