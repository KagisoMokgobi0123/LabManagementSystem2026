using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace NMBHLabSys.Migrations
{
    /// <inheritdoc />
    public partial class SeedTestRequestTestTypeModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "TestRequestTestTypes",
                columns: new[] { "TestRequestTestTypeId", "CompletionDateTime", "StartDateTime", "Status", "TechnicianId", "TestRequestId", "TestTypeId" },
                values: new object[,]
                {
                    { 1, null, null, "Submitted", null, 1, 1 },
                    { 2, null, null, "SamplesReceived", null, 2, 1 },
                    { 3, null, null, "SamplesReceived", null, 2, 2 },
                    { 4, null, null, "SamplesReceived", null, 3, 3 },
                    { 5, null, null, "Submitted", null, 5, 6 },
                    { 6, null, null, "SamplesReceived", null, 6, 3 },
                    { 7, null, null, "SamplesReceived", null, 9, 3 },
                    { 8, null, null, "Submitted", null, 10, 1 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 8);
        }
    }
}
