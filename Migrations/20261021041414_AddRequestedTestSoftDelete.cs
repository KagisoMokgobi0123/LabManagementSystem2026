using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NMBHLabSys.Migrations
{
    /// <inheritdoc />
    public partial class AddRequestedTestSoftDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeactivatedAt",
                table: "TestRequestTestTypes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeactivatedByTechnicianId",
                table: "TestRequestTestTypes",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeactivationReason",
                table: "TestRequestTestTypes",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "TestRequestTestTypes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "RequestedTestDeactivationAudits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TestRequestTestTypeId = table.Column<int>(type: "int", nullable: false),
                    TechnicianId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    DeactivatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestedTestDeactivationAudits", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1,
                columns: new[] { "DeactivatedAt", "DeactivatedByTechnicianId", "DeactivationReason", "IsDeleted" },
                values: new object[] { null, null, null, false });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 2,
                columns: new[] { "DeactivatedAt", "DeactivatedByTechnicianId", "DeactivationReason", "IsDeleted" },
                values: new object[] { null, null, null, false });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 3,
                columns: new[] { "DeactivatedAt", "DeactivatedByTechnicianId", "DeactivationReason", "IsDeleted" },
                values: new object[] { null, null, null, false });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 4,
                columns: new[] { "DeactivatedAt", "DeactivatedByTechnicianId", "DeactivationReason", "IsDeleted" },
                values: new object[] { null, null, null, false });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 5,
                columns: new[] { "DeactivatedAt", "DeactivatedByTechnicianId", "DeactivationReason", "IsDeleted" },
                values: new object[] { null, null, null, false });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 6,
                columns: new[] { "DeactivatedAt", "DeactivatedByTechnicianId", "DeactivationReason", "IsDeleted" },
                values: new object[] { null, null, null, false });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 7,
                columns: new[] { "DeactivatedAt", "DeactivatedByTechnicianId", "DeactivationReason", "IsDeleted" },
                values: new object[] { null, null, null, false });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 8,
                columns: new[] { "DeactivatedAt", "DeactivatedByTechnicianId", "DeactivationReason", "IsDeleted" },
                values: new object[] { null, null, null, false });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1001,
                columns: new[] { "DeactivatedAt", "DeactivatedByTechnicianId", "DeactivationReason", "IsDeleted" },
                values: new object[] { null, null, null, false });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1002,
                columns: new[] { "DeactivatedAt", "DeactivatedByTechnicianId", "DeactivationReason", "IsDeleted" },
                values: new object[] { null, null, null, false });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1003,
                columns: new[] { "DeactivatedAt", "DeactivatedByTechnicianId", "DeactivationReason", "IsDeleted" },
                values: new object[] { null, null, null, false });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1004,
                columns: new[] { "DeactivatedAt", "DeactivatedByTechnicianId", "DeactivationReason", "IsDeleted" },
                values: new object[] { null, null, null, false });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1101,
                columns: new[] { "DeactivatedAt", "DeactivatedByTechnicianId", "DeactivationReason", "IsDeleted" },
                values: new object[] { null, null, null, false });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1102,
                columns: new[] { "DeactivatedAt", "DeactivatedByTechnicianId", "DeactivationReason", "IsDeleted" },
                values: new object[] { null, null, null, false });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1103,
                columns: new[] { "DeactivatedAt", "DeactivatedByTechnicianId", "DeactivationReason", "IsDeleted" },
                values: new object[] { null, null, null, false });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1104,
                columns: new[] { "DeactivatedAt", "DeactivatedByTechnicianId", "DeactivationReason", "IsDeleted" },
                values: new object[] { null, null, null, false });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1105,
                columns: new[] { "DeactivatedAt", "DeactivatedByTechnicianId", "DeactivationReason", "IsDeleted" },
                values: new object[] { null, null, null, false });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1106,
                columns: new[] { "DeactivatedAt", "DeactivatedByTechnicianId", "DeactivationReason", "IsDeleted" },
                values: new object[] { null, null, null, false });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1107,
                columns: new[] { "DeactivatedAt", "DeactivatedByTechnicianId", "DeactivationReason", "IsDeleted" },
                values: new object[] { null, null, null, false });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1201,
                columns: new[] { "DeactivatedAt", "DeactivatedByTechnicianId", "DeactivationReason", "IsDeleted" },
                values: new object[] { null, null, null, false });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1202,
                columns: new[] { "DeactivatedAt", "DeactivatedByTechnicianId", "DeactivationReason", "IsDeleted" },
                values: new object[] { null, null, null, false });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1203,
                columns: new[] { "DeactivatedAt", "DeactivatedByTechnicianId", "DeactivationReason", "IsDeleted" },
                values: new object[] { null, null, null, false });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1204,
                columns: new[] { "DeactivatedAt", "DeactivatedByTechnicianId", "DeactivationReason", "IsDeleted" },
                values: new object[] { null, null, null, false });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1205,
                columns: new[] { "DeactivatedAt", "DeactivatedByTechnicianId", "DeactivationReason", "IsDeleted" },
                values: new object[] { null, null, null, false });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1206,
                columns: new[] { "DeactivatedAt", "DeactivatedByTechnicianId", "DeactivationReason", "IsDeleted" },
                values: new object[] { null, null, null, false });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1207,
                columns: new[] { "DeactivatedAt", "DeactivatedByTechnicianId", "DeactivationReason", "IsDeleted" },
                values: new object[] { null, null, null, false });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1208,
                columns: new[] { "DeactivatedAt", "DeactivatedByTechnicianId", "DeactivationReason", "IsDeleted" },
                values: new object[] { null, null, null, false });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1209,
                columns: new[] { "DeactivatedAt", "DeactivatedByTechnicianId", "DeactivationReason", "IsDeleted" },
                values: new object[] { null, null, null, false });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1210,
                columns: new[] { "DeactivatedAt", "DeactivatedByTechnicianId", "DeactivationReason", "IsDeleted" },
                values: new object[] { null, null, null, false });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1211,
                columns: new[] { "DeactivatedAt", "DeactivatedByTechnicianId", "DeactivationReason", "IsDeleted" },
                values: new object[] { null, null, null, false });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1212,
                columns: new[] { "DeactivatedAt", "DeactivatedByTechnicianId", "DeactivationReason", "IsDeleted" },
                values: new object[] { null, null, null, false });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1213,
                columns: new[] { "DeactivatedAt", "DeactivatedByTechnicianId", "DeactivationReason", "IsDeleted" },
                values: new object[] { null, null, null, false });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1214,
                columns: new[] { "DeactivatedAt", "DeactivatedByTechnicianId", "DeactivationReason", "IsDeleted" },
                values: new object[] { null, null, null, false });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1301,
                columns: new[] { "DeactivatedAt", "DeactivatedByTechnicianId", "DeactivationReason", "IsDeleted" },
                values: new object[] { null, null, null, false });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1302,
                columns: new[] { "DeactivatedAt", "DeactivatedByTechnicianId", "DeactivationReason", "IsDeleted" },
                values: new object[] { null, null, null, false });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1303,
                columns: new[] { "DeactivatedAt", "DeactivatedByTechnicianId", "DeactivationReason", "IsDeleted" },
                values: new object[] { null, null, null, false });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1304,
                columns: new[] { "DeactivatedAt", "DeactivatedByTechnicianId", "DeactivationReason", "IsDeleted" },
                values: new object[] { null, null, null, false });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1305,
                columns: new[] { "DeactivatedAt", "DeactivatedByTechnicianId", "DeactivationReason", "IsDeleted" },
                values: new object[] { null, null, null, false });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1306,
                columns: new[] { "DeactivatedAt", "DeactivatedByTechnicianId", "DeactivationReason", "IsDeleted" },
                values: new object[] { null, null, null, false });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1307,
                columns: new[] { "DeactivatedAt", "DeactivatedByTechnicianId", "DeactivationReason", "IsDeleted" },
                values: new object[] { null, null, null, false });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1308,
                columns: new[] { "DeactivatedAt", "DeactivatedByTechnicianId", "DeactivationReason", "IsDeleted" },
                values: new object[] { null, null, null, false });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1309,
                columns: new[] { "DeactivatedAt", "DeactivatedByTechnicianId", "DeactivationReason", "IsDeleted" },
                values: new object[] { null, null, null, false });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1310,
                columns: new[] { "DeactivatedAt", "DeactivatedByTechnicianId", "DeactivationReason", "IsDeleted" },
                values: new object[] { null, null, null, false });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1311,
                columns: new[] { "DeactivatedAt", "DeactivatedByTechnicianId", "DeactivationReason", "IsDeleted" },
                values: new object[] { null, null, null, false });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1312,
                columns: new[] { "DeactivatedAt", "DeactivatedByTechnicianId", "DeactivationReason", "IsDeleted" },
                values: new object[] { null, null, null, false });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1313,
                columns: new[] { "DeactivatedAt", "DeactivatedByTechnicianId", "DeactivationReason", "IsDeleted" },
                values: new object[] { null, null, null, false });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1314,
                columns: new[] { "DeactivatedAt", "DeactivatedByTechnicianId", "DeactivationReason", "IsDeleted" },
                values: new object[] { null, null, null, false });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1315,
                columns: new[] { "DeactivatedAt", "DeactivatedByTechnicianId", "DeactivationReason", "IsDeleted" },
                values: new object[] { null, null, null, false });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1316,
                columns: new[] { "DeactivatedAt", "DeactivatedByTechnicianId", "DeactivationReason", "IsDeleted" },
                values: new object[] { null, null, null, false });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1317,
                columns: new[] { "DeactivatedAt", "DeactivatedByTechnicianId", "DeactivationReason", "IsDeleted" },
                values: new object[] { null, null, null, false });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1318,
                columns: new[] { "DeactivatedAt", "DeactivatedByTechnicianId", "DeactivationReason", "IsDeleted" },
                values: new object[] { null, null, null, false });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1319,
                columns: new[] { "DeactivatedAt", "DeactivatedByTechnicianId", "DeactivationReason", "IsDeleted" },
                values: new object[] { null, null, null, false });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1320,
                columns: new[] { "DeactivatedAt", "DeactivatedByTechnicianId", "DeactivationReason", "IsDeleted" },
                values: new object[] { null, null, null, false });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1321,
                columns: new[] { "DeactivatedAt", "DeactivatedByTechnicianId", "DeactivationReason", "IsDeleted" },
                values: new object[] { null, null, null, false });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1322,
                columns: new[] { "DeactivatedAt", "DeactivatedByTechnicianId", "DeactivationReason", "IsDeleted" },
                values: new object[] { null, null, null, false });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1323,
                columns: new[] { "DeactivatedAt", "DeactivatedByTechnicianId", "DeactivationReason", "IsDeleted" },
                values: new object[] { null, null, null, false });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1324,
                columns: new[] { "DeactivatedAt", "DeactivatedByTechnicianId", "DeactivationReason", "IsDeleted" },
                values: new object[] { null, null, null, false });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1401,
                columns: new[] { "DeactivatedAt", "DeactivatedByTechnicianId", "DeactivationReason", "IsDeleted" },
                values: new object[] { null, null, null, false });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1402,
                columns: new[] { "DeactivatedAt", "DeactivatedByTechnicianId", "DeactivationReason", "IsDeleted" },
                values: new object[] { null, null, null, false });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1403,
                columns: new[] { "DeactivatedAt", "DeactivatedByTechnicianId", "DeactivationReason", "IsDeleted" },
                values: new object[] { null, null, null, false });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1404,
                columns: new[] { "DeactivatedAt", "DeactivatedByTechnicianId", "DeactivationReason", "IsDeleted" },
                values: new object[] { null, null, null, false });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1405,
                columns: new[] { "DeactivatedAt", "DeactivatedByTechnicianId", "DeactivationReason", "IsDeleted" },
                values: new object[] { null, null, null, false });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RequestedTestDeactivationAudits");

            migrationBuilder.DropColumn(
                name: "DeactivatedAt",
                table: "TestRequestTestTypes");

            migrationBuilder.DropColumn(
                name: "DeactivatedByTechnicianId",
                table: "TestRequestTestTypes");

            migrationBuilder.DropColumn(
                name: "DeactivationReason",
                table: "TestRequestTestTypes");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "TestRequestTestTypes");
        }
    }
}
