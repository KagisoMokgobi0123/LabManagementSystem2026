using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NMBHLabSys.Migrations
{
    /// <inheritdoc />
    public partial class AddNewTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TestRequestTestTypes",
                columns: table => new
                {
                    TestRequestTestTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TestRequestId = table.Column<int>(type: "int", nullable: false),
                    TestTypeId = table.Column<int>(type: "int", nullable: false),
                    TechnicianId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletionDateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestRequestTestTypes", x => x.TestRequestTestTypeId);
                    table.ForeignKey(
                        name: "FK_TestRequestTestTypes_AspNetUsers_TechnicianId",
                        column: x => x.TechnicianId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TestRequestTestTypes_TestRequests_TestRequestId",
                        column: x => x.TestRequestId,
                        principalTable: "TestRequests",
                        principalColumn: "TestRequestId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TestRequestTestTypes_TestTypes_TestTypeId",
                        column: x => x.TestTypeId,
                        principalTable: "TestTypes",
                        principalColumn: "TestTypeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TestRequestTestTypes_TechnicianId",
                table: "TestRequestTestTypes",
                column: "TechnicianId");

            migrationBuilder.CreateIndex(
                name: "IX_TestRequestTestTypes_TestRequestId",
                table: "TestRequestTestTypes",
                column: "TestRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_TestRequestTestTypes_TestTypeId",
                table: "TestRequestTestTypes",
                column: "TestTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TestRequestTestTypes");
        }
    }
}
