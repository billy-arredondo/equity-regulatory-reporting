using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace equity_regulatory_reporting.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPersonLegacyIdRepresentativeDescription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "legacy_id",
                table: "persons",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "representative_description",
                table: "persons",
                type: "character varying(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_persons_legacy_id",
                table: "persons",
                column: "legacy_id",
                unique: true,
                filter: "legacy_id IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_persons_legacy_id",
                table: "persons");

            migrationBuilder.DropColumn(
                name: "legacy_id",
                table: "persons");

            migrationBuilder.DropColumn(
                name: "representative_description",
                table: "persons");
        }
    }
}
