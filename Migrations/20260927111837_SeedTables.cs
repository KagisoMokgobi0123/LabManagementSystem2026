using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace NMBHLabSys.Migrations
{
    /// <inheritdoc />
    public partial class SeedTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Consumables",
                columns: table => new
                {
                    ConsumableId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    QuantityOnHand = table.Column<int>(type: "int", nullable: false),
                    ReorderLevel = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Consumables", x => x.ConsumableId);
                });

            migrationBuilder.CreateTable(
                name: "MeasurementUnits",
                columns: table => new
                {
                    MeasurementUnitId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Unit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeasurementUnits", x => x.MeasurementUnitId);
                });

            migrationBuilder.CreateTable(
                name: "SampleTypes",
                columns: table => new
                {
                    SampleTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SampleTypes", x => x.SampleTypeId);
                });

            migrationBuilder.CreateTable(
                name: "TestCategories",
                columns: table => new
                {
                    TestCategoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestCategories", x => x.TestCategoryId);
                });

            migrationBuilder.CreateTable(
                name: "TestRequests",
                columns: table => new
                {
                    TestRequestId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PatientId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DoctorId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RequestDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Urgency = table.Column<int>(type: "int", nullable: false),
                    ClinicalNotes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestRequests", x => x.TestRequestId);
                    table.ForeignKey(
                        name: "FK_TestRequests_AspNetUsers_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction
                        );
                    table.ForeignKey(
                        name: "FK_TestRequests_AspNetUsers_PatientId",
                        column: x => x.PatientId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "Samples",
                columns: table => new
                {
                    SampleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TestRequestId = table.Column<int>(type: "int", nullable: false),
                    Barcode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SampleTypeId = table.Column<int>(type: "int", nullable: false),
                    TechnicianId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    DateReceived = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsReceived = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Samples", x => x.SampleId);
                    table.ForeignKey(
                        name: "FK_Samples_AspNetUsers_TechnicianId",
                        column: x => x.TechnicianId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Samples_SampleTypes_SampleTypeId",
                        column: x => x.SampleTypeId,
                        principalTable: "SampleTypes",
                        principalColumn: "SampleTypeId",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Samples_TestRequests_TestRequestId",
                        column: x => x.TestRequestId,
                        principalTable: "TestRequests",
                        principalColumn: "TestRequestId",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "TestTypes",
                columns: table => new
                {
                    TestTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    TestCategoryId = table.Column<int>(type: "int", nullable: false),
                    SampleTypeId = table.Column<int>(type: "int", nullable: false),
                    MeasurementUnit = table.Column<int>(type: "int", nullable: false),
                    NormalMin = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    NormalMax = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    TurnAroundTimeHours = table.Column<int>(type: "int", nullable: false),
                    ConsumableId = table.Column<int>(type: "int", nullable: false),
                    TestRequestsModelTestRequestId = table.Column<int>(type: "int", nullable: true),
                    TestTypeModelTestTypeId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestTypes", x => x.TestTypeId);
                    table.ForeignKey(
                        name: "FK_TestTypes_Consumables_ConsumableId",
                        column: x => x.ConsumableId,
                        principalTable: "Consumables",
                        principalColumn: "ConsumableId",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_TestTypes_MeasurementUnits_MeasurementUnit",
                        column: x => x.MeasurementUnit,
                        principalTable: "MeasurementUnits",
                        principalColumn: "MeasurementUnitId",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_TestTypes_SampleTypes_SampleTypeId",
                        column: x => x.SampleTypeId,
                        principalTable: "SampleTypes",
                        principalColumn: "SampleTypeId",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_TestTypes_TestCategories_TestCategoryId",
                        column: x => x.TestCategoryId,
                        principalTable: "TestCategories",
                        principalColumn: "TestCategoryId",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_TestTypes_TestRequests_TestRequestsModelTestRequestId",
                        column: x => x.TestRequestsModelTestRequestId,
                        principalTable: "TestRequests",
                        principalColumn: "TestRequestId",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_TestTypes_TestTypes_TestTypeModelTestTypeId",
                        column: x => x.TestTypeModelTestTypeId,
                        principalTable: "TestTypes",
                        principalColumn: "TestTypeId",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "TechnicianTestTypes",
                columns: table => new
                {
                    TechnicianTestTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TechnicianId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TestTypeId = table.Column<int>(type: "int", nullable: false),
                    ManagerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AssignedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TechnicianTestTypes", x => x.TechnicianTestTypeId);
                    table.ForeignKey(
                        name: "FK_TechnicianTestTypes_AspNetUsers_ManagerId",
                        column: x => x.ManagerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_TechnicianTestTypes_AspNetUsers_TechnicianId",
                        column: x => x.TechnicianId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_TechnicianTestTypes_TestTypes_TestTypeId",
                        column: x => x.TestTypeId,
                        principalTable: "TestTypes",
                        principalColumn: "TestTypeId",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "TestTypeConsumables",
                columns: table => new
                {
                    TestTypeConsumableId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TestTypeId = table.Column<int>(type: "int", nullable: false),
                    ConsumableId = table.Column<int>(type: "int", nullable: false),
                    QuantityRequired = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestTypeConsumables", x => x.TestTypeConsumableId);
                    table.ForeignKey(
                        name: "FK_TestTypeConsumables_Consumables_ConsumableId",
                        column: x => x.ConsumableId,
                        principalTable: "Consumables",
                        principalColumn: "ConsumableId",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_TestTypeConsumables_TestTypes_TestTypeId",
                        column: x => x.TestTypeId,
                        principalTable: "TestTypes",
                        principalColumn: "TestTypeId",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.InsertData(
                table: "Consumables",
                columns: new[] { "ConsumableId", "Name", "QuantityOnHand", "ReorderLevel" },
                values: new object[,]
                {
                    { 1, "Test Tubes (EDTA)", 500, 100 },
                    { 2, "Test Tubes (Sodium Citrate)", 300, 75 },
                    { 3, "Syringes 5ml", 300, 50 },
                    { 4, "Blood Smear Slides", 1000, 200 },
                    { 5, "ESR Racks and Tubes", 50, 15 },
                    { 6, "Coagulation Test Cartridges", 200, 50 },
                    { 7, "D-Dimer Test Kits", 100, 30 },
                    { 8, "Chemistry Analyzer Reagents", 50, 15 },
                    { 9, "Urine Dipsticks", 500, 100 },
                    { 10, "Microscope Slides (Urine)", 500, 100 },
                    { 11, "Culture Media Plates", 300, 80 },
                    { 12, "Blood Culture Bottles", 150, 40 }
                });

            migrationBuilder.InsertData(
                table: "MeasurementUnits",
                columns: new[] { "MeasurementUnitId", "Description", "Unit" },
                values: new object[,]
                {
                    { 1, "Thousand cells per microliter", "x10³/μL" },
                    { 2, "Percentage", "%" },
                    { 3, "Positive/Negative", "Qualitative" },
                    { 4, "Millimeters per hour", "mm/hr" },
                    { 5, "Seconds", "seconds" },
                    { 6, "Ratio", "ratio" },
                    { 7, "Nanograms per milliliter", "ng/mL" },
                    { 8, "Milligrams per deciliter", "mg/dL" },
                    { 9, "Millimoles per liter", "mmol/L" },
                    { 10, "Units per liter", "U/L" },
                    { 11, "Colony forming units per milliliter", "CFU/mL" },
                    { 12, "Grams per deciliter", "g/dL" },
                    { 13, "Femtoliters", "fL" },
                    { 14, "Picograms", "pg" },
                    { 15, "Million cells per microliter", "x10⁶/μL" },
                    { 16, "Micromoles per liter", "μmol/L" }
                });

            migrationBuilder.InsertData(
                table: "SampleTypes",
                columns: new[] { "SampleTypeId", "IsActive", "Name" },
                values: new object[,]
                {
                    { 1, true, "Blood" },
                    { 2, true, "Urine" },
                    { 3, true, "Saliva" },
                    { 4, true, "Plasma" },
                    { 5, true, "Serum" },
                    { 6, true, "Bone Marrow" },
                    { 7, true, "CSF" },
                    { 8, true, "Stool" },
                    { 9, true, "Sputum" },
                    { 10, true, "Swab" }
                });

            migrationBuilder.InsertData(
                table: "TestCategories",
                columns: new[] { "TestCategoryId", "Description" },
                values: new object[,]
                {
                    { 1, "Hematology" },
                    { 2, "Coagulation" },
                    { 3, "Clinical Chemistry" },
                    { 4, "Urinalysis" },
                    { 5, "Microbiology" }
                });

            migrationBuilder.InsertData(
                table: "TestRequests",
                columns: new[] { "TestRequestId", "ClinicalNotes", "DoctorId", "PatientId", "RequestDate", "RequestNumber", "Status", "Urgency" },
                values: new object[,]
                {
                    { 1, "Routine blood test - annual checkup.", "953f333d-2e68-46f6-ad26-0412fe75d908", "c0fbfbb4-daf5-44e3-bf57-acfded7baceb", new DateTime(2026, 9, 20, 9, 0, 0, 0, DateTimeKind.Unspecified), "NMB-REQ-000001", "Submitted", 1 },
                    { 2, "Patient presenting with fever and chills. Suspected infection.", "953f333d-2e68-46f6-ad26-0412fe75d908", "c0fbfbb4-daf5-44e3-bf57-acfded7baceb", new DateTime(2026, 9, 21, 10, 30, 0, 0, DateTimeKind.Unspecified), "NMB-REQ-000002", "SamplesReceived", 2 },
                    { 3, "Emergency request - patient collapsed. Suspected DVT.", "953f333d-2e68-46f6-ad26-0412fe75d908", "c0fbfbb4-daf5-44e3-bf57-acfded7baceb", new DateTime(2026, 9, 22, 8, 15, 0, 0, DateTimeKind.Unspecified), "NMB-REQ-000003", "InProgress", 3 },
                    { 4, "Diabetes monitoring - HbA1c and glucose levels.", "953f333d-2e68-46f6-ad26-0412fe75d908", "c0fbfbb4-daf5-44e3-bf57-acfded7baceb", new DateTime(2026, 9, 22, 14, 0, 0, 0, DateTimeKind.Unspecified), "NMB-REQ-000004", "Completed", 1 },
                    { 5, "Chest pain - urgent laboratory investigation requested.", "953f333d-2e68-46f6-ad26-0412fe75d908", "c0fbfbb4-daf5-44e3-bf57-acfded7baceb", new DateTime(2026, 9, 23, 11, 20, 0, 0, DateTimeKind.Unspecified), "NMB-REQ-000005", "Submitted", 2 },
                    { 6, "Routine liver function test for patient on medication.", "953f333d-2e68-46f6-ad26-0412fe75d908", "c0fbfbb4-daf5-44e3-bf57-acfded7baceb", new DateTime(2026, 9, 24, 9, 45, 0, 0, DateTimeKind.Unspecified), "NMB-REQ-000006", "SamplesReceived", 1 },
                    { 7, "Pre-surgery coagulation screen - STAT request.", "953f333d-2e68-46f6-ad26-0412fe75d908", "c0fbfbb4-daf5-44e3-bf57-acfded7baceb", new DateTime(2026, 9, 25, 7, 30, 0, 0, DateTimeKind.Unspecified), "NMB-REQ-000007", "Submitted", 3 },
                    { 8, "Routine urinalysis - suspected UTI.", "953f333d-2e68-46f6-ad26-0412fe75d908", "c0fbfbb4-daf5-44e3-bf57-acfded7baceb", new DateTime(2026, 9, 25, 13, 0, 0, 0, DateTimeKind.Unspecified), "NMB-REQ-000008", "Completed", 1 },
                    { 9, "Suspected bloodstream infection - blood culture requested.", "953f333d-2e68-46f6-ad26-0412fe75d908", "c0fbfbb4-daf5-44e3-bf57-acfded7baceb", new DateTime(2026, 9, 26, 8, 45, 0, 0, DateTimeKind.Unspecified), "NMB-REQ-000009", "InProgress", 2 },
                    { 10, "Annual physical examination - full blood count and chemistry panel.", "953f333d-2e68-46f6-ad26-0412fe75d908", "c0fbfbb4-daf5-44e3-bf57-acfded7baceb", new DateTime(2026, 9, 26, 14, 30, 0, 0, DateTimeKind.Unspecified), "NMB-REQ-000010", "Submitted", 1 }
                });

            migrationBuilder.InsertData(
                table: "Samples",
                columns: new[] { "SampleId", "Barcode", "DateReceived", "IsDeleted", "IsReceived", "SampleTypeId", "Status", "TechnicianId", "TestRequestId" },
                values: new object[,]
                {
                    { 1, "SMP-2026-000001", null, false, false, 1, "Submitted", null, 1 },
                    { 2, "SMP-2026-000002", new DateTime(2026, 9, 21, 11, 0, 0, 0, DateTimeKind.Unspecified), false, true, 1, "Samples Received", "85a94ac3-e56a-4ae2-bea1-85d398486185", 2 },
                    { 3, "SMP-2026-000003", new DateTime(2026, 9, 22, 8, 30, 0, 0, DateTimeKind.Unspecified), false, true, 4, "Samples Received", "85a94ac3-e56a-4ae2-bea1-85d398486185", 3 },
                    { 4, "SMP-2026-000005", null, false, false, 4, "Submitted", null, 5 },
                    { 5, "SMP-2026-000006", new DateTime(2026, 9, 24, 10, 15, 0, 0, DateTimeKind.Unspecified), false, true, 4, "Samples Received", "85a94ac3-e56a-4ae2-bea1-85d398486185", 6 },
                    { 6, "SMP-2026-000007", null, false, false, 4, "Submitted", null, 7 },
                    { 7, "SMP-2026-000010", null, false, false, 1, "Submitted", null, 10 }
                });

            migrationBuilder.InsertData(
                table: "TestTypes",
                columns: new[] { "TestTypeId", "ConsumableId", "MeasurementUnit", "Name", "NormalMax", "NormalMin", "SampleTypeId", "TestCategoryId", "TestRequestsModelTestRequestId", "TestTypeModelTestTypeId", "TurnAroundTimeHours" },
                values: new object[,]
                {
                    { 1, 1, 1, "Full Blood Count (FBC)", 11.0m, 4.5m, 1, 1, null, null, 2 },
                    { 2, 1, 2, "White Blood Cell Differential", 0m, 0m, 1, 1, null, null, 3 },
                    { 3, 4, 3, "Peripheral Blood Film", 0m, 0m, 1, 1, null, null, 4 },
                    { 6, 6, 5, "Prothrombin Time (PT)", 13.5m, 11.0m, 4, 2, null, null, 2 },
                    { 7, 6, 5, "Activated Partial Thromboplastin Time (aPTT)", 35m, 25m, 4, 2, null, null, 2 },
                    { 8, 6, 6, "International Normalized Ratio (INR)", 1.2m, 0.8m, 4, 2, null, null, 2 },
                    { 11, 8, 9, "Glucose (Fasting)", 5.5m, 3.9m, 4, 3, null, null, 1 },
                    { 12, 1, 2, "Hemoglobin A1C (HbA1c)", 5.6m, 4.0m, 1, 3, null, null, 2 },
                    { 17, 9, 3, "Urinalysis (Dipstick)", 0m, 0m, 2, 4, null, null, 1 },
                    { 20, 12, 3, "Blood Culture", 0m, 0m, 1, 5, null, null, 72 }
                });

            migrationBuilder.InsertData(
                table: "TestTypeConsumables",
                columns: new[] { "TestTypeConsumableId", "ConsumableId", "QuantityRequired", "TestTypeId" },
                values: new object[,]
                {
                    { 1, 1, 1, 1 },
                    { 2, 1, 1, 2 },
                    { 3, 4, 2, 3 },
                    { 4, 6, 1, 6 },
                    { 5, 6, 1, 7 },
                    { 6, 6, 1, 8 },
                    { 7, 8, 1, 11 },
                    { 8, 1, 1, 12 },
                    { 9, 9, 1, 17 },
                    { 10, 12, 2, 20 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Samples_SampleTypeId",
                table: "Samples",
                column: "SampleTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Samples_TechnicianId",
                table: "Samples",
                column: "TechnicianId");

            migrationBuilder.CreateIndex(
                name: "IX_Samples_TestRequestId",
                table: "Samples",
                column: "TestRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_TechnicianTestTypes_ManagerId",
                table: "TechnicianTestTypes",
                column: "ManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_TechnicianTestTypes_TechnicianId",
                table: "TechnicianTestTypes",
                column: "TechnicianId");

            migrationBuilder.CreateIndex(
                name: "IX_TechnicianTestTypes_TestTypeId",
                table: "TechnicianTestTypes",
                column: "TestTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_TestRequests_DoctorId",
                table: "TestRequests",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_TestRequests_PatientId",
                table: "TestRequests",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_TestTypeConsumables_ConsumableId",
                table: "TestTypeConsumables",
                column: "ConsumableId");

            migrationBuilder.CreateIndex(
                name: "IX_TestTypeConsumables_TestTypeId",
                table: "TestTypeConsumables",
                column: "TestTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_TestTypes_ConsumableId",
                table: "TestTypes",
                column: "ConsumableId");

            migrationBuilder.CreateIndex(
                name: "IX_TestTypes_MeasurementUnit",
                table: "TestTypes",
                column: "MeasurementUnit");

            migrationBuilder.CreateIndex(
                name: "IX_TestTypes_SampleTypeId",
                table: "TestTypes",
                column: "SampleTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_TestTypes_TestCategoryId",
                table: "TestTypes",
                column: "TestCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_TestTypes_TestRequestsModelTestRequestId",
                table: "TestTypes",
                column: "TestRequestsModelTestRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_TestTypes_TestTypeModelTestTypeId",
                table: "TestTypes",
                column: "TestTypeModelTestTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Samples");

            migrationBuilder.DropTable(
                name: "TechnicianTestTypes");

            migrationBuilder.DropTable(
                name: "TestTypeConsumables");

            migrationBuilder.DropTable(
                name: "TestTypes");

            migrationBuilder.DropTable(
                name: "Consumables");

            migrationBuilder.DropTable(
                name: "MeasurementUnits");

            migrationBuilder.DropTable(
                name: "SampleTypes");

            migrationBuilder.DropTable(
                name: "TestCategories");

            migrationBuilder.DropTable(
                name: "TestRequests");
        }
    }
}
