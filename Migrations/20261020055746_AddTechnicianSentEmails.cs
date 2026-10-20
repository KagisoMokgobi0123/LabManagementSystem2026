using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NMBHLabSys.Migrations
{
    /// <inheritdoc />
    public partial class AddTechnicianSentEmails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TechnicianSentEmails",
                columns: table => new
                {
                    SentEmailId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TestRequestId = table.Column<int>(type: "int", nullable: false),
                    RecipientEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Body = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PdfFileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    PdfAttachment = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SentAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ErrorMessage = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TechnicianSentEmails", x => x.SentEmailId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TechnicianSentEmails_TestRequestId",
                table: "TechnicianSentEmails",
                column: "TestRequestId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TechnicianSentEmails");
        }
    }
}
