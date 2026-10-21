using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NMBHLabSys.Migrations
{
    /// <inheritdoc />
    public partial class AddSampleDeactivationAudit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeactivatedAt",
                table: "Samples",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeactivatedByTechnicianId",
                table: "Samples",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeactivationReason",
                table: "Samples",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SampleDeactivationAudits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SampleId = table.Column<int>(type: "int", nullable: false),
                    TechnicianId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    DeactivatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SampleDeactivationAudits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SampleDeactivationAudits_AspNetUsers_TechnicianId",
                        column: x => x.TechnicianId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SampleDeactivationAudits_Samples_SampleId",
                        column: x => x.SampleId,
                        principalTable: "Samples",
                        principalColumn: "SampleId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "Samples",
                keyColumn: "SampleId",
                keyValue: 1,
                columns: new[] { "DeactivatedAt", "DeactivatedByTechnicianId", "DeactivationReason" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Samples",
                keyColumn: "SampleId",
                keyValue: 2,
                columns: new[] { "DeactivatedAt", "DeactivatedByTechnicianId", "DeactivationReason" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Samples",
                keyColumn: "SampleId",
                keyValue: 3,
                columns: new[] { "DeactivatedAt", "DeactivatedByTechnicianId", "DeactivationReason" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Samples",
                keyColumn: "SampleId",
                keyValue: 4,
                columns: new[] { "DeactivatedAt", "DeactivatedByTechnicianId", "DeactivationReason" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Samples",
                keyColumn: "SampleId",
                keyValue: 5,
                columns: new[] { "DeactivatedAt", "DeactivatedByTechnicianId", "DeactivationReason" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Samples",
                keyColumn: "SampleId",
                keyValue: 6,
                columns: new[] { "DeactivatedAt", "DeactivatedByTechnicianId", "DeactivationReason" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Samples",
                keyColumn: "SampleId",
                keyValue: 7,
                columns: new[] { "DeactivatedAt", "DeactivatedByTechnicianId", "DeactivationReason" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Samples",
                keyColumn: "SampleId",
                keyValue: 8101,
                columns: new[] { "DeactivatedAt", "DeactivatedByTechnicianId", "DeactivationReason" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Samples",
                keyColumn: "SampleId",
                keyValue: 8102,
                columns: new[] { "DeactivatedAt", "DeactivatedByTechnicianId", "DeactivationReason" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Samples",
                keyColumn: "SampleId",
                keyValue: 8103,
                columns: new[] { "DeactivatedAt", "DeactivatedByTechnicianId", "DeactivationReason" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Samples",
                keyColumn: "SampleId",
                keyValue: 8104,
                columns: new[] { "DeactivatedAt", "DeactivatedByTechnicianId", "DeactivationReason" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Samples",
                keyColumn: "SampleId",
                keyValue: 8201,
                columns: new[] { "DeactivatedAt", "DeactivatedByTechnicianId", "DeactivationReason" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Samples",
                keyColumn: "SampleId",
                keyValue: 8202,
                columns: new[] { "DeactivatedAt", "DeactivatedByTechnicianId", "DeactivationReason" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Samples",
                keyColumn: "SampleId",
                keyValue: 8203,
                columns: new[] { "DeactivatedAt", "DeactivatedByTechnicianId", "DeactivationReason" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Samples",
                keyColumn: "SampleId",
                keyValue: 8204,
                columns: new[] { "DeactivatedAt", "DeactivatedByTechnicianId", "DeactivationReason" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Samples",
                keyColumn: "SampleId",
                keyValue: 8301,
                columns: new[] { "DeactivatedAt", "DeactivatedByTechnicianId", "DeactivationReason" },
                values: new object[] { null, null, null });

            migrationBuilder.CreateIndex(
                name: "IX_SampleDeactivationAudits_SampleId",
                table: "SampleDeactivationAudits",
                column: "SampleId");

            migrationBuilder.CreateIndex(
                name: "IX_SampleDeactivationAudits_TechnicianId",
                table: "SampleDeactivationAudits",
                column: "TechnicianId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SampleDeactivationAudits");

            migrationBuilder.DropColumn(
                name: "DeactivatedAt",
                table: "Samples");

            migrationBuilder.DropColumn(
                name: "DeactivatedByTechnicianId",
                table: "Samples");

            migrationBuilder.DropColumn(
                name: "DeactivationReason",
                table: "Samples");
        }
    }
}
