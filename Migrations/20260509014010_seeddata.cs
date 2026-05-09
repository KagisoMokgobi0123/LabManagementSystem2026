using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace NMBHLabSys.Migrations
{
    /// <inheritdoc />
    public partial class seeddata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestTypes_Samples_SampleTypeId",
                table: "TestTypes");

            migrationBuilder.RenameColumn(
                name: "SampleName",
                table: "SampleTypes",
                newName: "Name");

            migrationBuilder.AddColumn<int>(
                name: "TestCategoryId",
                table: "TestTypes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "RequestNumber",
                table: "TestRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "TestCategoryModel",
                columns: table => new
                {
                    TestCategoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestCategoryModel", x => x.TestCategoryId);
                });

            migrationBuilder.InsertData(
                table: "Consumables",
                columns: new[] { "ConsumableId", "Name", "QuantityOnHand", "ReorderLevel" },
                values: new object[,]
                {
                    { 1, "Test Tubes (EDTA)", 500, 100 },
                    { 2, "Syringes 5ml", 300, 50 },
                    { 3, "Alcohol Swabs", 1000, 200 },
                    { 4, "Gloves (Medium)", 800, 150 }
                });

            migrationBuilder.InsertData(
                table: "SampleTypes",
                columns: new[] { "SampleTypeId", "Name" },
                values: new object[,]
                {
                    { 1, "Blood" },
                    { 2, "Urine" },
                    { 3, "Saliva" },
                    { 4, "Plasma" },
                    { 5, "Serum" }
                });

            migrationBuilder.InsertData(
                table: "TestCategoryModel",
                columns: new[] { "TestCategoryId", "Description" },
                values: new object[,]
                {
                    { 1, "Hematology" },
                    { 2, "Biochemistry" },
                    { 3, "Microbiology" },
                    { 4, "Immunology" },
                    { 5, "Blood Banking" }
                });

            migrationBuilder.InsertData(
                table: "TestRequests",
                columns: new[] { "TestRequestId", "ClinicalNotes", "DoctorId", "PatientId", "RequestDate", "RequestNumber", "Status", "TestTypeModelTestTypeId", "Urgency" },
                values: new object[,]
                {
                    { 1, "Routine blood test", "cad131b2-791f-4d47-ae17-06f5e3864ef1", "7f8a9985-54df-4e2c-8433-14089d075dbf", new DateTime(2026, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "NMB-REQ-000001", "Pending", null, "Routine" },
                    { 2, "Suspected infection", "cad131b2-791f-4d47-ae17-06f5e3864ef1", "e6fa143c-9f79-4af2-8b53-97fb8ee94947", new DateTime(2026, 5, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "NMB-REQ-000002", "Samples Received", null, "Urgent" },
                    { 3, "Emergency test required", "b5c8ea7a-50fa-4df6-8e93-3b722c5377a2", "e6fa143c-9f79-4af2-8b53-97fb8ee94947", new DateTime(2026, 5, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "NMB-REQ-000003", "Pending", null, "Stat" }
                });

            migrationBuilder.InsertData(
                table: "TestTypes",
                columns: new[] { "TestTypeId", "ConsumableId", "MeasurementUnit", "Name", "NormalMax", "NormalMin", "SampleTypeId", "TestCategoryId", "TurnAroundTimeHours" },
                values: new object[,]
                {
                    { 1, 1, 1.0, "Full Blood Count", 11.0m, 4.5m, 1, 1, 2 },
                    { 2, 2, 1.0, "Urine Culture", 5m, 0m, 2, 2, 4 },
                    { 3, 3, 1.0, "Glucose Test", 5.5m, 3.9m, 1, 3, 1 }
                });

            migrationBuilder.InsertData(
                table: "TechnicianTestTypes",
                columns: new[] { "TechnicianTestTypeId", "AssignmentDate", "ManagerId", "RegisterUserId", "Status", "TechnicianId", "TestTypeId" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "9e586fad-975e-44b3-9b44-16bdf420354e", null, "Submitted", "6f9e21e3-6ae1-450d-90cf-9d89d3afe07a", 1 },
                    { 2, new DateTime(2026, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "9e586fad-975e-44b3-9b44-16bdf420354e", null, "Submitted", "6f9e21e3-6ae1-450d-90cf-9d89d3afe07a", 2 },
                    { 3, new DateTime(2026, 1, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "1ae2c09e-4743-4950-a377-fc9484ea4794", null, "Submitted", "6f9e21e3-6ae1-450d-90cf-9d89d3afe07a", 3 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_TestTypes_TestCategoryId",
                table: "TestTypes",
                column: "TestCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_TestTypes_SampleTypes_SampleTypeId",
                table: "TestTypes",
                column: "SampleTypeId",
                principalTable: "SampleTypes",
                principalColumn: "SampleTypeId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TestTypes_TestCategoryModel_TestCategoryId",
                table: "TestTypes",
                column: "TestCategoryId",
                principalTable: "TestCategoryModel",
                principalColumn: "TestCategoryId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestTypes_SampleTypes_SampleTypeId",
                table: "TestTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_TestTypes_TestCategoryModel_TestCategoryId",
                table: "TestTypes");

            migrationBuilder.DropTable(
                name: "TestCategoryModel");

            migrationBuilder.DropIndex(
                name: "IX_TestTypes_TestCategoryId",
                table: "TestTypes");

            migrationBuilder.DeleteData(
                table: "Consumables",
                keyColumn: "ConsumableId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "SampleTypes",
                keyColumn: "SampleTypeId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "SampleTypes",
                keyColumn: "SampleTypeId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "SampleTypes",
                keyColumn: "SampleTypeId",
                keyValue: 5);

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

            migrationBuilder.DeleteData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Consumables",
                keyColumn: "ConsumableId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Consumables",
                keyColumn: "ConsumableId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Consumables",
                keyColumn: "ConsumableId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "SampleTypes",
                keyColumn: "SampleTypeId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "SampleTypes",
                keyColumn: "SampleTypeId",
                keyValue: 2);

            migrationBuilder.DropColumn(
                name: "TestCategoryId",
                table: "TestTypes");

            migrationBuilder.DropColumn(
                name: "RequestNumber",
                table: "TestRequests");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "SampleTypes",
                newName: "SampleName");

            migrationBuilder.AddForeignKey(
                name: "FK_TestTypes_Samples_SampleTypeId",
                table: "TestTypes",
                column: "SampleTypeId",
                principalTable: "Samples",
                principalColumn: "SampleId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
