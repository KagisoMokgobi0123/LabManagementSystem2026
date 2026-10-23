using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NMB_HLabSys.Migrations
{
    /// <inheritdoc />
    public partial class SyncPendingModelChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestTypes_TestCategories_TestCategoryModelTestCategoryId",
                table: "TestTypes");

            migrationBuilder.DropIndex(
                name: "IX_TestTypes_TestCategoryModelTestCategoryId",
                table: "TestTypes");

            migrationBuilder.DropColumn(
                name: "TestCategoryModelTestCategoryId",
                table: "TestTypes");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "PatientMedications",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "PatientConditions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "PatientAllergies",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Medications",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Medications",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "MedicationCategories",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "MedicationCategories",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "MedicalConditions",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "MedicalConditions",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "MedicalConditionCategories",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "MedicalConditionCategories",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AllergyCategories",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "AllergyCategories",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Allergies",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Allergies",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "PatientAllergies",
                keyColumn: "PatientAllergyId",
                keyValue: 6001,
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "PatientAllergies",
                keyColumn: "PatientAllergyId",
                keyValue: 6002,
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "PatientConditions",
                keyColumn: "PatientConditionId",
                keyValue: 5001,
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "PatientConditions",
                keyColumn: "PatientConditionId",
                keyValue: 5002,
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "PatientMedications",
                keyColumn: "PatientMedicationId",
                keyValue: 7001,
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "PatientMedications",
                keyColumn: "PatientMedicationId",
                keyValue: 7002,
                column: "IsDeleted",
                value: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "PatientMedications");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "PatientConditions");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "PatientAllergies");

            migrationBuilder.AddColumn<int>(
                name: "TestCategoryModelTestCategoryId",
                table: "TestTypes",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Medications",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Medications",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "MedicationCategories",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "MedicationCategories",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "MedicalConditions",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "MedicalConditions",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "MedicalConditionCategories",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "MedicalConditionCategories",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AllergyCategories",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "AllergyCategories",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Allergies",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Allergies",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 1,
                column: "TestCategoryModelTestCategoryId",
                value: null);

            migrationBuilder.UpdateData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 2,
                column: "TestCategoryModelTestCategoryId",
                value: null);

            migrationBuilder.UpdateData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 3,
                column: "TestCategoryModelTestCategoryId",
                value: null);

            migrationBuilder.UpdateData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 6,
                column: "TestCategoryModelTestCategoryId",
                value: null);

            migrationBuilder.UpdateData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 7,
                column: "TestCategoryModelTestCategoryId",
                value: null);

            migrationBuilder.UpdateData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 8,
                column: "TestCategoryModelTestCategoryId",
                value: null);

            migrationBuilder.UpdateData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 11,
                column: "TestCategoryModelTestCategoryId",
                value: null);

            migrationBuilder.UpdateData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 12,
                column: "TestCategoryModelTestCategoryId",
                value: null);

            migrationBuilder.UpdateData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 17,
                column: "TestCategoryModelTestCategoryId",
                value: null);

            migrationBuilder.UpdateData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 20,
                column: "TestCategoryModelTestCategoryId",
                value: null);

            migrationBuilder.CreateIndex(
                name: "IX_TestTypes_TestCategoryModelTestCategoryId",
                table: "TestTypes",
                column: "TestCategoryModelTestCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_TestTypes_TestCategories_TestCategoryModelTestCategoryId",
                table: "TestTypes",
                column: "TestCategoryModelTestCategoryId",
                principalTable: "TestCategories",
                principalColumn: "TestCategoryId");
        }
    }
}
