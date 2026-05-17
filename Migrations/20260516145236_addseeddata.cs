using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace NMBHLabSys.Migrations
{
    /// <inheritdoc />
    public partial class addseeddata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "TechnicianTestTypes",
                columns: new[] { "TechnicianTestTypeId", "AssignmentDate", "ManagerId", "RegisterUserId", "Status", "TechnicianId", "TestTypeId" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "af381d22-8f2c-4401-ae8b-b31bb60e1c39", null, "Submitted", "2ec716df-770e-465e-a7b7-8d3926b560a2", 1 },
                    { 2, new DateTime(2026, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "af381d22-8f2c-4401-ae8b-b31bb60e1c39", null, "Submitted", "2ec716df-770e-465e-a7b7-8d3926b560a2", 2 },
                    { 3, new DateTime(2026, 1, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "af381d22-8f2c-4401-ae8b-b31bb60e1c39", null, "Submitted", "2ec716df-770e-465e-a7b7-8d3926b560a2", 3 }
                });

            migrationBuilder.InsertData(
                table: "TestRequests",
                columns: new[] { "TestRequestId", "ClinicalNotes", "DoctorId", "PatientId", "RequestDate", "RequestNumber", "Status", "TestTypeModelTestTypeId", "Urgency" },
                values: new object[,]
                {
                    { 1, "Routine blood test", "e6eec045-31c8-42c6-b67d-11bb28d980be", "7b0c0099-788d-4e4a-94f5-0f07c0b2013c", new DateTime(2026, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "NMB-REQ-000001", "Pending", null, "Routine" },
                    { 2, "Suspected infection", "e6eec045-31c8-42c6-b67d-11bb28d980be", "7b0c0099-788d-4e4a-94f5-0f07c0b2013c", new DateTime(2026, 5, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "NMB-REQ-000002", "Samples Received", null, "Urgent" },
                    { 3, "Emergency test required", "e6eec045-31c8-42c6-b67d-11bb28d980be", "7b0c0099-788d-4e4a-94f5-0f07c0b2013c", new DateTime(2026, 5, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "NMB-REQ-000003", "Pending", null, "Stat" }
                });

            migrationBuilder.InsertData(
                table: "Samples",
                columns: new[] { "SampleId", "Barcode", "DateReceived", "IsReceived", "SampleTypeId", "Status", "TechnicianId", "TestRequestId" },
                values: new object[,]
                {
                    { 1, "SMP-2024-000001", new DateTime(2024, 1, 15, 10, 30, 0, 0, DateTimeKind.Unspecified), true, 1, "Received", "2ec716df-770e-465e-a7b7-8d3926b560a2", 1 },
                    { 2, "SMP-2024-000002", new DateTime(2024, 1, 16, 11, 45, 0, 0, DateTimeKind.Unspecified), true, 2, "Received", "2ec716df-770e-465e-a7b7-8d3926b560a2", 1 },
                    { 3, "SMP-2024-000003", new DateTime(2024, 1, 17, 9, 15, 0, 0, DateTimeKind.Unspecified), true, 3, "Received", "2ec716df-770e-465e-a7b7-8d3926b560a2", 2 },
                    { 4, "SMP-2024-000004", new DateTime(2024, 1, 18, 14, 20, 0, 0, DateTimeKind.Unspecified), false, 1, "Pending", null, 2 },
                    { 5, "SMP-2024-000005", new DateTime(2024, 1, 19, 8, 0, 0, 0, DateTimeKind.Unspecified), true, 4, "Received", "2ec716df-770e-465e-a7b7-8d3926b560a2", 3 },
                    { 6, "SMP-2024-000006", new DateTime(2024, 1, 20, 13, 30, 0, 0, DateTimeKind.Unspecified), false, 5, "Pending", null, 3 },
                    { 9, "SMP-2024-000009", new DateTime(2024, 1, 23, 11, 20, 0, 0, DateTimeKind.Unspecified), false, 3, "Pending", null, 3 }
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

            migrationBuilder.DeleteData(
                table: "Samples",
                keyColumn: "SampleId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Samples",
                keyColumn: "SampleId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Samples",
                keyColumn: "SampleId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Samples",
                keyColumn: "SampleId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Samples",
                keyColumn: "SampleId",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "TechnicianTestTypes",
                keyColumn: "TechnicianTestTypeId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "TechnicianTestTypes",
                keyColumn: "TechnicianTestTypeId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "TechnicianTestTypes",
                keyColumn: "TechnicianTestTypeId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "TestRequests",
                keyColumn: "TestRequestId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "TestRequests",
                keyColumn: "TestRequestId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "TestRequests",
                keyColumn: "TestRequestId",
                keyValue: 3);
        }
    }
}
