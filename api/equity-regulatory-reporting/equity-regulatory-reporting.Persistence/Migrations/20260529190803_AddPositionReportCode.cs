using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace equity_regulatory_reporting.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPositionReportCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "report_code",
                table: "positions",
                type: "character varying(2)",
                maxLength: 2,
                nullable: true);

            // Assign temporary unique sequential codes to any pre-existing rows.
            // The seeder will overwrite these with the final canonical values on startup.
            migrationBuilder.Sql(@"
                UPDATE positions
                SET report_code = lpad(CAST(rn AS TEXT), 2, '0')
                FROM (
                    SELECT id, ROW_NUMBER() OVER (ORDER BY created_at) AS rn
                    FROM positions
                    WHERE report_code IS NULL
                ) sub
                WHERE positions.id = sub.id
            ");

            migrationBuilder.AlterColumn<string>(
                name: "report_code",
                table: "positions",
                type: "character varying(2)",
                maxLength: 2,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(2)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_positions_report_code",
                table: "positions",
                column: "report_code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_positions_report_code",
                table: "positions");

            migrationBuilder.DropColumn(
                name: "report_code",
                table: "positions");
        }
    }
}
