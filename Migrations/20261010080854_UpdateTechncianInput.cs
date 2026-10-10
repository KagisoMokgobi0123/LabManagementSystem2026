using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NMBHLabSys.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTechncianInput : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TestTypeMeasurements",
                columns: table => new
                {
                    TestTypeMeasurementId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TestTypeId = table.Column<int>(type: "int", nullable: false),
                    MeasurementName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    MeasurementUnitId = table.Column<int>(type: "int", nullable: false),
                    NormalMin = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    NormalMax = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestTypeMeasurements", x => x.TestTypeMeasurementId);
                    table.ForeignKey(
                        name: "FK_TestTypeMeasurements_MeasurementUnits_MeasurementUnitId",
                        column: x => x.MeasurementUnitId,
                        principalTable: "MeasurementUnits",
                        principalColumn: "MeasurementUnitId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TestTypeMeasurements_TestTypes_TestTypeId",
                        column: x => x.TestTypeId,
                        principalTable: "TestTypes",
                        principalColumn: "TestTypeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TestResults",
                columns: table => new
                {
                    TestResultId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TestRequestTestTypeId = table.Column<int>(type: "int", nullable: false),
                    ResultName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ResultValue = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    MeasurementUnit = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ReferenceMin = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    ReferenceMax = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    IsAbnormal = table.Column<bool>(type: "bit", nullable: false),
                    ResultFlag = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TechnicianId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CapturedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    VerifiedByTechnicianId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    VerifiedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    VerificationNotes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    RequiresReview = table.Column<bool>(type: "bit", nullable: false),
                    ReviewNotes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    PreviousMeasurementUnit = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    NewMeasurementUnit = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PreviousReferenceMin = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    NewReferenceMin = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PreviousReferenceMax = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    NewReferenceMax = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PreviousIsAbnormal = table.Column<bool>(type: "bit", nullable: true),
                    NewIsAbnormal = table.Column<bool>(type: "bit", nullable: true),
                    PreviousResultFlag = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    NewResultFlag = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreviousResultNotes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    NewResultNotes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    TestTypeMeasurementId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestResults", x => x.TestResultId);
                    table.ForeignKey(
                        name: "FK_TestResults_AspNetUsers_TechnicianId",
                        column: x => x.TechnicianId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestResults_AspNetUsers_VerifiedByTechnicianId",
                        column: x => x.VerifiedByTechnicianId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestResults_TestRequestTestTypes_TestRequestTestTypeId",
                        column: x => x.TestRequestTestTypeId,
                        principalTable: "TestRequestTestTypes",
                        principalColumn: "TestRequestTestTypeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestResults_TestTypeMeasurements_TestTypeMeasurementId",
                        column: x => x.TestTypeMeasurementId,
                        principalTable: "TestTypeMeasurements",
                        principalColumn: "TestTypeMeasurementId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TestResultVerificationHistories",
                columns: table => new
                {
                    TestResultVerificationHistoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TestResultId = table.Column<int>(type: "int", nullable: false),
                    ActionByTechnicianId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ActionDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PreviousResultValue = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    NewResultValue = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PreviousMeasurementUnit = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    NewMeasurementUnit = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PreviousReferenceMin = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    NewReferenceMin = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PreviousReferenceMax = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    NewReferenceMax = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PreviousIsAbnormal = table.Column<bool>(type: "bit", nullable: true),
                    NewIsAbnormal = table.Column<bool>(type: "bit", nullable: true),
                    PreviousResultFlag = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    NewResultFlag = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PreviousResultNotes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    NewResultNotes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestResultVerificationHistories", x => x.TestResultVerificationHistoryId);
                    table.ForeignKey(
                        name: "FK_TestResultVerificationHistories_AspNetUsers_ActionByTechnicianId",
                        column: x => x.ActionByTechnicianId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestResultVerificationHistories_TestResults_TestResultId",
                        column: x => x.TestResultId,
                        principalTable: "TestResults",
                        principalColumn: "TestResultId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TestResults_TechnicianId",
                table: "TestResults",
                column: "TechnicianId");

            migrationBuilder.CreateIndex(
                name: "IX_TestResults_TestRequestTestTypeId_TestTypeMeasurementId",
                table: "TestResults",
                columns: new[] { "TestRequestTestTypeId", "TestTypeMeasurementId" },
                unique: true,
                filter: "[TestTypeMeasurementId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_TestResults_TestTypeMeasurementId",
                table: "TestResults",
                column: "TestTypeMeasurementId");

            migrationBuilder.CreateIndex(
                name: "IX_TestResults_VerifiedByTechnicianId",
                table: "TestResults",
                column: "VerifiedByTechnicianId");

            migrationBuilder.CreateIndex(
                name: "IX_TestResultVerificationHistories_ActionByTechnicianId",
                table: "TestResultVerificationHistories",
                column: "ActionByTechnicianId");

            migrationBuilder.CreateIndex(
                name: "IX_TestResultVerificationHistories_TestResultId",
                table: "TestResultVerificationHistories",
                column: "TestResultId");

            migrationBuilder.CreateIndex(
                name: "IX_TestTypeMeasurements_MeasurementUnitId",
                table: "TestTypeMeasurements",
                column: "MeasurementUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_TestTypeMeasurements_TestTypeId",
                table: "TestTypeMeasurements",
                column: "TestTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TestResultVerificationHistories");

            migrationBuilder.DropTable(
                name: "TestResults");

            migrationBuilder.DropTable(
                name: "TestTypeMeasurements");
        }
    }
}
