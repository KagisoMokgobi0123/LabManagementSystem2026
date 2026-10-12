using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NMBHLabSys.Migrations
{
    /// <inheritdoc />
    public partial class DocAndTechProfiles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TechnicianProfileId",
                table: "TechnicianTestTypes",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DoctorProfiles",
                columns: table => new
                {
                    DoctorProfileId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    HPCSANumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DoctorProfiles", x => x.DoctorProfileId);
                    table.ForeignKey(
                        name: "FK_DoctorProfiles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TechnicianProfiles",
                columns: table => new
                {
                    TechnicianProfileId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IdNumber = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: false),
                    EmployeeNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TechnicianProfiles", x => x.TechnicianProfileId);
                    table.ForeignKey(
                        name: "FK_TechnicianProfiles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "TestRequests",
                keyColumn: "TestRequestId",
                keyValue: 7,
                column: "PatientId",
                value: "953f333d-2e68-46f6-ad26-0412fe75d908");

            migrationBuilder.CreateIndex(
                name: "IX_TechnicianTestTypes_TechnicianProfileId",
                table: "TechnicianTestTypes",
                column: "TechnicianProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_DoctorProfiles_HPCSANumber",
                table: "DoctorProfiles",
                column: "HPCSANumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DoctorProfiles_UserId",
                table: "DoctorProfiles",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TechnicianProfiles_EmployeeNumber",
                table: "TechnicianProfiles",
                column: "EmployeeNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TechnicianProfiles_IdNumber",
                table: "TechnicianProfiles",
                column: "IdNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TechnicianProfiles_UserId",
                table: "TechnicianProfiles",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TechnicianTestTypes_TechnicianProfiles_TechnicianProfileId",
                table: "TechnicianTestTypes",
                column: "TechnicianProfileId",
                principalTable: "TechnicianProfiles",
                principalColumn: "TechnicianProfileId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TechnicianTestTypes_TechnicianProfiles_TechnicianProfileId",
                table: "TechnicianTestTypes");

            migrationBuilder.DropTable(
                name: "DoctorProfiles");

            migrationBuilder.DropTable(
                name: "TechnicianProfiles");

            migrationBuilder.DropIndex(
                name: "IX_TechnicianTestTypes_TechnicianProfileId",
                table: "TechnicianTestTypes");

            migrationBuilder.DropColumn(
                name: "TechnicianProfileId",
                table: "TechnicianTestTypes");

            migrationBuilder.UpdateData(
                table: "TestRequests",
                keyColumn: "TestRequestId",
                keyValue: 7,
                column: "PatientId",
                value: "c0fbfbb4-daf5-44e3-bf57-acfded7baceb");
        }
    }
}
