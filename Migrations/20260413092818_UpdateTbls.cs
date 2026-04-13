using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NMBHLabSys.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTbls : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClinicalNotes_TestRequests_TestRequestModelTestRequestId",
                table: "ClinicalNotes");

            migrationBuilder.DropTable(
                name: "Verifications");

            migrationBuilder.RenameColumn(
                name: "RecievedDateTime",
                table: "Samples",
                newName: "DateReceived");

            migrationBuilder.RenameColumn(
                name: "BarcodeNumber",
                table: "Samples",
                newName: "Barcode");

            migrationBuilder.RenameIndex(
                name: "IX_Samples_BarcodeNumber",
                table: "Samples",
                newName: "IX_Samples_Barcode");

            migrationBuilder.RenameColumn(
                name: "TestRequestModelTestRequestId",
                table: "ClinicalNotes",
                newName: "TestRequestsModelTestRequestId");

            migrationBuilder.RenameIndex(
                name: "IX_ClinicalNotes_TestRequestModelTestRequestId",
                table: "ClinicalNotes",
                newName: "IX_ClinicalNotes_TestRequestsModelTestRequestId");

            migrationBuilder.AddColumn<string>(
                name: "RequestNumber",
                table: "Samples",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "AssignTestTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TestRequestId = table.Column<int>(type: "int", nullable: false),
                    TestTypeId = table.Column<int>(type: "int", nullable: false),
                    TechnicianId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    StartDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletionDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssignTestTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssignTestTypes_AspNetUsers_TechnicianId",
                        column: x => x.TechnicianId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssignTestTypes_TestRequests_TestRequestId",
                        column: x => x.TestRequestId,
                        principalTable: "TestRequests",
                        principalColumn: "TestRequestId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssignTestTypes_TestTypes_TestTypeId",
                        column: x => x.TestTypeId,
                        principalTable: "TestTypes",
                        principalColumn: "TestTypeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssignTestTypes_TechnicianId",
                table: "AssignTestTypes",
                column: "TechnicianId");

            migrationBuilder.CreateIndex(
                name: "IX_AssignTestTypes_TestRequestId",
                table: "AssignTestTypes",
                column: "TestRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_AssignTestTypes_TestTypeId",
                table: "AssignTestTypes",
                column: "TestTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClinicalNotes_TestRequests_TestRequestsModelTestRequestId",
                table: "ClinicalNotes",
                column: "TestRequestsModelTestRequestId",
                principalTable: "TestRequests",
                principalColumn: "TestRequestId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClinicalNotes_TestRequests_TestRequestsModelTestRequestId",
                table: "ClinicalNotes");

            migrationBuilder.DropTable(
                name: "AssignTestTypes");

            migrationBuilder.DropColumn(
                name: "RequestNumber",
                table: "Samples");

            migrationBuilder.RenameColumn(
                name: "DateReceived",
                table: "Samples",
                newName: "RecievedDateTime");

            migrationBuilder.RenameColumn(
                name: "Barcode",
                table: "Samples",
                newName: "BarcodeNumber");

            migrationBuilder.RenameIndex(
                name: "IX_Samples_Barcode",
                table: "Samples",
                newName: "IX_Samples_BarcodeNumber");

            migrationBuilder.RenameColumn(
                name: "TestRequestsModelTestRequestId",
                table: "ClinicalNotes",
                newName: "TestRequestModelTestRequestId");

            migrationBuilder.RenameIndex(
                name: "IX_ClinicalNotes_TestRequestsModelTestRequestId",
                table: "ClinicalNotes",
                newName: "IX_ClinicalNotes_TestRequestModelTestRequestId");

            migrationBuilder.CreateTable(
                name: "Verifications",
                columns: table => new
                {
                    VerificationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestId = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TestRequestTestModelTestRequestTestId = table.Column<int>(type: "int", nullable: true),
                    VerificationDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Verifications", x => x.VerificationId);
                    table.ForeignKey(
                        name: "FK_Verifications_TestRequestTests_TestRequestTestModelTestRequestTestId",
                        column: x => x.TestRequestTestModelTestRequestTestId,
                        principalTable: "TestRequestTests",
                        principalColumn: "TestRequestTestId");
                    table.ForeignKey(
                        name: "FK_Verifications_TestRequests_RequestId",
                        column: x => x.RequestId,
                        principalTable: "TestRequests",
                        principalColumn: "TestRequestId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Verifications_RequestId",
                table: "Verifications",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_Verifications_TestRequestTestModelTestRequestTestId",
                table: "Verifications",
                column: "TestRequestTestModelTestRequestTestId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClinicalNotes_TestRequests_TestRequestModelTestRequestId",
                table: "ClinicalNotes",
                column: "TestRequestModelTestRequestId",
                principalTable: "TestRequests",
                principalColumn: "TestRequestId");
        }
    }
}
