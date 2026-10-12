using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NMBHLabSys.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDecimalWarning : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestResults_TestTypeMeasurements_TestTypeMeasurementId",
                table: "TestResults");

            migrationBuilder.DropIndex(
                name: "IX_TestResults_TestRequestTestTypeId_TestTypeMeasurementId",
                table: "TestResults");

            migrationBuilder.AlterColumn<decimal>(
                name: "PreviousReferenceMin",
                table: "TestResultVerificationHistories",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "PreviousReferenceMax",
                table: "TestResultVerificationHistories",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "NewReferenceMin",
                table: "TestResultVerificationHistories",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "NewReferenceMax",
                table: "TestResultVerificationHistories",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "PreviousReferenceMin",
                table: "TestResults",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "PreviousReferenceMax",
                table: "TestResults",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "NewReferenceMin",
                table: "TestResults",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "NewReferenceMax",
                table: "TestResults",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TestResults_TestRequestTestTypeId",
                table: "TestResults",
                column: "TestRequestTestTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_TestResults_TestTypeMeasurements_TestTypeMeasurementId",
                table: "TestResults",
                column: "TestTypeMeasurementId",
                principalTable: "TestTypeMeasurements",
                principalColumn: "TestTypeMeasurementId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestResults_TestTypeMeasurements_TestTypeMeasurementId",
                table: "TestResults");

            migrationBuilder.DropIndex(
                name: "IX_TestResults_TestRequestTestTypeId",
                table: "TestResults");

            migrationBuilder.AlterColumn<decimal>(
                name: "PreviousReferenceMin",
                table: "TestResultVerificationHistories",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)",
                oldPrecision: 18,
                oldScale: 4,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "PreviousReferenceMax",
                table: "TestResultVerificationHistories",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)",
                oldPrecision: 18,
                oldScale: 4,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "NewReferenceMin",
                table: "TestResultVerificationHistories",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)",
                oldPrecision: 18,
                oldScale: 4,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "NewReferenceMax",
                table: "TestResultVerificationHistories",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)",
                oldPrecision: 18,
                oldScale: 4,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "PreviousReferenceMin",
                table: "TestResults",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)",
                oldPrecision: 18,
                oldScale: 4,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "PreviousReferenceMax",
                table: "TestResults",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)",
                oldPrecision: 18,
                oldScale: 4,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "NewReferenceMin",
                table: "TestResults",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)",
                oldPrecision: 18,
                oldScale: 4,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "NewReferenceMax",
                table: "TestResults",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)",
                oldPrecision: 18,
                oldScale: 4,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TestResults_TestRequestTestTypeId_TestTypeMeasurementId",
                table: "TestResults",
                columns: new[] { "TestRequestTestTypeId", "TestTypeMeasurementId" },
                unique: true,
                filter: "[TestTypeMeasurementId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_TestResults_TestTypeMeasurements_TestTypeMeasurementId",
                table: "TestResults",
                column: "TestTypeMeasurementId",
                principalTable: "TestTypeMeasurements",
                principalColumn: "TestTypeMeasurementId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
