using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace NMBHLabSys.Migrations
{
    /// <inheritdoc />
    public partial class FixSampleSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "DateReceived",
                table: "Samples",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.UpdateData(
                table: "Samples",
                keyColumn: "SampleId",
                keyValue: 1,
                columns: new[] { "Barcode", "CollectedAt", "DateReceived", "IsReceived", "Status", "TechnicianId" },
                values: new object[] { "0099GGRM", new DateTime(2026, 5, 1, 8, 0, 0, 0, DateTimeKind.Unspecified), null, false, "Submitted", null });

            migrationBuilder.UpdateData(
                table: "Samples",
                keyColumn: "SampleId",
                keyValue: 2,
                columns: new[] { "Barcode", "CollectedAt", "DateReceived", "IsReceived", "SampleTypeId", "Status", "TechnicianId", "TestRequestId" },
                values: new object[] { "00TTR1YMM", new DateTime(2026, 5, 2, 8, 0, 0, 0, DateTimeKind.Unspecified), null, false, 4, "Submitted", null, 2 });

            migrationBuilder.UpdateData(
                table: "Samples",
                keyColumn: "SampleId",
                keyValue: 3,
                columns: new[] { "Barcode", "CollectedAt", "DateReceived", "IsReceived", "SampleTypeId", "Status", "TechnicianId", "TestRequestId" },
                values: new object[] { "00FFD1YMM", new DateTime(2026, 5, 3, 8, 0, 0, 0, DateTimeKind.Unspecified), null, false, 4, "Submitted", null, 3 });

            migrationBuilder.UpdateData(
                table: "Samples",
                keyColumn: "SampleId",
                keyValue: 4,
                columns: new[] { "Barcode", "CollectedAt", "DateReceived", "Status", "TestRequestId" },
                values: new object[] { "0045ESM", new DateTime(2026, 5, 4, 8, 0, 0, 0, DateTimeKind.Unspecified), null, "Submitted", 4 });

            migrationBuilder.UpdateData(
                table: "Samples",
                keyColumn: "SampleId",
                keyValue: 5,
                columns: new[] { "Barcode", "CollectedAt", "DateReceived", "IsReceived", "Status", "TechnicianId", "TestRequestId" },
                values: new object[] { "005433YMV", new DateTime(2026, 5, 5, 8, 0, 0, 0, DateTimeKind.Unspecified), null, false, "Submitted", null, 5 });

            migrationBuilder.UpdateData(
                table: "Samples",
                keyColumn: "SampleId",
                keyValue: 6,
                columns: new[] { "Barcode", "CollectedAt", "DateReceived", "SampleTypeId", "Status", "TestRequestId" },
                values: new object[] { "009988HGM", new DateTime(2026, 5, 6, 8, 0, 0, 0, DateTimeKind.Unspecified), null, 4, "Submitted", 6 });

            migrationBuilder.UpdateData(
                table: "Samples",
                keyColumn: "SampleId",
                keyValue: 9,
                columns: new[] { "Barcode", "CollectedAt", "DateReceived", "SampleTypeId", "Status", "TestRequestId" },
                values: new object[] { "00HHHYMM", new DateTime(2026, 5, 9, 8, 0, 0, 0, DateTimeKind.Unspecified), null, 1, "Submitted", 9 });

            migrationBuilder.InsertData(
                table: "Samples",
                columns: new[] { "SampleId", "Barcode", "CollectedAt", "DateReceived", "DeletedAt", "DeletedBy", "IsDeleted", "IsReceived", "RestoredAt", "RestoredBy", "SampleTypeId", "Status", "TechnicianId", "TestRequestId", "TestRequestsModelTestRequestId" },
                values: new object[,]
                {
                    { 7, "00876YMM", new DateTime(2026, 5, 7, 8, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, false, null, null, 4, "Submitted", null, 7, null },
                    { 8, "00991YMM", new DateTime(2026, 5, 8, 8, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, false, null, null, 2, "Submitted", null, 8, null },
                    { 10, "04561YMM", new DateTime(2026, 5, 10, 8, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, false, false, null, null, 1, "Submitted", null, 10, null }
                });

            migrationBuilder.UpdateData(
                table: "TestRequestTests",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1,
                columns: new[] { "CompletionDateTime", "DueDate", "StartDateTime" },
                values: new object[] { new DateTime(2026, 9, 21, 11, 21, 41, 207, DateTimeKind.Local).AddTicks(1630), new DateTime(2026, 9, 21, 13, 21, 41, 207, DateTimeKind.Local).AddTicks(1631), new DateTime(2026, 9, 21, 11, 21, 41, 207, DateTimeKind.Local).AddTicks(1621) });

            migrationBuilder.UpdateData(
                table: "TestRequestTests",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 2,
                columns: new[] { "CompletionDateTime", "DueDate", "StartDateTime" },
                values: new object[] { new DateTime(2026, 9, 21, 11, 21, 41, 207, DateTimeKind.Local).AddTicks(1643), new DateTime(2026, 9, 21, 12, 21, 41, 207, DateTimeKind.Local).AddTicks(1644), new DateTime(2026, 9, 21, 11, 21, 41, 207, DateTimeKind.Local).AddTicks(1642) });

            migrationBuilder.UpdateData(
                table: "TestRequestTests",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 3,
                columns: new[] { "CompletionDateTime", "DueDate", "StartDateTime" },
                values: new object[] { new DateTime(2026, 9, 21, 11, 21, 41, 207, DateTimeKind.Local).AddTicks(1647), new DateTime(2026, 9, 21, 13, 21, 41, 207, DateTimeKind.Local).AddTicks(1648), new DateTime(2026, 9, 21, 9, 21, 41, 207, DateTimeKind.Local).AddTicks(1646) });

            migrationBuilder.UpdateData(
                table: "TestRequestTests",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 4,
                columns: new[] { "CompletionDateTime", "DueDate", "StartDateTime" },
                values: new object[] { new DateTime(2026, 9, 21, 11, 21, 41, 207, DateTimeKind.Local).AddTicks(1651), new DateTime(2026, 9, 21, 13, 21, 41, 207, DateTimeKind.Local).AddTicks(1652), new DateTime(2026, 9, 21, 11, 21, 41, 207, DateTimeKind.Local).AddTicks(1650) });

            migrationBuilder.UpdateData(
                table: "TestRequestTests",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 5,
                columns: new[] { "CompletionDateTime", "DueDate", "StartDateTime" },
                values: new object[] { new DateTime(2026, 9, 21, 10, 21, 41, 207, DateTimeKind.Local).AddTicks(1655), new DateTime(2026, 9, 21, 10, 21, 41, 207, DateTimeKind.Local).AddTicks(1656), new DateTime(2026, 9, 21, 8, 21, 41, 207, DateTimeKind.Local).AddTicks(1655) });

            migrationBuilder.UpdateData(
                table: "TestRequestTests",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 6,
                columns: new[] { "CompletionDateTime", "DueDate", "StartDateTime", "VerificationDateTime" },
                values: new object[] { new DateTime(2026, 9, 21, 9, 21, 41, 207, DateTimeKind.Local).AddTicks(1660), new DateTime(2026, 9, 21, 9, 21, 41, 207, DateTimeKind.Local).AddTicks(1661), new DateTime(2026, 9, 21, 8, 21, 41, 207, DateTimeKind.Local).AddTicks(1659), new DateTime(2026, 9, 21, 9, 51, 41, 207, DateTimeKind.Local).AddTicks(1664) });

            migrationBuilder.UpdateData(
                table: "TestRequestTests",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 7,
                columns: new[] { "CompletionDateTime", "DueDate", "StartDateTime", "VerificationDateTime" },
                values: new object[] { new DateTime(2026, 9, 21, 10, 51, 41, 207, DateTimeKind.Local).AddTicks(1669), new DateTime(2026, 9, 21, 11, 21, 41, 207, DateTimeKind.Local).AddTicks(1669), new DateTime(2026, 9, 21, 9, 21, 41, 207, DateTimeKind.Local).AddTicks(1668), new DateTime(2026, 9, 21, 10, 51, 41, 207, DateTimeKind.Local).AddTicks(1671) });

            migrationBuilder.UpdateData(
                table: "TestRequests",
                keyColumn: "TestRequestId",
                keyValue: 2,
                column: "Status",
                value: "Submitted");

            migrationBuilder.UpdateData(
                table: "TestRequests",
                keyColumn: "TestRequestId",
                keyValue: 3,
                column: "Status",
                value: "Submitted");

            migrationBuilder.UpdateData(
                table: "TestRequests",
                keyColumn: "TestRequestId",
                keyValue: 4,
                column: "Status",
                value: "Submitted");

            migrationBuilder.UpdateData(
                table: "TestRequests",
                keyColumn: "TestRequestId",
                keyValue: 5,
                column: "Status",
                value: "InProgress");

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 1,
                column: "VerificationDate",
                value: new DateTime(2026, 9, 21, 11, 21, 41, 207, DateTimeKind.Local).AddTicks(2090));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 2,
                column: "VerificationDate",
                value: new DateTime(2026, 9, 21, 11, 21, 41, 207, DateTimeKind.Local).AddTicks(2094));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 3,
                column: "VerificationDate",
                value: new DateTime(2026, 9, 21, 11, 21, 41, 207, DateTimeKind.Local).AddTicks(2095));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 4,
                column: "VerificationDate",
                value: new DateTime(2026, 9, 21, 11, 21, 41, 207, DateTimeKind.Local).AddTicks(2096));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 5,
                column: "VerificationDate",
                value: new DateTime(2026, 9, 21, 11, 21, 41, 207, DateTimeKind.Local).AddTicks(2098));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 6,
                column: "VerificationDate",
                value: new DateTime(2026, 9, 21, 11, 21, 41, 207, DateTimeKind.Local).AddTicks(2099));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 7,
                column: "VerificationDate",
                value: new DateTime(2026, 9, 21, 11, 21, 41, 207, DateTimeKind.Local).AddTicks(2107));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 8,
                column: "VerificationDate",
                value: new DateTime(2026, 9, 21, 11, 21, 41, 207, DateTimeKind.Local).AddTicks(2120));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 9,
                column: "VerificationDate",
                value: new DateTime(2026, 9, 21, 11, 21, 41, 207, DateTimeKind.Local).AddTicks(2122));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 10,
                column: "VerificationDate",
                value: new DateTime(2026, 9, 21, 11, 21, 41, 207, DateTimeKind.Local).AddTicks(2123));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 11,
                column: "VerificationDate",
                value: new DateTime(2026, 9, 21, 11, 21, 41, 207, DateTimeKind.Local).AddTicks(2124));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 12,
                column: "VerificationDate",
                value: new DateTime(2026, 9, 21, 11, 21, 41, 207, DateTimeKind.Local).AddTicks(2125));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 13,
                column: "VerificationDate",
                value: new DateTime(2026, 9, 21, 11, 21, 41, 207, DateTimeKind.Local).AddTicks(2127));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 14,
                column: "VerificationDate",
                value: new DateTime(2026, 9, 21, 11, 21, 41, 207, DateTimeKind.Local).AddTicks(2128));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 15,
                column: "VerificationDate",
                value: new DateTime(2026, 9, 21, 11, 21, 41, 207, DateTimeKind.Local).AddTicks(2129));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 16,
                column: "VerificationDate",
                value: new DateTime(2026, 9, 21, 11, 21, 41, 207, DateTimeKind.Local).AddTicks(2130));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 17,
                column: "VerificationDate",
                value: new DateTime(2026, 9, 21, 11, 21, 41, 207, DateTimeKind.Local).AddTicks(2131));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 18,
                column: "VerificationDate",
                value: new DateTime(2026, 9, 21, 11, 21, 41, 207, DateTimeKind.Local).AddTicks(2133));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 19,
                column: "VerificationDate",
                value: new DateTime(2026, 9, 21, 11, 21, 41, 207, DateTimeKind.Local).AddTicks(2134));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 20,
                column: "VerificationDate",
                value: new DateTime(2026, 9, 21, 11, 21, 41, 207, DateTimeKind.Local).AddTicks(2135));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 21,
                column: "VerificationDate",
                value: new DateTime(2026, 9, 21, 11, 21, 41, 207, DateTimeKind.Local).AddTicks(2136));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 22,
                column: "VerificationDate",
                value: new DateTime(2026, 9, 21, 11, 21, 41, 207, DateTimeKind.Local).AddTicks(2138));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Samples",
                keyColumn: "SampleId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Samples",
                keyColumn: "SampleId",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Samples",
                keyColumn: "SampleId",
                keyValue: 10);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateReceived",
                table: "Samples",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Samples",
                keyColumn: "SampleId",
                keyValue: 1,
                columns: new[] { "Barcode", "CollectedAt", "DateReceived", "IsReceived", "Status", "TechnicianId" },
                values: new object[] { "SMP-2024-000001", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 15, 10, 30, 0, 0, DateTimeKind.Unspecified), true, "Received", "2ec716df-770e-465e-a7b7-8d3926b560a2" });

            migrationBuilder.UpdateData(
                table: "Samples",
                keyColumn: "SampleId",
                keyValue: 2,
                columns: new[] { "Barcode", "CollectedAt", "DateReceived", "IsReceived", "SampleTypeId", "Status", "TechnicianId", "TestRequestId" },
                values: new object[] { "SMP-2024-000002", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 16, 11, 45, 0, 0, DateTimeKind.Unspecified), true, 2, "Received", "2ec716df-770e-465e-a7b7-8d3926b560a2", 1 });

            migrationBuilder.UpdateData(
                table: "Samples",
                keyColumn: "SampleId",
                keyValue: 3,
                columns: new[] { "Barcode", "CollectedAt", "DateReceived", "IsReceived", "SampleTypeId", "Status", "TechnicianId", "TestRequestId" },
                values: new object[] { "SMP-2024-000003", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 17, 9, 15, 0, 0, DateTimeKind.Unspecified), true, 3, "Received", "2ec716df-770e-465e-a7b7-8d3926b560a2", 2 });

            migrationBuilder.UpdateData(
                table: "Samples",
                keyColumn: "SampleId",
                keyValue: 4,
                columns: new[] { "Barcode", "CollectedAt", "DateReceived", "Status", "TestRequestId" },
                values: new object[] { "SMP-2024-000004", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 18, 14, 20, 0, 0, DateTimeKind.Unspecified), "Pending", 2 });

            migrationBuilder.UpdateData(
                table: "Samples",
                keyColumn: "SampleId",
                keyValue: 5,
                columns: new[] { "Barcode", "CollectedAt", "DateReceived", "IsReceived", "Status", "TechnicianId", "TestRequestId" },
                values: new object[] { "SMP-2024-000005", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 19, 8, 0, 0, 0, DateTimeKind.Unspecified), true, "Received", "2ec716df-770e-465e-a7b7-8d3926b560a2", 3 });

            migrationBuilder.UpdateData(
                table: "Samples",
                keyColumn: "SampleId",
                keyValue: 6,
                columns: new[] { "Barcode", "CollectedAt", "DateReceived", "SampleTypeId", "Status", "TestRequestId" },
                values: new object[] { "SMP-2024-000006", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 20, 13, 30, 0, 0, DateTimeKind.Unspecified), 5, "Pending", 3 });

            migrationBuilder.UpdateData(
                table: "Samples",
                keyColumn: "SampleId",
                keyValue: 9,
                columns: new[] { "Barcode", "CollectedAt", "DateReceived", "SampleTypeId", "Status", "TestRequestId" },
                values: new object[] { "SMP-2024-000009", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 23, 11, 20, 0, 0, DateTimeKind.Unspecified), 3, "Pending", 3 });

            migrationBuilder.UpdateData(
                table: "TestRequestTests",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1,
                columns: new[] { "CompletionDateTime", "DueDate", "StartDateTime" },
                values: new object[] { new DateTime(2026, 9, 8, 8, 11, 30, 488, DateTimeKind.Local).AddTicks(2286), new DateTime(2026, 9, 8, 10, 11, 30, 488, DateTimeKind.Local).AddTicks(2288), new DateTime(2026, 9, 8, 8, 11, 30, 488, DateTimeKind.Local).AddTicks(2270) });

            migrationBuilder.UpdateData(
                table: "TestRequestTests",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 2,
                columns: new[] { "CompletionDateTime", "DueDate", "StartDateTime" },
                values: new object[] { new DateTime(2026, 9, 8, 8, 11, 30, 488, DateTimeKind.Local).AddTicks(2300), new DateTime(2026, 9, 8, 9, 11, 30, 488, DateTimeKind.Local).AddTicks(2301), new DateTime(2026, 9, 8, 8, 11, 30, 488, DateTimeKind.Local).AddTicks(2300) });

            migrationBuilder.UpdateData(
                table: "TestRequestTests",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 3,
                columns: new[] { "CompletionDateTime", "DueDate", "StartDateTime" },
                values: new object[] { new DateTime(2026, 9, 8, 8, 11, 30, 488, DateTimeKind.Local).AddTicks(2305), new DateTime(2026, 9, 8, 10, 11, 30, 488, DateTimeKind.Local).AddTicks(2306), new DateTime(2026, 9, 8, 6, 11, 30, 488, DateTimeKind.Local).AddTicks(2304) });

            migrationBuilder.UpdateData(
                table: "TestRequestTests",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 4,
                columns: new[] { "CompletionDateTime", "DueDate", "StartDateTime" },
                values: new object[] { new DateTime(2026, 9, 8, 8, 11, 30, 488, DateTimeKind.Local).AddTicks(2309), new DateTime(2026, 9, 8, 10, 11, 30, 488, DateTimeKind.Local).AddTicks(2310), new DateTime(2026, 9, 8, 8, 11, 30, 488, DateTimeKind.Local).AddTicks(2308) });

            migrationBuilder.UpdateData(
                table: "TestRequestTests",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 5,
                columns: new[] { "CompletionDateTime", "DueDate", "StartDateTime" },
                values: new object[] { new DateTime(2026, 9, 8, 7, 11, 30, 488, DateTimeKind.Local).AddTicks(2314), new DateTime(2026, 9, 8, 7, 11, 30, 488, DateTimeKind.Local).AddTicks(2316), new DateTime(2026, 9, 8, 5, 11, 30, 488, DateTimeKind.Local).AddTicks(2313) });

            migrationBuilder.UpdateData(
                table: "TestRequestTests",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 6,
                columns: new[] { "CompletionDateTime", "DueDate", "StartDateTime", "VerificationDateTime" },
                values: new object[] { new DateTime(2026, 9, 8, 6, 11, 30, 488, DateTimeKind.Local).AddTicks(2321), new DateTime(2026, 9, 8, 6, 11, 30, 488, DateTimeKind.Local).AddTicks(2322), new DateTime(2026, 9, 8, 5, 11, 30, 488, DateTimeKind.Local).AddTicks(2319), new DateTime(2026, 9, 8, 6, 41, 30, 488, DateTimeKind.Local).AddTicks(2325) });

            migrationBuilder.UpdateData(
                table: "TestRequestTests",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 7,
                columns: new[] { "CompletionDateTime", "DueDate", "StartDateTime", "VerificationDateTime" },
                values: new object[] { new DateTime(2026, 9, 8, 7, 41, 30, 488, DateTimeKind.Local).AddTicks(2329), new DateTime(2026, 9, 8, 8, 11, 30, 488, DateTimeKind.Local).AddTicks(2331), new DateTime(2026, 9, 8, 6, 11, 30, 488, DateTimeKind.Local).AddTicks(2328), new DateTime(2026, 9, 8, 7, 41, 30, 488, DateTimeKind.Local).AddTicks(2333) });

            migrationBuilder.UpdateData(
                table: "TestRequests",
                keyColumn: "TestRequestId",
                keyValue: 2,
                column: "Status",
                value: "Samples Received");

            migrationBuilder.UpdateData(
                table: "TestRequests",
                keyColumn: "TestRequestId",
                keyValue: 3,
                column: "Status",
                value: "InProgress");

            migrationBuilder.UpdateData(
                table: "TestRequests",
                keyColumn: "TestRequestId",
                keyValue: 4,
                column: "Status",
                value: "Completed");

            migrationBuilder.UpdateData(
                table: "TestRequests",
                keyColumn: "TestRequestId",
                keyValue: 5,
                column: "Status",
                value: "Submitted");

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 1,
                column: "VerificationDate",
                value: new DateTime(2026, 9, 8, 8, 11, 30, 488, DateTimeKind.Local).AddTicks(2878));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 2,
                column: "VerificationDate",
                value: new DateTime(2026, 9, 8, 8, 11, 30, 488, DateTimeKind.Local).AddTicks(2882));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 3,
                column: "VerificationDate",
                value: new DateTime(2026, 9, 8, 8, 11, 30, 488, DateTimeKind.Local).AddTicks(2884));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 4,
                column: "VerificationDate",
                value: new DateTime(2026, 9, 8, 8, 11, 30, 488, DateTimeKind.Local).AddTicks(2886));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 5,
                column: "VerificationDate",
                value: new DateTime(2026, 9, 8, 8, 11, 30, 488, DateTimeKind.Local).AddTicks(2888));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 6,
                column: "VerificationDate",
                value: new DateTime(2026, 9, 8, 8, 11, 30, 488, DateTimeKind.Local).AddTicks(2890));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 7,
                column: "VerificationDate",
                value: new DateTime(2026, 9, 8, 8, 11, 30, 488, DateTimeKind.Local).AddTicks(2892));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 8,
                column: "VerificationDate",
                value: new DateTime(2026, 9, 8, 8, 11, 30, 488, DateTimeKind.Local).AddTicks(2893));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 9,
                column: "VerificationDate",
                value: new DateTime(2026, 9, 8, 8, 11, 30, 488, DateTimeKind.Local).AddTicks(2895));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 10,
                column: "VerificationDate",
                value: new DateTime(2026, 9, 8, 8, 11, 30, 488, DateTimeKind.Local).AddTicks(2896));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 11,
                column: "VerificationDate",
                value: new DateTime(2026, 9, 8, 8, 11, 30, 488, DateTimeKind.Local).AddTicks(2898));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 12,
                column: "VerificationDate",
                value: new DateTime(2026, 9, 8, 8, 11, 30, 488, DateTimeKind.Local).AddTicks(2899));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 13,
                column: "VerificationDate",
                value: new DateTime(2026, 9, 8, 8, 11, 30, 488, DateTimeKind.Local).AddTicks(2901));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 14,
                column: "VerificationDate",
                value: new DateTime(2026, 9, 8, 8, 11, 30, 488, DateTimeKind.Local).AddTicks(2902));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 15,
                column: "VerificationDate",
                value: new DateTime(2026, 9, 8, 8, 11, 30, 488, DateTimeKind.Local).AddTicks(2904));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 16,
                column: "VerificationDate",
                value: new DateTime(2026, 9, 8, 8, 11, 30, 488, DateTimeKind.Local).AddTicks(2905));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 17,
                column: "VerificationDate",
                value: new DateTime(2026, 9, 8, 8, 11, 30, 488, DateTimeKind.Local).AddTicks(2907));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 18,
                column: "VerificationDate",
                value: new DateTime(2026, 9, 8, 8, 11, 30, 488, DateTimeKind.Local).AddTicks(2908));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 19,
                column: "VerificationDate",
                value: new DateTime(2026, 9, 8, 8, 11, 30, 488, DateTimeKind.Local).AddTicks(2910));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 20,
                column: "VerificationDate",
                value: new DateTime(2026, 9, 8, 8, 11, 30, 488, DateTimeKind.Local).AddTicks(2912));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 21,
                column: "VerificationDate",
                value: new DateTime(2026, 9, 8, 8, 11, 30, 488, DateTimeKind.Local).AddTicks(2914));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 22,
                column: "VerificationDate",
                value: new DateTime(2026, 9, 8, 8, 11, 30, 488, DateTimeKind.Local).AddTicks(2916));
        }
    }
}
