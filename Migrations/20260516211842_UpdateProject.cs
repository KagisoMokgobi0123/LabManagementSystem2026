using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NMBHLabSys.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAbnormal",
                table: "TestRequestTests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    NotificationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IssuedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DoctorId = table.Column<string>(type: "nvarchar(450)", nullable: false),
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

            migrationBuilder.CreateTable(
                name: "StockTransactions",
                columns: table => new
                {
                    TransactionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ConsumableId = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    PreviousQuantity = table.Column<int>(type: "int", nullable: false),
                    NewQuantity = table.Column<int>(type: "int", nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Reference = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PerformedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockTransactions", x => x.TransactionId);
                    table.ForeignKey(
                        name: "FK_StockTransactions_Consumables_ConsumableId",
                        column: x => x.ConsumableId,
                        principalTable: "Consumables",
                        principalColumn: "ConsumableId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "TestRequestTests",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1,
                columns: new[] { "CompletionDateTime", "DueDate", "IsAbnormal", "StartDateTime" },
                values: new object[] { new DateTime(2026, 5, 16, 23, 18, 41, 223, DateTimeKind.Local).AddTicks(3658), new DateTime(2026, 5, 17, 1, 18, 41, 223, DateTimeKind.Local).AddTicks(3660), false, new DateTime(2026, 5, 16, 23, 18, 41, 223, DateTimeKind.Local).AddTicks(3649) });

            migrationBuilder.UpdateData(
                table: "TestRequestTests",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 2,
                columns: new[] { "CompletionDateTime", "DueDate", "IsAbnormal", "StartDateTime" },
                values: new object[] { new DateTime(2026, 5, 16, 23, 18, 41, 223, DateTimeKind.Local).AddTicks(3669), new DateTime(2026, 5, 17, 0, 18, 41, 223, DateTimeKind.Local).AddTicks(3670), false, new DateTime(2026, 5, 16, 23, 18, 41, 223, DateTimeKind.Local).AddTicks(3668) });

            migrationBuilder.UpdateData(
                table: "TestRequestTests",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 3,
                columns: new[] { "CompletionDateTime", "DueDate", "IsAbnormal", "StartDateTime" },
                values: new object[] { new DateTime(2026, 5, 16, 23, 18, 41, 223, DateTimeKind.Local).AddTicks(3673), new DateTime(2026, 5, 17, 1, 18, 41, 223, DateTimeKind.Local).AddTicks(3673), false, new DateTime(2026, 5, 16, 21, 18, 41, 223, DateTimeKind.Local).AddTicks(3672) });

            migrationBuilder.UpdateData(
                table: "TestRequestTests",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 4,
                columns: new[] { "CompletionDateTime", "DueDate", "IsAbnormal", "StartDateTime" },
                values: new object[] { new DateTime(2026, 5, 16, 23, 18, 41, 223, DateTimeKind.Local).AddTicks(3676), new DateTime(2026, 5, 17, 1, 18, 41, 223, DateTimeKind.Local).AddTicks(3677), false, new DateTime(2026, 5, 16, 23, 18, 41, 223, DateTimeKind.Local).AddTicks(3676) });

            migrationBuilder.UpdateData(
                table: "TestRequestTests",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 5,
                columns: new[] { "CompletionDateTime", "DueDate", "IsAbnormal", "StartDateTime" },
                values: new object[] { new DateTime(2026, 5, 16, 22, 18, 41, 223, DateTimeKind.Local).AddTicks(3681), new DateTime(2026, 5, 16, 22, 18, 41, 223, DateTimeKind.Local).AddTicks(3682), false, new DateTime(2026, 5, 16, 20, 18, 41, 223, DateTimeKind.Local).AddTicks(3680) });

            migrationBuilder.UpdateData(
                table: "TestRequestTests",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 6,
                columns: new[] { "CompletionDateTime", "DueDate", "IsAbnormal", "StartDateTime", "VerificationDateTime" },
                values: new object[] { new DateTime(2026, 5, 16, 21, 18, 41, 223, DateTimeKind.Local).AddTicks(3686), new DateTime(2026, 5, 16, 21, 18, 41, 223, DateTimeKind.Local).AddTicks(3687), false, new DateTime(2026, 5, 16, 20, 18, 41, 223, DateTimeKind.Local).AddTicks(3685), new DateTime(2026, 5, 16, 21, 48, 41, 223, DateTimeKind.Local).AddTicks(3690) });

            migrationBuilder.UpdateData(
                table: "TestRequestTests",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 7,
                columns: new[] { "CompletionDateTime", "DueDate", "IsAbnormal", "StartDateTime", "VerificationDateTime" },
                values: new object[] { new DateTime(2026, 5, 16, 22, 48, 41, 223, DateTimeKind.Local).AddTicks(3693), new DateTime(2026, 5, 16, 23, 18, 41, 223, DateTimeKind.Local).AddTicks(3695), false, new DateTime(2026, 5, 16, 21, 18, 41, 223, DateTimeKind.Local).AddTicks(3693), new DateTime(2026, 5, 16, 22, 48, 41, 223, DateTimeKind.Local).AddTicks(3696) });

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

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_DoctorId",
                table: "Notifications",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_StockTransactions_ConsumableId",
                table: "StockTransactions",
                column: "ConsumableId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "StockTransactions");

            migrationBuilder.DropColumn(
                name: "IsAbnormal",
                table: "TestRequestTests");

            migrationBuilder.UpdateData(
                table: "TestRequestTests",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 1,
                columns: new[] { "CompletionDateTime", "DueDate", "StartDateTime" },
                values: new object[] { new DateTime(2026, 5, 16, 21, 35, 38, 76, DateTimeKind.Local).AddTicks(9396), new DateTime(2026, 5, 16, 23, 35, 38, 76, DateTimeKind.Local).AddTicks(9396), new DateTime(2026, 5, 16, 21, 35, 38, 76, DateTimeKind.Local).AddTicks(9385) });

            migrationBuilder.UpdateData(
                table: "TestRequestTests",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 2,
                columns: new[] { "CompletionDateTime", "DueDate", "StartDateTime" },
                values: new object[] { new DateTime(2026, 5, 16, 21, 35, 38, 76, DateTimeKind.Local).AddTicks(9407), new DateTime(2026, 5, 16, 22, 35, 38, 76, DateTimeKind.Local).AddTicks(9407), new DateTime(2026, 5, 16, 21, 35, 38, 76, DateTimeKind.Local).AddTicks(9406) });

            migrationBuilder.UpdateData(
                table: "TestRequestTests",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 3,
                columns: new[] { "CompletionDateTime", "DueDate", "StartDateTime" },
                values: new object[] { new DateTime(2026, 5, 16, 21, 35, 38, 76, DateTimeKind.Local).AddTicks(9411), new DateTime(2026, 5, 16, 23, 35, 38, 76, DateTimeKind.Local).AddTicks(9411), new DateTime(2026, 5, 16, 19, 35, 38, 76, DateTimeKind.Local).AddTicks(9410) });

            migrationBuilder.UpdateData(
                table: "TestRequestTests",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 4,
                columns: new[] { "CompletionDateTime", "DueDate", "StartDateTime" },
                values: new object[] { new DateTime(2026, 5, 16, 21, 35, 38, 76, DateTimeKind.Local).AddTicks(9416), new DateTime(2026, 5, 16, 23, 35, 38, 76, DateTimeKind.Local).AddTicks(9416), new DateTime(2026, 5, 16, 21, 35, 38, 76, DateTimeKind.Local).AddTicks(9415) });

            migrationBuilder.UpdateData(
                table: "TestRequestTests",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 5,
                columns: new[] { "CompletionDateTime", "DueDate", "StartDateTime" },
                values: new object[] { new DateTime(2026, 5, 16, 20, 35, 38, 76, DateTimeKind.Local).AddTicks(9420), new DateTime(2026, 5, 16, 20, 35, 38, 76, DateTimeKind.Local).AddTicks(9421), new DateTime(2026, 5, 16, 18, 35, 38, 76, DateTimeKind.Local).AddTicks(9419) });

            migrationBuilder.UpdateData(
                table: "TestRequestTests",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 6,
                columns: new[] { "CompletionDateTime", "DueDate", "StartDateTime", "VerificationDateTime" },
                values: new object[] { new DateTime(2026, 5, 16, 19, 35, 38, 76, DateTimeKind.Local).AddTicks(9426), new DateTime(2026, 5, 16, 19, 35, 38, 76, DateTimeKind.Local).AddTicks(9427), new DateTime(2026, 5, 16, 18, 35, 38, 76, DateTimeKind.Local).AddTicks(9425), new DateTime(2026, 5, 16, 20, 5, 38, 76, DateTimeKind.Local).AddTicks(9430) });

            migrationBuilder.UpdateData(
                table: "TestRequestTests",
                keyColumn: "TestRequestTestTypeId",
                keyValue: 7,
                columns: new[] { "CompletionDateTime", "DueDate", "StartDateTime", "VerificationDateTime" },
                values: new object[] { new DateTime(2026, 5, 16, 21, 5, 38, 76, DateTimeKind.Local).AddTicks(9437), new DateTime(2026, 5, 16, 21, 35, 38, 76, DateTimeKind.Local).AddTicks(9438), new DateTime(2026, 5, 16, 19, 35, 38, 76, DateTimeKind.Local).AddTicks(9436), new DateTime(2026, 5, 16, 21, 5, 38, 76, DateTimeKind.Local).AddTicks(9440) });

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 1,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 21, 35, 38, 76, DateTimeKind.Local).AddTicks(9826));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 2,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 21, 35, 38, 76, DateTimeKind.Local).AddTicks(9829));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 3,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 21, 35, 38, 76, DateTimeKind.Local).AddTicks(9830));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 4,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 21, 35, 38, 76, DateTimeKind.Local).AddTicks(9831));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 5,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 21, 35, 38, 76, DateTimeKind.Local).AddTicks(9832));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 6,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 21, 35, 38, 76, DateTimeKind.Local).AddTicks(9834));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 7,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 21, 35, 38, 76, DateTimeKind.Local).AddTicks(9836));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 8,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 21, 35, 38, 76, DateTimeKind.Local).AddTicks(9837));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 9,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 21, 35, 38, 76, DateTimeKind.Local).AddTicks(9838));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 10,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 21, 35, 38, 76, DateTimeKind.Local).AddTicks(9839));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 11,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 21, 35, 38, 76, DateTimeKind.Local).AddTicks(9840));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 12,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 21, 35, 38, 76, DateTimeKind.Local).AddTicks(9841));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 13,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 21, 35, 38, 76, DateTimeKind.Local).AddTicks(9842));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 14,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 21, 35, 38, 76, DateTimeKind.Local).AddTicks(9844));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 15,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 21, 35, 38, 76, DateTimeKind.Local).AddTicks(9845));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 16,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 21, 35, 38, 76, DateTimeKind.Local).AddTicks(9846));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 17,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 21, 35, 38, 76, DateTimeKind.Local).AddTicks(9847));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 18,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 21, 35, 38, 76, DateTimeKind.Local).AddTicks(9848));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 19,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 21, 35, 38, 76, DateTimeKind.Local).AddTicks(9849));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 20,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 21, 35, 38, 76, DateTimeKind.Local).AddTicks(9850));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 21,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 21, 35, 38, 76, DateTimeKind.Local).AddTicks(9852));

            migrationBuilder.UpdateData(
                table: "TestTypeConsumables",
                keyColumn: "TestTypeConsumableId",
                keyValue: 22,
                column: "VerificationDate",
                value: new DateTime(2026, 5, 16, 21, 35, 38, 76, DateTimeKind.Local).AddTicks(9853));
        }
    }
}
