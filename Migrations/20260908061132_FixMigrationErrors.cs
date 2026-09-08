using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NMBHLabSys.Migrations
{
    /// <inheritdoc />
    public partial class FixMigrationErrors : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Medications_MedicationCategory_MedicationCategoryId",
                table: "Medications");

            migrationBuilder.DropForeignKey(
                name: "FK_ResultModel_AspNetUsers_TechnicianId",
                table: "ResultModel");

            migrationBuilder.DropForeignKey(
                name: "FK_ResultModel_TestRequestItems_TestRequestItemID",
                table: "ResultModel");

            migrationBuilder.DropForeignKey(
                name: "FK_ResultModel_TestRequestTests_TestRequestTestTypeModelTestRequestTestTypeId",
                table: "ResultModel");

            migrationBuilder.DropForeignKey(
                name: "FK_ResultModel_TestRequests_RequestTestId",
                table: "ResultModel");

            migrationBuilder.DropForeignKey(
                name: "FK_Samples_LabTechnician_ReceivedByTechnicianId",
                table: "Samples");

            migrationBuilder.DropForeignKey(
                name: "FK_TestTypes_TestCategoryModel_TestCategoryId",
                table: "TestTypes");

            migrationBuilder.DropIndex(
                name: "IX_Samples_ReceivedByTechnicianId",
                table: "Samples");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TestCategoryModel",
                table: "TestCategoryModel");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ResultModel",
                table: "ResultModel");

            migrationBuilder.DropIndex(
                name: "IX_ResultModel_RequestTestId",
                table: "ResultModel");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MedicationCategory",
                table: "MedicationCategory");

            migrationBuilder.DropColumn(
                name: "ReceivedByTechnicianId",
                table: "Samples");

            migrationBuilder.DropColumn(
                name: "RequestTestId",
                table: "ResultModel");

            migrationBuilder.DropColumn(
                name: "ResultDateTime",
                table: "ResultModel");

            migrationBuilder.DropColumn(
                name: "ResultValue",
                table: "ResultModel");

            migrationBuilder.DropColumn(
                name: "VerifiedDateTime",
                table: "ResultModel");

            migrationBuilder.RenameTable(
                name: "TestCategoryModel",
                newName: "TestCategories");

            migrationBuilder.RenameTable(
                name: "ResultModel",
                newName: "Results");

            migrationBuilder.RenameTable(
                name: "MedicationCategory",
                newName: "MedicationCategories");

            migrationBuilder.RenameColumn(
                name: "ReceivedAt",
                table: "Samples",
                newName: "RestoredAt");

            migrationBuilder.RenameColumn(
                name: "TestRequestItemID",
                table: "Results",
                newName: "TestRequestItemId");

            migrationBuilder.RenameIndex(
                name: "IX_ResultModel_TestRequestTestTypeModelTestRequestTestTypeId",
                table: "Results",
                newName: "IX_Results_TestRequestTestTypeModelTestRequestTestTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_ResultModel_TestRequestItemID",
                table: "Results",
                newName: "IX_Results_TestRequestItemId");

            migrationBuilder.RenameIndex(
                name: "IX_ResultModel_TechnicianId",
                table: "Results",
                newName: "IX_Results_TechnicianId");

            migrationBuilder.AddColumn<int>(
                name: "TestTypeId",
                table: "TestRequests",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Samples",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DeletedAt",
                table: "Samples",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<string>(
                name: "RestoredBy",
                table: "Samples",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_TestCategories",
                table: "TestCategories",
                column: "TestCategoryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Results",
                table: "Results",
                column: "ResultId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MedicationCategories",
                table: "MedicationCategories",
                column: "MedicationCategoryId");

            migrationBuilder.UpdateData(
                table: "Samples",
                keyColumn: "SampleId",
                keyValue: 1,
                columns: new[] { "DeletedAt", "RestoredBy" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Samples",
                keyColumn: "SampleId",
                keyValue: 2,
                columns: new[] { "DeletedAt", "RestoredBy" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Samples",
                keyColumn: "SampleId",
                keyValue: 3,
                columns: new[] { "DeletedAt", "RestoredBy" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Samples",
                keyColumn: "SampleId",
                keyValue: 4,
                columns: new[] { "DeletedAt", "RestoredBy" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Samples",
                keyColumn: "SampleId",
                keyValue: 5,
                columns: new[] { "DeletedAt", "RestoredBy" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Samples",
                keyColumn: "SampleId",
                keyValue: 6,
                columns: new[] { "DeletedAt", "RestoredBy" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Samples",
                keyColumn: "SampleId",
                keyValue: 9,
                columns: new[] { "DeletedAt", "RestoredBy" },
                values: new object[] { null, null });

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
                keyValue: 1,
                column: "TestTypeId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "TestRequests",
                keyColumn: "TestRequestId",
                keyValue: 2,
                column: "TestTypeId",
                value: 16);

            migrationBuilder.UpdateData(
                table: "TestRequests",
                keyColumn: "TestRequestId",
                keyValue: 3,
                column: "TestTypeId",
                value: 6);

            migrationBuilder.UpdateData(
                table: "TestRequests",
                keyColumn: "TestRequestId",
                keyValue: 4,
                column: "TestTypeId",
                value: 12);

            migrationBuilder.UpdateData(
                table: "TestRequests",
                keyColumn: "TestRequestId",
                keyValue: 5,
                column: "TestTypeId",
                value: 13);

            migrationBuilder.UpdateData(
                table: "TestRequests",
                keyColumn: "TestRequestId",
                keyValue: 6,
                column: "TestTypeId",
                value: 14);

            migrationBuilder.UpdateData(
                table: "TestRequests",
                keyColumn: "TestRequestId",
                keyValue: 7,
                column: "TestTypeId",
                value: 6);

            migrationBuilder.UpdateData(
                table: "TestRequests",
                keyColumn: "TestRequestId",
                keyValue: 8,
                column: "TestTypeId",
                value: 17);

            migrationBuilder.UpdateData(
                table: "TestRequests",
                keyColumn: "TestRequestId",
                keyValue: 9,
                column: "TestTypeId",
                value: 20);

            migrationBuilder.UpdateData(
                table: "TestRequests",
                keyColumn: "TestRequestId",
                keyValue: 10,
                column: "TestTypeId",
                value: 1);

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

            migrationBuilder.CreateIndex(
                name: "IX_TestRequests_TestTypeId",
                table: "TestRequests",
                column: "TestTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Medications_MedicationCategories_MedicationCategoryId",
                table: "Medications",
                column: "MedicationCategoryId",
                principalTable: "MedicationCategories",
                principalColumn: "MedicationCategoryId",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Results_AspNetUsers_TechnicianId",
                table: "Results",
                column: "TechnicianId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Results_TestRequestItems_TestRequestItemId",
                table: "Results",
                column: "TestRequestItemId",
                principalTable: "TestRequestItems",
                principalColumn: "TestRequestItemId",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Results_TestRequestTests_TestRequestTestTypeModelTestRequestTestTypeId",
                table: "Results",
                column: "TestRequestTestTypeModelTestRequestTestTypeId",
                principalTable: "TestRequestTests",
                principalColumn: "TestRequestTestTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_TestRequests_TestTypes_TestTypeId",
                table: "TestRequests",
                column: "TestTypeId",
                principalTable: "TestTypes",
                principalColumn: "TestTypeId",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_TestTypes_TestCategories_TestCategoryId",
                table: "TestTypes",
                column: "TestCategoryId",
                principalTable: "TestCategories",
                principalColumn: "TestCategoryId",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Medications_MedicationCategories_MedicationCategoryId",
                table: "Medications");

            migrationBuilder.DropForeignKey(
                name: "FK_Results_AspNetUsers_TechnicianId",
                table: "Results");

            migrationBuilder.DropForeignKey(
                name: "FK_Results_TestRequestItems_TestRequestItemId",
                table: "Results");

            migrationBuilder.DropForeignKey(
                name: "FK_Results_TestRequestTests_TestRequestTestTypeModelTestRequestTestTypeId",
                table: "Results");

            migrationBuilder.DropForeignKey(
                name: "FK_TestRequests_TestTypes_TestTypeId",
                table: "TestRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_TestTypes_TestCategories_TestCategoryId",
                table: "TestTypes");

            migrationBuilder.DropIndex(
                name: "IX_TestRequests_TestTypeId",
                table: "TestRequests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TestCategories",
                table: "TestCategories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Results",
                table: "Results");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MedicationCategories",
                table: "MedicationCategories");

            migrationBuilder.DropColumn(
                name: "TestTypeId",
                table: "TestRequests");

            migrationBuilder.DropColumn(
                name: "RestoredBy",
                table: "Samples");

            migrationBuilder.RenameTable(
                name: "TestCategories",
                newName: "TestCategoryModel");

            migrationBuilder.RenameTable(
                name: "Results",
                newName: "ResultModel");

            migrationBuilder.RenameTable(
                name: "MedicationCategories",
                newName: "MedicationCategory");

            migrationBuilder.RenameColumn(
                name: "RestoredAt",
                table: "Samples",
                newName: "ReceivedAt");

            migrationBuilder.RenameColumn(
                name: "TestRequestItemId",
                table: "ResultModel",
                newName: "TestRequestItemID");

            migrationBuilder.RenameIndex(
                name: "IX_Results_TestRequestTestTypeModelTestRequestTestTypeId",
                table: "ResultModel",
                newName: "IX_ResultModel_TestRequestTestTypeModelTestRequestTestTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_Results_TestRequestItemId",
                table: "ResultModel",
                newName: "IX_ResultModel_TestRequestItemID");

            migrationBuilder.RenameIndex(
                name: "IX_Results_TechnicianId",
                table: "ResultModel",
                newName: "IX_ResultModel_TechnicianId");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Samples",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DeletedAt",
                table: "Samples",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReceivedByTechnicianId",
                table: "Samples",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RequestTestId",
                table: "ResultModel",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ResultDateTime",
                table: "ResultModel",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ResultValue",
                table: "ResultModel",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "VerifiedDateTime",
                table: "ResultModel",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddPrimaryKey(
                name: "PK_TestCategoryModel",
                table: "TestCategoryModel",
                column: "TestCategoryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ResultModel",
                table: "ResultModel",
                column: "ResultId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MedicationCategory",
                table: "MedicationCategory",
                column: "MedicationCategoryId");

            migrationBuilder.UpdateData(
                table: "Samples",
                keyColumn: "SampleId",
                keyValue: 1,
                columns: new[] { "DeletedAt", "ReceivedByTechnicianId" },
                values: new object[] { new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null });

            migrationBuilder.UpdateData(
                table: "Samples",
                keyColumn: "SampleId",
                keyValue: 2,
                columns: new[] { "DeletedAt", "ReceivedByTechnicianId" },
                values: new object[] { new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null });

            migrationBuilder.UpdateData(
                table: "Samples",
                keyColumn: "SampleId",
                keyValue: 3,
                columns: new[] { "DeletedAt", "ReceivedByTechnicianId" },
                values: new object[] { new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null });

            migrationBuilder.UpdateData(
                table: "Samples",
                keyColumn: "SampleId",
                keyValue: 4,
                columns: new[] { "DeletedAt", "ReceivedByTechnicianId" },
                values: new object[] { new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null });

            migrationBuilder.UpdateData(
                table: "Samples",
                keyColumn: "SampleId",
                keyValue: 5,
                columns: new[] { "DeletedAt", "ReceivedByTechnicianId" },
                values: new object[] { new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null });

            migrationBuilder.UpdateData(
                table: "Samples",
                keyColumn: "SampleId",
                keyValue: 6,
                columns: new[] { "DeletedAt", "ReceivedByTechnicianId" },
                values: new object[] { new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null });

            migrationBuilder.UpdateData(
                table: "Samples",
                keyColumn: "SampleId",
                keyValue: 9,
                columns: new[] { "DeletedAt", "ReceivedByTechnicianId" },
                values: new object[] { new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null });

            migrationBuilder.UpdateData(
                table: "TestRequestTests",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1,
                columns: new[] { "CompletionDateTime", "DueDate", "StartDateTime" },
                values: new object[] { new DateTime(2026, 5, 20, 12, 55, 59, 529, DateTimeKind.Local).AddTicks(4123), new DateTime(2026, 5, 20, 14, 55, 59, 529, DateTimeKind.Local).AddTicks(4124), new DateTime(2026, 5, 20, 12, 55, 59, 529, DateTimeKind.Local).AddTicks(4115) });

            migrationBuilder.UpdateData(
                table: "TestRequestTests",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 2,
                columns: new[] { "CompletionDateTime", "DueDate", "StartDateTime" },
                values: new object[] { new DateTime(2026, 5, 20, 12, 55, 59, 529, DateTimeKind.Local).AddTicks(4133), new DateTime(2026, 5, 20, 13, 55, 59, 529, DateTimeKind.Local).AddTicks(4133), new DateTime(2026, 5, 20, 12, 55, 59, 529, DateTimeKind.Local).AddTicks(4132) });

            migrationBuilder.UpdateData(
                table: "TestRequestTests",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 3,
                columns: new[] { "CompletionDateTime", "DueDate", "StartDateTime" },
                values: new object[] { new DateTime(2026, 5, 20, 12, 55, 59, 529, DateTimeKind.Local).AddTicks(4135), new DateTime(2026, 5, 20, 14, 55, 59, 529, DateTimeKind.Local).AddTicks(4136), new DateTime(2026, 5, 20, 10, 55, 59, 529, DateTimeKind.Local).AddTicks(4135) });

            migrationBuilder.UpdateData(
                table: "TestRequestTests",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 4,
                columns: new[] { "CompletionDateTime", "DueDate", "StartDateTime" },
                values: new object[] { new DateTime(2026, 5, 20, 12, 55, 59, 529, DateTimeKind.Local).AddTicks(4138), new DateTime(2026, 5, 20, 14, 55, 59, 529, DateTimeKind.Local).AddTicks(4139), new DateTime(2026, 5, 20, 12, 55, 59, 529, DateTimeKind.Local).AddTicks(4138) });

            migrationBuilder.UpdateData(
                table: "TestRequestTests",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 5,
                columns: new[] { "CompletionDateTime", "DueDate", "StartDateTime" },
                values: new object[] { new DateTime(2026, 5, 20, 11, 55, 59, 529, DateTimeKind.Local).AddTicks(4141), new DateTime(2026, 5, 20, 11, 55, 59, 529, DateTimeKind.Local).AddTicks(4141), new DateTime(2026, 5, 20, 9, 55, 59, 529, DateTimeKind.Local).AddTicks(4140) });

            migrationBuilder.UpdateData(
                table: "TestRequestTests",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 6,
                columns: new[] { "CompletionDateTime", "DueDate", "StartDateTime", "VerificationDateTime" },
                values: new object[] { new DateTime(2026, 5, 20, 10, 55, 59, 529, DateTimeKind.Local).AddTicks(4145), new DateTime(2026, 5, 20, 10, 55, 59, 529, DateTimeKind.Local).AddTicks(4145), new DateTime(2026, 5, 20, 9, 55, 59, 529, DateTimeKind.Local).AddTicks(4144), new DateTime(2026, 5, 20, 11, 25, 59, 529, DateTimeKind.Local).AddTicks(4147) });

            migrationBuilder.UpdateData(
                table: "TestRequestTests",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 7,
                columns: new[] { "CompletionDateTime", "DueDate", "StartDateTime", "VerificationDateTime" },
                values: new object[] { new DateTime(2026, 5, 20, 12, 25, 59, 529, DateTimeKind.Local).AddTicks(4149), new DateTime(2026, 5, 20, 12, 55, 59, 529, DateTimeKind.Local).AddTicks(4150), new DateTime(2026, 5, 20, 10, 55, 59, 529, DateTimeKind.Local).AddTicks(4148), new DateTime(2026, 5, 20, 12, 25, 59, 529, DateTimeKind.Local).AddTicks(4151) });

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 1,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 20, 12, 55, 59, 529, DateTimeKind.Local).AddTicks(4404));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 2,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 20, 12, 55, 59, 529, DateTimeKind.Local).AddTicks(4406));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 3,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 20, 12, 55, 59, 529, DateTimeKind.Local).AddTicks(4408));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 4,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 20, 12, 55, 59, 529, DateTimeKind.Local).AddTicks(4409));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 5,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 20, 12, 55, 59, 529, DateTimeKind.Local).AddTicks(4410));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 6,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 20, 12, 55, 59, 529, DateTimeKind.Local).AddTicks(4411));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 7,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 20, 12, 55, 59, 529, DateTimeKind.Local).AddTicks(4412));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 8,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 20, 12, 55, 59, 529, DateTimeKind.Local).AddTicks(4413));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 9,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 20, 12, 55, 59, 529, DateTimeKind.Local).AddTicks(4414));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 10,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 20, 12, 55, 59, 529, DateTimeKind.Local).AddTicks(4414));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 11,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 20, 12, 55, 59, 529, DateTimeKind.Local).AddTicks(4415));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 12,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 20, 12, 55, 59, 529, DateTimeKind.Local).AddTicks(4416));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 13,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 20, 12, 55, 59, 529, DateTimeKind.Local).AddTicks(4417));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 14,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 20, 12, 55, 59, 529, DateTimeKind.Local).AddTicks(4418));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 15,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 20, 12, 55, 59, 529, DateTimeKind.Local).AddTicks(4419));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 16,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 20, 12, 55, 59, 529, DateTimeKind.Local).AddTicks(4420));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 17,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 20, 12, 55, 59, 529, DateTimeKind.Local).AddTicks(4421));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 18,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 20, 12, 55, 59, 529, DateTimeKind.Local).AddTicks(4422));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 19,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 20, 12, 55, 59, 529, DateTimeKind.Local).AddTicks(4423));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 20,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 20, 12, 55, 59, 529, DateTimeKind.Local).AddTicks(4424));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 21,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 20, 12, 55, 59, 529, DateTimeKind.Local).AddTicks(4425));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 22,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 20, 12, 55, 59, 529, DateTimeKind.Local).AddTicks(4426));

            migrationBuilder.CreateIndex(
                name: "IX_Samples_ReceivedByTechnicianId",
                table: "Samples",
                column: "ReceivedByTechnicianId");

            migrationBuilder.CreateIndex(
                name: "IX_ResultModel_RequestTestId",
                table: "ResultModel",
                column: "RequestTestId");

            migrationBuilder.AddForeignKey(
                name: "FK_Medications_MedicationCategory_MedicationCategoryId",
                table: "Medications",
                column: "MedicationCategoryId",
                principalTable: "MedicationCategory",
                principalColumn: "MedicationCategoryId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ResultModel_AspNetUsers_TechnicianId",
                table: "ResultModel",
                column: "TechnicianId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ResultModel_TestRequestItems_TestRequestItemID",
                table: "ResultModel",
                column: "TestRequestItemID",
                principalTable: "TestRequestItems",
                principalColumn: "TestRequestItemId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ResultModel_TestRequestTests_TestRequestTestTypeModelTestRequestTestTypeId",
                table: "ResultModel",
                column: "TestRequestTestTypeModelTestRequestTestTypeId",
                principalTable: "TestRequestTests",
                principalColumn: "TestRequestTestTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_ResultModel_TestRequests_RequestTestId",
                table: "ResultModel",
                column: "RequestTestId",
                principalTable: "TestRequests",
                principalColumn: "TestRequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_Samples_LabTechnician_ReceivedByTechnicianId",
                table: "Samples",
                column: "ReceivedByTechnicianId",
                principalTable: "LabTechnician",
                principalColumn: "LabTechnicianID");

            migrationBuilder.AddForeignKey(
                name: "FK_TestTypes_TestCategoryModel_TestCategoryId",
                table: "TestTypes",
                column: "TestCategoryId",
                principalTable: "TestCategoryModel",
                principalColumn: "TestCategoryId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
