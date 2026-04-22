using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace NMBHLabSys.Migrations
{
    /// <inheritdoc />
    public partial class updateSubsystems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssignTestTypes");

            migrationBuilder.CreateTable(
                name: "TestRequestsModelTestTypeModel",
                columns: table => new
                {
                    TestRequestsTestRequestId = table.Column<int>(type: "int", nullable: false),
                    TestTypesTestTypeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestRequestsModelTestTypeModel", x => new { x.TestRequestsTestRequestId, x.TestTypesTestTypeId });
                    table.ForeignKey(
                        name: "FK_TestRequestsModelTestTypeModel_TestRequests_TestRequestsTestRequestId",
                        column: x => x.TestRequestsTestRequestId,
                        principalTable: "TestRequests",
                        principalColumn: "TestRequestId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TestRequestsModelTestTypeModel_TestTypes_TestTypesTestTypeId",
                        column: x => x.TestTypesTestTypeId,
                        principalTable: "TestTypes",
                        principalColumn: "TestTypeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Samples",
                keyColumn: "SampleId",
                keyValue: 1,
                column: "DateReceived",
                value: new DateTime(2026, 4, 22, 2, 0, 4, 957, DateTimeKind.Local).AddTicks(5856));

            migrationBuilder.UpdateData(
                table: "Samples",
                keyColumn: "SampleId",
                keyValue: 2,
                column: "DateReceived",
                value: new DateTime(2026, 4, 22, 2, 0, 4, 957, DateTimeKind.Local).AddTicks(5859));

            migrationBuilder.CreateIndex(
                name: "IX_TestRequestsModelTestTypeModel_TestTypesTestTypeId",
                table: "TestRequestsModelTestTypeModel",
                column: "TestTypesTestTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TestRequestsModelTestTypeModel");

            migrationBuilder.CreateTable(
                name: "AssignTestTypes",
                columns: table => new
                {
                    AssignTestId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ManagerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TechnicianId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    TestRequestId = table.Column<int>(type: "int", nullable: false),
                    TestTypeId = table.Column<int>(type: "int", nullable: false),
                    CompletionDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StartDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssignTestTypes", x => x.AssignTestId);
                    table.ForeignKey(
                        name: "FK_AssignTestTypes_AspNetUsers_ManagerId",
                        column: x => x.ManagerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssignTestTypes_AspNetUsers_TechnicianId",
                        column: x => x.TechnicianId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AssignTestTypes_TestRequests_TestRequestId",
                        column: x => x.TestRequestId,
                        principalTable: "TestRequests",
                        principalColumn: "TestRequestId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssignTestTypes_TestTypes_TestTypeId",
                        column: x => x.TestTypeId,
                        principalTable: "TestTypes",
                        principalColumn: "TestTypeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AssignTestTypes",
                columns: new[] { "AssignTestId", "CompletionDateTime", "ManagerId", "StartDateTime", "Status", "TechnicianId", "TestRequestId", "TestTypeId" },
                values: new object[,]
                {
                    { 1, null, "7cfd1034-f3a8-42ed-b13e-61f3b19ed8ec", null, 3, null, 1, 1 },
                    { 2, null, "7cfd1034-f3a8-42ed-b13e-61f3b19ed8ec", new DateTime(2026, 4, 18, 8, 0, 0, 0, DateTimeKind.Unspecified), 1, "bdd52efb-571c-4ea7-9a18-9e9c66c92731", 2, 2 }
                });

            migrationBuilder.UpdateData(
                table: "Samples",
                keyColumn: "SampleId",
                keyValue: 1,
                column: "DateReceived",
                value: new DateTime(2026, 4, 21, 12, 8, 32, 789, DateTimeKind.Local).AddTicks(1602));

            migrationBuilder.UpdateData(
                table: "Samples",
                keyColumn: "SampleId",
                keyValue: 2,
                column: "DateReceived",
                value: new DateTime(2026, 4, 21, 12, 8, 32, 789, DateTimeKind.Local).AddTicks(1607));

            migrationBuilder.CreateIndex(
                name: "IX_AssignTestTypes_ManagerId",
                table: "AssignTestTypes",
                column: "ManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_AssignTestTypes_TechnicianId",
                table: "AssignTestTypes",
                column: "TechnicianId");

            migrationBuilder.CreateIndex(
                name: "IX_AssignTestTypes_TestRequestId",
                table: "AssignTestTypes",
                column: "TestRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_AssignTestTypes_TestTypeId",
                table: "AssignTestTypes",
                column: "TestTypeId");
        }
    }
}
