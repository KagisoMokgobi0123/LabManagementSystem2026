using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace NMBHLabSys.Migrations
{
    /// <inheritdoc />
    public partial class AddFiveNumericResultsPreserveHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "TestResults",
                columns: new[] { "TestResultId", "CapturedDateTime", "IsAbnormal", "MeasurementUnit", "NewIsAbnormal", "NewMeasurementUnit", "NewReferenceMax", "NewReferenceMin", "NewResultFlag", "NewResultNotes", "Notes", "PreviousIsAbnormal", "PreviousMeasurementUnit", "PreviousReferenceMax", "PreviousReferenceMin", "PreviousResultFlag", "PreviousResultNotes", "ReferenceMax", "ReferenceMin", "RequiresReview", "ResultFlag", "ResultName", "ResultValue", "ReviewNotes", "TechnicianId", "TestRequestTestTypeId", "TestTypeMeasurementId", "VerificationNotes", "VerifiedByTechnicianId", "VerifiedDateTime" },
                values: new object[,]
                {
                    { 2310, new DateTime(2026, 10, 2, 10, 45, 0, 0, DateTimeKind.Utc), false, "seconds", null, null, null, null, null, null, "Synthetic teaching-case measurement; not for clinical use.", null, null, null, null, null, null, 13.5m, 11.0m, false, "Normal", "Prothrombin Time (PT)", "12.4", null, "63950297-85b0-4b81-934a-bad2bf77f3cb", 1302, null, null, "85a94ac3-e56a-4ae2-bea1-85d398486185", new DateTime(2026, 10, 2, 11, 15, 0, 0, DateTimeKind.Utc) },
                    { 2311, new DateTime(2026, 10, 7, 10, 45, 0, 0, DateTimeKind.Utc), true, "seconds", null, null, null, null, null, null, "Synthetic teaching-case measurement; not for clinical use.", null, null, null, null, null, null, 35m, 25m, false, "High", "Activated Partial Thromboplastin Time (aPTT)", "42.0", null, "63950297-85b0-4b81-934a-bad2bf77f3cb", 1307, null, null, "85a94ac3-e56a-4ae2-bea1-85d398486185", new DateTime(2026, 10, 7, 11, 15, 0, 0, DateTimeKind.Utc) },
                    { 2312, new DateTime(2026, 10, 8, 10, 45, 0, 0, DateTimeKind.Utc), false, "%", null, null, null, null, null, null, "Synthetic teaching-case measurement; not for clinical use.", null, null, null, null, null, null, 5.6m, 4.0m, false, "Normal", "Hemoglobin A1C (HbA1c)", "5.3", null, "63950297-85b0-4b81-934a-bad2bf77f3cb", 1308, null, null, "85a94ac3-e56a-4ae2-bea1-85d398486185", new DateTime(2026, 10, 8, 11, 15, 0, 0, DateTimeKind.Utc) },
                    { 2313, new DateTime(2026, 10, 9, 10, 45, 0, 0, DateTimeKind.Utc), true, "x10³/μL", null, null, null, null, null, null, "Synthetic teaching-case measurement; not for clinical use.", null, null, null, null, null, null, 11.0m, 4.5m, false, "Low", "White Blood Cell Count", "3.2", null, "63950297-85b0-4b81-934a-bad2bf77f3cb", 1309, null, null, "85a94ac3-e56a-4ae2-bea1-85d398486185", new DateTime(2026, 10, 9, 11, 15, 0, 0, DateTimeKind.Utc) },
                    { 2314, new DateTime(2026, 10, 10, 10, 45, 0, 0, DateTimeKind.Utc), true, "seconds", null, null, null, null, null, null, "Synthetic teaching-case measurement; not for clinical use.", null, null, null, null, null, null, 13.5m, 11.0m, false, "High", "Prothrombin Time (PT)", "16.8", null, "63950297-85b0-4b81-934a-bad2bf77f3cb", 1310, null, null, "85a94ac3-e56a-4ae2-bea1-85d398486185", new DateTime(2026, 10, 10, 11, 15, 0, 0, DateTimeKind.Utc) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "TestResults",
                keyColumn: "TestResultId",
                keyValue: 2310);

            migrationBuilder.DeleteData(
                table: "TestResults",
                keyColumn: "TestResultId",
                keyValue: 2311);

            migrationBuilder.DeleteData(
                table: "TestResults",
                keyColumn: "TestResultId",
                keyValue: 2312);

            migrationBuilder.DeleteData(
                table: "TestResults",
                keyColumn: "TestResultId",
                keyValue: 2313);

            migrationBuilder.DeleteData(
                table: "TestResults",
                keyColumn: "TestResultId",
                keyValue: 2314);
        }
    }
}
