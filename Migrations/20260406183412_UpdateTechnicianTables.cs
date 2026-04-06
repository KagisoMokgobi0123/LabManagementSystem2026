using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NMBHLabSys.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTechnicianTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Samples_AspNetUsers_TechnicianId",
                table: "Samples");

            migrationBuilder.DropForeignKey(
                name: "FK_Samples_TestRequests_RequestTestRequestId",
                table: "Samples");

            migrationBuilder.DropIndex(
                name: "IX_Samples_RequestTestRequestId",
                table: "Samples");

            migrationBuilder.DropColumn(
                name: "RequestTestRequestId",
                table: "Samples");

            migrationBuilder.DropColumn(
                name: "TechcianId",
                table: "Samples");

            migrationBuilder.AlterColumn<string>(
                name: "TechnicianId",
                table: "Samples",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "BarcodeNumber",
                table: "Samples",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "TestRequestId",
                table: "Samples",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Samples_BarcodeNumber",
                table: "Samples",
                column: "BarcodeNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Samples_TestRequestId",
                table: "Samples",
                column: "TestRequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_Samples_AspNetUsers_TechnicianId",
                table: "Samples",
                column: "TechnicianId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Samples_TestRequests_TestRequestId",
                table: "Samples",
                column: "TestRequestId",
                principalTable: "TestRequests",
                principalColumn: "TestRequestId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Samples_AspNetUsers_TechnicianId",
                table: "Samples");

            migrationBuilder.DropForeignKey(
                name: "FK_Samples_TestRequests_TestRequestId",
                table: "Samples");

            migrationBuilder.DropIndex(
                name: "IX_Samples_BarcodeNumber",
                table: "Samples");

            migrationBuilder.DropIndex(
                name: "IX_Samples_TestRequestId",
                table: "Samples");

            migrationBuilder.DropColumn(
                name: "TestRequestId",
                table: "Samples");

            migrationBuilder.AlterColumn<string>(
                name: "TechnicianId",
                table: "Samples",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<int>(
                name: "BarcodeNumber",
                table: "Samples",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<int>(
                name: "RequestTestRequestId",
                table: "Samples",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TechcianId",
                table: "Samples",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Samples_RequestTestRequestId",
                table: "Samples",
                column: "RequestTestRequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_Samples_AspNetUsers_TechnicianId",
                table: "Samples",
                column: "TechnicianId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Samples_TestRequests_RequestTestRequestId",
                table: "Samples",
                column: "RequestTestRequestId",
                principalTable: "TestRequests",
                principalColumn: "TestRequestId");
        }
    }
}
