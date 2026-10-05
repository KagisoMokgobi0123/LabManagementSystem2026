using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NMBHLabSys.Migrations
{
    /// <inheritdoc />
    public partial class updatedCategoryFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IsActive",
                table: "TestTypes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 1,
                column: "IsActive",
                value: 1);

            migrationBuilder.UpdateData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 2,
                column: "IsActive",
                value: 1);

            migrationBuilder.UpdateData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 3,
                column: "IsActive",
                value: 1);

            migrationBuilder.UpdateData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 6,
                column: "IsActive",
                value: 1);

            migrationBuilder.UpdateData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 7,
                column: "IsActive",
                value: 1);

            migrationBuilder.UpdateData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 8,
                column: "IsActive",
                value: 1);

            migrationBuilder.UpdateData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 11,
                column: "IsActive",
                value: 1);

            migrationBuilder.UpdateData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 12,
                column: "IsActive",
                value: 1);

            migrationBuilder.UpdateData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 17,
                column: "IsActive",
                value: 1);

            migrationBuilder.UpdateData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 20,
                column: "IsActive",
                value: 1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "TestTypes");
        }
    }
}
