using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NMBHLabSys.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSampleTypeModelIsActive : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "SampleTypes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Samples",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "Samples",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Samples",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "RestoredAt",
                table: "Samples",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RestoredBy",
                table: "Samples",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "SampleTypes",
                keyColumn: "SampleTypeId",
                keyValue: 1,
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                table: "SampleTypes",
                keyColumn: "SampleTypeId",
                keyValue: 2,
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                table: "SampleTypes",
                keyColumn: "SampleTypeId",
                keyValue: 3,
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                table: "SampleTypes",
                keyColumn: "SampleTypeId",
                keyValue: 4,
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                table: "SampleTypes",
                keyColumn: "SampleTypeId",
                keyValue: 5,
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                table: "SampleTypes",
                keyColumn: "SampleTypeId",
                keyValue: 6,
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                table: "SampleTypes",
                keyColumn: "SampleTypeId",
                keyValue: 7,
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                table: "SampleTypes",
                keyColumn: "SampleTypeId",
                keyValue: 8,
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                table: "SampleTypes",
                keyColumn: "SampleTypeId",
                keyValue: 9,
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                table: "SampleTypes",
                keyColumn: "SampleTypeId",
                keyValue: 10,
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                table: "Samples",
                keyColumn: "SampleId",
                keyValue: 1,
                columns: new[] { "DeletedAt", "DeletedBy", "IsDeleted", "RestoredAt", "RestoredBy" },
                values: new object[] { null, null, false, null, null });

            migrationBuilder.UpdateData(
                table: "Samples",
                keyColumn: "SampleId",
                keyValue: 2,
                columns: new[] { "DeletedAt", "DeletedBy", "IsDeleted", "RestoredAt", "RestoredBy" },
                values: new object[] { null, null, false, null, null });

            migrationBuilder.UpdateData(
                table: "Samples",
                keyColumn: "SampleId",
                keyValue: 3,
                columns: new[] { "DeletedAt", "DeletedBy", "IsDeleted", "RestoredAt", "RestoredBy" },
                values: new object[] { null, null, false, null, null });

            migrationBuilder.UpdateData(
                table: "Samples",
                keyColumn: "SampleId",
                keyValue: 4,
                columns: new[] { "DeletedAt", "DeletedBy", "IsDeleted", "RestoredAt", "RestoredBy" },
                values: new object[] { null, null, false, null, null });

            migrationBuilder.UpdateData(
                table: "Samples",
                keyColumn: "SampleId",
                keyValue: 5,
                columns: new[] { "DeletedAt", "DeletedBy", "IsDeleted", "RestoredAt", "RestoredBy" },
                values: new object[] { null, null, false, null, null });

            migrationBuilder.UpdateData(
                table: "Samples",
                keyColumn: "SampleId",
                keyValue: 6,
                columns: new[] { "DeletedAt", "DeletedBy", "IsDeleted", "RestoredAt", "RestoredBy" },
                values: new object[] { null, null, false, null, null });

            migrationBuilder.UpdateData(
                table: "Samples",
                keyColumn: "SampleId",
                keyValue: 9,
                columns: new[] { "DeletedAt", "DeletedBy", "IsDeleted", "RestoredAt", "RestoredBy" },
                values: new object[] { null, null, false, null, null });

            migrationBuilder.UpdateData(
                table: "TestRequestTests",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1,
                columns: new[] { "CompletionDateTime", "DueDate", "StartDateTime" },
                values: new object[] { new DateTime(2026, 5, 16, 21, 35, 38, 76, DateTimeKind.Local).AddTicks(9396), new DateTime(2026, 5, 16, 23, 35, 38, 76, DateTimeKind.Local).AddTicks(9396), new DateTime(2026, 5, 16, 21, 35, 38, 76, DateTimeKind.Local).AddTicks(9385) });

            migrationBuilder.UpdateData(
                table: "TestRequestTests",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 2,
                columns: new[] { "CompletionDateTime", "DueDate", "StartDateTime" },
                values: new object[] { new DateTime(2026, 5, 16, 21, 35, 38, 76, DateTimeKind.Local).AddTicks(9407), new DateTime(2026, 5, 16, 22, 35, 38, 76, DateTimeKind.Local).AddTicks(9407), new DateTime(2026, 5, 16, 21, 35, 38, 76, DateTimeKind.Local).AddTicks(9406) });

            migrationBuilder.UpdateData(
                table: "TestRequestTests",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 3,
                columns: new[] { "CompletionDateTime", "DueDate", "StartDateTime" },
                values: new object[] { new DateTime(2026, 5, 16, 21, 35, 38, 76, DateTimeKind.Local).AddTicks(9411), new DateTime(2026, 5, 16, 23, 35, 38, 76, DateTimeKind.Local).AddTicks(9411), new DateTime(2026, 5, 16, 19, 35, 38, 76, DateTimeKind.Local).AddTicks(9410) });

            migrationBuilder.UpdateData(
                table: "TestRequestTests",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 4,
                columns: new[] { "CompletionDateTime", "DueDate", "StartDateTime" },
                values: new object[] { new DateTime(2026, 5, 16, 21, 35, 38, 76, DateTimeKind.Local).AddTicks(9416), new DateTime(2026, 5, 16, 23, 35, 38, 76, DateTimeKind.Local).AddTicks(9416), new DateTime(2026, 5, 16, 21, 35, 38, 76, DateTimeKind.Local).AddTicks(9415) });

            migrationBuilder.UpdateData(
                table: "TestRequestTests",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 5,
                columns: new[] { "CompletionDateTime", "DueDate", "StartDateTime" },
                values: new object[] { new DateTime(2026, 5, 16, 20, 35, 38, 76, DateTimeKind.Local).AddTicks(9420), new DateTime(2026, 5, 16, 20, 35, 38, 76, DateTimeKind.Local).AddTicks(9421), new DateTime(2026, 5, 16, 18, 35, 38, 76, DateTimeKind.Local).AddTicks(9419) });

            migrationBuilder.UpdateData(
                table: "TestRequestTests",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 6,
                columns: new[] { "CompletionDateTime", "DueDate", "StartDateTime", "VerificationDateTime" },
                values: new object[] { new DateTime(2026, 5, 16, 19, 35, 38, 76, DateTimeKind.Local).AddTicks(9426), new DateTime(2026, 5, 16, 19, 35, 38, 76, DateTimeKind.Local).AddTicks(9427), new DateTime(2026, 5, 16, 18, 35, 38, 76, DateTimeKind.Local).AddTicks(9425), new DateTime(2026, 5, 16, 20, 5, 38, 76, DateTimeKind.Local).AddTicks(9430) });

            migrationBuilder.UpdateData(
                table: "TestRequestTests",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 7,
                columns: new[] { "CompletionDateTime", "DueDate", "StartDateTime", "VerificationDateTime" },
                values: new object[] { new DateTime(2026, 5, 16, 21, 5, 38, 76, DateTimeKind.Local).AddTicks(9437), new DateTime(2026, 5, 16, 21, 35, 38, 76, DateTimeKind.Local).AddTicks(9438), new DateTime(2026, 5, 16, 19, 35, 38, 76, DateTimeKind.Local).AddTicks(9436), new DateTime(2026, 5, 16, 21, 5, 38, 76, DateTimeKind.Local).AddTicks(9440) });

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 1,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 21, 35, 38, 76, DateTimeKind.Local).AddTicks(9826));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 2,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 21, 35, 38, 76, DateTimeKind.Local).AddTicks(9829));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 3,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 21, 35, 38, 76, DateTimeKind.Local).AddTicks(9830));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 4,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 21, 35, 38, 76, DateTimeKind.Local).AddTicks(9831));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 5,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 21, 35, 38, 76, DateTimeKind.Local).AddTicks(9832));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 6,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 21, 35, 38, 76, DateTimeKind.Local).AddTicks(9834));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 7,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 21, 35, 38, 76, DateTimeKind.Local).AddTicks(9836));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 8,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 21, 35, 38, 76, DateTimeKind.Local).AddTicks(9837));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 9,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 21, 35, 38, 76, DateTimeKind.Local).AddTicks(9838));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 10,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 21, 35, 38, 76, DateTimeKind.Local).AddTicks(9839));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 11,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 21, 35, 38, 76, DateTimeKind.Local).AddTicks(9840));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 12,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 21, 35, 38, 76, DateTimeKind.Local).AddTicks(9841));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 13,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 21, 35, 38, 76, DateTimeKind.Local).AddTicks(9842));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 14,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 21, 35, 38, 76, DateTimeKind.Local).AddTicks(9844));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 15,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 21, 35, 38, 76, DateTimeKind.Local).AddTicks(9845));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 16,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 21, 35, 38, 76, DateTimeKind.Local).AddTicks(9846));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 17,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 21, 35, 38, 76, DateTimeKind.Local).AddTicks(9847));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 18,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 21, 35, 38, 76, DateTimeKind.Local).AddTicks(9848));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 19,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 21, 35, 38, 76, DateTimeKind.Local).AddTicks(9849));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 20,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 21, 35, 38, 76, DateTimeKind.Local).AddTicks(9850));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 21,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 21, 35, 38, 76, DateTimeKind.Local).AddTicks(9852));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 22,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 21, 35, 38, 76, DateTimeKind.Local).AddTicks(9853));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "SampleTypes");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Samples");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Samples");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Samples");

            migrationBuilder.DropColumn(
                name: "RestoredAt",
                table: "Samples");

            migrationBuilder.DropColumn(
                name: "RestoredBy",
                table: "Samples");

            migrationBuilder.UpdateData(
                table: "TestRequestTests",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1,
                columns: new[] { "CompletionDateTime", "DueDate", "StartDateTime" },
                values: new object[] { new DateTime(2026, 5, 16, 20, 25, 55, 252, DateTimeKind.Local).AddTicks(6125), new DateTime(2026, 5, 16, 22, 25, 55, 252, DateTimeKind.Local).AddTicks(6126), new DateTime(2026, 5, 16, 20, 25, 55, 252, DateTimeKind.Local).AddTicks(6116) });

            migrationBuilder.UpdateData(
                table: "TestRequestTests",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 2,
                columns: new[] { "CompletionDateTime", "DueDate", "StartDateTime" },
                values: new object[] { new DateTime(2026, 5, 16, 20, 25, 55, 252, DateTimeKind.Local).AddTicks(6135), new DateTime(2026, 5, 16, 21, 25, 55, 252, DateTimeKind.Local).AddTicks(6135), new DateTime(2026, 5, 16, 20, 25, 55, 252, DateTimeKind.Local).AddTicks(6134) });

            migrationBuilder.UpdateData(
                table: "TestRequestTests",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 3,
                columns: new[] { "CompletionDateTime", "DueDate", "StartDateTime" },
                values: new object[] { new DateTime(2026, 5, 16, 20, 25, 55, 252, DateTimeKind.Local).AddTicks(6139), new DateTime(2026, 5, 16, 22, 25, 55, 252, DateTimeKind.Local).AddTicks(6139), new DateTime(2026, 5, 16, 18, 25, 55, 252, DateTimeKind.Local).AddTicks(6138) });

            migrationBuilder.UpdateData(
                table: "TestRequestTests",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 4,
                columns: new[] { "CompletionDateTime", "DueDate", "StartDateTime" },
                values: new object[] { new DateTime(2026, 5, 16, 20, 25, 55, 252, DateTimeKind.Local).AddTicks(6143), new DateTime(2026, 5, 16, 22, 25, 55, 252, DateTimeKind.Local).AddTicks(6143), new DateTime(2026, 5, 16, 20, 25, 55, 252, DateTimeKind.Local).AddTicks(6142) });

            migrationBuilder.UpdateData(
                table: "TestRequestTests",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 5,
                columns: new[] { "CompletionDateTime", "DueDate", "StartDateTime" },
                values: new object[] { new DateTime(2026, 5, 16, 19, 25, 55, 252, DateTimeKind.Local).AddTicks(6147), new DateTime(2026, 5, 16, 19, 25, 55, 252, DateTimeKind.Local).AddTicks(6148), new DateTime(2026, 5, 16, 17, 25, 55, 252, DateTimeKind.Local).AddTicks(6146) });

            migrationBuilder.UpdateData(
                table: "TestRequestTests",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 6,
                columns: new[] { "CompletionDateTime", "DueDate", "StartDateTime", "VerificationDateTime" },
                values: new object[] { new DateTime(2026, 5, 16, 18, 25, 55, 252, DateTimeKind.Local).AddTicks(6152), new DateTime(2026, 5, 16, 18, 25, 55, 252, DateTimeKind.Local).AddTicks(6153), new DateTime(2026, 5, 16, 17, 25, 55, 252, DateTimeKind.Local).AddTicks(6151), new DateTime(2026, 5, 16, 18, 55, 55, 252, DateTimeKind.Local).AddTicks(6156) });

            migrationBuilder.UpdateData(
                table: "TestRequestTests",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 7,
                columns: new[] { "CompletionDateTime", "DueDate", "StartDateTime", "VerificationDateTime" },
                values: new object[] { new DateTime(2026, 5, 16, 19, 55, 55, 252, DateTimeKind.Local).AddTicks(6159), new DateTime(2026, 5, 16, 20, 25, 55, 252, DateTimeKind.Local).AddTicks(6161), new DateTime(2026, 5, 16, 18, 25, 55, 252, DateTimeKind.Local).AddTicks(6158), new DateTime(2026, 5, 16, 19, 55, 55, 252, DateTimeKind.Local).AddTicks(6162) });

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 1,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 20, 25, 55, 252, DateTimeKind.Local).AddTicks(6502));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 2,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 20, 25, 55, 252, DateTimeKind.Local).AddTicks(6504));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 3,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 20, 25, 55, 252, DateTimeKind.Local).AddTicks(6506));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 4,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 20, 25, 55, 252, DateTimeKind.Local).AddTicks(6507));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 5,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 20, 25, 55, 252, DateTimeKind.Local).AddTicks(6508));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 6,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 20, 25, 55, 252, DateTimeKind.Local).AddTicks(6509));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 7,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 20, 25, 55, 252, DateTimeKind.Local).AddTicks(6510));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 8,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 20, 25, 55, 252, DateTimeKind.Local).AddTicks(6511));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 9,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 20, 25, 55, 252, DateTimeKind.Local).AddTicks(6513));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 10,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 20, 25, 55, 252, DateTimeKind.Local).AddTicks(6514));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 11,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 20, 25, 55, 252, DateTimeKind.Local).AddTicks(6515));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 12,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 20, 25, 55, 252, DateTimeKind.Local).AddTicks(6516));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 13,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 20, 25, 55, 252, DateTimeKind.Local).AddTicks(6517));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 14,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 20, 25, 55, 252, DateTimeKind.Local).AddTicks(6518));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 15,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 20, 25, 55, 252, DateTimeKind.Local).AddTicks(6519));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 16,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 20, 25, 55, 252, DateTimeKind.Local).AddTicks(6521));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 17,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 20, 25, 55, 252, DateTimeKind.Local).AddTicks(6522));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 18,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 20, 25, 55, 252, DateTimeKind.Local).AddTicks(6523));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 19,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 20, 25, 55, 252, DateTimeKind.Local).AddTicks(6524));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 20,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 20, 25, 55, 252, DateTimeKind.Local).AddTicks(6525));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 21,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 20, 25, 55, 252, DateTimeKind.Local).AddTicks(6526));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 22,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 20, 25, 55, 252, DateTimeKind.Local).AddTicks(6527));
        }
    }
}
