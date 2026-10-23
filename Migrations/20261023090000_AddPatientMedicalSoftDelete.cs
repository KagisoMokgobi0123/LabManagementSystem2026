
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using NMB_HLabSys.Data;

#nullable disable

namespace NMB_HLabSys.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20261023090000_AddPatientMedicalSoftDelete")]
    public partial class AddPatientMedicalSoftDelete : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "PatientAllergies",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "PatientAllergies");
        }
    }
}
