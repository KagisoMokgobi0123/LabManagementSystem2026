using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NMBHLabSys.Migrations
{
    /// <inheritdoc />
    public partial class AddDoctorReleaseFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsVerified",
                table: "TestRequestTestTypes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "VerifiedAt",
                table: "TestRequestTestTypes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VerifiedByTechnicianId",
                table: "TestRequestTestTypes",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DoctorReleaseNote",
                table: "TestRequests",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PatientNotificationMethod",
                table: "TestRequests",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReleasedByDoctorId",
                table: "TestRequests",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ResultsReleasedAt",
                table: "TestRequests",
                type: "datetime2",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1,
                columns: new[] { "IsVerified", "VerifiedAt", "VerifiedByTechnicianId" },
                values: new object[] { false, null, null });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 2,
                columns: new[] { "IsVerified", "VerifiedAt", "VerifiedByTechnicianId" },
                values: new object[] { false, null, null });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 3,
                columns: new[] { "IsVerified", "VerifiedAt", "VerifiedByTechnicianId" },
                values: new object[] { false, null, null });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 4,
                columns: new[] { "IsVerified", "VerifiedAt", "VerifiedByTechnicianId" },
                values: new object[] { false, null, null });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 5,
                columns: new[] { "IsVerified", "VerifiedAt", "VerifiedByTechnicianId" },
                values: new object[] { false, null, null });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 6,
                columns: new[] { "IsVerified", "VerifiedAt", "VerifiedByTechnicianId" },
                values: new object[] { false, null, null });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 7,
                columns: new[] { "IsVerified", "VerifiedAt", "VerifiedByTechnicianId" },
                values: new object[] { false, null, null });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 8,
                columns: new[] { "IsVerified", "VerifiedAt", "VerifiedByTechnicianId" },
                values: new object[] { false, null, null });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1001,
                columns: new[] { "IsVerified", "VerifiedAt", "VerifiedByTechnicianId" },
                values: new object[] { false, null, null });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1002,
                columns: new[] { "IsVerified", "VerifiedAt", "VerifiedByTechnicianId" },
                values: new object[] { false, null, null });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1003,
                columns: new[] { "IsVerified", "VerifiedAt", "VerifiedByTechnicianId" },
                values: new object[] { false, null, null });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1004,
                columns: new[] { "IsVerified", "VerifiedAt", "VerifiedByTechnicianId" },
                values: new object[] { false, null, null });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1101,
                columns: new[] { "IsVerified", "VerifiedAt", "VerifiedByTechnicianId" },
                values: new object[] { false, null, null });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1102,
                columns: new[] { "IsVerified", "VerifiedAt", "VerifiedByTechnicianId" },
                values: new object[] { false, null, null });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1103,
                columns: new[] { "IsVerified", "VerifiedAt", "VerifiedByTechnicianId" },
                values: new object[] { false, null, null });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1104,
                columns: new[] { "IsVerified", "VerifiedAt", "VerifiedByTechnicianId" },
                values: new object[] { false, null, null });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1105,
                columns: new[] { "IsVerified", "VerifiedAt", "VerifiedByTechnicianId" },
                values: new object[] { false, null, null });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1106,
                columns: new[] { "IsVerified", "VerifiedAt", "VerifiedByTechnicianId" },
                values: new object[] { false, null, null });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1107,
                columns: new[] { "IsVerified", "VerifiedAt", "VerifiedByTechnicianId" },
                values: new object[] { false, null, null });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1201,
                columns: new[] { "IsVerified", "VerifiedAt", "VerifiedByTechnicianId" },
                values: new object[] { false, null, null });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1202,
                columns: new[] { "IsVerified", "VerifiedAt", "VerifiedByTechnicianId" },
                values: new object[] { false, null, null });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1203,
                columns: new[] { "IsVerified", "VerifiedAt", "VerifiedByTechnicianId" },
                values: new object[] { false, null, null });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1204,
                columns: new[] { "IsVerified", "VerifiedAt", "VerifiedByTechnicianId" },
                values: new object[] { false, null, null });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1205,
                columns: new[] { "IsVerified", "VerifiedAt", "VerifiedByTechnicianId" },
                values: new object[] { false, null, null });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1206,
                columns: new[] { "IsVerified", "VerifiedAt", "VerifiedByTechnicianId" },
                values: new object[] { false, null, null });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1207,
                columns: new[] { "IsVerified", "VerifiedAt", "VerifiedByTechnicianId" },
                values: new object[] { false, null, null });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1208,
                columns: new[] { "IsVerified", "VerifiedAt", "VerifiedByTechnicianId" },
                values: new object[] { false, null, null });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1209,
                columns: new[] { "IsVerified", "VerifiedAt", "VerifiedByTechnicianId" },
                values: new object[] { false, null, null });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1210,
                columns: new[] { "IsVerified", "VerifiedAt", "VerifiedByTechnicianId" },
                values: new object[] { false, null, null });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1211,
                columns: new[] { "IsVerified", "VerifiedAt", "VerifiedByTechnicianId" },
                values: new object[] { false, null, null });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1212,
                columns: new[] { "IsVerified", "VerifiedAt", "VerifiedByTechnicianId" },
                values: new object[] { false, null, null });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1213,
                columns: new[] { "IsVerified", "VerifiedAt", "VerifiedByTechnicianId" },
                values: new object[] { false, null, null });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1214,
                columns: new[] { "IsVerified", "VerifiedAt", "VerifiedByTechnicianId" },
                values: new object[] { false, null, null });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1301,
                columns: new[] { "IsVerified", "VerifiedAt", "VerifiedByTechnicianId" },
                values: new object[] { false, null, null });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1302,
                columns: new[] { "IsVerified", "VerifiedAt", "VerifiedByTechnicianId" },
                values: new object[] { false, null, null });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1303,
                columns: new[] { "IsVerified", "VerifiedAt", "VerifiedByTechnicianId" },
                values: new object[] { false, null, null });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1304,
                columns: new[] { "IsVerified", "VerifiedAt", "VerifiedByTechnicianId" },
                values: new object[] { false, null, null });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1305,
                columns: new[] { "IsVerified", "VerifiedAt", "VerifiedByTechnicianId" },
                values: new object[] { false, null, null });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1306,
                columns: new[] { "IsVerified", "VerifiedAt", "VerifiedByTechnicianId" },
                values: new object[] { false, null, null });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1307,
                columns: new[] { "IsVerified", "VerifiedAt", "VerifiedByTechnicianId" },
                values: new object[] { false, null, null });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1308,
                columns: new[] { "IsVerified", "VerifiedAt", "VerifiedByTechnicianId" },
                values: new object[] { false, null, null });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1309,
                columns: new[] { "IsVerified", "VerifiedAt", "VerifiedByTechnicianId" },
                values: new object[] { false, null, null });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1310,
                columns: new[] { "IsVerified", "VerifiedAt", "VerifiedByTechnicianId" },
                values: new object[] { false, null, null });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1311,
                columns: new[] { "IsVerified", "VerifiedAt", "VerifiedByTechnicianId" },
                values: new object[] { false, null, null });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1312,
                columns: new[] { "IsVerified", "VerifiedAt", "VerifiedByTechnicianId" },
                values: new object[] { false, null, null });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1313,
                columns: new[] { "IsVerified", "VerifiedAt", "VerifiedByTechnicianId" },
                values: new object[] { false, null, null });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1314,
                columns: new[] { "IsVerified", "VerifiedAt", "VerifiedByTechnicianId" },
                values: new object[] { false, null, null });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1315,
                columns: new[] { "IsVerified", "VerifiedAt", "VerifiedByTechnicianId" },
                values: new object[] { false, null, null });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1316,
                columns: new[] { "IsVerified", "VerifiedAt", "VerifiedByTechnicianId" },
                values: new object[] { false, null, null });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1317,
                columns: new[] { "IsVerified", "VerifiedAt", "VerifiedByTechnicianId" },
                values: new object[] { false, null, null });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1318,
                columns: new[] { "IsVerified", "VerifiedAt", "VerifiedByTechnicianId" },
                values: new object[] { false, null, null });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1319,
                columns: new[] { "IsVerified", "VerifiedAt", "VerifiedByTechnicianId" },
                values: new object[] { false, null, null });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1320,
                columns: new[] { "IsVerified", "VerifiedAt", "VerifiedByTechnicianId" },
                values: new object[] { false, null, null });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1321,
                columns: new[] { "IsVerified", "VerifiedAt", "VerifiedByTechnicianId" },
                values: new object[] { false, null, null });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1322,
                columns: new[] { "IsVerified", "VerifiedAt", "VerifiedByTechnicianId" },
                values: new object[] { false, null, null });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1323,
                columns: new[] { "IsVerified", "VerifiedAt", "VerifiedByTechnicianId" },
                values: new object[] { false, null, null });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1324,
                columns: new[] { "IsVerified", "VerifiedAt", "VerifiedByTechnicianId" },
                values: new object[] { false, null, null });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1401,
                columns: new[] { "IsVerified", "VerifiedAt", "VerifiedByTechnicianId" },
                values: new object[] { false, null, null });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1402,
                columns: new[] { "IsVerified", "VerifiedAt", "VerifiedByTechnicianId" },
                values: new object[] { false, null, null });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1403,
                columns: new[] { "IsVerified", "VerifiedAt", "VerifiedByTechnicianId" },
                values: new object[] { false, null, null });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1404,
                columns: new[] { "IsVerified", "VerifiedAt", "VerifiedByTechnicianId" },
                values: new object[] { false, null, null });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1405,
                columns: new[] { "IsVerified", "VerifiedAt", "VerifiedByTechnicianId" },
                values: new object[] { false, null, null });

            migrationBuilder.UpdateData(
                table: "TestRequests",
                keyColumn: "TestRequestId",
                keyValue: 1,
                columns: new[] { "DoctorReleaseNote", "PatientNotificationMethod", "ReleasedByDoctorId", "ResultsReleasedAt" },
                values: new object[] { null, null, null, null });

            migrationBuilder.UpdateData(
                table: "TestRequests",
                keyColumn: "TestRequestId",
                keyValue: 2,
                columns: new[] { "DoctorReleaseNote", "PatientNotificationMethod", "ReleasedByDoctorId", "ResultsReleasedAt" },
                values: new object[] { null, null, null, null });

            migrationBuilder.UpdateData(
                table: "TestRequests",
                keyColumn: "TestRequestId",
                keyValue: 3,
                columns: new[] { "DoctorReleaseNote", "PatientNotificationMethod", "ReleasedByDoctorId", "ResultsReleasedAt" },
                values: new object[] { null, null, null, null });

            migrationBuilder.UpdateData(
                table: "TestRequests",
                keyColumn: "TestRequestId",
                keyValue: 4,
                columns: new[] { "DoctorReleaseNote", "PatientNotificationMethod", "ReleasedByDoctorId", "ResultsReleasedAt" },
                values: new object[] { null, null, null, null });

            migrationBuilder.UpdateData(
                table: "TestRequests",
                keyColumn: "TestRequestId",
                keyValue: 5,
                columns: new[] { "DoctorReleaseNote", "PatientNotificationMethod", "ReleasedByDoctorId", "ResultsReleasedAt" },
                values: new object[] { null, null, null, null });

            migrationBuilder.UpdateData(
                table: "TestRequests",
                keyColumn: "TestRequestId",
                keyValue: 6,
                columns: new[] { "DoctorReleaseNote", "PatientNotificationMethod", "ReleasedByDoctorId", "ResultsReleasedAt" },
                values: new object[] { null, null, null, null });

            migrationBuilder.UpdateData(
                table: "TestRequests",
                keyColumn: "TestRequestId",
                keyValue: 7,
                columns: new[] { "DoctorReleaseNote", "PatientNotificationMethod", "ReleasedByDoctorId", "ResultsReleasedAt" },
                values: new object[] { null, null, null, null });

            migrationBuilder.UpdateData(
                table: "TestRequests",
                keyColumn: "TestRequestId",
                keyValue: 8,
                columns: new[] { "DoctorReleaseNote", "PatientNotificationMethod", "ReleasedByDoctorId", "ResultsReleasedAt" },
                values: new object[] { null, null, null, null });

            migrationBuilder.UpdateData(
                table: "TestRequests",
                keyColumn: "TestRequestId",
                keyValue: 9,
                columns: new[] { "DoctorReleaseNote", "PatientNotificationMethod", "ReleasedByDoctorId", "ResultsReleasedAt" },
                values: new object[] { null, null, null, null });

            migrationBuilder.UpdateData(
                table: "TestRequests",
                keyColumn: "TestRequestId",
                keyValue: 10,
                columns: new[] { "DoctorReleaseNote", "PatientNotificationMethod", "ReleasedByDoctorId", "ResultsReleasedAt" },
                values: new object[] { null, null, null, null });

            migrationBuilder.UpdateData(
                table: "TestRequests",
                keyColumn: "TestRequestId",
                keyValue: 11,
                columns: new[] { "DoctorReleaseNote", "PatientNotificationMethod", "ReleasedByDoctorId", "ResultsReleasedAt" },
                values: new object[] { null, null, null, null });

            migrationBuilder.CreateIndex(
                name: "IX_TestRequestTestTypes_VerifiedByTechnicianId",
                table: "TestRequestTestTypes",
                column: "VerifiedByTechnicianId");

            migrationBuilder.CreateIndex(
                name: "IX_TestRequests_ReleasedByDoctorId",
                table: "TestRequests",
                column: "ReleasedByDoctorId");

            migrationBuilder.AddForeignKey(
                name: "FK_TestRequests_AspNetUsers_ReleasedByDoctorId",
                table: "TestRequests",
                column: "ReleasedByDoctorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TestRequestTestTypes_AspNetUsers_VerifiedByTechnicianId",
                table: "TestRequestTestTypes",
                column: "VerifiedByTechnicianId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestRequests_AspNetUsers_ReleasedByDoctorId",
                table: "TestRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_TestRequestTestTypes_AspNetUsers_VerifiedByTechnicianId",
                table: "TestRequestTestTypes");

            migrationBuilder.DropIndex(
                name: "IX_TestRequestTestTypes_VerifiedByTechnicianId",
                table: "TestRequestTestTypes");

            migrationBuilder.DropIndex(
                name: "IX_TestRequests_ReleasedByDoctorId",
                table: "TestRequests");

            migrationBuilder.DropColumn(
                name: "IsVerified",
                table: "TestRequestTestTypes");

            migrationBuilder.DropColumn(
                name: "VerifiedAt",
                table: "TestRequestTestTypes");

            migrationBuilder.DropColumn(
                name: "VerifiedByTechnicianId",
                table: "TestRequestTestTypes");

            migrationBuilder.DropColumn(
                name: "DoctorReleaseNote",
                table: "TestRequests");

            migrationBuilder.DropColumn(
                name: "PatientNotificationMethod",
                table: "TestRequests");

            migrationBuilder.DropColumn(
                name: "ReleasedByDoctorId",
                table: "TestRequests");

            migrationBuilder.DropColumn(
                name: "ResultsReleasedAt",
                table: "TestRequests");
        }
    }
}
