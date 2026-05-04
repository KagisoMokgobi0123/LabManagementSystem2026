using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace NMBHLabSys.Migrations
{
    /// <inheritdoc />
    public partial class seedsamplemodel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Samples_AspNetUsers_TechnicianId",
                table: "Samples");

            migrationBuilder.DropIndex(
                name: "IX_Samples_TechnicianId",
                table: "Samples");

            migrationBuilder.DropColumn(
                name: "TechnicianId",
                table: "Samples");

            migrationBuilder.InsertData(
                table: "Samples",
                columns: new[] { "SampleId", "Barcode", "DateReceived", "SampleType", "TestRequestId" },
                values: new object[,]
                {
                    { 1, "NMB-2026-0001", new DateTime(2026, 4, 13, 15, 44, 4, 767, DateTimeKind.Local).AddTicks(9270), "Whole Blood", 1 },
                    { 2, "NMB-2026-0002", new DateTime(2026, 4, 13, 15, 44, 4, 767, DateTimeKind.Local).AddTicks(9272), "Blood Film", 2 }
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

            migrationBuilder.AddColumn<string>(
                name: "TechnicianId",
                table: "Samples",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Samples_TechnicianId",
                table: "Samples",
                column: "TechnicianId");

            migrationBuilder.AddForeignKey(
                name: "FK_Samples_AspNetUsers_TechnicianId",
                table: "Samples",
                column: "TechnicianId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }
    }
}
