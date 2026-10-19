using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NMBHLabSys.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSampleBarcodes2026 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Samples",
                keyColumn: "SampleId",
                keyValue: 8101,
                column: "Barcode",
                value: "SMP-2026-008101");

            migrationBuilder.UpdateData(
                table: "Samples",
                keyColumn: "SampleId",
                keyValue: 8102,
                column: "Barcode",
                value: "SMP-2026-008102");

            migrationBuilder.UpdateData(
                table: "Samples",
                keyColumn: "SampleId",
                keyValue: 8103,
                column: "Barcode",
                value: "SMP-2026-008103");

            migrationBuilder.UpdateData(
                table: "Samples",
                keyColumn: "SampleId",
                keyValue: 8104,
                column: "Barcode",
                value: "SMP-2026-008104");

            migrationBuilder.UpdateData(
                table: "Samples",
                keyColumn: "SampleId",
                keyValue: 8201,
                column: "Barcode",
                value: "SMP-2026-008201");

            migrationBuilder.UpdateData(
                table: "Samples",
                keyColumn: "SampleId",
                keyValue: 8202,
                column: "Barcode",
                value: "SMP-2026-008202");

            migrationBuilder.UpdateData(
                table: "Samples",
                keyColumn: "SampleId",
                keyValue: 8203,
                column: "Barcode",
                value: "SMP-2026-008203");

            migrationBuilder.UpdateData(
                table: "Samples",
                keyColumn: "SampleId",
                keyValue: 8204,
                column: "Barcode",
                value: "SMP-2026-008204");

            migrationBuilder.UpdateData(
                table: "Samples",
                keyColumn: "SampleId",
                keyValue: 8301,
                column: "Barcode",
                value: "SMP-2026-008301");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Samples",
                keyColumn: "SampleId",
                keyValue: 8101,
                column: "Barcode",
                value: "DEMO-SMP-8101");

            migrationBuilder.UpdateData(
                table: "Samples",
                keyColumn: "SampleId",
                keyValue: 8102,
                column: "Barcode",
                value: "DEMO-SMP-8102");

            migrationBuilder.UpdateData(
                table: "Samples",
                keyColumn: "SampleId",
                keyValue: 8103,
                column: "Barcode",
                value: "DEMO-SMP-8103");

            migrationBuilder.UpdateData(
                table: "Samples",
                keyColumn: "SampleId",
                keyValue: 8104,
                column: "Barcode",
                value: "DEMO-SMP-8104");

            migrationBuilder.UpdateData(
                table: "Samples",
                keyColumn: "SampleId",
                keyValue: 8201,
                column: "Barcode",
                value: "DEMO-SMP-8201");

            migrationBuilder.UpdateData(
                table: "Samples",
                keyColumn: "SampleId",
                keyValue: 8202,
                column: "Barcode",
                value: "DEMO-SMP-8202");

            migrationBuilder.UpdateData(
                table: "Samples",
                keyColumn: "SampleId",
                keyValue: 8203,
                column: "Barcode",
                value: "DEMO-SMP-8203");

            migrationBuilder.UpdateData(
                table: "Samples",
                keyColumn: "SampleId",
                keyValue: 8204,
                column: "Barcode",
                value: "DEMO-SMP-8204");

            migrationBuilder.UpdateData(
                table: "Samples",
                keyColumn: "SampleId",
                keyValue: 8301,
                column: "Barcode",
                value: "DEMO-SMP-8301");
        }
    }
}
