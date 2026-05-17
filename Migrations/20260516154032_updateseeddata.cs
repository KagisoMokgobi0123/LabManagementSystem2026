using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace NMBHLabSys.Migrations
{
    /// <inheritdoc />
    public partial class updateseeddata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MeasurementUnitModelMeasurementUnitId",
                table: "TestTypes",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MeasurementUnits",
                columns: table => new
                {
                    MeasurementUnitId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Unit = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeasurementUnits", x => x.MeasurementUnitId);
                });

            migrationBuilder.UpdateData(
                table: "Consumables",
                keyColumn: "ConsumableId",
                keyValue: 2,
                columns: new[] { "Name", "ReorderLevel" },
                values: new object[] { "Test Tubes (Sodium Citrate)", 75 });

            migrationBuilder.UpdateData(
                table: "Consumables",
                keyColumn: "ConsumableId",
                keyValue: 3,
                columns: new[] { "Name", "QuantityOnHand", "ReorderLevel" },
                values: new object[] { "Syringes 5ml", 300, 50 });

            migrationBuilder.UpdateData(
                table: "Consumables",
                keyColumn: "ConsumableId",
                keyValue: 4,
                columns: new[] { "Name", "QuantityOnHand", "ReorderLevel" },
                values: new object[] { "Blood Smear Slides", 1000, 200 });

            migrationBuilder.InsertData(
                table: "Consumables",
                columns: new[] { "ConsumableId", "Name", "QuantityOnHand", "ReorderLevel" },
                values: new object[,]
                {
                    { 5, "ESR Racks and Tubes", 50, 15 },
                    { 6, "Coagulation Test Cartridges", 200, 50 },
                    { 7, "D-Dimer Test Kits", 100, 30 },
                    { 8, "Chemistry Analyzer Reagents", 50, 15 },
                    { 9, "Urine Dipsticks", 500, 100 },
                    { 10, "Microscope Slides (Urine)", 500, 100 },
                    { 11, "Culture Media Plates", 300, 80 },
                    { 12, "Blood Culture Bottles", 150, 40 },
                    { 13, "Alcohol Swabs", 1000, 200 },
                    { 14, "Gloves (Medium)", 800, 150 },
                    { 15, "Gloves (Large)", 600, 120 },
                    { 16, "Face Masks", 500, 100 },
                    { 17, "Lab Coats (Disposable)", 100, 25 },
                    { 18, "Centrifuge Tubes", 1000, 200 },
                    { 19, "Pipette Tips", 2000, 400 },
                    { 20, "Quality Control Materials", 50, 15 }
                });

            migrationBuilder.InsertData(
                table: "MeasurementUnits",
                columns: new[] { "MeasurementUnitId", "Description", "IsActive", "Unit" },
                values: new object[,]
                {
                    { 1, "Thousand cells per microliter", true, "x10³/μL" },
                    { 2, "Percentage", true, "%" },
                    { 3, "Positive/Negative", true, "Qualitative" },
                    { 4, "Millimeters per hour", true, "mm/hr" },
                    { 5, "Seconds", true, "seconds" },
                    { 6, "Ratio", true, "ratio" },
                    { 7, "Nanograms per milliliter", true, "ng/mL" },
                    { 8, "Milligrams per deciliter", true, "mg/dL" },
                    { 9, "Millimoles per liter", true, "mmol/L" },
                    { 10, "Units per liter", true, "U/L" },
                    { 11, "Colony forming units per milliliter", true, "CFU/mL" },
                    { 12, "Grams per deciliter", true, "g/dL" },
                    { 13, "Femtoliters", true, "fL" },
                    { 14, "Picograms", true, "pg" },
                    { 15, "Million cells per microliter", true, "x10⁶/μL" },
                    { 16, "Micromoles per liter", true, "μmol/L" }
                });

            migrationBuilder.InsertData(
                table: "SampleTypes",
                columns: new[] { "SampleTypeId", "Name" },
                values: new object[,]
                {
                    { 6, "Bone Marrow" },
                    { 7, "CSF" },
                    { 8, "Stool" },
                    { 9, "Sputum" },
                    { 10, "Swab" }
                });

            migrationBuilder.InsertData(
                table: "TestRequestTestTypeModel",
                columns: new[] { "TestRequestTestTypeId", "CompletionDateTime", "DueDate", "Notes", "ResultValue", "StartDateTime", "Status", "TechnicianId", "TestRequestId", "TestTypeId", "VerificationDateTime", "VerificationNote", "VerifiedByTechnicianId", "VerifyTechnicianId" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 5, 16, 17, 40, 32, 265, DateTimeKind.Local).AddTicks(2835), new DateTime(2026, 5, 16, 19, 40, 32, 265, DateTimeKind.Local).AddTicks(2836), null, null, new DateTime(2026, 5, 16, 17, 40, 32, 265, DateTimeKind.Local).AddTicks(2827), "Submitted", null, 1, 1, null, null, null, null },
                    { 3, new DateTime(2026, 5, 16, 17, 40, 32, 265, DateTimeKind.Local).AddTicks(2851), new DateTime(2026, 5, 16, 19, 40, 32, 265, DateTimeKind.Local).AddTicks(2852), null, null, new DateTime(2026, 5, 16, 15, 40, 32, 265, DateTimeKind.Local).AddTicks(2850), "InProgress", "2ec716df-770e-465e-a7b7-8d3926b560a2", 2, 1, null, null, null, null },
                    { 5, new DateTime(2026, 5, 16, 16, 40, 32, 265, DateTimeKind.Local).AddTicks(2859), new DateTime(2026, 5, 16, 16, 40, 32, 265, DateTimeKind.Local).AddTicks(2861), "Results show elevated white blood cells", "WBC: 11.2, RBC: 4.8, HGB: 14.2, PLT: 250", new DateTime(2026, 5, 16, 14, 40, 32, 265, DateTimeKind.Local).AddTicks(2858), "Completed", "2ec716df-770e-465e-a7b7-8d3926b560a2", 3, 1, null, null, null, null }
                });

            migrationBuilder.InsertData(
                table: "TestTypeConsumables",
                columns: new[] { "TestTypeConsumableId", "ConsumableId", "QuantityRequired", "TestTypeId", "VerificationDate" },
                values: new object[,]
                {
                    { 1, 1, 1, 1, new DateTime(2026, 5, 16, 17, 40, 32, 265, DateTimeKind.Local).AddTicks(3167) },
                    { 2, 1, 1, 2, new DateTime(2026, 5, 16, 17, 40, 32, 265, DateTimeKind.Local).AddTicks(3170) },
                    { 3, 4, 2, 3, new DateTime(2026, 5, 16, 17, 40, 32, 265, DateTimeKind.Local).AddTicks(3171) }
                });

            migrationBuilder.UpdateData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 1,
                columns: new[] { "MeasurementUnitModelMeasurementUnitId", "Name" },
                values: new object[] { null, "Full Blood Count (FBC)" });

            migrationBuilder.UpdateData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 2,
                columns: new[] { "ConsumableId", "MeasurementUnit", "MeasurementUnitModelMeasurementUnitId", "Name", "NormalMax", "SampleTypeId", "TestCategoryId", "TurnAroundTimeHours" },
                values: new object[] { 1, 2.0, null, "White Blood Cell Differential", 0m, 1, 1, 3 });

            migrationBuilder.UpdateData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 3,
                columns: new[] { "ConsumableId", "MeasurementUnit", "MeasurementUnitModelMeasurementUnitId", "Name", "NormalMax", "NormalMin", "TestCategoryId", "TurnAroundTimeHours" },
                values: new object[] { 4, 3.0, null, "Peripheral Blood Film", 0m, 0m, 1, 4 });

            migrationBuilder.InsertData(
                table: "TestTypes",
                columns: new[] { "TestTypeId", "ConsumableId", "MeasurementUnit", "MeasurementUnitModelMeasurementUnitId", "Name", "NormalMax", "NormalMin", "SampleTypeId", "TestCategoryId", "TurnAroundTimeHours" },
                values: new object[,]
                {
                    { 4, 1, 2.0, null, "Reticulocyte Count", 2.5m, 0.5m, 1, 1, 2 },
                    { 12, 1, 2.0, null, "Hemoglobin A1C (HbA1c)", 5.6m, 4.0m, 1, 3, 2 }
                });

            migrationBuilder.InsertData(
                table: "TestTypeConsumables",
                columns: new[] { "TestTypeConsumableId", "ConsumableId", "QuantityRequired", "TestTypeId", "VerificationDate" },
                values: new object[,]
                {
                    { 4, 1, 1, 4, new DateTime(2026, 5, 16, 17, 40, 32, 265, DateTimeKind.Local).AddTicks(3172) },
                    { 12, 8, 1, 12, new DateTime(2026, 5, 16, 17, 40, 32, 265, DateTimeKind.Local).AddTicks(3181) }
                });

            migrationBuilder.InsertData(
                table: "TestTypes",
                columns: new[] { "TestTypeId", "ConsumableId", "MeasurementUnit", "MeasurementUnitModelMeasurementUnitId", "Name", "NormalMax", "NormalMin", "SampleTypeId", "TestCategoryId", "TurnAroundTimeHours" },
                values: new object[,]
                {
                    { 5, 5, 4.0, null, "Erythrocyte Sedimentation Rate (ESR)", 20m, 0m, 1, 1, 1 },
                    { 6, 6, 5.0, null, "Prothrombin Time (PT)", 13.5m, 11.0m, 4, 2, 2 },
                    { 7, 6, 5.0, null, "Activated Partial Thromboplastin Time (aPTT)", 35.0m, 25.0m, 4, 2, 2 },
                    { 8, 6, 6.0, null, "International Normalized Ratio (INR)", 1.2m, 0.8m, 4, 2, 2 },
                    { 9, 7, 7.0, null, "D-Dimer", 500m, 0m, 4, 2, 2 },
                    { 10, 6, 8.0, null, "Fibrinogen Level", 400m, 200m, 4, 2, 3 },
                    { 11, 8, 9.0, null, "Glucose (Fasting)", 5.5m, 3.9m, 4, 3, 1 },
                    { 13, 8, 9.0, null, "Lipid Profile", 0m, 0m, 4, 3, 4 },
                    { 14, 8, 10.0, null, "Liver Function Test (LFT)", 0m, 0m, 4, 3, 4 },
                    { 15, 8, 9.0, null, "Renal Function Test (RFT)", 0m, 0m, 4, 3, 3 },
                    { 16, 8, 7.0, null, "C-Reactive Protein (CRP)", 5m, 0m, 4, 3, 2 },
                    { 17, 9, 3.0, null, "Urinalysis (Dipstick)", 0m, 0m, 2, 4, 1 },
                    { 18, 10, 1.0, null, "Urine Microscopy", 0m, 0m, 2, 4, 2 },
                    { 19, 11, 11.0, null, "Urine Culture and Sensitivity", 10000m, 0m, 2, 4, 48 },
                    { 20, 12, 3.0, null, "Blood Culture", 0m, 0m, 1, 5, 72 },
                    { 21, 11, 3.0, null, "Sputum Culture", 0m, 0m, 9, 5, 48 },
                    { 22, 11, 3.0, null, "Wound Swab Culture", 0m, 0m, 10, 5, 48 }
                });

            migrationBuilder.InsertData(
                table: "TestRequestTestTypeModel",
                columns: new[] { "TestRequestTestTypeId", "CompletionDateTime", "DueDate", "Notes", "ResultValue", "StartDateTime", "Status", "TechnicianId", "TestRequestId", "TestTypeId", "VerificationDateTime", "VerificationNote", "VerifiedByTechnicianId", "VerifyTechnicianId" },
                values: new object[,]
                {
                    { 2, new DateTime(2026, 5, 16, 17, 40, 32, 265, DateTimeKind.Local).AddTicks(2846), new DateTime(2026, 5, 16, 18, 40, 32, 265, DateTimeKind.Local).AddTicks(2847), null, null, new DateTime(2026, 5, 16, 17, 40, 32, 265, DateTimeKind.Local).AddTicks(2846), "Submitted", null, 1, 11, null, null, null, null },
                    { 4, new DateTime(2026, 5, 16, 17, 40, 32, 265, DateTimeKind.Local).AddTicks(2855), new DateTime(2026, 5, 16, 19, 40, 32, 265, DateTimeKind.Local).AddTicks(2856), null, null, new DateTime(2026, 5, 16, 17, 40, 32, 265, DateTimeKind.Local).AddTicks(2854), "Submitted", null, 2, 6, null, null, null, null },
                    { 6, new DateTime(2026, 5, 16, 15, 40, 32, 265, DateTimeKind.Local).AddTicks(2865), new DateTime(2026, 5, 16, 15, 40, 32, 265, DateTimeKind.Local).AddTicks(2866), "Normal range", "5.6 mmol/L", new DateTime(2026, 5, 16, 14, 40, 32, 265, DateTimeKind.Local).AddTicks(2864), "Verified", "2ec716df-770e-465e-a7b7-8d3926b560a2", 3, 11, new DateTime(2026, 5, 16, 16, 10, 32, 265, DateTimeKind.Local).AddTicks(2869), "Verified - within normal range", "af381d22-8f2c-4401-ae8b-b31bb60e1c39", null },
                    { 7, new DateTime(2026, 5, 16, 17, 10, 32, 265, DateTimeKind.Local).AddTicks(2873), new DateTime(2026, 5, 16, 17, 40, 32, 265, DateTimeKind.Local).AddTicks(2874), "Abnormal high INR - needs review", "INR: 3.5", new DateTime(2026, 5, 16, 15, 40, 32, 265, DateTimeKind.Local).AddTicks(2872), "ToBeReviewed", "2ec716df-770e-465e-a7b7-8d3926b560a2", 3, 6, new DateTime(2026, 5, 16, 17, 10, 32, 265, DateTimeKind.Local).AddTicks(2876), "Result appears abnormal. Please review clinical notes.", "af381d22-8f2c-4401-ae8b-b31bb60e1c39", null }
                });

            migrationBuilder.InsertData(
                table: "TestTypeConsumables",
                columns: new[] { "TestTypeConsumableId", "ConsumableId", "QuantityRequired", "TestTypeId", "VerificationDate" },
                values: new object[,]
                {
                    { 5, 5, 1, 5, new DateTime(2026, 5, 16, 17, 40, 32, 265, DateTimeKind.Local).AddTicks(3173) },
                    { 6, 2, 1, 6, new DateTime(2026, 5, 16, 17, 40, 32, 265, DateTimeKind.Local).AddTicks(3175) },
                    { 7, 2, 1, 7, new DateTime(2026, 5, 16, 17, 40, 32, 265, DateTimeKind.Local).AddTicks(3176) },
                    { 8, 2, 1, 8, new DateTime(2026, 5, 16, 17, 40, 32, 265, DateTimeKind.Local).AddTicks(3177) },
                    { 9, 7, 1, 9, new DateTime(2026, 5, 16, 17, 40, 32, 265, DateTimeKind.Local).AddTicks(3178) },
                    { 10, 6, 1, 10, new DateTime(2026, 5, 16, 17, 40, 32, 265, DateTimeKind.Local).AddTicks(3179) },
                    { 11, 8, 1, 11, new DateTime(2026, 5, 16, 17, 40, 32, 265, DateTimeKind.Local).AddTicks(3180) },
                    { 13, 8, 2, 13, new DateTime(2026, 5, 16, 17, 40, 32, 265, DateTimeKind.Local).AddTicks(3182) },
                    { 14, 8, 2, 14, new DateTime(2026, 5, 16, 17, 40, 32, 265, DateTimeKind.Local).AddTicks(3183) },
                    { 15, 8, 1, 15, new DateTime(2026, 5, 16, 17, 40, 32, 265, DateTimeKind.Local).AddTicks(3185) },
                    { 16, 8, 1, 16, new DateTime(2026, 5, 16, 17, 40, 32, 265, DateTimeKind.Local).AddTicks(3186) },
                    { 17, 9, 1, 17, new DateTime(2026, 5, 16, 17, 40, 32, 265, DateTimeKind.Local).AddTicks(3188) },
                    { 18, 10, 1, 18, new DateTime(2026, 5, 16, 17, 40, 32, 265, DateTimeKind.Local).AddTicks(3189) },
                    { 19, 11, 1, 19, new DateTime(2026, 5, 16, 17, 40, 32, 265, DateTimeKind.Local).AddTicks(3190) },
                    { 20, 12, 2, 20, new DateTime(2026, 5, 16, 17, 40, 32, 265, DateTimeKind.Local).AddTicks(3191) },
                    { 21, 11, 1, 21, new DateTime(2026, 5, 16, 17, 40, 32, 265, DateTimeKind.Local).AddTicks(3192) },
                    { 22, 11, 1, 22, new DateTime(2026, 5, 16, 17, 40, 32, 265, DateTimeKind.Local).AddTicks(3193) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_TestTypes_MeasurementUnitModelMeasurementUnitId",
                table: "TestTypes",
                column: "MeasurementUnitModelMeasurementUnitId");

            migrationBuilder.AddForeignKey(
                name: "FK_TestTypes_MeasurementUnits_MeasurementUnitModelMeasurementUnitId",
                table: "TestTypes",
                column: "MeasurementUnitModelMeasurementUnitId",
                principalTable: "MeasurementUnits",
                principalColumn: "MeasurementUnitId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestTypes_MeasurementUnits_MeasurementUnitModelMeasurementUnitId",
                table: "TestTypes");

            migrationBuilder.DropTable(
                name: "MeasurementUnits");

            migrationBuilder.DropIndex(
                name: "IX_TestTypes_MeasurementUnitModelMeasurementUnitId",
                table: "TestTypes");

            migrationBuilder.DeleteData(
                table: "Consumables",
                keyColumn: "ConsumableId",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Consumables",
                keyColumn: "ConsumableId",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Consumables",
                keyColumn: "ConsumableId",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Consumables",
                keyColumn: "ConsumableId",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Consumables",
                keyColumn: "ConsumableId",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Consumables",
                keyColumn: "ConsumableId",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Consumables",
                keyColumn: "ConsumableId",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Consumables",
                keyColumn: "ConsumableId",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "SampleTypes",
                keyColumn: "SampleTypeId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "SampleTypes",
                keyColumn: "SampleTypeId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "SampleTypes",
                keyColumn: "SampleTypeId",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "TestRequestTestTypeModel",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "TestRequestTestTypeModel",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "TestRequestTestTypeModel",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "TestRequestTestTypeModel",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "TestRequestTestTypeModel",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "TestRequestTestTypeModel",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "TestRequestTestTypeModel",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "Consumables",
                keyColumn: "ConsumableId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Consumables",
                keyColumn: "ConsumableId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Consumables",
                keyColumn: "ConsumableId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Consumables",
                keyColumn: "ConsumableId",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Consumables",
                keyColumn: "ConsumableId",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Consumables",
                keyColumn: "ConsumableId",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Consumables",
                keyColumn: "ConsumableId",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Consumables",
                keyColumn: "ConsumableId",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "SampleTypes",
                keyColumn: "SampleTypeId",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "SampleTypes",
                keyColumn: "SampleTypeId",
                keyValue: 10);

            migrationBuilder.DropColumn(
                name: "MeasurementUnitModelMeasurementUnitId",
                table: "TestTypes");

            migrationBuilder.UpdateData(
                table: "Consumables",
                keyColumn: "ConsumableId",
                keyValue: 2,
                columns: new[] { "Name", "ReorderLevel" },
                values: new object[] { "Syringes 5ml", 50 });

            migrationBuilder.UpdateData(
                table: "Consumables",
                keyColumn: "ConsumableId",
                keyValue: 3,
                columns: new[] { "Name", "QuantityOnHand", "ReorderLevel" },
                values: new object[] { "Alcohol Swabs", 1000, 200 });

            migrationBuilder.UpdateData(
                table: "Consumables",
                keyColumn: "ConsumableId",
                keyValue: 4,
                columns: new[] { "Name", "QuantityOnHand", "ReorderLevel" },
                values: new object[] { "Gloves (Medium)", 800, 150 });

            migrationBuilder.UpdateData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 1,
                column: "Name",
                value: "Full Blood Count");

            migrationBuilder.UpdateData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 2,
                columns: new[] { "ConsumableId", "MeasurementUnit", "Name", "NormalMax", "SampleTypeId", "TestCategoryId", "TurnAroundTimeHours" },
                values: new object[] { 2, 1.0, "Urine Culture", 5m, 2, 2, 4 });

            migrationBuilder.UpdateData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 3,
                columns: new[] { "ConsumableId", "MeasurementUnit", "Name", "NormalMax", "NormalMin", "TestCategoryId", "TurnAroundTimeHours" },
                values: new object[] { 3, 1.0, "Glucose Test", 5.5m, 3.9m, 3, 1 });
        }
    }
}
