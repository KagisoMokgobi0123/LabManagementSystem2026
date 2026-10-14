using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NMBHLabSys.Migrations
{
    /// <inheritdoc />
    public partial class AddMissingResultColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAbnormal",
                table: "TestRequestTestTypes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ResultNotes",
                table: "TestRequestTestTypes",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ResultValue",
                table: "TestRequestTestTypes",
                type: "decimal(18,4)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "NoCurrentMedications",
                table: "Patients",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "NoKnownAllergies",
                table: "Patients",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "NoKnownMedicalConditions",
                table: "Patients",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1,
                columns: new[] { "IsAbnormal", "ResultNotes", "ResultValue" },
                values: new object[] { false, null, null });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 2,
                columns: new[] { "IsAbnormal", "ResultNotes", "ResultValue" },
                values: new object[] { false, null, null });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 3,
                columns: new[] { "IsAbnormal", "ResultNotes", "ResultValue" },
                values: new object[] { false, null, null });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 4,
                columns: new[] { "IsAbnormal", "ResultNotes", "ResultValue" },
                values: new object[] { false, null, null });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 5,
                columns: new[] { "IsAbnormal", "ResultNotes", "ResultValue" },
                values: new object[] { false, null, null });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 6,
                columns: new[] { "IsAbnormal", "ResultNotes", "ResultValue" },
                values: new object[] { false, null, null });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 7,
                columns: new[] { "IsAbnormal", "ResultNotes", "ResultValue" },
                values: new object[] { false, null, null });

            migrationBuilder.UpdateData(
                table: "TestRequestTestTypes",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 8,
                columns: new[] { "IsAbnormal", "ResultNotes", "ResultValue" },
                values: new object[] { false, null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAbnormal",
                table: "TestRequestTestTypes");

            migrationBuilder.DropColumn(
                name: "ResultNotes",
                table: "TestRequestTestTypes");

            migrationBuilder.DropColumn(
                name: "ResultValue",
                table: "TestRequestTestTypes");

            migrationBuilder.DropColumn(
                name: "NoCurrentMedications",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "NoKnownAllergies",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "NoKnownMedicalConditions",
                table: "Patients");
        }
    }
}
