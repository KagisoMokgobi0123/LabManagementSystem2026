using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace NMBHLabSys.Migrations
{
    /// <inheritdoc />
    public partial class SeedTechnicianTestTypesTbl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "TechnicianTestTypes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                table: "TechnicianTestTypes",
                columns: new[] { "TechnicianTestTypeId", "AssignedDate", "IsActive", "ManagerId", "Notes", "TechnicianId", "TechnicianProfileId", "TestTypeId" },
                values: new object[,]
                {
                    { 4, new DateTime(2026, 1, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "1f4afa33-baa8-4f46-8143-2e2740c68fe8", "Assignment temporarily deactivated pending review.", "85a94ac3-e56a-4ae2-bea1-85d398486185", null, 1 },
                    { 5, new DateTime(2026, 1, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "1f4afa33-baa8-4f46-8143-2e2740c68fe8", "Assigned additional testing responsibilities.", "63950297-85b0-4b81-934a-bad2bf77f3cb", null, 3 },
                    { 20, new DateTime(2026, 1, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "1f4afa33-baa8-4f46-8143-2e2740c68fe8", "Assigned to perform routine laboratory tests.", "63950297-85b0-4b81-934a-bad2bf77f3cb", null, 1 },
                    { 30, new DateTime(2026, 1, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "1f4afa33-baa8-4f46-8143-2e2740c68fe8", "Responsible for processing and analysing test samples.", "63950297-85b0-4b81-934a-bad2bf77f3cb", null, 2 },
                    { 40, new DateTime(2026, 1, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "1f4afa33-baa8-4f46-8143-2e2740c68fe8", "Authorised to conduct specialised laboratory tests.", "85a94ac3-e56a-4ae2-bea1-85d398486185", null, 3 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "TechnicianTestTypes",
                keyColumn: "TechnicianTestTypeId",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "TechnicianTestTypes",
                keyColumn: "TechnicianTestTypeId",
                keyValue: 40);

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "TechnicianTestTypes");
        }
    }
}
