using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demo.Migrations
{
    public partial class init2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateSigned",
                table: "TestRequestStatus",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "TestRequestStatus",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateSigned",
                table: "LeaveStatus",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "LeaveStatus",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateSigned",
                table: "CashRequestStatus",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "CashRequestStatus",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateSigned",
                table: "TestRequestStatus");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "TestRequestStatus");

            migrationBuilder.DropColumn(
                name: "DateSigned",
                table: "LeaveStatus");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "LeaveStatus");

            migrationBuilder.DropColumn(
                name: "DateSigned",
                table: "CashRequestStatus");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "CashRequestStatus");
        }
    }
}
