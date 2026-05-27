using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClearPathPayroll.Migrations
{
    public partial class AddEmployerSetupAndPayrollItems : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "County",
                table: "Companies",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DepositSchedulePlaceholder",
                table: "Companies",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EmployerTaxEffectiveDate",
                table: "Companies",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmployerTaxNotes",
                table: "Companies",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FilingFrequencyPlaceholder",
                table: "Companies",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "FutaRatePlaceholder",
                table: "Companies",
                type: "decimal(9,4)",
                precision: 9,
                scale: 4,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LocalTaxAccountNumberPlaceholder",
                table: "Companies",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LocalTaxLocalityPlaceholder",
                table: "Companies",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StateWithholdingAccountNumberPlaceholder",
                table: "Companies",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SutaEmployerAccountNumberPlaceholder",
                table: "Companies",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SutaRate",
                table: "Companies",
                type: "decimal(9,4)",
                precision: 9,
                scale: 4,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SutaState",
                table: "Companies",
                type: "nvarchar(2)",
                maxLength: 2,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PayrollItems",
                columns: table => new
                {
                    PayrollItemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    ItemCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ItemName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ItemType = table.Column<int>(type: "int", nullable: false),
                    CalculationType = table.Column<int>(type: "int", nullable: false),
                    DefaultAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    DefaultRate = table.Column<decimal>(type: "decimal(9,4)", precision: 9, scale: 4, nullable: true),
                    IsTaxableFederal = table.Column<bool>(type: "bit", nullable: false),
                    IsTaxableState = table.Column<bool>(type: "bit", nullable: false),
                    IsTaxableLocal = table.Column<bool>(type: "bit", nullable: false),
                    IsSubjectToSocialSecurity = table.Column<bool>(type: "bit", nullable: false),
                    IsSubjectToMedicare = table.Column<bool>(type: "bit", nullable: false),
                    IsPretaxDeduction = table.Column<bool>(type: "bit", nullable: false),
                    IsPosttaxDeduction = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayrollItems", x => x.PayrollItemId);
                    table.ForeignKey(
                        name: "FK_PayrollItems_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PayrollItems_CompanyId_ItemCode",
                table: "PayrollItems",
                columns: new[] { "CompanyId", "ItemCode" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "PayrollItems");

            migrationBuilder.DropColumn(name: "County", table: "Companies");
            migrationBuilder.DropColumn(name: "DepositSchedulePlaceholder", table: "Companies");
            migrationBuilder.DropColumn(name: "EmployerTaxEffectiveDate", table: "Companies");
            migrationBuilder.DropColumn(name: "EmployerTaxNotes", table: "Companies");
            migrationBuilder.DropColumn(name: "FilingFrequencyPlaceholder", table: "Companies");
            migrationBuilder.DropColumn(name: "FutaRatePlaceholder", table: "Companies");
            migrationBuilder.DropColumn(name: "LocalTaxAccountNumberPlaceholder", table: "Companies");
            migrationBuilder.DropColumn(name: "LocalTaxLocalityPlaceholder", table: "Companies");
            migrationBuilder.DropColumn(name: "StateWithholdingAccountNumberPlaceholder", table: "Companies");
            migrationBuilder.DropColumn(name: "SutaEmployerAccountNumberPlaceholder", table: "Companies");
            migrationBuilder.DropColumn(name: "SutaRate", table: "Companies");
            migrationBuilder.DropColumn(name: "SutaState", table: "Companies");
        }
    }
}
