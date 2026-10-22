using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NMBHLabSys.Migrations
{
    /// <inheritdoc />
    public partial class SupportIndividualPreliminaryReports : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TechnicianSentEmails_TestRequestId",
                table: "TechnicianSentEmails");

            migrationBuilder.AddColumn<string>(
                name: "ReportType",
                table: "TechnicianSentEmails",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "TestRequestTestTypeId",
                table: "TechnicianSentEmails",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TechnicianSentEmails_OneFinalPerRequest",
                table: "TechnicianSentEmails",
                column: "TestRequestId",
                unique: true,
                filter: "[ReportType] = 'Final'");

            migrationBuilder.CreateIndex(
                name: "IX_TechnicianSentEmails_TestRequestTestTypeId_ReportType",
                table: "TechnicianSentEmails",
                columns: new[] { "TestRequestTestTypeId", "ReportType" },
                unique: true,
                filter: "[TestRequestTestTypeId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TechnicianSentEmails_OneFinalPerRequest",
                table: "TechnicianSentEmails");

            migrationBuilder.DropIndex(
                name: "IX_TechnicianSentEmails_TestRequestTestTypeId_ReportType",
                table: "TechnicianSentEmails");

            migrationBuilder.DropColumn(
                name: "ReportType",
                table: "TechnicianSentEmails");

            migrationBuilder.DropColumn(
                name: "TestRequestTestTypeId",
                table: "TechnicianSentEmails");

            migrationBuilder.CreateIndex(
                name: "IX_TechnicianSentEmails_TestRequestId",
                table: "TechnicianSentEmails",
                column: "TestRequestId",
                unique: true);
        }
    }
}
