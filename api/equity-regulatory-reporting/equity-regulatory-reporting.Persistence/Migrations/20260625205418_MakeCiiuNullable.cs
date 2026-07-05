using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace equity_regulatory_reporting.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MakeCiiuNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ciiu",
                table: "persons",
                type: "character varying(10)",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character(10)",
                oldFixedLength: true,
                oldMaxLength: 10);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ciiu",
                table: "persons",
                type: "character(10)",
                fixedLength: true,
                maxLength: 10,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(10)",
                oldMaxLength: 10,
                oldNullable: true);
        }
    }
}
