using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace NMBHLabSys.Migrations
{
    /// <inheritdoc />
    public partial class updateapplicationDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClinicalNotes_TestRequests_TestRequestsModelTestRequestId",
                table: "ClinicalNotes");

            migrationBuilder.DropForeignKey(
                name: "FK_TestTypes_AspNetUsers_TechnicianId",
                table: "TestTypes");

            migrationBuilder.DropIndex(
                name: "IX_TestTypes_TechnicianId",
                table: "TestTypes");

            migrationBuilder.DropIndex(
                name: "IX_ClinicalNotes_TestRequestsModelTestRequestId",
                table: "ClinicalNotes");

            migrationBuilder.DropColumn(
                name: "TechnicianId",
                table: "TestTypes");

            migrationBuilder.DropColumn(
                name: "TestRequestsModelTestRequestId",
                table: "ClinicalNotes");

            migrationBuilder.AddColumn<string>(
                name: "ClinicalNotes",
                table: "TestRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Urgency",
                table: "TestRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "TestRequests",
                keyColumn: "TestRequestId",
                keyValue: 1,
                columns: new[] { "ClinicalNotes", "Urgency" },
                values: new object[] { "Patient complaining of persistent fatigue. Check for anemia.", "Routine" });

            migrationBuilder.UpdateData(
                table: "TestRequests",
                keyColumn: "TestRequestId",
                keyValue: 2,
                columns: new[] { "ClinicalNotes", "Urgency" },
                values: new object[] { "Suspected Malaria. High fever and chills.", "Stat" });

            migrationBuilder.InsertData(
                table: "TestTypes",
                columns: new[] { "TestTypeId", "Description", "TurnAroundTime" },
                values: new object[,]
                {
                    { 1, "Full Blood Count (FBC)", new DateTime(2026, 1, 1, 4, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, "Liver Function Test (LFT)", new DateTime(2026, 1, 1, 8, 0, 0, 0, DateTimeKind.Unspecified) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 2);

            migrationBuilder.DropColumn(
                name: "ClinicalNotes",
                table: "TestRequests");

            migrationBuilder.DropColumn(
                name: "Urgency",
                table: "TestRequests");

            migrationBuilder.AddColumn<string>(
                name: "TechnicianId",
                table: "TestTypes",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "TestRequestsModelTestRequestId",
                table: "ClinicalNotes",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TestTypes_TechnicianId",
                table: "TestTypes",
                column: "TechnicianId");

            migrationBuilder.CreateIndex(
                name: "IX_ClinicalNotes_TestRequestsModelTestRequestId",
                table: "ClinicalNotes",
                column: "TestRequestsModelTestRequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClinicalNotes_TestRequests_TestRequestsModelTestRequestId",
                table: "ClinicalNotes",
                column: "TestRequestsModelTestRequestId",
                principalTable: "TestRequests",
                principalColumn: "TestRequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_TestTypes_AspNetUsers_TechnicianId",
                table: "TestTypes",
                column: "TechnicianId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
