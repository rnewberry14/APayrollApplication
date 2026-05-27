using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClearPathPayroll.Migrations
{
    public partial class AddTaxDepositRecords : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TaxDepositRecords",
                columns: table => new
                {
                    TaxDepositRecordId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    DepositDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TaxPeriodStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TaxPeriodEnd = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TaxType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Agency = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ConfirmationNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PaymentMethod = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    RecordSource = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ImportBatchId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaxDepositRecords", x => x.TaxDepositRecordId);
                    table.ForeignKey(
                        name: "FK_TaxDepositRecords_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TaxDepositRecords_ImportBatches_ImportBatchId",
                        column: x => x.ImportBatchId,
                        principalTable: "ImportBatches",
                        principalColumn: "ImportBatchId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TaxDepositRecords_CompanyId",
                table: "TaxDepositRecords",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_TaxDepositRecords_ImportBatchId",
                table: "TaxDepositRecords",
                column: "ImportBatchId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "TaxDepositRecords");
        }
    }
}
