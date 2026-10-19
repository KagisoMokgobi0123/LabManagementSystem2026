using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace NMBHLabSys.Migrations
{
    /// <inheritdoc />
    public partial class AddTechnicianReportDemoData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AllergyModel",
                columns: new[] { "AllergyId", "AllergyCategoryId", "AllergyName", "Description" },
                values: new object[,]
                {
                    { 1, 1, "Penicillin", "DEMO DATA ONLY" },
                    { 2, 1, "Peanuts", "DEMO DATA ONLY" }
                });

            migrationBuilder.InsertData(
                table: "MedicalConditionCategories",
                columns: new[] { "MedicalConditionCategoryId", "Description", "Name", "Status" },
                values: new object[] { 9001, "DEMO DATA ONLY - laboratory assessment", "Demo chronic conditions", 1 });

            migrationBuilder.InsertData(
                table: "MedicationModel",
                columns: new[] { "MedicationId", "Description", "MedicationCategoryId", "MedicationName" },
                values: new object[,]
                {
                    { 1, "DEMO DATA ONLY", 1, "Metformin" },
                    { 2, "DEMO DATA ONLY", 1, "Amlodipine" }
                });

            migrationBuilder.InsertData(
                table: "Patients",
                columns: new[] { "PatientId", "Address", "City", "ContactNumber", "CreatedDate", "DateOfBirth", "Email", "IDNumber", "LastUpdatedDate", "Name", "NoCurrentMedications", "NoKnownAllergies", "NoKnownMedicalConditions", "PostalCode", "Province", "Suburb", "Surname", "UserId" },
                values: new object[] { "f71c5f4a-6a9e-4c8d-91b2-7e6d2f304a15", "Demo Address", "Gqeberha", "0710000000", new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "lerato.demo@example.com", "0001015009087", null, "Lerato", false, false, false, "6001", "Eastern Cape", "Central", "Patient", "c0fbfbb4-daf5-44e3-bf57-acfded7baceb" });

            migrationBuilder.InsertData(
                table: "Samples",
                columns: new[] { "SampleId", "Barcode", "DateReceived", "IsDeleted", "IsReceived", "SampleTypeId", "Status", "TechnicianId", "TestRequestId" },
                values: new object[,]
                {
                    { 8101, "DEMO-SMP-8101", null, false, false, 1, "Submitted", null, 1 },
                    { 8102, "DEMO-SMP-8102", null, false, false, 4, "Submitted", null, 5 },
                    { 8103, "DEMO-SMP-8103", null, false, false, 4, "Submitted", null, 7 },
                    { 8104, "DEMO-SMP-8104", null, false, false, 1, "Submitted", null, 10 }
                });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 2,
                column: "Status",
                value: "Submitted");

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 3,
                column: "Status",
                value: "Submitted");

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 4,
                column: "Status",
                value: "Submitted");

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 6,
                column: "Status",
                value: "Submitted");

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 7,
                column: "Status",
                value: "Submitted");

            migrationBuilder.InsertData(
                table: "TestRequestTestTypes",
                columns: new[] { "TestRequestTestTypeId", "CompletionDateTime", "IsAbnormal", "ResultNotes", "ResultValue", "StartDateTime", "Status", "TechnicianId", "TestRequestId", "TestTypeId" },
                values: new object[,]
                {
                    { 1101, null, false, null, null, new DateTime(2026, 10, 15, 8, 0, 0, 0, DateTimeKind.Unspecified), "In Progress", "63950297-85b0-4b81-934a-bad2bf77f3cb", 2, 1 },
                    { 1102, null, false, null, null, new DateTime(2026, 10, 15, 9, 0, 0, 0, DateTimeKind.Unspecified), "In Progress", "63950297-85b0-4b81-934a-bad2bf77f3cb", 2, 2 },
                    { 1103, null, false, null, null, new DateTime(2026, 10, 15, 10, 0, 0, 0, DateTimeKind.Unspecified), "In Progress", "63950297-85b0-4b81-934a-bad2bf77f3cb", 3, 3 },
                    { 1104, null, false, null, null, new DateTime(2026, 10, 15, 11, 0, 0, 0, DateTimeKind.Unspecified), "In Progress", "63950297-85b0-4b81-934a-bad2bf77f3cb", 6, 3 },
                    { 1105, null, false, null, null, new DateTime(2026, 10, 15, 12, 0, 0, 0, DateTimeKind.Unspecified), "In Progress", "63950297-85b0-4b81-934a-bad2bf77f3cb", 9, 3 },
                    { 1106, new DateTime(2026, 10, 14, 9, 0, 0, 0, DateTimeKind.Unspecified), false, null, null, new DateTime(2026, 10, 14, 8, 0, 0, 0, DateTimeKind.Unspecified), "Completed", "85a94ac3-e56a-4ae2-bea1-85d398486185", 8, 3 },
                    { 1107, new DateTime(2026, 10, 14, 9, 0, 0, 0, DateTimeKind.Unspecified), false, null, null, new DateTime(2026, 10, 14, 8, 0, 0, 0, DateTimeKind.Unspecified), "To Be Reviewed", "63950297-85b0-4b81-934a-bad2bf77f3cb", 3, 3 },
                    { 1201, new DateTime(2026, 10, 1, 9, 0, 0, 0, DateTimeKind.Unspecified), false, null, null, new DateTime(2026, 10, 1, 8, 0, 0, 0, DateTimeKind.Unspecified), "Verified", "63950297-85b0-4b81-934a-bad2bf77f3cb", 4, 3 },
                    { 1202, new DateTime(2026, 10, 2, 9, 0, 0, 0, DateTimeKind.Unspecified), false, null, null, new DateTime(2026, 10, 2, 8, 0, 0, 0, DateTimeKind.Unspecified), "Verified", "63950297-85b0-4b81-934a-bad2bf77f3cb", 4, 3 },
                    { 1203, new DateTime(2026, 10, 3, 9, 0, 0, 0, DateTimeKind.Unspecified), false, null, null, new DateTime(2026, 10, 3, 8, 0, 0, 0, DateTimeKind.Unspecified), "Verified", "63950297-85b0-4b81-934a-bad2bf77f3cb", 4, 3 },
                    { 1204, new DateTime(2026, 10, 4, 9, 0, 0, 0, DateTimeKind.Unspecified), false, null, null, new DateTime(2026, 10, 4, 8, 0, 0, 0, DateTimeKind.Unspecified), "Verified", "63950297-85b0-4b81-934a-bad2bf77f3cb", 4, 3 },
                    { 1205, new DateTime(2026, 10, 5, 9, 0, 0, 0, DateTimeKind.Unspecified), false, null, null, new DateTime(2026, 10, 5, 8, 0, 0, 0, DateTimeKind.Unspecified), "Verified", "63950297-85b0-4b81-934a-bad2bf77f3cb", 4, 3 },
                    { 1206, new DateTime(2026, 10, 6, 9, 0, 0, 0, DateTimeKind.Unspecified), false, null, null, new DateTime(2026, 10, 6, 8, 0, 0, 0, DateTimeKind.Unspecified), "Verified", "63950297-85b0-4b81-934a-bad2bf77f3cb", 4, 3 },
                    { 1207, new DateTime(2026, 10, 7, 9, 0, 0, 0, DateTimeKind.Unspecified), false, null, null, new DateTime(2026, 10, 7, 8, 0, 0, 0, DateTimeKind.Unspecified), "Verified", "63950297-85b0-4b81-934a-bad2bf77f3cb", 4, 3 },
                    { 1208, new DateTime(2026, 10, 8, 9, 0, 0, 0, DateTimeKind.Unspecified), false, null, null, new DateTime(2026, 10, 8, 8, 0, 0, 0, DateTimeKind.Unspecified), "Verified", "63950297-85b0-4b81-934a-bad2bf77f3cb", 4, 3 },
                    { 1209, new DateTime(2026, 10, 9, 9, 0, 0, 0, DateTimeKind.Unspecified), false, null, null, new DateTime(2026, 10, 9, 8, 0, 0, 0, DateTimeKind.Unspecified), "Verified", "63950297-85b0-4b81-934a-bad2bf77f3cb", 4, 3 },
                    { 1210, new DateTime(2026, 10, 10, 9, 0, 0, 0, DateTimeKind.Unspecified), false, null, null, new DateTime(2026, 10, 10, 8, 0, 0, 0, DateTimeKind.Unspecified), "Verified", "63950297-85b0-4b81-934a-bad2bf77f3cb", 4, 3 },
                    { 1211, new DateTime(2026, 10, 11, 9, 0, 0, 0, DateTimeKind.Unspecified), false, null, null, new DateTime(2026, 10, 11, 8, 0, 0, 0, DateTimeKind.Unspecified), "Verified", "63950297-85b0-4b81-934a-bad2bf77f3cb", 4, 3 },
                    { 1212, new DateTime(2026, 10, 12, 9, 0, 0, 0, DateTimeKind.Unspecified), false, null, null, new DateTime(2026, 10, 12, 8, 0, 0, 0, DateTimeKind.Unspecified), "Verified", "63950297-85b0-4b81-934a-bad2bf77f3cb", 4, 3 },
                    { 1213, new DateTime(2026, 10, 13, 9, 0, 0, 0, DateTimeKind.Unspecified), false, null, null, new DateTime(2026, 10, 13, 8, 0, 0, 0, DateTimeKind.Unspecified), "Verified", "63950297-85b0-4b81-934a-bad2bf77f3cb", 4, 3 },
                    { 1214, new DateTime(2026, 10, 14, 9, 0, 0, 0, DateTimeKind.Unspecified), false, null, null, new DateTime(2026, 10, 14, 8, 0, 0, 0, DateTimeKind.Unspecified), "Verified", "63950297-85b0-4b81-934a-bad2bf77f3cb", 4, 3 },
                    { 1301, new DateTime(2026, 10, 1, 11, 0, 0, 0, DateTimeKind.Utc), false, null, null, new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc), "Verified", "63950297-85b0-4b81-934a-bad2bf77f3cb", 4, 1 },
                    { 1302, new DateTime(2026, 10, 2, 11, 0, 0, 0, DateTimeKind.Utc), false, null, null, new DateTime(2026, 10, 2, 10, 0, 0, 0, DateTimeKind.Utc), "Verified", "63950297-85b0-4b81-934a-bad2bf77f3cb", 8, 6 },
                    { 1303, new DateTime(2026, 10, 3, 11, 0, 0, 0, DateTimeKind.Utc), false, null, null, new DateTime(2026, 10, 3, 10, 0, 0, 0, DateTimeKind.Utc), "Verified", "63950297-85b0-4b81-934a-bad2bf77f3cb", 2, 11 },
                    { 1304, new DateTime(2026, 10, 4, 11, 0, 0, 0, DateTimeKind.Utc), false, null, null, new DateTime(2026, 10, 4, 10, 0, 0, 0, DateTimeKind.Utc), "Verified", "63950297-85b0-4b81-934a-bad2bf77f3cb", 6, 17 },
                    { 1305, new DateTime(2026, 10, 5, 11, 0, 0, 0, DateTimeKind.Utc), false, null, null, new DateTime(2026, 10, 5, 10, 0, 0, 0, DateTimeKind.Utc), "Verified", "63950297-85b0-4b81-934a-bad2bf77f3cb", 9, 20 },
                    { 1306, new DateTime(2026, 10, 6, 11, 0, 0, 0, DateTimeKind.Utc), false, null, null, new DateTime(2026, 10, 6, 10, 0, 0, 0, DateTimeKind.Utc), "Verified", "63950297-85b0-4b81-934a-bad2bf77f3cb", 4, 2 },
                    { 1307, new DateTime(2026, 10, 7, 11, 0, 0, 0, DateTimeKind.Utc), false, null, null, new DateTime(2026, 10, 7, 10, 0, 0, 0, DateTimeKind.Utc), "Verified", "63950297-85b0-4b81-934a-bad2bf77f3cb", 8, 7 },
                    { 1308, new DateTime(2026, 10, 8, 11, 0, 0, 0, DateTimeKind.Utc), false, null, null, new DateTime(2026, 10, 8, 10, 0, 0, 0, DateTimeKind.Utc), "Verified", "63950297-85b0-4b81-934a-bad2bf77f3cb", 2, 12 },
                    { 1309, new DateTime(2026, 10, 9, 11, 0, 0, 0, DateTimeKind.Utc), false, null, null, new DateTime(2026, 10, 9, 10, 0, 0, 0, DateTimeKind.Utc), "Verified", "63950297-85b0-4b81-934a-bad2bf77f3cb", 6, 1 },
                    { 1310, new DateTime(2026, 10, 10, 11, 0, 0, 0, DateTimeKind.Utc), false, null, null, new DateTime(2026, 10, 10, 10, 0, 0, 0, DateTimeKind.Utc), "Verified", "63950297-85b0-4b81-934a-bad2bf77f3cb", 9, 6 },
                    { 1311, new DateTime(2026, 10, 11, 11, 0, 0, 0, DateTimeKind.Utc), false, null, null, new DateTime(2026, 10, 11, 10, 0, 0, 0, DateTimeKind.Utc), "Verified", "63950297-85b0-4b81-934a-bad2bf77f3cb", 4, 11 },
                    { 1312, new DateTime(2026, 10, 12, 11, 0, 0, 0, DateTimeKind.Utc), false, null, null, new DateTime(2026, 10, 12, 10, 0, 0, 0, DateTimeKind.Utc), "Verified", "63950297-85b0-4b81-934a-bad2bf77f3cb", 8, 17 },
                    { 1313, new DateTime(2026, 10, 13, 11, 0, 0, 0, DateTimeKind.Utc), false, null, null, new DateTime(2026, 10, 13, 10, 0, 0, 0, DateTimeKind.Utc), "Verified", "63950297-85b0-4b81-934a-bad2bf77f3cb", 2, 20 },
                    { 1314, new DateTime(2026, 10, 14, 11, 0, 0, 0, DateTimeKind.Utc), false, null, null, new DateTime(2026, 10, 14, 10, 0, 0, 0, DateTimeKind.Utc), "Verified", "63950297-85b0-4b81-934a-bad2bf77f3cb", 6, 2 },
                    { 1315, new DateTime(2026, 10, 1, 11, 0, 0, 0, DateTimeKind.Utc), false, null, null, new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc), "Verified", "63950297-85b0-4b81-934a-bad2bf77f3cb", 9, 7 },
                    { 1316, new DateTime(2026, 10, 2, 11, 0, 0, 0, DateTimeKind.Utc), false, null, null, new DateTime(2026, 10, 2, 10, 0, 0, 0, DateTimeKind.Utc), "Verified", "63950297-85b0-4b81-934a-bad2bf77f3cb", 4, 12 },
                    { 1317, new DateTime(2026, 10, 3, 11, 0, 0, 0, DateTimeKind.Utc), false, null, null, new DateTime(2026, 10, 3, 10, 0, 0, 0, DateTimeKind.Utc), "Verified", "63950297-85b0-4b81-934a-bad2bf77f3cb", 8, 1 },
                    { 1318, new DateTime(2026, 10, 4, 11, 0, 0, 0, DateTimeKind.Utc), false, null, null, new DateTime(2026, 10, 4, 10, 0, 0, 0, DateTimeKind.Utc), "Verified", "63950297-85b0-4b81-934a-bad2bf77f3cb", 2, 6 },
                    { 1319, new DateTime(2026, 10, 5, 11, 0, 0, 0, DateTimeKind.Utc), false, null, null, new DateTime(2026, 10, 5, 10, 0, 0, 0, DateTimeKind.Utc), "Verified", "63950297-85b0-4b81-934a-bad2bf77f3cb", 6, 11 },
                    { 1320, new DateTime(2026, 10, 6, 11, 0, 0, 0, DateTimeKind.Utc), false, null, null, new DateTime(2026, 10, 6, 10, 0, 0, 0, DateTimeKind.Utc), "Verified", "63950297-85b0-4b81-934a-bad2bf77f3cb", 9, 17 },
                    { 1321, new DateTime(2026, 10, 7, 11, 0, 0, 0, DateTimeKind.Utc), false, null, null, new DateTime(2026, 10, 7, 10, 0, 0, 0, DateTimeKind.Utc), "Verified", "63950297-85b0-4b81-934a-bad2bf77f3cb", 4, 20 },
                    { 1322, new DateTime(2026, 10, 8, 11, 0, 0, 0, DateTimeKind.Utc), false, null, null, new DateTime(2026, 10, 8, 10, 0, 0, 0, DateTimeKind.Utc), "Verified", "63950297-85b0-4b81-934a-bad2bf77f3cb", 8, 2 },
                    { 1323, new DateTime(2026, 10, 9, 11, 0, 0, 0, DateTimeKind.Utc), false, null, null, new DateTime(2026, 10, 9, 10, 0, 0, 0, DateTimeKind.Utc), "Verified", "63950297-85b0-4b81-934a-bad2bf77f3cb", 2, 7 },
                    { 1324, new DateTime(2026, 10, 10, 11, 0, 0, 0, DateTimeKind.Utc), false, null, null, new DateTime(2026, 10, 10, 10, 0, 0, 0, DateTimeKind.Utc), "Verified", "63950297-85b0-4b81-934a-bad2bf77f3cb", 6, 12 }
                });

            migrationBuilder.UpdateData(
                table: "TestRequests",
                keyColumn: "TestRequestId",
                keyValue: 7,
                column: "Status",
                value: "SamplesReceived");

            migrationBuilder.InsertData(
                table: "TestResultVerificationHistories",
                columns: new[] { "TestResultVerificationHistoryId", "Action", "ActionByTechnicianId", "ActionDateTime", "NewIsAbnormal", "NewMeasurementUnit", "NewReferenceMax", "NewReferenceMin", "NewResultFlag", "NewResultNotes", "NewResultValue", "Notes", "PreviousIsAbnormal", "PreviousMeasurementUnit", "PreviousReferenceMax", "PreviousReferenceMin", "PreviousResultFlag", "PreviousResultNotes", "PreviousResultValue", "TestResultId" },
                values: new object[,]
                {
                    { 3101, "ReturnedForReview", "63950297-85b0-4b81-934a-bad2bf77f3cb", new DateTime(2026, 10, 14, 12, 0, 0, 0, DateTimeKind.Utc), null, null, null, null, null, null, null, "DEMO: verification attempt 1 returned", null, null, null, null, null, null, null, 2002 },
                    { 3102, "ResubmittedForVerification", "85a94ac3-e56a-4ae2-bea1-85d398486185", new DateTime(2026, 10, 14, 13, 0, 0, 0, DateTimeKind.Utc), null, null, null, null, null, null, null, "DEMO: corrected after attempt 1", null, null, null, null, null, null, null, 2002 },
                    { 3103, "ReturnedForReview", "63950297-85b0-4b81-934a-bad2bf77f3cb", new DateTime(2026, 10, 14, 14, 0, 0, 0, DateTimeKind.Utc), null, null, null, null, null, null, null, "DEMO: verification attempt 2 returned", null, null, null, null, null, null, null, 2002 },
                    { 3104, "ResubmittedForVerification", "85a94ac3-e56a-4ae2-bea1-85d398486185", new DateTime(2026, 10, 14, 15, 0, 0, 0, DateTimeKind.Utc), null, null, null, null, null, null, null, "DEMO: third verification now pending", null, null, null, null, null, null, null, 2002 },
                    { 3111, "ResubmittedForVerification", "63950297-85b0-4b81-934a-bad2bf77f3cb", new DateTime(2026, 10, 13, 10, 0, 0, 0, DateTimeKind.Utc), null, null, null, null, null, null, null, "DEMO: second verification requested", null, null, null, null, null, null, null, 2003 },
                    { 3112, "ReturnedForReview", "85a94ac3-e56a-4ae2-bea1-85d398486185", new DateTime(2026, 10, 13, 11, 0, 0, 0, DateTimeKind.Utc), null, null, null, null, null, null, null, "DEMO: verification attempt 2 returned", null, null, null, null, null, null, null, 2003 },
                    { 3113, "ResubmittedForVerification", "63950297-85b0-4b81-934a-bad2bf77f3cb", new DateTime(2026, 10, 13, 12, 0, 0, 0, DateTimeKind.Utc), null, null, null, null, null, null, null, "DEMO: third verification requested", null, null, null, null, null, null, null, 2003 },
                    { 3114, "ReturnedForReview", "85a94ac3-e56a-4ae2-bea1-85d398486185", new DateTime(2026, 10, 13, 13, 0, 0, 0, DateTimeKind.Utc), null, null, null, null, null, null, null, "DEMO: verification attempt 3 returned, recapture pending", null, null, null, null, null, null, null, 2003 }
                });

            migrationBuilder.InsertData(
                table: "MedicalConditionModel",
                columns: new[] { "MedicalConditionId", "ConditionName", "Description", "MedicalConditionCategoryId" },
                values: new object[,]
                {
                    { 1, "Hypertension", "DEMO DATA ONLY", 9001 },
                    { 2, "Type 2 diabetes", "DEMO DATA ONLY", 9001 }
                });

            migrationBuilder.InsertData(
                table: "PatientAllergies",
                columns: new[] { "PatientAllergyId", "AllergyId", "Notes", "PatientId", "Severity" },
                values: new object[,]
                {
                    { 6001, 1, "DEMO DATA ONLY - fictional allergy history.", "f71c5f4a-6a9e-4c8d-91b2-7e6d2f304a15", "Moderate" },
                    { 6002, 2, "DEMO DATA ONLY - fictional allergy history.", "f71c5f4a-6a9e-4c8d-91b2-7e6d2f304a15", "Mild" }
                });

            migrationBuilder.InsertData(
                table: "PatientMedications",
                columns: new[] { "PatientMedicationId", "Dosage", "EndDate", "Frequency", "MedicationId", "Notes", "PatientId", "StartDate" },
                values: new object[,]
                {
                    { 7001, "500 mg", null, "Twice daily", 1, "DEMO DATA ONLY - fictional medication history.", "f71c5f4a-6a9e-4c8d-91b2-7e6d2f304a15", new DateTime(2026, 1, 10, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 7002, "10 mg", null, "Once daily", 2, "DEMO DATA ONLY - fictional medication history.", "f71c5f4a-6a9e-4c8d-91b2-7e6d2f304a15", new DateTime(2026, 2, 15, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "TestResults",
                columns: new[] { "TestResultId", "CapturedDateTime", "IsAbnormal", "MeasurementUnit", "NewIsAbnormal", "NewMeasurementUnit", "NewReferenceMax", "NewReferenceMin", "NewResultFlag", "NewResultNotes", "Notes", "PreviousIsAbnormal", "PreviousMeasurementUnit", "PreviousReferenceMax", "PreviousReferenceMin", "PreviousResultFlag", "PreviousResultNotes", "ReferenceMax", "ReferenceMin", "RequiresReview", "ResultFlag", "ResultName", "ResultValue", "ReviewNotes", "TechnicianId", "TestRequestTestTypeId", "TestTypeMeasurementId", "VerificationNotes", "VerifiedByTechnicianId", "VerifiedDateTime" },
                values: new object[,]
                {
                    { 2106, new DateTime(2026, 10, 14, 8, 50, 0, 0, DateTimeKind.Utc), false, "Qualitative", null, null, null, null, null, null, "DEMO DATA ONLY", null, null, null, null, null, null, null, null, false, "Not Evaluated", "Peripheral Blood Film", "DEMO: morphology review", null, "85a94ac3-e56a-4ae2-bea1-85d398486185", 1106, null, null, null, null },
                    { 2107, new DateTime(2026, 10, 14, 8, 50, 0, 0, DateTimeKind.Utc), false, "Qualitative", null, null, null, null, null, null, "DEMO DATA ONLY", null, null, null, null, null, null, null, null, true, "Not Evaluated", "Peripheral Blood Film", "DEMO: morphology review", "DEMO: repeat examination required", "63950297-85b0-4b81-934a-bad2bf77f3cb", 1107, null, null, null, null },
                    { 2201, new DateTime(2026, 10, 1, 8, 50, 0, 0, DateTimeKind.Utc), false, "Qualitative", null, null, null, null, null, null, "DEMO DATA ONLY", null, null, null, null, null, null, null, null, false, "Not Evaluated", "Peripheral Blood Film", "DEMO: no abnormal cells seen", null, "63950297-85b0-4b81-934a-bad2bf77f3cb", 1201, null, null, "85a94ac3-e56a-4ae2-bea1-85d398486185", new DateTime(2026, 10, 1, 9, 15, 0, 0, DateTimeKind.Utc) },
                    { 2202, new DateTime(2026, 10, 2, 8, 50, 0, 0, DateTimeKind.Utc), false, "Qualitative", null, null, null, null, null, null, "DEMO DATA ONLY", null, null, null, null, null, null, null, null, false, "Not Evaluated", "Peripheral Blood Film", "DEMO: no abnormal cells seen", null, "63950297-85b0-4b81-934a-bad2bf77f3cb", 1202, null, null, "85a94ac3-e56a-4ae2-bea1-85d398486185", new DateTime(2026, 10, 2, 9, 15, 0, 0, DateTimeKind.Utc) },
                    { 2203, new DateTime(2026, 10, 3, 8, 50, 0, 0, DateTimeKind.Utc), false, "Qualitative", null, null, null, null, null, null, "DEMO DATA ONLY", null, null, null, null, null, null, null, null, false, "Not Evaluated", "Peripheral Blood Film", "DEMO: no abnormal cells seen", null, "63950297-85b0-4b81-934a-bad2bf77f3cb", 1203, null, null, "85a94ac3-e56a-4ae2-bea1-85d398486185", new DateTime(2026, 10, 3, 9, 15, 0, 0, DateTimeKind.Utc) },
                    { 2204, new DateTime(2026, 10, 4, 8, 50, 0, 0, DateTimeKind.Utc), false, "Qualitative", null, null, null, null, null, null, "DEMO DATA ONLY", null, null, null, null, null, null, null, null, false, "Not Evaluated", "Peripheral Blood Film", "DEMO: no abnormal cells seen", null, "63950297-85b0-4b81-934a-bad2bf77f3cb", 1204, null, null, "85a94ac3-e56a-4ae2-bea1-85d398486185", new DateTime(2026, 10, 4, 9, 15, 0, 0, DateTimeKind.Utc) },
                    { 2205, new DateTime(2026, 10, 5, 8, 50, 0, 0, DateTimeKind.Utc), false, "Qualitative", null, null, null, null, null, null, "DEMO DATA ONLY", null, null, null, null, null, null, null, null, false, "Not Evaluated", "Peripheral Blood Film", "DEMO: no abnormal cells seen", null, "63950297-85b0-4b81-934a-bad2bf77f3cb", 1205, null, null, "85a94ac3-e56a-4ae2-bea1-85d398486185", new DateTime(2026, 10, 5, 9, 15, 0, 0, DateTimeKind.Utc) },
                    { 2206, new DateTime(2026, 10, 6, 8, 50, 0, 0, DateTimeKind.Utc), false, "Qualitative", null, null, null, null, null, null, "DEMO DATA ONLY", null, null, null, null, null, null, null, null, false, "Not Evaluated", "Peripheral Blood Film", "DEMO: no abnormal cells seen", null, "63950297-85b0-4b81-934a-bad2bf77f3cb", 1206, null, null, "85a94ac3-e56a-4ae2-bea1-85d398486185", new DateTime(2026, 10, 6, 9, 15, 0, 0, DateTimeKind.Utc) },
                    { 2207, new DateTime(2026, 10, 7, 8, 50, 0, 0, DateTimeKind.Utc), false, "Qualitative", null, null, null, null, null, null, "DEMO DATA ONLY", null, null, null, null, null, null, null, null, false, "Not Evaluated", "Peripheral Blood Film", "DEMO: no abnormal cells seen", null, "63950297-85b0-4b81-934a-bad2bf77f3cb", 1207, null, null, "85a94ac3-e56a-4ae2-bea1-85d398486185", new DateTime(2026, 10, 7, 9, 15, 0, 0, DateTimeKind.Utc) },
                    { 2208, new DateTime(2026, 10, 8, 8, 50, 0, 0, DateTimeKind.Utc), false, "Qualitative", null, null, null, null, null, null, "DEMO DATA ONLY", null, null, null, null, null, null, null, null, false, "Not Evaluated", "Peripheral Blood Film", "DEMO: no abnormal cells seen", null, "63950297-85b0-4b81-934a-bad2bf77f3cb", 1208, null, null, "85a94ac3-e56a-4ae2-bea1-85d398486185", new DateTime(2026, 10, 8, 9, 15, 0, 0, DateTimeKind.Utc) },
                    { 2209, new DateTime(2026, 10, 9, 8, 50, 0, 0, DateTimeKind.Utc), false, "Qualitative", null, null, null, null, null, null, "DEMO DATA ONLY", null, null, null, null, null, null, null, null, false, "Not Evaluated", "Peripheral Blood Film", "DEMO: no abnormal cells seen", null, "63950297-85b0-4b81-934a-bad2bf77f3cb", 1209, null, null, "85a94ac3-e56a-4ae2-bea1-85d398486185", new DateTime(2026, 10, 9, 9, 15, 0, 0, DateTimeKind.Utc) },
                    { 2210, new DateTime(2026, 10, 10, 8, 50, 0, 0, DateTimeKind.Utc), false, "Qualitative", null, null, null, null, null, null, "DEMO DATA ONLY", null, null, null, null, null, null, null, null, false, "Not Evaluated", "Peripheral Blood Film", "DEMO: no abnormal cells seen", null, "63950297-85b0-4b81-934a-bad2bf77f3cb", 1210, null, null, "85a94ac3-e56a-4ae2-bea1-85d398486185", new DateTime(2026, 10, 10, 9, 15, 0, 0, DateTimeKind.Utc) },
                    { 2211, new DateTime(2026, 10, 11, 8, 50, 0, 0, DateTimeKind.Utc), false, "Qualitative", null, null, null, null, null, null, "DEMO DATA ONLY", null, null, null, null, null, null, null, null, false, "Not Evaluated", "Peripheral Blood Film", "DEMO: no abnormal cells seen", null, "63950297-85b0-4b81-934a-bad2bf77f3cb", 1211, null, null, "85a94ac3-e56a-4ae2-bea1-85d398486185", new DateTime(2026, 10, 11, 9, 15, 0, 0, DateTimeKind.Utc) },
                    { 2212, new DateTime(2026, 10, 12, 8, 50, 0, 0, DateTimeKind.Utc), false, "Qualitative", null, null, null, null, null, null, "DEMO DATA ONLY", null, null, null, null, null, null, null, null, false, "Not Evaluated", "Peripheral Blood Film", "DEMO: no abnormal cells seen", null, "63950297-85b0-4b81-934a-bad2bf77f3cb", 1212, null, null, "85a94ac3-e56a-4ae2-bea1-85d398486185", new DateTime(2026, 10, 12, 9, 15, 0, 0, DateTimeKind.Utc) },
                    { 2213, new DateTime(2026, 10, 13, 8, 50, 0, 0, DateTimeKind.Utc), false, "Qualitative", null, null, null, null, null, null, "DEMO DATA ONLY", null, null, null, null, null, null, null, null, false, "Not Evaluated", "Peripheral Blood Film", "DEMO: no abnormal cells seen", null, "63950297-85b0-4b81-934a-bad2bf77f3cb", 1213, null, null, "85a94ac3-e56a-4ae2-bea1-85d398486185", new DateTime(2026, 10, 13, 9, 15, 0, 0, DateTimeKind.Utc) },
                    { 2214, new DateTime(2026, 10, 14, 8, 50, 0, 0, DateTimeKind.Utc), false, "Qualitative", null, null, null, null, null, null, "DEMO DATA ONLY", null, null, null, null, null, null, null, null, false, "Not Evaluated", "Peripheral Blood Film", "DEMO: no abnormal cells seen", null, "63950297-85b0-4b81-934a-bad2bf77f3cb", 1214, null, null, "85a94ac3-e56a-4ae2-bea1-85d398486185", new DateTime(2026, 10, 14, 9, 15, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "PatientConditions",
                columns: new[] { "PatientConditionId", "DateDiagnosed", "MedicalConditionId", "Notes", "PatientId" },
                values: new object[,]
                {
                    { 5001, new DateTime(2024, 3, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "DEMO DATA ONLY - example medical condition.", "f71c5f4a-6a9e-4c8d-91b2-7e6d2f304a15" },
                    { 5002, new DateTime(2025, 6, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, "DEMO DATA ONLY - example medical condition.", "f71c5f4a-6a9e-4c8d-91b2-7e6d2f304a15" }
                });

            migrationBuilder.InsertData(
                table: "TestResultVerificationHistories",
                columns: new[] { "TestResultVerificationHistoryId", "Action", "ActionByTechnicianId", "ActionDateTime", "NewIsAbnormal", "NewMeasurementUnit", "NewReferenceMax", "NewReferenceMin", "NewResultFlag", "NewResultNotes", "NewResultValue", "Notes", "PreviousIsAbnormal", "PreviousMeasurementUnit", "PreviousReferenceMax", "PreviousReferenceMin", "PreviousResultFlag", "PreviousResultNotes", "PreviousResultValue", "TestResultId" },
                values: new object[,]
                {
                    { 3121, "SubmittedForVerification", "85a94ac3-e56a-4ae2-bea1-85d398486185", new DateTime(2026, 10, 14, 9, 0, 0, 0, DateTimeKind.Utc), null, null, null, null, null, null, null, "DEMO: awaiting independent verification", null, null, null, null, null, null, null, 2106 },
                    { 3122, "SubmittedForVerification", "63950297-85b0-4b81-934a-bad2bf77f3cb", new DateTime(2026, 10, 14, 9, 0, 0, 0, DateTimeKind.Utc), null, null, null, null, null, null, null, "DEMO: submitted for review", null, null, null, null, null, null, null, 2107 },
                    { 3123, "ReturnedForReview", "85a94ac3-e56a-4ae2-bea1-85d398486185", new DateTime(2026, 10, 14, 10, 0, 0, 0, DateTimeKind.Utc), null, null, null, null, null, null, null, "DEMO: correction required", null, null, null, null, null, null, null, 2107 },
                    { 3201, "Verified", "85a94ac3-e56a-4ae2-bea1-85d398486185", new DateTime(2026, 10, 1, 9, 15, 0, 0, DateTimeKind.Utc), null, null, null, null, null, null, null, "DEMO: historical verification for report", null, null, null, null, null, null, null, 2201 },
                    { 3202, "Verified", "85a94ac3-e56a-4ae2-bea1-85d398486185", new DateTime(2026, 10, 2, 9, 15, 0, 0, DateTimeKind.Utc), null, null, null, null, null, null, null, "DEMO: historical verification for report", null, null, null, null, null, null, null, 2202 },
                    { 3203, "Verified", "85a94ac3-e56a-4ae2-bea1-85d398486185", new DateTime(2026, 10, 3, 9, 15, 0, 0, DateTimeKind.Utc), null, null, null, null, null, null, null, "DEMO: historical verification for report", null, null, null, null, null, null, null, 2203 },
                    { 3204, "Verified", "85a94ac3-e56a-4ae2-bea1-85d398486185", new DateTime(2026, 10, 4, 9, 15, 0, 0, DateTimeKind.Utc), null, null, null, null, null, null, null, "DEMO: historical verification for report", null, null, null, null, null, null, null, 2204 },
                    { 3205, "Verified", "85a94ac3-e56a-4ae2-bea1-85d398486185", new DateTime(2026, 10, 5, 9, 15, 0, 0, DateTimeKind.Utc), null, null, null, null, null, null, null, "DEMO: historical verification for report", null, null, null, null, null, null, null, 2205 },
                    { 3206, "Verified", "85a94ac3-e56a-4ae2-bea1-85d398486185", new DateTime(2026, 10, 6, 9, 15, 0, 0, DateTimeKind.Utc), null, null, null, null, null, null, null, "DEMO: historical verification for report", null, null, null, null, null, null, null, 2206 },
                    { 3207, "Verified", "85a94ac3-e56a-4ae2-bea1-85d398486185", new DateTime(2026, 10, 7, 9, 15, 0, 0, DateTimeKind.Utc), null, null, null, null, null, null, null, "DEMO: historical verification for report", null, null, null, null, null, null, null, 2207 },
                    { 3208, "Verified", "85a94ac3-e56a-4ae2-bea1-85d398486185", new DateTime(2026, 10, 8, 9, 15, 0, 0, DateTimeKind.Utc), null, null, null, null, null, null, null, "DEMO: historical verification for report", null, null, null, null, null, null, null, 2208 },
                    { 3209, "Verified", "85a94ac3-e56a-4ae2-bea1-85d398486185", new DateTime(2026, 10, 9, 9, 15, 0, 0, DateTimeKind.Utc), null, null, null, null, null, null, null, "DEMO: historical verification for report", null, null, null, null, null, null, null, 2209 },
                    { 3210, "Verified", "85a94ac3-e56a-4ae2-bea1-85d398486185", new DateTime(2026, 10, 10, 9, 15, 0, 0, DateTimeKind.Utc), null, null, null, null, null, null, null, "DEMO: historical verification for report", null, null, null, null, null, null, null, 2210 },
                    { 3211, "Verified", "85a94ac3-e56a-4ae2-bea1-85d398486185", new DateTime(2026, 10, 11, 9, 15, 0, 0, DateTimeKind.Utc), null, null, null, null, null, null, null, "DEMO: historical verification for report", null, null, null, null, null, null, null, 2211 },
                    { 3212, "Verified", "85a94ac3-e56a-4ae2-bea1-85d398486185", new DateTime(2026, 10, 12, 9, 15, 0, 0, DateTimeKind.Utc), null, null, null, null, null, null, null, "DEMO: historical verification for report", null, null, null, null, null, null, null, 2212 },
                    { 3213, "Verified", "85a94ac3-e56a-4ae2-bea1-85d398486185", new DateTime(2026, 10, 13, 9, 15, 0, 0, DateTimeKind.Utc), null, null, null, null, null, null, null, "DEMO: historical verification for report", null, null, null, null, null, null, null, 2213 },
                    { 3214, "Verified", "85a94ac3-e56a-4ae2-bea1-85d398486185", new DateTime(2026, 10, 14, 9, 15, 0, 0, DateTimeKind.Utc), null, null, null, null, null, null, null, "DEMO: historical verification for report", null, null, null, null, null, null, null, 2214 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "PatientAllergies",
                keyColumn: "PatientAllergyId",
                keyValue: 6001);

            migrationBuilder.DeleteData(
                table: "PatientAllergies",
                keyColumn: "PatientAllergyId",
                keyValue: 6002);

            migrationBuilder.DeleteData(
                table: "PatientConditions",
                keyColumn: "PatientConditionId",
                keyValue: 5001);

            migrationBuilder.DeleteData(
                table: "PatientConditions",
                keyColumn: "PatientConditionId",
                keyValue: 5002);

            migrationBuilder.DeleteData(
                table: "PatientMedications",
                keyColumn: "PatientMedicationId",
                keyValue: 7001);

            migrationBuilder.DeleteData(
                table: "PatientMedications",
                keyColumn: "PatientMedicationId",
                keyValue: 7002);

            migrationBuilder.DeleteData(
                table: "Samples",
                keyColumn: "SampleId",
                keyValue: 8101);

            migrationBuilder.DeleteData(
                table: "Samples",
                keyColumn: "SampleId",
                keyValue: 8102);

            migrationBuilder.DeleteData(
                table: "Samples",
                keyColumn: "SampleId",
                keyValue: 8103);

            migrationBuilder.DeleteData(
                table: "Samples",
                keyColumn: "SampleId",
                keyValue: 8104);

            migrationBuilder.DeleteData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1101);

            migrationBuilder.DeleteData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1102);

            migrationBuilder.DeleteData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1103);

            migrationBuilder.DeleteData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1104);

            migrationBuilder.DeleteData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1105);

            migrationBuilder.DeleteData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1301);

            migrationBuilder.DeleteData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1302);

            migrationBuilder.DeleteData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1303);

            migrationBuilder.DeleteData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1304);

            migrationBuilder.DeleteData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1305);

            migrationBuilder.DeleteData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1306);

            migrationBuilder.DeleteData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1307);

            migrationBuilder.DeleteData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1308);

            migrationBuilder.DeleteData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1309);

            migrationBuilder.DeleteData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1310);

            migrationBuilder.DeleteData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1311);

            migrationBuilder.DeleteData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1312);

            migrationBuilder.DeleteData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1313);

            migrationBuilder.DeleteData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1314);

            migrationBuilder.DeleteData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1315);

            migrationBuilder.DeleteData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1316);

            migrationBuilder.DeleteData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1317);

            migrationBuilder.DeleteData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1318);

            migrationBuilder.DeleteData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1319);

            migrationBuilder.DeleteData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1320);

            migrationBuilder.DeleteData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1321);

            migrationBuilder.DeleteData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1322);

            migrationBuilder.DeleteData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1323);

            migrationBuilder.DeleteData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1324);

            migrationBuilder.DeleteData(
                table: "TestResultVerificationHistories",
                keyColumn: "TestResultVerificationHistoryId",
                keyValue: 3101);

            migrationBuilder.DeleteData(
                table: "TestResultVerificationHistories",
                keyColumn: "TestResultVerificationHistoryId",
                keyValue: 3102);

            migrationBuilder.DeleteData(
                table: "TestResultVerificationHistories",
                keyColumn: "TestResultVerificationHistoryId",
                keyValue: 3103);

            migrationBuilder.DeleteData(
                table: "TestResultVerificationHistories",
                keyColumn: "TestResultVerificationHistoryId",
                keyValue: 3104);

            migrationBuilder.DeleteData(
                table: "TestResultVerificationHistories",
                keyColumn: "TestResultVerificationHistoryId",
                keyValue: 3111);

            migrationBuilder.DeleteData(
                table: "TestResultVerificationHistories",
                keyColumn: "TestResultVerificationHistoryId",
                keyValue: 3112);

            migrationBuilder.DeleteData(
                table: "TestResultVerificationHistories",
                keyColumn: "TestResultVerificationHistoryId",
                keyValue: 3113);

            migrationBuilder.DeleteData(
                table: "TestResultVerificationHistories",
                keyColumn: "TestResultVerificationHistoryId",
                keyValue: 3114);

            migrationBuilder.DeleteData(
                table: "TestResultVerificationHistories",
                keyColumn: "TestResultVerificationHistoryId",
                keyValue: 3121);

            migrationBuilder.DeleteData(
                table: "TestResultVerificationHistories",
                keyColumn: "TestResultVerificationHistoryId",
                keyValue: 3122);

            migrationBuilder.DeleteData(
                table: "TestResultVerificationHistories",
                keyColumn: "TestResultVerificationHistoryId",
                keyValue: 3123);

            migrationBuilder.DeleteData(
                table: "TestResultVerificationHistories",
                keyColumn: "TestResultVerificationHistoryId",
                keyValue: 3201);

            migrationBuilder.DeleteData(
                table: "TestResultVerificationHistories",
                keyColumn: "TestResultVerificationHistoryId",
                keyValue: 3202);

            migrationBuilder.DeleteData(
                table: "TestResultVerificationHistories",
                keyColumn: "TestResultVerificationHistoryId",
                keyValue: 3203);

            migrationBuilder.DeleteData(
                table: "TestResultVerificationHistories",
                keyColumn: "TestResultVerificationHistoryId",
                keyValue: 3204);

            migrationBuilder.DeleteData(
                table: "TestResultVerificationHistories",
                keyColumn: "TestResultVerificationHistoryId",
                keyValue: 3205);

            migrationBuilder.DeleteData(
                table: "TestResultVerificationHistories",
                keyColumn: "TestResultVerificationHistoryId",
                keyValue: 3206);

            migrationBuilder.DeleteData(
                table: "TestResultVerificationHistories",
                keyColumn: "TestResultVerificationHistoryId",
                keyValue: 3207);

            migrationBuilder.DeleteData(
                table: "TestResultVerificationHistories",
                keyColumn: "TestResultVerificationHistoryId",
                keyValue: 3208);

            migrationBuilder.DeleteData(
                table: "TestResultVerificationHistories",
                keyColumn: "TestResultVerificationHistoryId",
                keyValue: 3209);

            migrationBuilder.DeleteData(
                table: "TestResultVerificationHistories",
                keyColumn: "TestResultVerificationHistoryId",
                keyValue: 3210);

            migrationBuilder.DeleteData(
                table: "TestResultVerificationHistories",
                keyColumn: "TestResultVerificationHistoryId",
                keyValue: 3211);

            migrationBuilder.DeleteData(
                table: "TestResultVerificationHistories",
                keyColumn: "TestResultVerificationHistoryId",
                keyValue: 3212);

            migrationBuilder.DeleteData(
                table: "TestResultVerificationHistories",
                keyColumn: "TestResultVerificationHistoryId",
                keyValue: 3213);

            migrationBuilder.DeleteData(
                table: "TestResultVerificationHistories",
                keyColumn: "TestResultVerificationHistoryId",
                keyValue: 3214);

            migrationBuilder.DeleteData(
                table: "AllergyModel",
                keyColumn: "AllergyId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "AllergyModel",
                keyColumn: "AllergyId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "MedicalConditionModel",
                keyColumn: "MedicalConditionId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "MedicalConditionModel",
                keyColumn: "MedicalConditionId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "MedicationModel",
                keyColumn: "MedicationId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "MedicationModel",
                keyColumn: "MedicationId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Patients",
                keyColumn: "PatientId",
                keyValue: "f71c5f4a-6a9e-4c8d-91b2-7e6d2f304a15");

            migrationBuilder.DeleteData(
                table: "TestResults",
                keyColumn: "TestResultId",
                keyValue: 2106);

            migrationBuilder.DeleteData(
                table: "TestResults",
                keyColumn: "TestResultId",
                keyValue: 2107);

            migrationBuilder.DeleteData(
                table: "TestResults",
                keyColumn: "TestResultId",
                keyValue: 2201);

            migrationBuilder.DeleteData(
                table: "TestResults",
                keyColumn: "TestResultId",
                keyValue: 2202);

            migrationBuilder.DeleteData(
                table: "TestResults",
                keyColumn: "TestResultId",
                keyValue: 2203);

            migrationBuilder.DeleteData(
                table: "TestResults",
                keyColumn: "TestResultId",
                keyValue: 2204);

            migrationBuilder.DeleteData(
                table: "TestResults",
                keyColumn: "TestResultId",
                keyValue: 2205);

            migrationBuilder.DeleteData(
                table: "TestResults",
                keyColumn: "TestResultId",
                keyValue: 2206);

            migrationBuilder.DeleteData(
                table: "TestResults",
                keyColumn: "TestResultId",
                keyValue: 2207);

            migrationBuilder.DeleteData(
                table: "TestResults",
                keyColumn: "TestResultId",
                keyValue: 2208);

            migrationBuilder.DeleteData(
                table: "TestResults",
                keyColumn: "TestResultId",
                keyValue: 2209);

            migrationBuilder.DeleteData(
                table: "TestResults",
                keyColumn: "TestResultId",
                keyValue: 2210);

            migrationBuilder.DeleteData(
                table: "TestResults",
                keyColumn: "TestResultId",
                keyValue: 2211);

            migrationBuilder.DeleteData(
                table: "TestResults",
                keyColumn: "TestResultId",
                keyValue: 2212);

            migrationBuilder.DeleteData(
                table: "TestResults",
                keyColumn: "TestResultId",
                keyValue: 2213);

            migrationBuilder.DeleteData(
                table: "TestResults",
                keyColumn: "TestResultId",
                keyValue: 2214);

            migrationBuilder.DeleteData(
                table: "MedicalConditionCategories",
                keyColumn: "MedicalConditionCategoryId",
                keyValue: 9001);

            migrationBuilder.DeleteData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1106);

            migrationBuilder.DeleteData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1107);

            migrationBuilder.DeleteData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1201);

            migrationBuilder.DeleteData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1202);

            migrationBuilder.DeleteData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1203);

            migrationBuilder.DeleteData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1204);

            migrationBuilder.DeleteData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1205);

            migrationBuilder.DeleteData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1206);

            migrationBuilder.DeleteData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1207);

            migrationBuilder.DeleteData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1208);

            migrationBuilder.DeleteData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1209);

            migrationBuilder.DeleteData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1210);

            migrationBuilder.DeleteData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1211);

            migrationBuilder.DeleteData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1212);

            migrationBuilder.DeleteData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1213);

            migrationBuilder.DeleteData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1214);

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 2,
                column: "Status",
                value: "SamplesReceived");

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 3,
                column: "Status",
                value: "SamplesReceived");

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 4,
                column: "Status",
                value: "SamplesReceived");

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 6,
                column: "Status",
                value: "SamplesReceived");

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 7,
                column: "Status",
                value: "SamplesReceived");

            migrationBuilder.UpdateData(
                table: "TestRequests",
                keyColumn: "TestRequestId",
                keyValue: 7,
                column: "Status",
                value: "Submitted");
        }
    }
}
