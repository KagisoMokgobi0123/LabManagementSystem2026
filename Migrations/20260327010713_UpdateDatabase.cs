using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NMB_HLabSys.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AccessFailedCount",
                table: "LoggedInUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ConcurrencyStamp",
                table: "LoggedInUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EmailConfirmed",
                table: "LoggedInUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "LockoutEnabled",
                table: "LoggedInUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LockoutEnd",
                table: "LoggedInUsers",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NormalizedEmail",
                table: "LoggedInUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NormalizedUserName",
                table: "LoggedInUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "LoggedInUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "LoggedInUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PhoneNumberConfirmed",
                table: "LoggedInUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SecurityStamp",
                table: "LoggedInUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "TwoFactorEnabled",
                table: "LoggedInUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "LoggedInUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccessFailedCount",
                table: "LoggedInUsers");

            migrationBuilder.DropColumn(
                name: "ConcurrencyStamp",
                table: "LoggedInUsers");

            migrationBuilder.DropColumn(
                name: "EmailConfirmed",
                table: "LoggedInUsers");

            migrationBuilder.DropColumn(
                name: "LockoutEnabled",
                table: "LoggedInUsers");

            migrationBuilder.DropColumn(
                name: "LockoutEnd",
                table: "LoggedInUsers");

            migrationBuilder.DropColumn(
                name: "NormalizedEmail",
                table: "LoggedInUsers");

            migrationBuilder.DropColumn(
                name: "NormalizedUserName",
                table: "LoggedInUsers");

            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "LoggedInUsers");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "LoggedInUsers");

            migrationBuilder.DropColumn(
                name: "PhoneNumberConfirmed",
                table: "LoggedInUsers");

            migrationBuilder.DropColumn(
                name: "SecurityStamp",
                table: "LoggedInUsers");

            migrationBuilder.DropColumn(
                name: "TwoFactorEnabled",
                table: "LoggedInUsers");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "LoggedInUsers");
        }
    }
}
