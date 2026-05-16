using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace NMBHLabSys.Migrations
{
    /// <inheritdoc />
    public partial class updatetechncianviews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Samples",
                columns: new[] { "SampleId", "Barcode", "DateReceived", "IsReceived", "SampleTypeId", "Status", "TechnicianId", "TestRequestId" },
                values: new object[,]
                {
                    { 1, "SMP-2024-000001", new DateTime(2024, 1, 15, 10, 30, 0, 0, DateTimeKind.Unspecified), true, 1, "Received", "6f9e21e3-6ae1-450d-90cf-9d89d3afe07a", 1 },
                    { 2, "SMP-2024-000002", new DateTime(2024, 1, 16, 11, 45, 0, 0, DateTimeKind.Unspecified), true, 2, "Received", "6f9e21e3-6ae1-450d-90cf-9d89d3afe07a", 1 },
                    { 3, "SMP-2024-000003", new DateTime(2024, 1, 17, 9, 15, 0, 0, DateTimeKind.Unspecified), true, 3, "Received", "6f9e21e3-6ae1-450d-90cf-9d89d3afe07a", 2 },
                    { 4, "SMP-2024-000004", new DateTime(2024, 1, 18, 14, 20, 0, 0, DateTimeKind.Unspecified), false, 1, "Pending", null, 2 },
                    { 5, "SMP-2024-000005", new DateTime(2024, 1, 19, 8, 0, 0, 0, DateTimeKind.Unspecified), true, 4, "Received", "6f9e21e3-6ae1-450d-90cf-9d89d3afe07a", 3 },
                    { 6, "SMP-2024-000006", new DateTime(2024, 1, 20, 13, 30, 0, 0, DateTimeKind.Unspecified), false, 5, "Pending", null, 3 },
                    { 9, "SMP-2024-000009", new DateTime(2024, 1, 23, 11, 20, 0, 0, DateTimeKind.Unspecified), false, 3, "Pending", null, 5 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Samples",
                keyColumn: "SampleId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Samples",
                keyColumn: "SampleId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Samples",
                keyColumn: "SampleId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Samples",
                keyColumn: "SampleId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Samples",
                keyColumn: "SampleId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Samples",
                keyColumn: "SampleId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Samples",
                keyColumn: "SampleId",
                keyValue: 9);
        }
    }
}
