using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NMBHLabSys.Migrations
{
    /// <inheritdoc />
    public partial class RemoveConsumableIdFromTestType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TechnicianTestTypes_TestTypes_TestTypeId",
                table: "TechnicianTestTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_TestTypeConsumables_TestTypes_TestTypeModelTestTypeId",
                table: "TestTypeConsumables");

            migrationBuilder.DropForeignKey(
                name: "FK_TestTypes_Consumables_ConsumableId",
                table: "TestTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_TestTypes_MeasurementUnits_MeasurementUnit",
                table: "TestTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_TestTypes_SampleTypes_SampleTypeId",
                table: "TestTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_TestTypes_TestCategories_TestCategoryId",
                table: "TestTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_TestTypes_TestTypes_TestTypeModelTestTypeId",
                table: "TestTypes");

            migrationBuilder.DropIndex(
                name: "IX_TestTypes_ConsumableId",
                table: "TestTypes");

            migrationBuilder.DropIndex(
                name: "IX_TestTypeConsumables_TestTypeModelTestTypeId",
                table: "TestTypeConsumables");

            migrationBuilder.DropColumn(
                name: "ConsumableId",
                table: "TestTypes");

            migrationBuilder.DropColumn(
                name: "TestTypeModelTestTypeId",
                table: "TestTypeConsumables");

            migrationBuilder.RenameColumn(
                name: "TestTypeModelTestTypeId",
                table: "TestTypes",
                newName: "TestCategoryModelTestCategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_TestTypes_TestTypeModelTestTypeId",
                table: "TestTypes",
                newName: "IX_TestTypes_TestCategoryModelTestCategoryId");

            migrationBuilder.AddColumn<int>(
                name: "MeasurementUnitModelMeasurementUnitId",
                table: "TestTypes",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 1,
                columns: new[] { "MeasurementUnitModelMeasurementUnitId", "TurnAroundTimeHours" },
                values: new object[] { null, 120 });

            migrationBuilder.UpdateData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 2,
                columns: new[] { "MeasurementUnitModelMeasurementUnitId", "TurnAroundTimeHours" },
                values: new object[] { null, 180 });

            migrationBuilder.UpdateData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 3,
                columns: new[] { "MeasurementUnitModelMeasurementUnitId", "TurnAroundTimeHours" },
                values: new object[] { null, 240 });

            migrationBuilder.UpdateData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 6,
                columns: new[] { "MeasurementUnitModelMeasurementUnitId", "TurnAroundTimeHours" },
                values: new object[] { null, 120 });

            migrationBuilder.UpdateData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 7,
                columns: new[] { "MeasurementUnitModelMeasurementUnitId", "TurnAroundTimeHours" },
                values: new object[] { null, 120 });

            migrationBuilder.UpdateData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 8,
                columns: new[] { "MeasurementUnitModelMeasurementUnitId", "TurnAroundTimeHours" },
                values: new object[] { null, 120 });

            migrationBuilder.UpdateData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 11,
                columns: new[] { "MeasurementUnitModelMeasurementUnitId", "TurnAroundTimeHours" },
                values: new object[] { null, 60 });

            migrationBuilder.UpdateData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 12,
                columns: new[] { "MeasurementUnitModelMeasurementUnitId", "TurnAroundTimeHours" },
                values: new object[] { null, 120 });

            migrationBuilder.UpdateData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 17,
                columns: new[] { "MeasurementUnitModelMeasurementUnitId", "TurnAroundTimeHours" },
                values: new object[] { null, 60 });

            migrationBuilder.UpdateData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 20,
                columns: new[] { "MeasurementUnitModelMeasurementUnitId", "TurnAroundTimeHours" },
                values: new object[] { null, 4320 });

            migrationBuilder.CreateIndex(
                name: "IX_TestTypes_MeasurementUnitModelMeasurementUnitId",
                table: "TestTypes",
                column: "MeasurementUnitModelMeasurementUnitId");

            migrationBuilder.AddForeignKey(
                name: "FK_TechnicianTestTypes_TestTypes_TestTypeId",
                table: "TechnicianTestTypes",
                column: "TestTypeId",
                principalTable: "TestTypes",
                principalColumn: "TestTypeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TestTypes_MeasurementUnits_MeasurementUnit",
                table: "TestTypes",
                column: "MeasurementUnit",
                principalTable: "MeasurementUnits",
                principalColumn: "MeasurementUnitId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TestTypes_MeasurementUnits_MeasurementUnitModelMeasurementUnitId",
                table: "TestTypes",
                column: "MeasurementUnitModelMeasurementUnitId",
                principalTable: "MeasurementUnits",
                principalColumn: "MeasurementUnitId");

            migrationBuilder.AddForeignKey(
                name: "FK_TestTypes_SampleTypes_SampleTypeId",
                table: "TestTypes",
                column: "SampleTypeId",
                principalTable: "SampleTypes",
                principalColumn: "SampleTypeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TestTypes_TestCategories_TestCategoryId",
                table: "TestTypes",
                column: "TestCategoryId",
                principalTable: "TestCategories",
                principalColumn: "TestCategoryId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TestTypes_TestCategories_TestCategoryModelTestCategoryId",
                table: "TestTypes",
                column: "TestCategoryModelTestCategoryId",
                principalTable: "TestCategories",
                principalColumn: "TestCategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TechnicianTestTypes_TestTypes_TestTypeId",
                table: "TechnicianTestTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_TestTypes_MeasurementUnits_MeasurementUnit",
                table: "TestTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_TestTypes_MeasurementUnits_MeasurementUnitModelMeasurementUnitId",
                table: "TestTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_TestTypes_SampleTypes_SampleTypeId",
                table: "TestTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_TestTypes_TestCategories_TestCategoryId",
                table: "TestTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_TestTypes_TestCategories_TestCategoryModelTestCategoryId",
                table: "TestTypes");

            migrationBuilder.DropIndex(
                name: "IX_TestTypes_MeasurementUnitModelMeasurementUnitId",
                table: "TestTypes");

            migrationBuilder.DropColumn(
                name: "MeasurementUnitModelMeasurementUnitId",
                table: "TestTypes");

            migrationBuilder.RenameColumn(
                name: "TestCategoryModelTestCategoryId",
                table: "TestTypes",
                newName: "TestTypeModelTestTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_TestTypes_TestCategoryModelTestCategoryId",
                table: "TestTypes",
                newName: "IX_TestTypes_TestTypeModelTestTypeId");

            migrationBuilder.AddColumn<int>(
                name: "ConsumableId",
                table: "TestTypes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TestTypeModelTestTypeId",
                table: "TestTypeConsumables",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 1,
                column: "TestTypeModelTestTypeId",
                value: null);

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 2,
                column: "TestTypeModelTestTypeId",
                value: null);

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 3,
                column: "TestTypeModelTestTypeId",
                value: null);

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 4,
                column: "TestTypeModelTestTypeId",
                value: null);

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 5,
                column: "TestTypeModelTestTypeId",
                value: null);

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 6,
                column: "TestTypeModelTestTypeId",
                value: null);

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 7,
                column: "TestTypeModelTestTypeId",
                value: null);

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 8,
                column: "TestTypeModelTestTypeId",
                value: null);

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 9,
                column: "TestTypeModelTestTypeId",
                value: null);

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 10,
                column: "TestTypeModelTestTypeId",
                value: null);

            migrationBuilder.UpdateData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 1,
                columns: new[] { "ConsumableId", "TurnAroundTimeHours" },
                values: new object[] { 1, 2 });

            migrationBuilder.UpdateData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 2,
                columns: new[] { "ConsumableId", "TurnAroundTimeHours" },
                values: new object[] { 1, 3 });

            migrationBuilder.UpdateData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 3,
                columns: new[] { "ConsumableId", "TurnAroundTimeHours" },
                values: new object[] { 4, 4 });

            migrationBuilder.UpdateData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 6,
                columns: new[] { "ConsumableId", "TurnAroundTimeHours" },
                values: new object[] { 6, 2 });

            migrationBuilder.UpdateData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 7,
                columns: new[] { "ConsumableId", "TurnAroundTimeHours" },
                values: new object[] { 6, 2 });

            migrationBuilder.UpdateData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 8,
                columns: new[] { "ConsumableId", "TurnAroundTimeHours" },
                values: new object[] { 6, 2 });

            migrationBuilder.UpdateData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 11,
                columns: new[] { "ConsumableId", "TurnAroundTimeHours" },
                values: new object[] { 8, 1 });

            migrationBuilder.UpdateData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 12,
                columns: new[] { "ConsumableId", "TurnAroundTimeHours" },
                values: new object[] { 1, 2 });

            migrationBuilder.UpdateData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 17,
                columns: new[] { "ConsumableId", "TurnAroundTimeHours" },
                values: new object[] { 9, 1 });

            migrationBuilder.UpdateData(
                table: "TestTypes",
                keyColumn: "TestTypeId",
                keyValue: 20,
                columns: new[] { "ConsumableId", "TurnAroundTimeHours" },
                values: new object[] { 12, 72 });

            migrationBuilder.CreateIndex(
                name: "IX_TestTypes_ConsumableId",
                table: "TestTypes",
                column: "ConsumableId");

            migrationBuilder.CreateIndex(
                name: "IX_TestTypeConsumables_TestTypeModelTestTypeId",
                table: "TestTypeConsumables",
                column: "TestTypeModelTestTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_TechnicianTestTypes_TestTypes_TestTypeId",
                table: "TechnicianTestTypes",
                column: "TestTypeId",
                principalTable: "TestTypes",
                principalColumn: "TestTypeId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TestTypeConsumables_TestTypes_TestTypeModelTestTypeId",
                table: "TestTypeConsumables",
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
                name: "FK_TestTypes_MeasurementUnits_MeasurementUnit",
                table: "TestTypes",
                column: "MeasurementUnit",
                principalTable: "MeasurementUnits",
                principalColumn: "MeasurementUnitId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TestTypes_SampleTypes_SampleTypeId",
                table: "TestTypes",
                column: "SampleTypeId",
                principalTable: "SampleTypes",
                principalColumn: "SampleTypeId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TestTypes_TestCategories_TestCategoryId",
                table: "TestTypes",
                column: "TestCategoryId",
                principalTable: "TestCategories",
                principalColumn: "TestCategoryId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TestTypes_TestTypes_TestTypeModelTestTypeId",
                table: "TestTypes",
                column: "TestTypeModelTestTypeId",
                principalTable: "TestTypes",
                principalColumn: "TestTypeId");
        }
    }
}
