using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace equity_regulatory_reporting.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddLocation_MakeAddressDocumentNumberOptional : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "internal_location",
                table: "persons");

            migrationBuilder.AlterColumn<string>(
                name: "document_number",
                table: "persons",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "address",
                table: "persons",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500);

            migrationBuilder.AddColumn<Guid>(
                name: "location_id",
                table: "persons",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "locations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false),
                    department = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    province = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    district = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_locations", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_persons_location_id",
                table: "persons",
                column: "location_id");

            migrationBuilder.CreateIndex(
                name: "ix_locations_code",
                table: "locations",
                column: "code",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_persons_locations_location_id",
                table: "persons",
                column: "location_id",
                principalTable: "locations",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_persons_locations_location_id",
                table: "persons");

            migrationBuilder.DropTable(
                name: "locations");

            migrationBuilder.DropIndex(
                name: "ix_persons_location_id",
                table: "persons");

            migrationBuilder.DropColumn(
                name: "location_id",
                table: "persons");

            migrationBuilder.AlterColumn<string>(
                name: "document_number",
                table: "persons",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "address",
                table: "persons",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "internal_location",
                table: "persons",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");
        }
    }
}
