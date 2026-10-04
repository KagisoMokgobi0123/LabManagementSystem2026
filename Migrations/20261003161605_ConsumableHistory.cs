using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NMBHLabSys.Migrations
{
    /// <inheritdoc />
    public partial class ConsumableHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StockStatus",
                table: "Consumables",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "StockStatusHistories",
                columns: table => new
                {
                    StockStatusHistoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ConsumableId = table.Column<int>(type: "int", nullable: false),
                    StockStatus = table.Column<int>(type: "int", nullable: false),
                    QuantityOnHand = table.Column<int>(type: "int", nullable: false),
                    ReorderLevel = table.Column<int>(type: "int", nullable: false),
                    DateRecorded = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockStatusHistories", x => x.StockStatusHistoryId);
                    table.ForeignKey(
                        name: "FK_StockStatusHistories_Consumables_ConsumableId",
                        column: x => x.ConsumableId,
                        principalTable: "Consumables",
                        principalColumn: "ConsumableId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Consumables",
                keyColumn: "ConsumableId",
                keyValue: 1,
                column: "StockStatus",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Consumables",
                keyColumn: "ConsumableId",
                keyValue: 2,
                column: "StockStatus",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Consumables",
                keyColumn: "ConsumableId",
                keyValue: 3,
                column: "StockStatus",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Consumables",
                keyColumn: "ConsumableId",
                keyValue: 4,
                column: "StockStatus",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Consumables",
                keyColumn: "ConsumableId",
                keyValue: 5,
                column: "StockStatus",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Consumables",
                keyColumn: "ConsumableId",
                keyValue: 6,
                column: "StockStatus",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Consumables",
                keyColumn: "ConsumableId",
                keyValue: 7,
                column: "StockStatus",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Consumables",
                keyColumn: "ConsumableId",
                keyValue: 8,
                column: "StockStatus",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Consumables",
                keyColumn: "ConsumableId",
                keyValue: 9,
                column: "StockStatus",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Consumables",
                keyColumn: "ConsumableId",
                keyValue: 10,
                column: "StockStatus",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Consumables",
                keyColumn: "ConsumableId",
                keyValue: 11,
                column: "StockStatus",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Consumables",
                keyColumn: "ConsumableId",
                keyValue: 12,
                column: "StockStatus",
                value: 1);

            migrationBuilder.CreateIndex(
                name: "IX_StockStatusHistories_ConsumableId",
                table: "StockStatusHistories",
                column: "ConsumableId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StockStatusHistories");

            migrationBuilder.DropColumn(
                name: "StockStatus",
                table: "Consumables");
        }
    }
}
