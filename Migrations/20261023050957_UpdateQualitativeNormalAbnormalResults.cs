using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NMBHLabSys.Migrations
{
    /// <summary>
    /// Safe replacement for an unapplied migration that mixed qualitative seed changes
    /// with unrelated, potentially destructive schema and verification updates.
    /// This migration intentionally changes no database data or schema.
    /// </summary>
    public partial class UpdateQualitativeNormalAbnormalResults : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Intentionally empty. The original generated migration attempted to:
            // - add IsVerified, which already exists in the database;
            // - remove a TestTypes column and foreign key;
            // - reset verification data on existing test assignments;
            // - alter unrelated tables and seed results.
            // Do not update patient results or verified findings as part of a schema repair.
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // No changes to reverse.
        }
    }
}
