using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace NMBHLabSys.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDoctorTechnicianManagerTbls : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SampleType",
                table: "Samples");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "TestTypes",
                newName: "TestName");

            migrationBuilder.AddColumn<int>(
                name: "SampleTypeId",
                table: "Samples",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "SampleTypes",
                columns: table => new
                {
                    SampleTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SampleName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SampleTypes", x => x.SampleTypeId);
                });

            migrationBuilder.InsertData(
                table: "SampleTypes",
                columns: new[] { "SampleTypeId", "SampleName" },
                values: new object[,]
                {
                    { 1, "Blood" },
                    { 2, "Urine" },
                    { 3, "Saliva" },
                    { 4, "Swab" }
                });

            migrationBuilder.UpdateData(
                table: "Samples",
                keyColumn: "SampleId",
                keyValue: 1,
                columns: new[] { "DateReceived", "SampleTypeId" },
                values: new object[] { new DateTime(2026, 4, 21, 12, 8, 32, 789, DateTimeKind.Local).AddTicks(1602), 1 });

            migrationBuilder.UpdateData(
                table: "Samples",
                keyColumn: "SampleId",
                keyValue: 2,
                columns: new[] { "DateReceived", "SampleTypeId" },
                values: new object[] { new DateTime(2026, 4, 21, 12, 8, 32, 789, DateTimeKind.Local).AddTicks(1607), 4 });

            migrationBuilder.CreateIndex(
                name: "IX_Samples_SampleTypeId",
                table: "Samples",
                column: "SampleTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Samples_SampleTypes_SampleTypeId",
                table: "Samples",
                column: "SampleTypeId",
                principalTable: "SampleTypes",
                principalColumn: "SampleTypeId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Samples_SampleTypes_SampleTypeId",
                table: "Samples");

            migrationBuilder.DropTable(
                name: "SampleTypes");

            migrationBuilder.DropIndex(
                name: "IX_Samples_SampleTypeId",
                table: "Samples");

            migrationBuilder.DropColumn(
                name: "SampleTypeId",
                table: "Samples");

            migrationBuilder.RenameColumn(
                name: "TestName",
                table: "TestTypes",
                newName: "Description");

            migrationBuilder.AddColumn<string>(
                name: "SampleType",
                table: "Samples",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Samples",
                keyColumn: "SampleId",
                keyValue: 1,
                columns: new[] { "DateReceived", "SampleType" },
                values: new object[] { new DateTime(2026, 4, 18, 2, 52, 35, 589, DateTimeKind.Local).AddTicks(173), "Whole Blood" });

            migrationBuilder.UpdateData(
                table: "Samples",
                keyColumn: "SampleId",
                keyValue: 2,
                columns: new[] { "DateReceived", "SampleType" },
                values: new object[] { new DateTime(2026, 4, 18, 2, 52, 35, 589, DateTimeKind.Local).AddTicks(175), "Blood Film" });
        }
    }
}
