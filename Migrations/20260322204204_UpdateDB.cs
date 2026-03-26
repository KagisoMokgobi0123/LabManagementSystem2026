using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NMB_HLabSys.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_LoggedUsers",
                table: "LoggedUsers");

            migrationBuilder.RenameTable(
                name: "LoggedUsers",
                newName: "LoggedInUsers");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LoggedInUsers",
                table: "LoggedInUsers",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_LoggedInUsers",
                table: "LoggedInUsers");

            migrationBuilder.RenameTable(
                name: "LoggedInUsers",
                newName: "LoggedUsers");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LoggedUsers",
                table: "LoggedUsers",
                column: "Id");
        }
    }
}
