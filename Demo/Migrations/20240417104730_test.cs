using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demo.Migrations
{
    public partial class test : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TestRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Flag = table.Column<bool>(type: "bit", nullable: false),
                    CurrentStatus = table.Column<int>(type: "int", nullable: true),
                    LastSignDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastSignBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestRequests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TestRequestStatus",
                columns: table => new
                {
                    TestRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Status = table.Column<int>(type: "int", nullable: false),
                    DateSigned = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SignedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestRequestStatus", x => new { x.TestRequestId, x.Id });
                    table.ForeignKey(
                        name: "FK_TestRequestStatus_TestRequests_TestRequestId",
                        column: x => x.TestRequestId,
                        principalTable: "TestRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TestRequestStatus");

            migrationBuilder.DropTable(
                name: "TestRequests");
        }
    }
}
