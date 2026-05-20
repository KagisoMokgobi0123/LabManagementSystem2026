using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NMBHLabSys.Migrations
{
    /// <inheritdoc />
    public partial class LinkDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Results_AspNetUsers_TechnicianId",
                table: "Results");

            migrationBuilder.DropForeignKey(
                name: "FK_Results_TestRequestTests_TestRequestTestTypeModelTestRequestTestTypeId",
                table: "Results");

            migrationBuilder.DropForeignKey(
                name: "FK_Samples_TestRequests_TestRequestId",
                table: "Samples");

            migrationBuilder.DropForeignKey(
                name: "FK_TestRequests_AspNetUsers_DoctorId",
                table: "TestRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_TestRequests_TestTypes_TestTypeModelTestTypeId",
                table: "TestRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_TestTypes_Consumables_ConsumableId",
                table: "TestTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_TestTypes_TestCategoryModel_TestCategoryId",
                table: "TestTypes");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_TestTypes_ConsumableId",
                table: "TestTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TestCategoryModel",
                table: "TestCategoryModel");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Results",
                table: "Results");

            migrationBuilder.RenameTable(
                name: "TestCategoryModel",
                newName: "TestCategories");

            migrationBuilder.RenameTable(
                name: "Results",
                newName: "ResultModel");

            migrationBuilder.RenameColumn(
                name: "TestTypeModelTestTypeId",
                table: "TestRequests",
                newName: "ResultReleaseId");

            migrationBuilder.RenameIndex(
                name: "IX_TestRequests_TestTypeModelTestTypeId",
                table: "TestRequests",
                newName: "IX_TestRequests_ResultReleaseId");

            migrationBuilder.RenameIndex(
                name: "IX_Results_TestRequestTestTypeModelTestRequestTestTypeId",
                table: "ResultModel",
                newName: "IX_ResultModel_TestRequestTestTypeModelTestRequestTestTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_Results_TechnicianId",
                table: "ResultModel",
                newName: "IX_ResultModel_TechnicianId");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "TestTypes",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "MeasurementUnit",
                table: "TestTypes",
                type: "int",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<string>(
                name: "ClinicalNotes",
                table: "TestRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Barcode",
                table: "TestRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CancelReason",
                table: "TestRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "TestRequests",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "DoctorId1",
                table: "TestRequests",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CollectedAt",
                table: "Samples",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "TestRequestsModelTestRequestId",
                table: "Samples",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TestRequestItemId",
                table: "ResultModel",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_TestCategories",
                table: "TestCategories",
                column: "TestCategoryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ResultModel",
                table: "ResultModel",
                column: "ResultId");

            migrationBuilder.CreateTable(
                name: "AllergyCategories",
                columns: table => new
                {
                    AllergyCategoryID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AllergyCategories", x => x.AllergyCategoryID);
                });

            migrationBuilder.CreateTable(
                name: "Doctortbl",
                columns: table => new
                {
                    DoctorId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HPCSANumber = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Doctortbl", x => x.DoctorId);
                });

            migrationBuilder.CreateTable(
                name: "LabTechnician",
                columns: table => new
                {
                    LabTechnicianID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Surname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmployeeNumber = table.Column<int>(type: "int", nullable: false),
                    CellPhone = table.Column<int>(type: "int", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LabTechnician", x => x.LabTechnicianID);
                });

            migrationBuilder.CreateTable(
                name: "MedicalConditionCategories",
                columns: table => new
                {
                    MedicalConditionCategoryID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicalConditionCategories", x => x.MedicalConditionCategoryID);
                });

            migrationBuilder.CreateTable(
                name: "MedicationCategories",
                columns: table => new
                {
                    MedicationCategoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicationCategories", x => x.MedicationCategoryId);
                });

            migrationBuilder.CreateTable(
                name: "Patients",
                columns: table => new
                {
                    PatientID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Surname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IDNumber = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CellPhone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HomeAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RegisterUserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patients", x => x.PatientID);
                    table.ForeignKey(
                        name: "FK_Patients_AspNetUsers_RegisterUserId",
                        column: x => x.RegisterUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    ReviewHistoryID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.ReviewHistoryID);
                });

            migrationBuilder.CreateTable(
                name: "Allergies",
                columns: table => new
                {
                    AllergyId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AllergyName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AllergyCategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Allergies", x => x.AllergyId);
                    table.ForeignKey(
                        name: "FK_Allergies_AllergyCategories_AllergyCategoryId",
                        column: x => x.AllergyCategoryId,
                        principalTable: "AllergyCategories",
                        principalColumn: "AllergyCategoryID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DoctorReports",
                columns: table => new
                {
                    DoctorReportId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DoctorId = table.Column<int>(type: "int", nullable: false),
                    DateFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateTo = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GeneratedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DoctorReports", x => x.DoctorReportId);
                    table.ForeignKey(
                        name: "FK_DoctorReports_Doctortbl_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "Doctortbl",
                        principalColumn: "DoctorId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ResultReleases",
                columns: table => new
                {
                    ResultReleaseId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TestRequestId = table.Column<int>(type: "int", nullable: false),
                    ReleasedByDoctorId = table.Column<int>(type: "int", nullable: false),
                    ReleasedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NotifyType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResultReleases", x => x.ResultReleaseId);
                    table.ForeignKey(
                        name: "FK_ResultReleases_Doctortbl_ReleasedByDoctorId",
                        column: x => x.ReleasedByDoctorId,
                        principalTable: "Doctortbl",
                        principalColumn: "DoctorId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ResultReleases_TestRequests_TestRequestId",
                        column: x => x.TestRequestId,
                        principalTable: "TestRequests",
                        principalColumn: "TestRequestId");
                });

            migrationBuilder.CreateTable(
                name: "TestRequestItems",
                columns: table => new
                {
                    TestRequestItemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TestRequestID = table.Column<int>(type: "int", nullable: false),
                    TestTypeID = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsAbnormal = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    AssignedTechnicianId = table.Column<int>(type: "int", nullable: true),
                    ResultValue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ResultNotes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletionDateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestRequestItems", x => x.TestRequestItemId);
                    table.ForeignKey(
                        name: "FK_TestRequestItems_LabTechnician_AssignedTechnicianId",
                        column: x => x.AssignedTechnicianId,
                        principalTable: "LabTechnician",
                        principalColumn: "LabTechnicianID");
                    table.ForeignKey(
                        name: "FK_TestRequestItems_TestRequests_TestRequestID",
                        column: x => x.TestRequestID,
                        principalTable: "TestRequests",
                        principalColumn: "TestRequestId");
                    table.ForeignKey(
                        name: "FK_TestRequestItems_TestTypes_TestTypeID",
                        column: x => x.TestTypeID,
                        principalTable: "TestTypes",
                        principalColumn: "TestTypeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MedicalConditions",
                columns: table => new
                {
                    MedicalConditionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ConditionName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MedicalConditionCategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicalConditions", x => x.MedicalConditionId);
                    table.ForeignKey(
                        name: "FK_MedicalConditions_MedicalConditionCategories_MedicalConditionCategoryId",
                        column: x => x.MedicalConditionCategoryId,
                        principalTable: "MedicalConditionCategories",
                        principalColumn: "MedicalConditionCategoryID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Medications",
                columns: table => new
                {
                    MedicationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MedicationName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MedicationCategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Medications", x => x.MedicationId);
                    table.ForeignKey(
                        name: "FK_Medications_MedicationCategories_MedicationCategoryId",
                        column: x => x.MedicationCategoryId,
                        principalTable: "MedicationCategories",
                        principalColumn: "MedicationCategoryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PatientConsents",
                columns: table => new
                {
                    PatientConsentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientId = table.Column<int>(type: "int", nullable: false),
                    DoctorId = table.Column<int>(type: "int", nullable: false),
                    GrantedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RevokedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientConsents", x => x.PatientConsentId);
                    table.ForeignKey(
                        name: "FK_PatientConsents_Doctortbl_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "Doctortbl",
                        principalColumn: "DoctorId");
                    table.ForeignKey(
                        name: "FK_PatientConsents_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "PatientID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PatientDoctorConsents",
                columns: table => new
                {
                    PatientDoctorConsentID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientID = table.Column<int>(type: "int", nullable: false),
                    DoctorUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientDoctorConsents", x => x.PatientDoctorConsentID);
                    table.ForeignKey(
                        name: "FK_PatientDoctorConsents_AspNetUsers_DoctorUserId",
                        column: x => x.DoctorUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PatientDoctorConsents_Patients_PatientID",
                        column: x => x.PatientID,
                        principalTable: "Patients",
                        principalColumn: "PatientID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PatientAllergies",
                columns: table => new
                {
                    PatientAllergyID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientID = table.Column<int>(type: "int", nullable: false),
                    AllergyID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientAllergies", x => x.PatientAllergyID);
                    table.ForeignKey(
                        name: "FK_PatientAllergies_Allergies_AllergyID",
                        column: x => x.AllergyID,
                        principalTable: "Allergies",
                        principalColumn: "AllergyId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PatientAllergies_Patients_PatientID",
                        column: x => x.PatientID,
                        principalTable: "Patients",
                        principalColumn: "PatientID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DoctorAlerts",
                columns: table => new
                {
                    DoctorAlertId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DoctorId = table.Column<int>(type: "int", nullable: false),
                    RequestItemId = table.Column<int>(type: "int", nullable: false),
                    TestRequestId = table.Column<int>(type: "int", nullable: false),
                    AlertDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DoctorId1 = table.Column<int>(type: "int", nullable: true),
                    TestRequestItemId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DoctorAlerts", x => x.DoctorAlertId);
                    table.ForeignKey(
                        name: "FK_DoctorAlerts_Doctortbl_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "Doctortbl",
                        principalColumn: "DoctorId");
                    table.ForeignKey(
                        name: "FK_DoctorAlerts_Doctortbl_DoctorId1",
                        column: x => x.DoctorId1,
                        principalTable: "Doctortbl",
                        principalColumn: "DoctorId");
                    table.ForeignKey(
                        name: "FK_DoctorAlerts_TestRequestItems_RequestItemId",
                        column: x => x.RequestItemId,
                        principalTable: "TestRequestItems",
                        principalColumn: "TestRequestItemId");
                    table.ForeignKey(
                        name: "FK_DoctorAlerts_TestRequestItems_TestRequestItemId",
                        column: x => x.TestRequestItemId,
                        principalTable: "TestRequestItems",
                        principalColumn: "TestRequestItemId");
                    table.ForeignKey(
                        name: "FK_DoctorAlerts_TestRequests_TestRequestId",
                        column: x => x.TestRequestId,
                        principalTable: "TestRequests",
                        principalColumn: "TestRequestId");
                });

            migrationBuilder.CreateTable(
                name: "PatientConditions",
                columns: table => new
                {
                    PatientConditionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientID = table.Column<int>(type: "int", nullable: false),
                    MedicalConditionID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientConditions", x => x.PatientConditionID);
                    table.ForeignKey(
                        name: "FK_PatientConditions_MedicalConditions_MedicalConditionID",
                        column: x => x.MedicalConditionID,
                        principalTable: "MedicalConditions",
                        principalColumn: "MedicalConditionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PatientConditions_Patients_PatientID",
                        column: x => x.PatientID,
                        principalTable: "Patients",
                        principalColumn: "PatientID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PatientMedications",
                columns: table => new
                {
                    PatientMedicationID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientID = table.Column<int>(type: "int", nullable: false),
                    MedicationId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientMedications", x => x.PatientMedicationID);
                    table.ForeignKey(
                        name: "FK_PatientMedications_Medications_MedicationId",
                        column: x => x.MedicationId,
                        principalTable: "Medications",
                        principalColumn: "MedicationId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PatientMedications_Patients_PatientID",
                        column: x => x.PatientID,
                        principalTable: "Patients",
                        principalColumn: "PatientID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ConsentRequestItems",
                columns: table => new
                {
                    ConsentRequestItemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ConsentRequestId = table.Column<int>(type: "int", nullable: false),
                    TestRequestID = table.Column<int>(type: "int", nullable: false),
                    PatientConsentID = table.Column<int>(type: "int", nullable: false),
                    GrantedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConsentRequestItems", x => x.ConsentRequestItemId);
                    table.ForeignKey(
                        name: "FK_ConsentRequestItems_PatientConsents_PatientConsentID",
                        column: x => x.PatientConsentID,
                        principalTable: "PatientConsents",
                        principalColumn: "PatientConsentId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ConsentRequestItems_TestRequests_TestRequestID",
                        column: x => x.TestRequestID,
                        principalTable: "TestRequests",
                        principalColumn: "TestRequestId");
                });

            migrationBuilder.UpdateData(
                table: "Samples",
                keyColumn: "SampleId",
                keyValue: 1,
                columns: new[] { "CollectedAt", "TestRequestsModelTestRequestId" },
                values: new object[] { new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null });

            migrationBuilder.UpdateData(
                table: "Samples",
                keyColumn: "SampleId",
                keyValue: 2,
                columns: new[] { "CollectedAt", "TestRequestsModelTestRequestId" },
                values: new object[] { new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null });

            migrationBuilder.UpdateData(
                table: "Samples",
                keyColumn: "SampleId",
                keyValue: 3,
                columns: new[] { "CollectedAt", "TestRequestsModelTestRequestId" },
                values: new object[] { new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null });

            migrationBuilder.UpdateData(
                table: "Samples",
                keyColumn: "SampleId",
                keyValue: 4,
                columns: new[] { "CollectedAt", "TestRequestsModelTestRequestId" },
                values: new object[] { new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null });

            migrationBuilder.UpdateData(
                table: "Samples",
                keyColumn: "SampleId",
                keyValue: 5,
                columns: new[] { "CollectedAt", "TestRequestsModelTestRequestId" },
                values: new object[] { new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null });

            migrationBuilder.UpdateData(
                table: "Samples",
                keyColumn: "SampleId",
                keyValue: 6,
                columns: new[] { "CollectedAt", "TestRequestsModelTestRequestId" },
                values: new object[] { new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null });

            migrationBuilder.UpdateData(
                table: "Samples",
                keyColumn: "SampleId",
                keyValue: 9,
                columns: new[] { "CollectedAt", "TestRequestsModelTestRequestId" },
                values: new object[] { new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null });

            migrationBuilder.UpdateData(
                table: "TestRequestTests",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1,
                columns: new[] { "CompletionDateTime", "DueDate", "StartDateTime" },
                values: new object[] { new DateTime(2026, 5, 20, 15, 37, 58, 871, DateTimeKind.Local).AddTicks(8279), new DateTime(2026, 5, 20, 17, 37, 58, 871, DateTimeKind.Local).AddTicks(8280), new DateTime(2026, 5, 20, 15, 37, 58, 871, DateTimeKind.Local).AddTicks(8266) });

            migrationBuilder.UpdateData(
                table: "TestRequestTests",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 2,
                columns: new[] { "CompletionDateTime", "DueDate", "StartDateTime" },
                values: new object[] { new DateTime(2026, 5, 20, 15, 37, 58, 871, DateTimeKind.Local).AddTicks(8289), new DateTime(2026, 5, 20, 16, 37, 58, 871, DateTimeKind.Local).AddTicks(8289), new DateTime(2026, 5, 20, 15, 37, 58, 871, DateTimeKind.Local).AddTicks(8288) });

            migrationBuilder.UpdateData(
                table: "TestRequestTests",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 3,
                columns: new[] { "CompletionDateTime", "DueDate", "StartDateTime" },
                values: new object[] { new DateTime(2026, 5, 20, 15, 37, 58, 871, DateTimeKind.Local).AddTicks(8292), new DateTime(2026, 5, 20, 17, 37, 58, 871, DateTimeKind.Local).AddTicks(8292), new DateTime(2026, 5, 20, 13, 37, 58, 871, DateTimeKind.Local).AddTicks(8291) });

            migrationBuilder.UpdateData(
                table: "TestRequestTests",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 4,
                columns: new[] { "CompletionDateTime", "DueDate", "StartDateTime" },
                values: new object[] { new DateTime(2026, 5, 20, 15, 37, 58, 871, DateTimeKind.Local).AddTicks(8294), new DateTime(2026, 5, 20, 17, 37, 58, 871, DateTimeKind.Local).AddTicks(8295), new DateTime(2026, 5, 20, 15, 37, 58, 871, DateTimeKind.Local).AddTicks(8294) });

            migrationBuilder.UpdateData(
                table: "TestRequestTests",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 5,
                columns: new[] { "CompletionDateTime", "DueDate", "StartDateTime" },
                values: new object[] { new DateTime(2026, 5, 20, 14, 37, 58, 871, DateTimeKind.Local).AddTicks(8298), new DateTime(2026, 5, 20, 14, 37, 58, 871, DateTimeKind.Local).AddTicks(8298), new DateTime(2026, 5, 20, 12, 37, 58, 871, DateTimeKind.Local).AddTicks(8297) });

            migrationBuilder.UpdateData(
                table: "TestRequestTests",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 6,
                columns: new[] { "CompletionDateTime", "DueDate", "StartDateTime", "VerificationDateTime" },
                values: new object[] { new DateTime(2026, 5, 20, 13, 37, 58, 871, DateTimeKind.Local).AddTicks(8302), new DateTime(2026, 5, 20, 13, 37, 58, 871, DateTimeKind.Local).AddTicks(8302), new DateTime(2026, 5, 20, 12, 37, 58, 871, DateTimeKind.Local).AddTicks(8301), new DateTime(2026, 5, 20, 14, 7, 58, 871, DateTimeKind.Local).AddTicks(8304) });

            migrationBuilder.UpdateData(
                table: "TestRequestTests",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 7,
                columns: new[] { "CompletionDateTime", "DueDate", "StartDateTime", "VerificationDateTime" },
                values: new object[] { new DateTime(2026, 5, 20, 15, 7, 58, 871, DateTimeKind.Local).AddTicks(8306), new DateTime(2026, 5, 20, 15, 37, 58, 871, DateTimeKind.Local).AddTicks(8306), new DateTime(2026, 5, 20, 13, 37, 58, 871, DateTimeKind.Local).AddTicks(8305), new DateTime(2026, 5, 20, 15, 7, 58, 871, DateTimeKind.Local).AddTicks(8307) });

            migrationBuilder.UpdateData(
                table: "TestRequests",
                keyColumn: "TestRequestId",
                keyValue: 1,
                columns: new[] { "Barcode", "CancelReason", "CreatedAt", "DoctorId1" },
                values: new object[] { "0099GGRM", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null });

            migrationBuilder.UpdateData(
                table: "TestRequests",
                keyColumn: "TestRequestId",
                keyValue: 2,
                columns: new[] { "Barcode", "CancelReason", "CreatedAt", "DoctorId1" },
                values: new object[] { "00TTR1YMM", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null });

            migrationBuilder.UpdateData(
                table: "TestRequests",
                keyColumn: "TestRequestId",
                keyValue: 3,
                columns: new[] { "Barcode", "CancelReason", "CreatedAt", "DoctorId1" },
                values: new object[] { "00FFD1YMM", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null });

            migrationBuilder.UpdateData(
                table: "TestRequests",
                keyColumn: "TestRequestId",
                keyValue: 4,
                columns: new[] { "Barcode", "CancelReason", "CreatedAt", "DoctorId1" },
                values: new object[] { "0045ESM", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null });

            migrationBuilder.UpdateData(
                table: "TestRequests",
                keyColumn: "TestRequestId",
                keyValue: 5,
                columns: new[] { "Barcode", "CancelReason", "CreatedAt", "DoctorId1" },
                values: new object[] { "005433YMV", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null });

            migrationBuilder.UpdateData(
                table: "TestRequests",
                keyColumn: "TestRequestId",
                keyValue: 6,
                columns: new[] { "Barcode", "CancelReason", "CreatedAt", "DoctorId1" },
                values: new object[] { "009988HGM", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null });

            migrationBuilder.UpdateData(
                table: "TestRequests",
                keyColumn: "TestRequestId",
                keyValue: 7,
                columns: new[] { "Barcode", "CancelReason", "CreatedAt", "DoctorId1" },
                values: new object[] { "00876YMM", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null });

            migrationBuilder.UpdateData(
                table: "TestRequests",
                keyColumn: "TestRequestId",
                keyValue: 8,
                columns: new[] { "Barcode", "CancelReason", "CreatedAt", "DoctorId1" },
                values: new object[] { "00991YMM", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null });

            migrationBuilder.UpdateData(
                table: "TestRequests",
                keyColumn: "TestRequestId",
                keyValue: 9,
                columns: new[] { "Barcode", "CancelReason", "CreatedAt", "DoctorId1" },
                values: new object[] { "00HHHYMM", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null });

            migrationBuilder.UpdateData(
                table: "TestRequests",
                keyColumn: "TestRequestId",
                keyValue: 10,
                columns: new[] { "Barcode", "CancelReason", "CreatedAt", "DoctorId1" },
                values: new object[] { "04561YMM", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null });

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 1,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 20, 15, 37, 58, 871, DateTimeKind.Local).AddTicks(8602));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 2,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 20, 15, 37, 58, 871, DateTimeKind.Local).AddTicks(8604));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 3,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 20, 15, 37, 58, 871, DateTimeKind.Local).AddTicks(8606));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 4,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 20, 15, 37, 58, 871, DateTimeKind.Local).AddTicks(8607));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 5,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 20, 15, 37, 58, 871, DateTimeKind.Local).AddTicks(8608));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 6,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 20, 15, 37, 58, 871, DateTimeKind.Local).AddTicks(8609));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 7,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 20, 15, 37, 58, 871, DateTimeKind.Local).AddTicks(8610));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 8,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 20, 15, 37, 58, 871, DateTimeKind.Local).AddTicks(8611));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 9,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 20, 15, 37, 58, 871, DateTimeKind.Local).AddTicks(8612));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 10,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 20, 15, 37, 58, 871, DateTimeKind.Local).AddTicks(8614));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 11,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 20, 15, 37, 58, 871, DateTimeKind.Local).AddTicks(8615));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 12,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 20, 15, 37, 58, 871, DateTimeKind.Local).AddTicks(8615));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 13,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 20, 15, 37, 58, 871, DateTimeKind.Local).AddTicks(8616));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 14,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 20, 15, 37, 58, 871, DateTimeKind.Local).AddTicks(8618));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 15,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 20, 15, 37, 58, 871, DateTimeKind.Local).AddTicks(8619));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 16,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 20, 15, 37, 58, 871, DateTimeKind.Local).AddTicks(8620));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 17,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 20, 15, 37, 58, 871, DateTimeKind.Local).AddTicks(8621));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 18,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 20, 15, 37, 58, 871, DateTimeKind.Local).AddTicks(8622));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 19,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 20, 15, 37, 58, 871, DateTimeKind.Local).AddTicks(8623));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 20,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 20, 15, 37, 58, 871, DateTimeKind.Local).AddTicks(8624));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 21,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 20, 15, 37, 58, 871, DateTimeKind.Local).AddTicks(8625));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 22,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 20, 15, 37, 58, 871, DateTimeKind.Local).AddTicks(8626));

            migrationBuilder.UpdateData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 1,
                column: "MeasurementUnit",
                value: 1);

            migrationBuilder.UpdateData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 2,
                column: "MeasurementUnit",
                value: 2);

            migrationBuilder.UpdateData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 3,
                column: "MeasurementUnit",
                value: 3);

            migrationBuilder.UpdateData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 4,
                column: "MeasurementUnit",
                value: 2);

            migrationBuilder.UpdateData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 5,
                column: "MeasurementUnit",
                value: 4);

            migrationBuilder.UpdateData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 6,
                column: "MeasurementUnit",
                value: 5);

            migrationBuilder.UpdateData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 7,
                column: "MeasurementUnit",
                value: 5);

            migrationBuilder.UpdateData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 8,
                column: "MeasurementUnit",
                value: 6);

            migrationBuilder.UpdateData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 9,
                column: "MeasurementUnit",
                value: 7);

            migrationBuilder.UpdateData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 10,
                column: "MeasurementUnit",
                value: 8);

            migrationBuilder.UpdateData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 11,
                column: "MeasurementUnit",
                value: 9);

            migrationBuilder.UpdateData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 12,
                column: "MeasurementUnit",
                value: 2);

            migrationBuilder.UpdateData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 13,
                column: "MeasurementUnit",
                value: 9);

            migrationBuilder.UpdateData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 14,
                column: "MeasurementUnit",
                value: 10);

            migrationBuilder.UpdateData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 15,
                column: "MeasurementUnit",
                value: 9);

            migrationBuilder.UpdateData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 16,
                column: "MeasurementUnit",
                value: 7);

            migrationBuilder.UpdateData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 17,
                column: "MeasurementUnit",
                value: 3);

            migrationBuilder.UpdateData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 18,
                column: "MeasurementUnit",
                value: 1);

            migrationBuilder.UpdateData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 19,
                column: "MeasurementUnit",
                value: 11);

            migrationBuilder.UpdateData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 20,
                column: "MeasurementUnit",
                value: 3);

            migrationBuilder.UpdateData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 21,
                column: "MeasurementUnit",
                value: 3);

            migrationBuilder.UpdateData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 22,
                column: "MeasurementUnit",
                value: 3);

            migrationBuilder.CreateIndex(
                name: "IX_TestRequests_DoctorId1",
                table: "TestRequests",
                column: "DoctorId1");

            migrationBuilder.CreateIndex(
                name: "IX_Samples_TestRequestsModelTestRequestId",
                table: "Samples",
                column: "TestRequestsModelTestRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_ResultModel_TestRequestItemId",
                table: "ResultModel",
                column: "TestRequestItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Allergies_AllergyCategoryId",
                table: "Allergies",
                column: "AllergyCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ConsentRequestItems_PatientConsentID",
                table: "ConsentRequestItems",
                column: "PatientConsentID");

            migrationBuilder.CreateIndex(
                name: "IX_ConsentRequestItems_TestRequestID",
                table: "ConsentRequestItems",
                column: "TestRequestID");

            migrationBuilder.CreateIndex(
                name: "IX_DoctorAlerts_DoctorId",
                table: "DoctorAlerts",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_DoctorAlerts_DoctorId1",
                table: "DoctorAlerts",
                column: "DoctorId1");

            migrationBuilder.CreateIndex(
                name: "IX_DoctorAlerts_RequestItemId",
                table: "DoctorAlerts",
                column: "RequestItemId");

            migrationBuilder.CreateIndex(
                name: "IX_DoctorAlerts_TestRequestId",
                table: "DoctorAlerts",
                column: "TestRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_DoctorAlerts_TestRequestItemId",
                table: "DoctorAlerts",
                column: "TestRequestItemId");

            migrationBuilder.CreateIndex(
                name: "IX_DoctorReports_DoctorId",
                table: "DoctorReports",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalConditions_MedicalConditionCategoryId",
                table: "MedicalConditions",
                column: "MedicalConditionCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Medications_MedicationCategoryId",
                table: "Medications",
                column: "MedicationCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientAllergies_AllergyID",
                table: "PatientAllergies",
                column: "AllergyID");

            migrationBuilder.CreateIndex(
                name: "IX_PatientAllergies_PatientID",
                table: "PatientAllergies",
                column: "PatientID");

            migrationBuilder.CreateIndex(
                name: "IX_PatientConditions_MedicalConditionID",
                table: "PatientConditions",
                column: "MedicalConditionID");

            migrationBuilder.CreateIndex(
                name: "IX_PatientConditions_PatientID",
                table: "PatientConditions",
                column: "PatientID");

            migrationBuilder.CreateIndex(
                name: "IX_PatientConsents_DoctorId",
                table: "PatientConsents",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientConsents_PatientId",
                table: "PatientConsents",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientDoctorConsents_DoctorUserId",
                table: "PatientDoctorConsents",
                column: "DoctorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientDoctorConsents_PatientID",
                table: "PatientDoctorConsents",
                column: "PatientID");

            migrationBuilder.CreateIndex(
                name: "IX_PatientMedications_MedicationId",
                table: "PatientMedications",
                column: "MedicationId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientMedications_PatientID",
                table: "PatientMedications",
                column: "PatientID");

            migrationBuilder.CreateIndex(
                name: "IX_Patients_RegisterUserId",
                table: "Patients",
                column: "RegisterUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ResultReleases_ReleasedByDoctorId",
                table: "ResultReleases",
                column: "ReleasedByDoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_ResultReleases_TestRequestId",
                table: "ResultReleases",
                column: "TestRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_TestRequestItems_AssignedTechnicianId",
                table: "TestRequestItems",
                column: "AssignedTechnicianId");

            migrationBuilder.CreateIndex(
                name: "IX_TestRequestItems_TestRequestID",
                table: "TestRequestItems",
                column: "TestRequestID");

            migrationBuilder.CreateIndex(
                name: "IX_TestRequestItems_TestTypeID",
                table: "TestRequestItems",
                column: "TestTypeID");

            migrationBuilder.AddForeignKey(
                name: "FK_ResultModel_AspNetUsers_TechnicianId",
                table: "ResultModel",
                column: "TechnicianId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ResultModel_TestRequestItems_TestRequestItemId",
                table: "ResultModel",
                column: "TestRequestItemId",
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
                name: "FK_Samples_TestRequests_TestRequestId",
                table: "Samples",
                column: "TestRequestId",
                principalTable: "TestRequests",
                principalColumn: "TestRequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_Samples_TestRequests_TestRequestsModelTestRequestId",
                table: "Samples",
                column: "TestRequestsModelTestRequestId",
                principalTable: "TestRequests",
                principalColumn: "TestRequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_TestRequests_AspNetUsers_DoctorId",
                table: "TestRequests",
                column: "DoctorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TestRequests_Doctortbl_DoctorId1",
                table: "TestRequests",
                column: "DoctorId1",
                principalTable: "Doctortbl",
                principalColumn: "DoctorId");

            migrationBuilder.AddForeignKey(
                name: "FK_TestRequests_ResultReleases_ResultReleaseId",
                table: "TestRequests",
                column: "ResultReleaseId",
                principalTable: "ResultReleases",
                principalColumn: "ResultReleaseId");

            migrationBuilder.AddForeignKey(
                name: "FK_TestTypes_TestCategories_TestCategoryId",
                table: "TestTypes",
                column: "TestCategoryId",
                principalTable: "TestCategories",
                principalColumn: "TestCategoryId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ResultModel_AspNetUsers_TechnicianId",
                table: "ResultModel");

            migrationBuilder.DropForeignKey(
                name: "FK_ResultModel_TestRequestItems_TestRequestItemId",
                table: "ResultModel");

            migrationBuilder.DropForeignKey(
                name: "FK_ResultModel_TestRequestTests_TestRequestTestTypeModelTestRequestTestTypeId",
                table: "ResultModel");

            migrationBuilder.DropForeignKey(
                name: "FK_Samples_TestRequests_TestRequestId",
                table: "Samples");

            migrationBuilder.DropForeignKey(
                name: "FK_Samples_TestRequests_TestRequestsModelTestRequestId",
                table: "Samples");

            migrationBuilder.DropForeignKey(
                name: "FK_TestRequests_AspNetUsers_DoctorId",
                table: "TestRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_TestRequests_Doctortbl_DoctorId1",
                table: "TestRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_TestRequests_ResultReleases_ResultReleaseId",
                table: "TestRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_TestTypes_TestCategories_TestCategoryId",
                table: "TestTypes");

            migrationBuilder.DropTable(
                name: "ConsentRequestItems");

            migrationBuilder.DropTable(
                name: "DoctorAlerts");

            migrationBuilder.DropTable(
                name: "DoctorReports");

            migrationBuilder.DropTable(
                name: "PatientAllergies");

            migrationBuilder.DropTable(
                name: "PatientConditions");

            migrationBuilder.DropTable(
                name: "PatientDoctorConsents");

            migrationBuilder.DropTable(
                name: "PatientMedications");

            migrationBuilder.DropTable(
                name: "ResultReleases");

            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropTable(
                name: "PatientConsents");

            migrationBuilder.DropTable(
                name: "TestRequestItems");

            migrationBuilder.DropTable(
                name: "Allergies");

            migrationBuilder.DropTable(
                name: "MedicalConditions");

            migrationBuilder.DropTable(
                name: "Medications");

            migrationBuilder.DropTable(
                name: "Doctortbl");

            migrationBuilder.DropTable(
                name: "Patients");

            migrationBuilder.DropTable(
                name: "LabTechnician");

            migrationBuilder.DropTable(
                name: "AllergyCategories");

            migrationBuilder.DropTable(
                name: "MedicalConditionCategories");

            migrationBuilder.DropTable(
                name: "MedicationCategories");

            migrationBuilder.DropIndex(
                name: "IX_TestRequests_DoctorId1",
                table: "TestRequests");

            migrationBuilder.DropIndex(
                name: "IX_Samples_TestRequestsModelTestRequestId",
                table: "Samples");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TestCategories",
                table: "TestCategories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ResultModel",
                table: "ResultModel");

            migrationBuilder.DropIndex(
                name: "IX_ResultModel_TestRequestItemId",
                table: "ResultModel");

            migrationBuilder.DropColumn(
                name: "Barcode",
                table: "TestRequests");

            migrationBuilder.DropColumn(
                name: "CancelReason",
                table: "TestRequests");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "TestRequests");

            migrationBuilder.DropColumn(
                name: "DoctorId1",
                table: "TestRequests");

            migrationBuilder.DropColumn(
                name: "CollectedAt",
                table: "Samples");

            migrationBuilder.DropColumn(
                name: "TestRequestsModelTestRequestId",
                table: "Samples");

            migrationBuilder.DropColumn(
                name: "TestRequestItemId",
                table: "ResultModel");

            migrationBuilder.RenameTable(
                name: "TestCategories",
                newName: "TestCategoryModel");

            migrationBuilder.RenameTable(
                name: "ResultModel",
                newName: "Results");

            migrationBuilder.RenameColumn(
                name: "ResultReleaseId",
                table: "TestRequests",
                newName: "TestTypeModelTestTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_TestRequests_ResultReleaseId",
                table: "TestRequests",
                newName: "IX_TestRequests_TestTypeModelTestTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_ResultModel_TestRequestTestTypeModelTestRequestTestTypeId",
                table: "Results",
                newName: "IX_Results_TestRequestTestTypeModelTestRequestTestTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_ResultModel_TechnicianId",
                table: "Results",
                newName: "IX_Results_TechnicianId");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "TestTypes",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<double>(
                name: "MeasurementUnit",
                table: "TestTypes",
                type: "float",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "ClinicalNotes",
                table: "TestRequests",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TestCategoryModel",
                table: "TestCategoryModel",
                column: "TestCategoryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Results",
                table: "Results",
                column: "ResultId");

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    NotificationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DoctorId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IssuedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.NotificationId);
                    table.ForeignKey(
                        name: "FK_Notifications_AspNetUsers_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "TestRequestTests",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1,
                columns: new[] { "CompletionDateTime", "DueDate", "StartDateTime" },
                values: new object[] { new DateTime(2026, 5, 16, 23, 18, 41, 223, DateTimeKind.Local).AddTicks(3658), new DateTime(2026, 5, 17, 1, 18, 41, 223, DateTimeKind.Local).AddTicks(3660), new DateTime(2026, 5, 16, 23, 18, 41, 223, DateTimeKind.Local).AddTicks(3649) });

            migrationBuilder.UpdateData(
                table: "TestRequestTests",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 2,
                columns: new[] { "CompletionDateTime", "DueDate", "StartDateTime" },
                values: new object[] { new DateTime(2026, 5, 16, 23, 18, 41, 223, DateTimeKind.Local).AddTicks(3669), new DateTime(2026, 5, 17, 0, 18, 41, 223, DateTimeKind.Local).AddTicks(3670), new DateTime(2026, 5, 16, 23, 18, 41, 223, DateTimeKind.Local).AddTicks(3668) });

            migrationBuilder.UpdateData(
                table: "TestRequestTests",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 3,
                columns: new[] { "CompletionDateTime", "DueDate", "StartDateTime" },
                values: new object[] { new DateTime(2026, 5, 16, 23, 18, 41, 223, DateTimeKind.Local).AddTicks(3673), new DateTime(2026, 5, 17, 1, 18, 41, 223, DateTimeKind.Local).AddTicks(3673), new DateTime(2026, 5, 16, 21, 18, 41, 223, DateTimeKind.Local).AddTicks(3672) });

            migrationBuilder.UpdateData(
                table: "TestRequestTests",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 4,
                columns: new[] { "CompletionDateTime", "DueDate", "StartDateTime" },
                values: new object[] { new DateTime(2026, 5, 16, 23, 18, 41, 223, DateTimeKind.Local).AddTicks(3676), new DateTime(2026, 5, 17, 1, 18, 41, 223, DateTimeKind.Local).AddTicks(3677), new DateTime(2026, 5, 16, 23, 18, 41, 223, DateTimeKind.Local).AddTicks(3676) });

            migrationBuilder.UpdateData(
                table: "TestRequestTests",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 5,
                columns: new[] { "CompletionDateTime", "DueDate", "StartDateTime" },
                values: new object[] { new DateTime(2026, 5, 16, 22, 18, 41, 223, DateTimeKind.Local).AddTicks(3681), new DateTime(2026, 5, 16, 22, 18, 41, 223, DateTimeKind.Local).AddTicks(3682), new DateTime(2026, 5, 16, 20, 18, 41, 223, DateTimeKind.Local).AddTicks(3680) });

            migrationBuilder.UpdateData(
                table: "TestRequestTests",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 6,
                columns: new[] { "CompletionDateTime", "DueDate", "StartDateTime", "VerificationDateTime" },
                values: new object[] { new DateTime(2026, 5, 16, 21, 18, 41, 223, DateTimeKind.Local).AddTicks(3686), new DateTime(2026, 5, 16, 21, 18, 41, 223, DateTimeKind.Local).AddTicks(3687), new DateTime(2026, 5, 16, 20, 18, 41, 223, DateTimeKind.Local).AddTicks(3685), new DateTime(2026, 5, 16, 21, 48, 41, 223, DateTimeKind.Local).AddTicks(3690) });

            migrationBuilder.UpdateData(
                table: "TestRequestTests",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 7,
                columns: new[] { "CompletionDateTime", "DueDate", "StartDateTime", "VerificationDateTime" },
                values: new object[] { new DateTime(2026, 5, 16, 22, 48, 41, 223, DateTimeKind.Local).AddTicks(3693), new DateTime(2026, 5, 16, 23, 18, 41, 223, DateTimeKind.Local).AddTicks(3695), new DateTime(2026, 5, 16, 21, 18, 41, 223, DateTimeKind.Local).AddTicks(3693), new DateTime(2026, 5, 16, 22, 48, 41, 223, DateTimeKind.Local).AddTicks(3696) });

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 1,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 23, 18, 41, 223, DateTimeKind.Local).AddTicks(3952));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 2,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 23, 18, 41, 223, DateTimeKind.Local).AddTicks(3955));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 3,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 23, 18, 41, 223, DateTimeKind.Local).AddTicks(3956));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 4,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 23, 18, 41, 223, DateTimeKind.Local).AddTicks(3957));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 5,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 23, 18, 41, 223, DateTimeKind.Local).AddTicks(3958));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 6,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 23, 18, 41, 223, DateTimeKind.Local).AddTicks(3960));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 7,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 23, 18, 41, 223, DateTimeKind.Local).AddTicks(3961));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 8,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 23, 18, 41, 223, DateTimeKind.Local).AddTicks(3962));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 9,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 23, 18, 41, 223, DateTimeKind.Local).AddTicks(3963));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 10,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 23, 18, 41, 223, DateTimeKind.Local).AddTicks(3967));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 11,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 23, 18, 41, 223, DateTimeKind.Local).AddTicks(3968));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 12,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 23, 18, 41, 223, DateTimeKind.Local).AddTicks(3969));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 13,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 23, 18, 41, 223, DateTimeKind.Local).AddTicks(3970));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 14,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 23, 18, 41, 223, DateTimeKind.Local).AddTicks(3971));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 15,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 23, 18, 41, 223, DateTimeKind.Local).AddTicks(3972));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 16,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 23, 18, 41, 223, DateTimeKind.Local).AddTicks(3974));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 17,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 23, 18, 41, 223, DateTimeKind.Local).AddTicks(3975));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 18,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 23, 18, 41, 223, DateTimeKind.Local).AddTicks(3976));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 19,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 23, 18, 41, 223, DateTimeKind.Local).AddTicks(3977));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 20,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 23, 18, 41, 223, DateTimeKind.Local).AddTicks(3978));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 21,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 23, 18, 41, 223, DateTimeKind.Local).AddTicks(3979));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 22,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 23, 18, 41, 223, DateTimeKind.Local).AddTicks(3980));

            migrationBuilder.UpdateData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 1,
                column: "MeasurementUnit",
                value: 1.0);

            migrationBuilder.UpdateData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 2,
                column: "MeasurementUnit",
                value: 2.0);

            migrationBuilder.UpdateData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 3,
                column: "MeasurementUnit",
                value: 3.0);

            migrationBuilder.UpdateData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 4,
                column: "MeasurementUnit",
                value: 2.0);

            migrationBuilder.UpdateData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 5,
                column: "MeasurementUnit",
                value: 4.0);

            migrationBuilder.UpdateData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 6,
                column: "MeasurementUnit",
                value: 5.0);

            migrationBuilder.UpdateData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 7,
                column: "MeasurementUnit",
                value: 5.0);

            migrationBuilder.UpdateData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 8,
                column: "MeasurementUnit",
                value: 6.0);

            migrationBuilder.UpdateData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 9,
                column: "MeasurementUnit",
                value: 7.0);

            migrationBuilder.UpdateData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 10,
                column: "MeasurementUnit",
                value: 8.0);

            migrationBuilder.UpdateData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 11,
                column: "MeasurementUnit",
                value: 9.0);

            migrationBuilder.UpdateData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 12,
                column: "MeasurementUnit",
                value: 2.0);

            migrationBuilder.UpdateData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 13,
                column: "MeasurementUnit",
                value: 9.0);

            migrationBuilder.UpdateData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 14,
                column: "MeasurementUnit",
                value: 10.0);

            migrationBuilder.UpdateData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 15,
                column: "MeasurementUnit",
                value: 9.0);

            migrationBuilder.UpdateData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 16,
                column: "MeasurementUnit",
                value: 7.0);

            migrationBuilder.UpdateData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 17,
                column: "MeasurementUnit",
                value: 3.0);

            migrationBuilder.UpdateData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 18,
                column: "MeasurementUnit",
                value: 1.0);

            migrationBuilder.UpdateData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 19,
                column: "MeasurementUnit",
                value: 11.0);

            migrationBuilder.UpdateData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 20,
                column: "MeasurementUnit",
                value: 3.0);

            migrationBuilder.UpdateData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 21,
                column: "MeasurementUnit",
                value: 3.0);

            migrationBuilder.UpdateData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 22,
                column: "MeasurementUnit",
                value: 3.0);

            migrationBuilder.CreateIndex(
                name: "IX_TestTypes_ConsumableId",
                table: "TestTypes",
                column: "ConsumableId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_DoctorId",
                table: "Notifications",
                column: "DoctorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Results_AspNetUsers_TechnicianId",
                table: "Results",
                column: "TechnicianId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Results_TestRequestTests_TestRequestTestTypeModelTestRequestTestTypeId",
                table: "Results",
                column: "TestRequestTestTypeModelTestRequestTestTypeId",
                principalTable: "TestRequestTests",
                principalColumn: "TestRequestTestTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Samples_TestRequests_TestRequestId",
                table: "Samples",
                column: "TestRequestId",
                principalTable: "TestRequests",
                principalColumn: "TestRequestId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TestRequests_AspNetUsers_DoctorId",
                table: "TestRequests",
                column: "DoctorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TestRequests_TestTypes_TestTypeModelTestTypeId",
                table: "TestRequests",
                column: "TestTypeModelTestTypeId",
                principalTable: "TestTypes",
                principalColumn: "TestTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_TestTypes_Consumables_ConsumableId",
                table: "TestTypes",
                column: "ConsumableId",
                principalTable: "Consumables",
                principalColumn: "ConsumableId",
                onDelete: ReferentialAction.Cascade);

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
