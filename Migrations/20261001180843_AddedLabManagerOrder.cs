using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace NMBHLabSys.Migrations
{
    /// <inheritdoc />
    public partial class AddedLabManagerOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestTypeConsumables_Consumables_ConsumableId",
                table: "TestTypeConsumables");

            migrationBuilder.DropForeignKey(
                name: "FK_TestTypeConsumables_TestTypes_TestTypeId",
                table: "TestTypeConsumables");

            migrationBuilder.AddColumn<int>(
                name: "TestTypeModelTestTypeId",
                table: "TestTypeConsumables",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Consumables",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "SupplierId",
                table: "Consumables",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "StockAdjustments",
                columns: table => new
                {
                    StockAdjustmentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ConsumableId = table.Column<int>(type: "int", nullable: false),
                    AdjustmentType = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    AdjustmentDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockAdjustments", x => x.StockAdjustmentId);
                    table.ForeignKey(
                        name: "FK_StockAdjustments_Consumables_ConsumableId",
                        column: x => x.ConsumableId,
                        principalTable: "Consumables",
                        principalColumn: "ConsumableId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Suppliers",
                columns: table => new
                {
                    SupplierId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SupplierName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ContactPerson = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(254)", maxLength: 254, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Suppliers", x => x.SupplierId);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    OrderId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SupplierId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    DateComplete = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CancellationReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DateCancelled = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.OrderId);
                    table.ForeignKey(
                        name: "FK_Orders_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "SupplierId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrderItems",
                columns: table => new
                {
                    OrderItemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    ConsumableId = table.Column<int>(type: "int", nullable: false),
                    QuantityOrdered = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    DateReceived = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CancellationReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DateCancelled = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItems", x => x.OrderItemId);
                    table.ForeignKey(
                        name: "FK_OrderItems_Consumables_ConsumableId",
                        column: x => x.ConsumableId,
                        principalTable: "Consumables",
                        principalColumn: "ConsumableId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderItems_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Consumables",
                keyColumn: "ConsumableId",
                keyValue: 1,
                columns: new[] { "IsDeleted", "SupplierId" },
                values: new object[] { false, 1 });

            migrationBuilder.UpdateData(
                table: "Consumables",
                keyColumn: "ConsumableId",
                keyValue: 2,
                columns: new[] { "IsDeleted", "SupplierId" },
                values: new object[] { false, 1 });

            migrationBuilder.UpdateData(
                table: "Consumables",
                keyColumn: "ConsumableId",
                keyValue: 3,
                columns: new[] { "IsDeleted", "SupplierId" },
                values: new object[] { false, 2 });

            migrationBuilder.UpdateData(
                table: "Consumables",
                keyColumn: "ConsumableId",
                keyValue: 4,
                columns: new[] { "IsDeleted", "SupplierId" },
                values: new object[] { false, 2 });

            migrationBuilder.UpdateData(
                table: "Consumables",
                keyColumn: "ConsumableId",
                keyValue: 5,
                columns: new[] { "IsDeleted", "SupplierId" },
                values: new object[] { false, 2 });

            migrationBuilder.UpdateData(
                table: "Consumables",
                keyColumn: "ConsumableId",
                keyValue: 6,
                columns: new[] { "IsDeleted", "SupplierId" },
                values: new object[] { false, 3 });

            migrationBuilder.UpdateData(
                table: "Consumables",
                keyColumn: "ConsumableId",
                keyValue: 7,
                columns: new[] { "IsDeleted", "SupplierId" },
                values: new object[] { false, 3 });

            migrationBuilder.UpdateData(
                table: "Consumables",
                keyColumn: "ConsumableId",
                keyValue: 8,
                columns: new[] { "IsDeleted", "SupplierId" },
                values: new object[] { false, 3 });

            migrationBuilder.UpdateData(
                table: "Consumables",
                keyColumn: "ConsumableId",
                keyValue: 9,
                columns: new[] { "IsDeleted", "SupplierId" },
                values: new object[] { false, 2 });

            migrationBuilder.UpdateData(
                table: "Consumables",
                keyColumn: "ConsumableId",
                keyValue: 10,
                columns: new[] { "IsDeleted", "SupplierId" },
                values: new object[] { false, 2 });

            migrationBuilder.UpdateData(
                table: "Consumables",
                keyColumn: "ConsumableId",
                keyValue: 11,
                columns: new[] { "IsDeleted", "SupplierId" },
                values: new object[] { false, 3 });

            migrationBuilder.UpdateData(
                table: "Consumables",
                keyColumn: "ConsumableId",
                keyValue: 12,
                columns: new[] { "IsDeleted", "SupplierId" },
                values: new object[] { false, 3 });

            migrationBuilder.InsertData(
                table: "Suppliers",
                columns: new[] { "SupplierId", "ContactPerson", "Email", "IsDeleted", "SupplierName" },
                values: new object[,]
                {
                    { 1, "Laboratory Supplies Department", "supplies@medlabsupplies.example", false, "MedLab Supplies" },
                    { 2, "Sales Department", "sales@biomedical.example", false, "BioMedical Distributors" },
                    { 3, "Diagnostics Sales", "orders@clinicaldiagnostics.example", false, "Clinical Diagnostics Ltd" }
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_TestTypeConsumables_TestTypeModelTestTypeId",
                table: "TestTypeConsumables",
                column: "TestTypeModelTestTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Consumables_Name",
                table: "Consumables",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Consumables_SupplierId",
                table: "Consumables",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_ConsumableId",
                table: "OrderItems",
                column: "ConsumableId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderId",
                table: "OrderItems",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_OrderNumber",
                table: "Orders",
                column: "OrderNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_SupplierId",
                table: "Orders",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_StockAdjustments_ConsumableId",
                table: "StockAdjustments",
                column: "ConsumableId");

            migrationBuilder.CreateIndex(
                name: "IX_Suppliers_SupplierName",
                table: "Suppliers",
                column: "SupplierName",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Consumables_Suppliers_SupplierId",
                table: "Consumables",
                column: "SupplierId",
                principalTable: "Suppliers",
                principalColumn: "SupplierId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TestTypeConsumables_Consumables_ConsumableId",
                table: "TestTypeConsumables",
                column: "ConsumableId",
                principalTable: "Consumables",
                principalColumn: "ConsumableId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TestTypeConsumables_TestTypes_TestTypeId",
                table: "TestTypeConsumables",
                column: "TestTypeId",
                principalTable: "TestTypes",
                principalColumn: "TestTypeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TestTypeConsumables_TestTypes_TestTypeModelTestTypeId",
                table: "TestTypeConsumables",
                column: "TestTypeModelTestTypeId",
                principalTable: "TestTypes",
                principalColumn: "TestTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Consumables_Suppliers_SupplierId",
                table: "Consumables");

            migrationBuilder.DropForeignKey(
                name: "FK_TestTypeConsumables_Consumables_ConsumableId",
                table: "TestTypeConsumables");

            migrationBuilder.DropForeignKey(
                name: "FK_TestTypeConsumables_TestTypes_TestTypeId",
                table: "TestTypeConsumables");

            migrationBuilder.DropForeignKey(
                name: "FK_TestTypeConsumables_TestTypes_TestTypeModelTestTypeId",
                table: "TestTypeConsumables");

            migrationBuilder.DropTable(
                name: "OrderItems");

            migrationBuilder.DropTable(
                name: "StockAdjustments");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Suppliers");

            migrationBuilder.DropIndex(
                name: "IX_TestTypeConsumables_TestTypeModelTestTypeId",
                table: "TestTypeConsumables");

            migrationBuilder.DropIndex(
                name: "IX_Consumables_Name",
                table: "Consumables");

            migrationBuilder.DropIndex(
                name: "IX_Consumables_SupplierId",
                table: "Consumables");

            migrationBuilder.DropColumn(
                name: "TestTypeModelTestTypeId",
                table: "TestTypeConsumables");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Consumables");

            migrationBuilder.DropColumn(
                name: "SupplierId",
                table: "Consumables");

            migrationBuilder.AddForeignKey(
                name: "FK_TestTypeConsumables_Consumables_ConsumableId",
                table: "TestTypeConsumables",
                column: "ConsumableId",
                principalTable: "Consumables",
                principalColumn: "ConsumableId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TestTypeConsumables_TestTypes_TestTypeId",
                table: "TestTypeConsumables",
                column: "TestTypeId",
                principalTable: "TestTypes",
                principalColumn: "TestTypeId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
