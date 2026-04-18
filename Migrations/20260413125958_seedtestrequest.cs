using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace NMBHLabSys.Migrations
{
    /// <inheritdoc />
    public partial class seedtestrequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssignTestTypes_AspNetUsers_TechnicianId",
                table: "AssignTestTypes");

            migrationBuilder.DropIndex(
                name: "IX_AssignTestTypes_TechnicianId",
                table: "AssignTestTypes");

            migrationBuilder.DropColumn(
                name: "RequestNumber",
                table: "Samples");

            migrationBuilder.DropColumn(
                name: "TechnicianId",
                table: "AssignTestTypes");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "AssignTestTypes",
                newName: "AssignTestId");

            migrationBuilder.InsertData(
                table: "TestRequests",
                columns: new[] { "TestRequestId", "DoctorId", "PatientId", "RequestDate", "Status" },
                values: new object[,]
                {
                    { 1, "24d75510-b1ad-4803-86e5-8e279b3ddb5a", "3663096b-0ae2-4ab9-9321-d2a991afef86", new DateTime(2026, 4, 10, 10, 30, 0, 0, DateTimeKind.Unspecified), "Submitted" },
                    { 2, "ebfc3689-96da-433d-a042-58a90b0da38a", "8ebb4a85-c399-43a9-a9ed-9d85211bc67d", new DateTime(2026, 4, 11, 9, 15, 0, 0, DateTimeKind.Unspecified), "Submitted" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "TestRequests",
                keyColumn: "TestRequestId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "TestRequests",
                keyColumn: "TestRequestId",
                keyValue: 2);

            migrationBuilder.RenameColumn(
                name: "AssignTestId",
                table: "AssignTestTypes",
                newName: "Id");

            migrationBuilder.AddColumn<string>(
                name: "RequestNumber",
                table: "Samples",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TechnicianId",
                table: "AssignTestTypes",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_AssignTestTypes_TechnicianId",
                table: "AssignTestTypes",
                column: "TechnicianId");

            migrationBuilder.AddForeignKey(
                name: "FK_AssignTestTypes_AspNetUsers_TechnicianId",
                table: "AssignTestTypes",
                column: "TechnicianId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
