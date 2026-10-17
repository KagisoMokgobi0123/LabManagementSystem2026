using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace NMBHLabSys.Migrations
{
    /// <inheritdoc />
    public partial class SeedReviewQueueTests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "TestRequestTestTypes",
                columns: new[] { "TestRequestTestTypeId", "CompletionDateTime", "IsAbnormal", "ResultNotes", "ResultValue", "StartDateTime", "Status", "TechnicianId", "TestRequestId", "TestTypeId" },
                values: new object[,]
                {
                    { 1001, new DateTime(2026, 10, 14, 10, 0, 0, 0, DateTimeKind.Unspecified), false, null, null, new DateTime(2026, 10, 14, 8, 30, 0, 0, DateTimeKind.Unspecified), "Completed", "63950297-85b0-4b81-934a-bad2bf77f3cb", 4, 3 },
                    { 1002, new DateTime(2026, 10, 14, 11, 15, 0, 0, DateTimeKind.Unspecified), false, null, null, new DateTime(2026, 10, 14, 9, 0, 0, 0, DateTimeKind.Unspecified), "Completed", "85a94ac3-e56a-4ae2-bea1-85d398486185", 8, 3 },
                    { 1003, new DateTime(2026, 10, 13, 9, 30, 0, 0, DateTimeKind.Unspecified), false, null, null, new DateTime(2026, 10, 13, 8, 0, 0, 0, DateTimeKind.Unspecified), "To Be Reviewed", "63950297-85b0-4b81-934a-bad2bf77f3cb", 3, 3 },
                    { 1004, new DateTime(2026, 10, 13, 12, 0, 0, 0, DateTimeKind.Unspecified), false, null, null, new DateTime(2026, 10, 13, 10, 0, 0, 0, DateTimeKind.Unspecified), "To Be Reviewed", "85a94ac3-e56a-4ae2-bea1-85d398486185", 6, 3 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1001);

            migrationBuilder.DeleteData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1002);

            migrationBuilder.DeleteData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1003);

            migrationBuilder.DeleteData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1004);
        }
    }
}
