using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClearPathPayroll.Migrations
{
    public partial class AddEmployerFieldFormatting : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "LocalEmployerTaxRate",
                table: "Companies",
                type: "decimal(9,4)",
                precision: 9,
                scale: 4,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SEIN",
                table: "Companies",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SUIN",
                table: "Companies",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LocalEmployerTaxRate",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "SEIN",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "SUIN",
                table: "Companies");
        }
    }
}
