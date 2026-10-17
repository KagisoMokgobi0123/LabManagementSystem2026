using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace NMBHLabSys.Migrations
{
    /// <inheritdoc />
    public partial class SeedReviewQueueResultsAndHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "TestResults",
                columns: new[] { "TestResultId", "CapturedDateTime", "IsAbnormal", "MeasurementUnit", "NewIsAbnormal", "NewMeasurementUnit", "NewReferenceMax", "NewReferenceMin", "NewResultFlag", "NewResultNotes", "Notes", "PreviousIsAbnormal", "PreviousMeasurementUnit", "PreviousReferenceMax", "PreviousReferenceMin", "PreviousResultFlag", "PreviousResultNotes", "ReferenceMax", "ReferenceMin", "RequiresReview", "ResultFlag", "ResultName", "ResultValue", "ReviewNotes", "TechnicianId", "TestRequestTestTypeId", "TestTypeMeasurementId", "VerificationNotes", "VerifiedByTechnicianId", "VerifiedDateTime" },
                values: new object[,]
                {
                    { 2001, new DateTime(2026, 10, 14, 9, 50, 0, 0, DateTimeKind.Utc), false, "Qualitative", null, null, null, null, null, null, "DEMO DATA ONLY - not for clinical use.", null, null, null, null, null, null, null, null, false, "Not Evaluated", "Peripheral Blood Film", "No abnormal cells seen", null, "63950297-85b0-4b81-934a-bad2bf77f3cb", 1001, null, null, null, null },
                    { 2002, new DateTime(2026, 10, 14, 11, 0, 0, 0, DateTimeKind.Utc), false, "Qualitative", null, null, null, null, null, null, "DEMO DATA ONLY - not for clinical use.", null, null, null, null, null, null, null, null, false, "Not Evaluated", "Peripheral Blood Film", "No abnormal cells seen", null, "85a94ac3-e56a-4ae2-bea1-85d398486185", 1002, null, null, null, null },
                    { 2003, new DateTime(2026, 10, 13, 9, 15, 0, 0, DateTimeKind.Utc), false, "Qualitative", null, null, null, null, null, null, "DEMO DATA ONLY - not for clinical use.", null, null, null, null, null, null, null, null, true, "Not Evaluated", "Peripheral Blood Film", "Film quality inadequate", "Please prepare a new blood film, review the morphology, and correct the recorded finding before resubmission.", "63950297-85b0-4b81-934a-bad2bf77f3cb", 1003, null, null, null, null },
                    { 2004, new DateTime(2026, 10, 13, 11, 45, 0, 0, DateTimeKind.Utc), false, "Qualitative", null, null, null, null, null, null, "DEMO DATA ONLY - not for clinical use.", null, null, null, null, null, null, null, null, true, "Not Evaluated", "Peripheral Blood Film", "Cell morphology unclear", "Please repeat the film examination and provide a clear qualitative interpretation before resubmission.", "85a94ac3-e56a-4ae2-bea1-85d398486185", 1004, null, null, null, null }
                });

            migrationBuilder.InsertData(
                table: "TestResultVerificationHistories",
                columns: new[] { "TestResultVerificationHistoryId", "Action", "ActionByTechnicianId", "ActionDateTime", "NewIsAbnormal", "NewMeasurementUnit", "NewReferenceMax", "NewReferenceMin", "NewResultFlag", "NewResultNotes", "NewResultValue", "Notes", "PreviousIsAbnormal", "PreviousMeasurementUnit", "PreviousReferenceMax", "PreviousReferenceMin", "PreviousResultFlag", "PreviousResultNotes", "PreviousResultValue", "TestResultId" },
                values: new object[,]
                {
                    { 3001, "SubmittedForVerification", "63950297-85b0-4b81-934a-bad2bf77f3cb", new DateTime(2026, 10, 14, 10, 0, 0, 0, DateTimeKind.Utc), null, null, null, null, null, null, null, "Test submitted for independent verification.", null, null, null, null, null, null, null, 2001 },
                    { 3002, "SubmittedForVerification", "85a94ac3-e56a-4ae2-bea1-85d398486185", new DateTime(2026, 10, 14, 11, 15, 0, 0, DateTimeKind.Utc), null, null, null, null, null, null, null, "Test submitted for independent verification.", null, null, null, null, null, null, null, 2002 },
                    { 3003, "ReturnedForReview", "85a94ac3-e56a-4ae2-bea1-85d398486185", new DateTime(2026, 10, 13, 9, 40, 0, 0, DateTimeKind.Utc), null, null, null, null, null, null, null, "Please prepare a new blood film, review the morphology, and correct the recorded finding before resubmission.", null, null, null, null, null, null, null, 2003 },
                    { 3004, "ReturnedForReview", "63950297-85b0-4b81-934a-bad2bf77f3cb", new DateTime(2026, 10, 13, 12, 10, 0, 0, DateTimeKind.Utc), null, null, null, null, null, null, null, "Please repeat the film examination and provide a clear qualitative interpretation before resubmission.", null, null, null, null, null, null, null, 2004 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "TestResultVerificationHistories",
                keyColumn: "TestResultVerificationHistoryId",
                keyValue: 3001);

            migrationBuilder.DeleteData(
                table: "TestResultVerificationHistories",
                keyColumn: "TestResultVerificationHistoryId",
                keyValue: 3002);

            migrationBuilder.DeleteData(
                table: "TestResultVerificationHistories",
                keyColumn: "TestResultVerificationHistoryId",
                keyValue: 3003);

            migrationBuilder.DeleteData(
                table: "TestResultVerificationHistories",
                keyColumn: "TestResultVerificationHistoryId",
                keyValue: 3004);

            migrationBuilder.DeleteData(
                table: "TestResults",
                keyColumn: "TestResultId",
                keyValue: 2001);

            migrationBuilder.DeleteData(
                table: "TestResults",
                keyColumn: "TestResultId",
                keyValue: 2002);

            migrationBuilder.DeleteData(
                table: "TestResults",
                keyColumn: "TestResultId",
                keyValue: 2003);

            migrationBuilder.DeleteData(
                table: "TestResults",
                keyColumn: "TestResultId",
                keyValue: 2004);
        }
    }
}
