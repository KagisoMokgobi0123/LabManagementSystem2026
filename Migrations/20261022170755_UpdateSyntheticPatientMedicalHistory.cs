using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NMBHLabSys.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSyntheticPatientMedicalHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AllergyModel",
                keyColumn: "AllergyId",
                keyValue: 1,
                column: "Description",
                value: "Reported beta-lactam antibiotic allergy");

            migrationBuilder.UpdateData(
                table: "AllergyModel",
                keyColumn: "AllergyId",
                keyValue: 2,
                column: "Description",
                value: "Reported food allergy to peanuts");

            migrationBuilder.UpdateData(
                table: "MedicalConditionCategories",
                keyColumn: "MedicalConditionCategoryId",
                keyValue: 9001,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Long-term conditions relevant to laboratory investigations", "Chronic medical conditions" });

            migrationBuilder.UpdateData(
                table: "MedicalConditionModel",
                keyColumn: "MedicalConditionId",
                keyValue: 1,
                column: "Description",
                value: "Chronic elevated blood pressure");

            migrationBuilder.UpdateData(
                table: "MedicalConditionModel",
                keyColumn: "MedicalConditionId",
                keyValue: 2,
                columns: new[] { "ConditionName", "Description" },
                values: new object[] { "Type 2 diabetes mellitus", "Chronic disorder of glucose regulation" });

            migrationBuilder.UpdateData(
                table: "MedicationModel",
                keyColumn: "MedicationId",
                keyValue: 1,
                column: "Description",
                value: "Oral antihyperglycaemic medicine");

            migrationBuilder.UpdateData(
                table: "MedicationModel",
                keyColumn: "MedicationId",
                keyValue: 2,
                column: "Description",
                value: "Calcium channel blocker used for blood pressure management");

            migrationBuilder.UpdateData(
                table: "PatientAllergies",
                keyColumn: "PatientAllergyId",
                keyValue: 6001,
                column: "Notes",
                value: "Reported rash after previous penicillin exposure; confirmation pending.");

            migrationBuilder.UpdateData(
                table: "PatientAllergies",
                keyColumn: "PatientAllergyId",
                keyValue: 6002,
                column: "Notes",
                value: "Reported itching after peanut exposure; confirmation pending.");

            migrationBuilder.UpdateData(
                table: "PatientConditions",
                keyColumn: "PatientConditionId",
                keyValue: 5001,
                column: "Notes",
                value: "Recorded history: blood pressure monitoring recommended.");

            migrationBuilder.UpdateData(
                table: "PatientConditions",
                keyColumn: "PatientConditionId",
                keyValue: 5002,
                column: "Notes",
                value: "Recorded history: periodic HbA1c and fasting glucose monitoring.");

            migrationBuilder.UpdateData(
                table: "PatientMedications",
                keyColumn: "PatientMedicationId",
                keyValue: 7001,
                column: "Notes",
                value: "Medication history entered for training; verify at every encounter.");

            migrationBuilder.UpdateData(
                table: "PatientMedications",
                keyColumn: "PatientMedicationId",
                keyValue: 7002,
                column: "Notes",
                value: "Medication history entered for training; verify at every encounter.");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AllergyModel",
                keyColumn: "AllergyId",
                keyValue: 1,
                column: "Description",
                value: "DEMO DATA ONLY");

            migrationBuilder.UpdateData(
                table: "AllergyModel",
                keyColumn: "AllergyId",
                keyValue: 2,
                column: "Description",
                value: "DEMO DATA ONLY");

            migrationBuilder.UpdateData(
                table: "MedicalConditionCategories",
                keyColumn: "MedicalConditionCategoryId",
                keyValue: 9001,
                columns: new[] { "Description", "Name" },
                values: new object[] { "DEMO DATA ONLY - laboratory assessment", "Demo chronic conditions" });

            migrationBuilder.UpdateData(
                table: "MedicalConditionModel",
                keyColumn: "MedicalConditionId",
                keyValue: 1,
                column: "Description",
                value: "DEMO DATA ONLY");

            migrationBuilder.UpdateData(
                table: "MedicalConditionModel",
                keyColumn: "MedicalConditionId",
                keyValue: 2,
                columns: new[] { "ConditionName", "Description" },
                values: new object[] { "Type 2 diabetes", "DEMO DATA ONLY" });

            migrationBuilder.UpdateData(
                table: "MedicationModel",
                keyColumn: "MedicationId",
                keyValue: 1,
                column: "Description",
                value: "DEMO DATA ONLY");

            migrationBuilder.UpdateData(
                table: "MedicationModel",
                keyColumn: "MedicationId",
                keyValue: 2,
                column: "Description",
                value: "DEMO DATA ONLY");

            migrationBuilder.UpdateData(
                table: "PatientAllergies",
                keyColumn: "PatientAllergyId",
                keyValue: 6001,
                column: "Notes",
                value: "DEMO DATA ONLY - fictional allergy history.");

            migrationBuilder.UpdateData(
                table: "PatientAllergies",
                keyColumn: "PatientAllergyId",
                keyValue: 6002,
                column: "Notes",
                value: "DEMO DATA ONLY - fictional allergy history.");

            migrationBuilder.UpdateData(
                table: "PatientConditions",
                keyColumn: "PatientConditionId",
                keyValue: 5001,
                column: "Notes",
                value: "DEMO DATA ONLY - example medical condition.");

            migrationBuilder.UpdateData(
                table: "PatientConditions",
                keyColumn: "PatientConditionId",
                keyValue: 5002,
                column: "Notes",
                value: "DEMO DATA ONLY - example medical condition.");

            migrationBuilder.UpdateData(
                table: "PatientMedications",
                keyColumn: "PatientMedicationId",
                keyValue: 7001,
                column: "Notes",
                value: "DEMO DATA ONLY - fictional medication history.");

            migrationBuilder.UpdateData(
                table: "PatientMedications",
                keyColumn: "PatientMedicationId",
                keyValue: 7002,
                column: "Notes",
                value: "DEMO DATA ONLY - fictional medication history.");
        }
    }
}
