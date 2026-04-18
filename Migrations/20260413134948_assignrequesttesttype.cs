using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace NMBHLabSys.Migrations
{
    /// <inheritdoc />
    public partial class assignrequesttesttype : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AssignTestTypes",
                columns: new[] { "AssignTestId", "CompletionDateTime", "StartDateTime", "Status", "TestRequestId", "TestTypeId" },
                values: new object[,]
                {
                    { 1, null, null, 0, 1, 1 },
                    { 2, null, null, 0, 2, 2 }
                });

            migrationBuilder.UpdateData(
                table: "Samples",
                keyColumn: "SampleId",
                keyValue: 1,
                column: "DateReceived",
                value: new DateTime(2026, 4, 13, 15, 49, 47, 591, DateTimeKind.Local).AddTicks(6372));

            migrationBuilder.UpdateData(
                table: "Samples",
                keyColumn: "SampleId",
                keyValue: 2,
                column: "DateReceived",
                value: new DateTime(2026, 4, 13, 15, 49, 47, 591, DateTimeKind.Local).AddTicks(6375));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AssignTestTypes",
                keyColumn: "AssignTestId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "AssignTestTypes",
                keyColumn: "AssignTestId",
                keyValue: 2);

            migrationBuilder.UpdateData(
                table: "Samples",
                keyColumn: "SampleId",
                keyValue: 1,
                column: "DateReceived",
                value: new DateTime(2026, 4, 13, 15, 44, 4, 767, DateTimeKind.Local).AddTicks(9270));

            migrationBuilder.UpdateData(
                table: "Samples",
                keyColumn: "SampleId",
                keyValue: 2,
                column: "DateReceived",
                value: new DateTime(2026, 4, 13, 15, 44, 4, 767, DateTimeKind.Local).AddTicks(9272));
        }
    }
}
