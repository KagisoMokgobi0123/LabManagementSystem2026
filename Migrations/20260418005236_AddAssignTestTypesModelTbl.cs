using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NMBHLabSys.Migrations
{
    /// <inheritdoc />
    public partial class AddAssignTestTypesModelTbl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ManagerId",
                table: "AssignTestTypes",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TechnicianId",
                table: "AssignTestTypes",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AssignTestTypes",
                keyColumn: "AssignTestId",
                keyValue: 1,
                columns: new[] { "ManagerId", "Status", "TechnicianId" },
                values: new object[] { "7cfd1034-f3a8-42ed-b13e-61f3b19ed8ec", 3, null });

            migrationBuilder.UpdateData(
                table: "AssignTestTypes",
                keyColumn: "AssignTestId",
                keyValue: 2,
                columns: new[] { "ManagerId", "StartDateTime", "Status", "TechnicianId" },
                values: new object[] { "7cfd1034-f3a8-42ed-b13e-61f3b19ed8ec", new DateTime(2026, 4, 18, 8, 0, 0, 0, DateTimeKind.Unspecified), 1, "bdd52efb-571c-4ea7-9a18-9e9c66c92731" });

            migrationBuilder.UpdateData(
                table: "Samples",
                keyColumn: "SampleId",
                keyValue: 1,
                column: "DateReceived",
                value: new DateTime(2026, 4, 18, 2, 52, 35, 589, DateTimeKind.Local).AddTicks(173));

            migrationBuilder.UpdateData(
                table: "Samples",
                keyColumn: "SampleId",
                keyValue: 2,
                column: "DateReceived",
                value: new DateTime(2026, 4, 18, 2, 52, 35, 589, DateTimeKind.Local).AddTicks(175));

            migrationBuilder.CreateIndex(
                name: "IX_AssignTestTypes_ManagerId",
                table: "AssignTestTypes",
                column: "ManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_AssignTestTypes_TechnicianId",
                table: "AssignTestTypes",
                column: "TechnicianId");

            migrationBuilder.AddForeignKey(
                name: "FK_AssignTestTypes_AspNetUsers_ManagerId",
                table: "AssignTestTypes",
                column: "ManagerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AssignTestTypes_AspNetUsers_TechnicianId",
                table: "AssignTestTypes",
                column: "TechnicianId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssignTestTypes_AspNetUsers_ManagerId",
                table: "AssignTestTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_AssignTestTypes_AspNetUsers_TechnicianId",
                table: "AssignTestTypes");

            migrationBuilder.DropIndex(
                name: "IX_AssignTestTypes_ManagerId",
                table: "AssignTestTypes");

            migrationBuilder.DropIndex(
                name: "IX_AssignTestTypes_TechnicianId",
                table: "AssignTestTypes");

            migrationBuilder.DropColumn(
                name: "ManagerId",
                table: "AssignTestTypes");

            migrationBuilder.DropColumn(
                name: "TechnicianId",
                table: "AssignTestTypes");

            migrationBuilder.UpdateData(
                table: "AssignTestTypes",
                keyColumn: "AssignTestId",
                keyValue: 1,
                column: "Status",
                value: 0);

            migrationBuilder.UpdateData(
                table: "AssignTestTypes",
                keyColumn: "AssignTestId",
                keyValue: 2,
                columns: new[] { "StartDateTime", "Status" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "Samples",
                keyColumn: "SampleId",
                keyValue: 1,
                column: "DateReceived",
                value: new DateTime(2026, 4, 13, 15, 49, 47, 591, DateTimeKind.Local).AddTicks(6372));

            migrationBuilder.UpdateData(
                table: "Samples",
                keyColumn: "SampleId",
                keyValue: 2,
                column: "DateReceived",
                value: new DateTime(2026, 4, 13, 15, 49, 47, 591, DateTimeKind.Local).AddTicks(6375));
        }
    }
}
