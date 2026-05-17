using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace NMBHLabSys.Migrations
{
    /// <inheritdoc />
    public partial class PushTechncianId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Results_TestRequestTestTypeModel_TestRequestTestTypeModelTestRequestTestTypeId",
                table: "Results");

            migrationBuilder.DropForeignKey(
                name: "FK_TestRequestTestTypeHistories_TestRequestTestTypeModel_TestRequestTestTypeId",
                table: "TestRequestTestTypeHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_TestRequestTestTypeModel_AspNetUsers_TechnicianId",
                table: "TestRequestTestTypeModel");

            migrationBuilder.DropForeignKey(
                name: "FK_TestRequestTestTypeModel_AspNetUsers_VerifyTechnicianId",
                table: "TestRequestTestTypeModel");

            migrationBuilder.DropForeignKey(
                name: "FK_TestRequestTestTypeModel_TestRequests_TestRequestId",
                table: "TestRequestTestTypeModel");

            migrationBuilder.DropForeignKey(
                name: "FK_TestRequestTestTypeModel_TestTypes_TestTypeId",
                table: "TestRequestTestTypeModel");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TestRequestTestTypeModel",
                table: "TestRequestTestTypeModel");

            migrationBuilder.RenameTable(
                name: "TestRequestTestTypeModel",
                newName: "TestRequestTests");

            migrationBuilder.RenameIndex(
                name: "IX_TestRequestTestTypeModel_VerifyTechnicianId",
                table: "TestRequestTests",
                newName: "IX_TestRequestTests_VerifyTechnicianId");

            migrationBuilder.RenameIndex(
                name: "IX_TestRequestTestTypeModel_TestTypeId",
                table: "TestRequestTests",
                newName: "IX_TestRequestTests_TestTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_TestRequestTestTypeModel_TestRequestId",
                table: "TestRequestTests",
                newName: "IX_TestRequestTests_TestRequestId");

            migrationBuilder.RenameIndex(
                name: "IX_TestRequestTestTypeModel_TechnicianId",
                table: "TestRequestTests",
                newName: "IX_TestRequestTests_TechnicianId");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "TestRequestTests",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "Submitted",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TestRequestTests",
                table: "TestRequestTests",
                column: "TestRequestTestTypeId");

            migrationBuilder.UpdateData(
                table: "TechnicianTestTypes",
                keyColumn: "TechnicianTestTypeId",
                keyValue: 1,
                column: "Status",
                value: "Approved");

            migrationBuilder.UpdateData(
                table: "TechnicianTestTypes",
                keyColumn: "TechnicianTestTypeId",
                keyValue: 2,
                column: "Status",
                value: "Approved");

            migrationBuilder.UpdateData(
                table: "TechnicianTestTypes",
                keyColumn: "TechnicianTestTypeId",
                keyValue: 3,
                column: "Status",
                value: "Approved");

            migrationBuilder.InsertData(
                table: "TechnicianTestTypes",
                columns: new[] { "TechnicianTestTypeId", "AssignmentDate", "ManagerId", "RegisterUserId", "Status", "TechnicianId", "TestTypeId" },
                values: new object[,]
                {
                    { 4, new DateTime(2026, 1, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), "af381d22-8f2c-4401-ae8b-b31bb60e1c39", null, "Approved", "2ec716df-770e-465e-a7b7-8d3926b560a2", 4 },
                    { 5, new DateTime(2026, 1, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "af381d22-8f2c-4401-ae8b-b31bb60e1c39", null, "Approved", "2ec716df-770e-465e-a7b7-8d3926b560a2", 5 },
                    { 6, new DateTime(2026, 1, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), "af381d22-8f2c-4401-ae8b-b31bb60e1c39", null, "Approved", "2ec716df-770e-465e-a7b7-8d3926b560a2", 6 },
                    { 7, new DateTime(2026, 1, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), "af381d22-8f2c-4401-ae8b-b31bb60e1c39", null, "Approved", "2ec716df-770e-465e-a7b7-8d3926b560a2", 7 },
                    { 8, new DateTime(2026, 1, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "af381d22-8f2c-4401-ae8b-b31bb60e1c39", null, "Approved", "2ec716df-770e-465e-a7b7-8d3926b560a2", 8 },
                    { 9, new DateTime(2026, 1, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), "af381d22-8f2c-4401-ae8b-b31bb60e1c39", null, "Approved", "2ec716df-770e-465e-a7b7-8d3926b560a2", 9 },
                    { 10, new DateTime(2026, 1, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "af381d22-8f2c-4401-ae8b-b31bb60e1c39", null, "Approved", "2ec716df-770e-465e-a7b7-8d3926b560a2", 11 },
                    { 11, new DateTime(2026, 1, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "af381d22-8f2c-4401-ae8b-b31bb60e1c39", null, "Approved", "2ec716df-770e-465e-a7b7-8d3926b560a2", 12 },
                    { 12, new DateTime(2026, 1, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "af381d22-8f2c-4401-ae8b-b31bb60e1c39", null, "Approved", "2ec716df-770e-465e-a7b7-8d3926b560a2", 16 },
                    { 13, new DateTime(2026, 1, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), "af381d22-8f2c-4401-ae8b-b31bb60e1c39", null, "Approved", "2ec716df-770e-465e-a7b7-8d3926b560a2", 17 },
                    { 14, new DateTime(2026, 1, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "af381d22-8f2c-4401-ae8b-b31bb60e1c39", null, "Approved", "2ec716df-770e-465e-a7b7-8d3926b560a2", 18 },
                    { 15, new DateTime(2026, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "af381d22-8f2c-4401-ae8b-b31bb60e1c39", null, "Approved", "8c537ab2-69dc-401a-bdd4-994b6d1bd66e", 19 },
                    { 16, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "af381d22-8f2c-4401-ae8b-b31bb60e1c39", null, "Approved", "8c537ab2-69dc-401a-bdd4-994b6d1bd66e", 20 },
                    { 17, new DateTime(2026, 1, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "af381d22-8f2c-4401-ae8b-b31bb60e1c39", null, "Approved", "8c537ab2-69dc-401a-bdd4-994b6d1bd66e", 21 },
                    { 18, new DateTime(2026, 1, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "af381d22-8f2c-4401-ae8b-b31bb60e1c39", null, "Approved", "8c537ab2-69dc-401a-bdd4-994b6d1bd66e", 22 },
                    { 19, new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "af381d22-8f2c-4401-ae8b-b31bb60e1c39", null, "Approved", "8c537ab2-69dc-401a-bdd4-994b6d1bd66e", 13 },
                    { 20, new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "af381d22-8f2c-4401-ae8b-b31bb60e1c39", null, "Approved", "8c537ab2-69dc-401a-bdd4-994b6d1bd66e", 14 },
                    { 21, new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "af381d22-8f2c-4401-ae8b-b31bb60e1c39", null, "Approved", "8c537ab2-69dc-401a-bdd4-994b6d1bd66e", 15 },
                    { 22, new DateTime(2026, 1, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "af381d22-8f2c-4401-ae8b-b31bb60e1c39", null, "Pending", "759b8e18-6bd2-4c7b-aa9b-86ab2148eb4d", 1 },
                    { 23, new DateTime(2026, 1, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), "af381d22-8f2c-4401-ae8b-b31bb60e1c39", null, "Pending", "759b8e18-6bd2-4c7b-aa9b-86ab2148eb4d", 6 },
                    { 24, new DateTime(2026, 1, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), "af381d22-8f2c-4401-ae8b-b31bb60e1c39", null, "Pending", "759b8e18-6bd2-4c7b-aa9b-86ab2148eb4d", 11 },
                    { 25, new DateTime(2026, 1, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "af381d22-8f2c-4401-ae8b-b31bb60e1c39", null, "Pending", "759b8e18-6bd2-4c7b-aa9b-86ab2148eb4d", 17 }
                });

            migrationBuilder.UpdateData(
                table: "TestRequestTests",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1,
                columns: new[] { "CompletionDateTime", "DueDate", "StartDateTime" },
                values: new object[] { new DateTime(2026, 5, 16, 19, 52, 51, 28, DateTimeKind.Local).AddTicks(944), new DateTime(2026, 5, 16, 21, 52, 51, 28, DateTimeKind.Local).AddTicks(945), new DateTime(2026, 5, 16, 19, 52, 51, 28, DateTimeKind.Local).AddTicks(935) });

            migrationBuilder.UpdateData(
                table: "TestRequestTests",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 2,
                columns: new[] { "CompletionDateTime", "DueDate", "StartDateTime" },
                values: new object[] { new DateTime(2026, 5, 16, 19, 52, 51, 28, DateTimeKind.Local).AddTicks(956), new DateTime(2026, 5, 16, 20, 52, 51, 28, DateTimeKind.Local).AddTicks(957), new DateTime(2026, 5, 16, 19, 52, 51, 28, DateTimeKind.Local).AddTicks(955) });

            migrationBuilder.UpdateData(
                table: "TestRequestTests",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 3,
                columns: new[] { "CompletionDateTime", "DueDate", "StartDateTime" },
                values: new object[] { new DateTime(2026, 5, 16, 19, 52, 51, 28, DateTimeKind.Local).AddTicks(961), new DateTime(2026, 5, 16, 21, 52, 51, 28, DateTimeKind.Local).AddTicks(962), new DateTime(2026, 5, 16, 17, 52, 51, 28, DateTimeKind.Local).AddTicks(960) });

            migrationBuilder.UpdateData(
                table: "TestRequestTests",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 4,
                columns: new[] { "CompletionDateTime", "DueDate", "StartDateTime" },
                values: new object[] { new DateTime(2026, 5, 16, 19, 52, 51, 28, DateTimeKind.Local).AddTicks(965), new DateTime(2026, 5, 16, 21, 52, 51, 28, DateTimeKind.Local).AddTicks(966), new DateTime(2026, 5, 16, 19, 52, 51, 28, DateTimeKind.Local).AddTicks(965) });

            migrationBuilder.UpdateData(
                table: "TestRequestTests",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 5,
                columns: new[] { "CompletionDateTime", "DueDate", "StartDateTime" },
                values: new object[] { new DateTime(2026, 5, 16, 18, 52, 51, 28, DateTimeKind.Local).AddTicks(972), new DateTime(2026, 5, 16, 18, 52, 51, 28, DateTimeKind.Local).AddTicks(973), new DateTime(2026, 5, 16, 16, 52, 51, 28, DateTimeKind.Local).AddTicks(971) });

            migrationBuilder.UpdateData(
                table: "TestRequestTests",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 6,
                columns: new[] { "CompletionDateTime", "DueDate", "StartDateTime", "VerificationDateTime" },
                values: new object[] { new DateTime(2026, 5, 16, 17, 52, 51, 28, DateTimeKind.Local).AddTicks(978), new DateTime(2026, 5, 16, 17, 52, 51, 28, DateTimeKind.Local).AddTicks(979), new DateTime(2026, 5, 16, 16, 52, 51, 28, DateTimeKind.Local).AddTicks(977), new DateTime(2026, 5, 16, 18, 22, 51, 28, DateTimeKind.Local).AddTicks(982) });

            migrationBuilder.UpdateData(
                table: "TestRequestTests",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 7,
                columns: new[] { "CompletionDateTime", "DueDate", "StartDateTime", "VerificationDateTime" },
                values: new object[] { new DateTime(2026, 5, 16, 19, 22, 51, 28, DateTimeKind.Local).AddTicks(986), new DateTime(2026, 5, 16, 19, 52, 51, 28, DateTimeKind.Local).AddTicks(987), new DateTime(2026, 5, 16, 17, 52, 51, 28, DateTimeKind.Local).AddTicks(985), new DateTime(2026, 5, 16, 19, 22, 51, 28, DateTimeKind.Local).AddTicks(989) });

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 1,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 19, 52, 51, 28, DateTimeKind.Local).AddTicks(1361));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 2,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 19, 52, 51, 28, DateTimeKind.Local).AddTicks(1364));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 3,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 19, 52, 51, 28, DateTimeKind.Local).AddTicks(1365));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 4,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 19, 52, 51, 28, DateTimeKind.Local).AddTicks(1366));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 5,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 19, 52, 51, 28, DateTimeKind.Local).AddTicks(1368));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 6,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 19, 52, 51, 28, DateTimeKind.Local).AddTicks(1369));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 7,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 19, 52, 51, 28, DateTimeKind.Local).AddTicks(1371));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 8,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 19, 52, 51, 28, DateTimeKind.Local).AddTicks(1373));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 9,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 19, 52, 51, 28, DateTimeKind.Local).AddTicks(1374));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 10,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 19, 52, 51, 28, DateTimeKind.Local).AddTicks(1375));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 11,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 19, 52, 51, 28, DateTimeKind.Local).AddTicks(1376));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 12,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 19, 52, 51, 28, DateTimeKind.Local).AddTicks(1377));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 13,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 19, 52, 51, 28, DateTimeKind.Local).AddTicks(1378));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 14,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 19, 52, 51, 28, DateTimeKind.Local).AddTicks(1379));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 15,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 19, 52, 51, 28, DateTimeKind.Local).AddTicks(1380));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 16,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 19, 52, 51, 28, DateTimeKind.Local).AddTicks(1382));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 17,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 19, 52, 51, 28, DateTimeKind.Local).AddTicks(1383));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 18,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 19, 52, 51, 28, DateTimeKind.Local).AddTicks(1384));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 19,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 19, 52, 51, 28, DateTimeKind.Local).AddTicks(1385));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 20,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 19, 52, 51, 28, DateTimeKind.Local).AddTicks(1386));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 21,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 19, 52, 51, 28, DateTimeKind.Local).AddTicks(1387));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 22,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 19, 52, 51, 28, DateTimeKind.Local).AddTicks(1388));

            migrationBuilder.AddForeignKey(
                name: "FK_Results_TestRequestTests_TestRequestTestTypeModelTestRequestTestTypeId",
                table: "Results",
                column: "TestRequestTestTypeModelTestRequestTestTypeId",
                principalTable: "TestRequestTests",
                principalColumn: "TestRequestTestTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_TestRequestTests_AspNetUsers_TechnicianId",
                table: "TestRequestTests",
                column: "TechnicianId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TestRequestTests_AspNetUsers_VerifyTechnicianId",
                table: "TestRequestTests",
                column: "VerifyTechnicianId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TestRequestTests_TestRequests_TestRequestId",
                table: "TestRequestTests",
                column: "TestRequestId",
                principalTable: "TestRequests",
                principalColumn: "TestRequestId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TestRequestTests_TestTypes_TestTypeId",
                table: "TestRequestTests",
                column: "TestTypeId",
                principalTable: "TestTypes",
                principalColumn: "TestTypeId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TestRequestTestTypeHistories_TestRequestTests_TestRequestTestTypeId",
                table: "TestRequestTestTypeHistories",
                column: "TestRequestTestTypeId",
                principalTable: "TestRequestTests",
                principalColumn: "TestRequestTestTypeId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Results_TestRequestTests_TestRequestTestTypeModelTestRequestTestTypeId",
                table: "Results");

            migrationBuilder.DropForeignKey(
                name: "FK_TestRequestTests_AspNetUsers_TechnicianId",
                table: "TestRequestTests");

            migrationBuilder.DropForeignKey(
                name: "FK_TestRequestTests_AspNetUsers_VerifyTechnicianId",
                table: "TestRequestTests");

            migrationBuilder.DropForeignKey(
                name: "FK_TestRequestTests_TestRequests_TestRequestId",
                table: "TestRequestTests");

            migrationBuilder.DropForeignKey(
                name: "FK_TestRequestTests_TestTypes_TestTypeId",
                table: "TestRequestTests");

            migrationBuilder.DropForeignKey(
                name: "FK_TestRequestTestTypeHistories_TestRequestTests_TestRequestTestTypeId",
                table: "TestRequestTestTypeHistories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TestRequestTests",
                table: "TestRequestTests");

            migrationBuilder.DeleteData(
                table: "TechnicianTestTypes",
                keyColumn: "TechnicianTestTypeId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "TechnicianTestTypes",
                keyColumn: "TechnicianTestTypeId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "TechnicianTestTypes",
                keyColumn: "TechnicianTestTypeId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "TechnicianTestTypes",
                keyColumn: "TechnicianTestTypeId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "TechnicianTestTypes",
                keyColumn: "TechnicianTestTypeId",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "TechnicianTestTypes",
                keyColumn: "TechnicianTestTypeId",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "TechnicianTestTypes",
                keyColumn: "TechnicianTestTypeId",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "TechnicianTestTypes",
                keyColumn: "TechnicianTestTypeId",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "TechnicianTestTypes",
                keyColumn: "TechnicianTestTypeId",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "TechnicianTestTypes",
                keyColumn: "TechnicianTestTypeId",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "TechnicianTestTypes",
                keyColumn: "TechnicianTestTypeId",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "TechnicianTestTypes",
                keyColumn: "TechnicianTestTypeId",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "TechnicianTestTypes",
                keyColumn: "TechnicianTestTypeId",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "TechnicianTestTypes",
                keyColumn: "TechnicianTestTypeId",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "TechnicianTestTypes",
                keyColumn: "TechnicianTestTypeId",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "TechnicianTestTypes",
                keyColumn: "TechnicianTestTypeId",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "TechnicianTestTypes",
                keyColumn: "TechnicianTestTypeId",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "TechnicianTestTypes",
                keyColumn: "TechnicianTestTypeId",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "TechnicianTestTypes",
                keyColumn: "TechnicianTestTypeId",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "TechnicianTestTypes",
                keyColumn: "TechnicianTestTypeId",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "TechnicianTestTypes",
                keyColumn: "TechnicianTestTypeId",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "TechnicianTestTypes",
                keyColumn: "TechnicianTestTypeId",
                keyValue: 25);

            migrationBuilder.RenameTable(
                name: "TestRequestTests",
                newName: "TestRequestTestTypeModel");

            migrationBuilder.RenameIndex(
                name: "IX_TestRequestTests_VerifyTechnicianId",
                table: "TestRequestTestTypeModel",
                newName: "IX_TestRequestTestTypeModel_VerifyTechnicianId");

            migrationBuilder.RenameIndex(
                name: "IX_TestRequestTests_TestTypeId",
                table: "TestRequestTestTypeModel",
                newName: "IX_TestRequestTestTypeModel_TestTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_TestRequestTests_TestRequestId",
                table: "TestRequestTestTypeModel",
                newName: "IX_TestRequestTestTypeModel_TestRequestId");

            migrationBuilder.RenameIndex(
                name: "IX_TestRequestTests_TechnicianId",
                table: "TestRequestTestTypeModel",
                newName: "IX_TestRequestTestTypeModel_TechnicianId");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "TestRequestTestTypeModel",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldDefaultValue: "Submitted");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TestRequestTestTypeModel",
                table: "TestRequestTestTypeModel",
                column: "TestRequestTestTypeId");

            migrationBuilder.UpdateData(
                table: "TechnicianTestTypes",
                keyColumn: "TechnicianTestTypeId",
                keyValue: 1,
                column: "Status",
                value: "Submitted");

            migrationBuilder.UpdateData(
                table: "TechnicianTestTypes",
                keyColumn: "TechnicianTestTypeId",
                keyValue: 2,
                column: "Status",
                value: "Submitted");

            migrationBuilder.UpdateData(
                table: "TechnicianTestTypes",
                keyColumn: "TechnicianTestTypeId",
                keyValue: 3,
                column: "Status",
                value: "Submitted");

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypeModel",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1,
                columns: new[] { "CompletionDateTime", "DueDate", "StartDateTime" },
                values: new object[] { new DateTime(2026, 5, 16, 17, 40, 32, 265, DateTimeKind.Local).AddTicks(2835), new DateTime(2026, 5, 16, 19, 40, 32, 265, DateTimeKind.Local).AddTicks(2836), new DateTime(2026, 5, 16, 17, 40, 32, 265, DateTimeKind.Local).AddTicks(2827) });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypeModel",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 2,
                columns: new[] { "CompletionDateTime", "DueDate", "StartDateTime" },
                values: new object[] { new DateTime(2026, 5, 16, 17, 40, 32, 265, DateTimeKind.Local).AddTicks(2846), new DateTime(2026, 5, 16, 18, 40, 32, 265, DateTimeKind.Local).AddTicks(2847), new DateTime(2026, 5, 16, 17, 40, 32, 265, DateTimeKind.Local).AddTicks(2846) });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypeModel",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 3,
                columns: new[] { "CompletionDateTime", "DueDate", "StartDateTime" },
                values: new object[] { new DateTime(2026, 5, 16, 17, 40, 32, 265, DateTimeKind.Local).AddTicks(2851), new DateTime(2026, 5, 16, 19, 40, 32, 265, DateTimeKind.Local).AddTicks(2852), new DateTime(2026, 5, 16, 15, 40, 32, 265, DateTimeKind.Local).AddTicks(2850) });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypeModel",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 4,
                columns: new[] { "CompletionDateTime", "DueDate", "StartDateTime" },
                values: new object[] { new DateTime(2026, 5, 16, 17, 40, 32, 265, DateTimeKind.Local).AddTicks(2855), new DateTime(2026, 5, 16, 19, 40, 32, 265, DateTimeKind.Local).AddTicks(2856), new DateTime(2026, 5, 16, 17, 40, 32, 265, DateTimeKind.Local).AddTicks(2854) });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypeModel",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 5,
                columns: new[] { "CompletionDateTime", "DueDate", "StartDateTime" },
                values: new object[] { new DateTime(2026, 5, 16, 16, 40, 32, 265, DateTimeKind.Local).AddTicks(2859), new DateTime(2026, 5, 16, 16, 40, 32, 265, DateTimeKind.Local).AddTicks(2861), new DateTime(2026, 5, 16, 14, 40, 32, 265, DateTimeKind.Local).AddTicks(2858) });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypeModel",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 6,
                columns: new[] { "CompletionDateTime", "DueDate", "StartDateTime", "VerificationDateTime" },
                values: new object[] { new DateTime(2026, 5, 16, 15, 40, 32, 265, DateTimeKind.Local).AddTicks(2865), new DateTime(2026, 5, 16, 15, 40, 32, 265, DateTimeKind.Local).AddTicks(2866), new DateTime(2026, 5, 16, 14, 40, 32, 265, DateTimeKind.Local).AddTicks(2864), new DateTime(2026, 5, 16, 16, 10, 32, 265, DateTimeKind.Local).AddTicks(2869) });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypeModel",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 7,
                columns: new[] { "CompletionDateTime", "DueDate", "StartDateTime", "VerificationDateTime" },
                values: new object[] { new DateTime(2026, 5, 16, 17, 10, 32, 265, DateTimeKind.Local).AddTicks(2873), new DateTime(2026, 5, 16, 17, 40, 32, 265, DateTimeKind.Local).AddTicks(2874), new DateTime(2026, 5, 16, 15, 40, 32, 265, DateTimeKind.Local).AddTicks(2872), new DateTime(2026, 5, 16, 17, 10, 32, 265, DateTimeKind.Local).AddTicks(2876) });

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 1,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 17, 40, 32, 265, DateTimeKind.Local).AddTicks(3167));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 2,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 17, 40, 32, 265, DateTimeKind.Local).AddTicks(3170));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 3,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 17, 40, 32, 265, DateTimeKind.Local).AddTicks(3171));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 4,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 17, 40, 32, 265, DateTimeKind.Local).AddTicks(3172));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 5,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 17, 40, 32, 265, DateTimeKind.Local).AddTicks(3173));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 6,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 17, 40, 32, 265, DateTimeKind.Local).AddTicks(3175));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 7,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 17, 40, 32, 265, DateTimeKind.Local).AddTicks(3176));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 8,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 17, 40, 32, 265, DateTimeKind.Local).AddTicks(3177));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 9,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 17, 40, 32, 265, DateTimeKind.Local).AddTicks(3178));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 10,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 17, 40, 32, 265, DateTimeKind.Local).AddTicks(3179));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 11,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 17, 40, 32, 265, DateTimeKind.Local).AddTicks(3180));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 12,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 17, 40, 32, 265, DateTimeKind.Local).AddTicks(3181));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 13,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 17, 40, 32, 265, DateTimeKind.Local).AddTicks(3182));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 14,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 17, 40, 32, 265, DateTimeKind.Local).AddTicks(3183));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 15,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 17, 40, 32, 265, DateTimeKind.Local).AddTicks(3185));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 16,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 17, 40, 32, 265, DateTimeKind.Local).AddTicks(3186));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 17,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 17, 40, 32, 265, DateTimeKind.Local).AddTicks(3188));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 18,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 17, 40, 32, 265, DateTimeKind.Local).AddTicks(3189));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 19,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 17, 40, 32, 265, DateTimeKind.Local).AddTicks(3190));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 20,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 17, 40, 32, 265, DateTimeKind.Local).AddTicks(3191));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 21,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 17, 40, 32, 265, DateTimeKind.Local).AddTicks(3192));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 22,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 17, 40, 32, 265, DateTimeKind.Local).AddTicks(3193));

            migrationBuilder.AddForeignKey(
                name: "FK_Results_TestRequestTestTypeModel_TestRequestTestTypeModelTestRequestTestTypeId",
                table: "Results",
                column: "TestRequestTestTypeModelTestRequestTestTypeId",
                principalTable: "TestRequestTestTypeModel",
                principalColumn: "TestRequestTestTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_TestRequestTestTypeHistories_TestRequestTestTypeModel_TestRequestTestTypeId",
                table: "TestRequestTestTypeHistories",
                column: "TestRequestTestTypeId",
                principalTable: "TestRequestTestTypeModel",
                principalColumn: "TestRequestTestTypeId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TestRequestTestTypeModel_AspNetUsers_TechnicianId",
                table: "TestRequestTestTypeModel",
                column: "TechnicianId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TestRequestTestTypeModel_AspNetUsers_VerifyTechnicianId",
                table: "TestRequestTestTypeModel",
                column: "VerifyTechnicianId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TestRequestTestTypeModel_TestRequests_TestRequestId",
                table: "TestRequestTestTypeModel",
                column: "TestRequestId",
                principalTable: "TestRequests",
                principalColumn: "TestRequestId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TestRequestTestTypeModel_TestTypes_TestTypeId",
                table: "TestRequestTestTypeModel",
                column: "TestTypeId",
                principalTable: "TestTypes",
                principalColumn: "TestTypeId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
