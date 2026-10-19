using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace NMBHLabSys.Migrations
{
    /// <inheritdoc />
    public partial class CompleteTechnicianAssessmentQueues : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Samples",
                columns: new[] { "SampleId", "Barcode", "DateReceived", "IsDeleted", "IsReceived", "SampleTypeId", "Status", "TechnicianId", "TestRequestId" },
                values: new object[,]
                {
                    { 8201, "DEMO-SMP-8201", null, false, false, 2, "Submitted", null, 1 },
                    { 8202, "DEMO-SMP-8202", null, false, false, 1, "Submitted", null, 5 },
                    { 8203, "DEMO-SMP-8203", null, false, false, 1, "Submitted", null, 7 },
                    { 8204, "DEMO-SMP-8204", null, false, false, 2, "Submitted", null, 10 }
                });

            migrationBuilder.InsertData(
                table: "TestRequestTestTypes",
                columns: new[] { "TestRequestTestTypeId", "CompletionDateTime", "IsAbnormal", "ResultNotes", "ResultValue", "StartDateTime", "Status", "TechnicianId", "TestRequestId", "TestTypeId" },
                values: new object[,]
                {
                    { 1401, null, false, null, null, null, "Submitted", null, 2, 1 },
                    { 1402, null, false, null, null, null, "Submitted", null, 2, 2 },
                    { 1403, null, false, null, null, null, "Submitted", null, 6, 3 }
                });

            migrationBuilder.UpdateData(
                table: "TestRequests",
                keyColumn: "TestRequestId",
                keyValue: 7,
                column: "Status",
                value: "Submitted");

            // Request 10 also has unreceived samples and must appear in the pending queue.
            migrationBuilder.UpdateData(
                table: "TestRequests",
                keyColumn: "TestRequestId",
                keyValue: 10,
                column: "Status",
                value: "Submitted");

            migrationBuilder.InsertData(
                table: "TestRequests",
                columns: new[] { "TestRequestId", "ClinicalNotes", "DoctorId", "PatientId", "PatientModelPatientId", "RequestDate", "RequestNumber", "Status", "Urgency" },
                values: new object[] { 11, "DEMO ONLY: urgent assessment workflow.", "953f333d-2e68-46f6-ad26-0412fe75d908", "c0fbfbb4-daf5-44e3-bf57-acfded7baceb", null, new DateTime(2026, 10, 15, 7, 30, 0, 0, DateTimeKind.Unspecified), "NMB-REQ-DEMO-000011", "SamplesReceived", 3 });

            migrationBuilder.InsertData(
                table: "Samples",
                columns: new[] { "SampleId", "Barcode", "DateReceived", "IsDeleted", "IsReceived", "SampleTypeId", "Status", "TechnicianId", "TestRequestId" },
                values: new object[] { 8301, "DEMO-SMP-8301", new DateTime(2026, 10, 15, 7, 45, 0, 0, DateTimeKind.Unspecified), false, true, 1, "SamplesReceived", "85a94ac3-e56a-4ae2-bea1-85d398486185", 11 });

            migrationBuilder.InsertData(
                table: "TestRequestTestTypes",
                columns: new[] { "TestRequestTestTypeId", "CompletionDateTime", "IsAbnormal", "ResultNotes", "ResultValue", "StartDateTime", "Status", "TechnicianId", "TestRequestId", "TestTypeId" },
                values: new object[,]
                {
                    { 1404, null, false, null, null, null, "Submitted", null, 11, 1 },
                    { 1405, null, false, null, null, null, "Submitted", null, 11, 2 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(table: "Samples", keyColumn: "SampleId", keyValue: 8201);
            migrationBuilder.DeleteData(table: "Samples", keyColumn: "SampleId", keyValue: 8202);
            migrationBuilder.DeleteData(table: "Samples", keyColumn: "SampleId", keyValue: 8203);
            migrationBuilder.DeleteData(table: "Samples", keyColumn: "SampleId", keyValue: 8204);
            migrationBuilder.DeleteData(table: "Samples", keyColumn: "SampleId", keyValue: 8301);

            migrationBuilder.DeleteData(table: "TestRequestTestTypes", keyColumn: "TestRequestTestTypeId", keyValue: 1401);
            migrationBuilder.DeleteData(table: "TestRequestTestTypes", keyColumn: "TestRequestTestTypeId", keyValue: 1402);
            migrationBuilder.DeleteData(table: "TestRequestTestTypes", keyColumn: "TestRequestTestTypeId", keyValue: 1403);
            migrationBuilder.DeleteData(table: "TestRequestTestTypes", keyColumn: "TestRequestTestTypeId", keyValue: 1404);
            migrationBuilder.DeleteData(table: "TestRequestTestTypes", keyColumn: "TestRequestTestTypeId", keyValue: 1405);

            migrationBuilder.DeleteData(table: "TestRequests", keyColumn: "TestRequestId", keyValue: 11);

            migrationBuilder.UpdateData(
                table: "TestRequests",
                keyColumn: "TestRequestId",
                keyValue: 7,
                column: "Status",
                value: "SamplesReceived");

            migrationBuilder.UpdateData(
                table: "TestRequests",
                keyColumn: "TestRequestId",
                keyValue: 10,
                column: "Status",
                value: "SamplesReceived");
        }
    }
}
