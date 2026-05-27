using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClearPathPayroll.Migrations
{
    public partial class AddW2HistoricalRecords : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "W2HistoricalRecords",
                columns: table => new
                {
                    W2HistoricalRecordId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TaxYear = table.Column<int>(type: "int", nullable: false),
                    EmployerName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    EmployerEINMasked = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    EmployerAddress = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    EmployeeFirstName = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    EmployeeLastName = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    EmployeeSSNLast4 = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    EmployeeAddress = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Box1Wages = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Box2FederalTaxWithheld = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Box3SocialSecurityWages = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Box4SocialSecurityTaxWithheld = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Box5MedicareWages = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Box6MedicareTaxWithheld = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Box12CodeAndAmountPlaceholders = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Box14DescriptionAndAmountPlaceholders = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    StateWages = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    StateTaxWithheld = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    LocalWages = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    LocalTaxWithheld = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    RecordSource = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ImportBatchId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_W2HistoricalRecords", x => x.W2HistoricalRecordId);
                    table.ForeignKey(
                        name: "FK_W2HistoricalRecords_ImportBatches_ImportBatchId",
                        column: x => x.ImportBatchId,
                        principalTable: "ImportBatches",
                        principalColumn: "ImportBatchId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_W2HistoricalRecords_ImportBatchId",
                table: "W2HistoricalRecords",
                column: "ImportBatchId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "W2HistoricalRecords");
        }
    }
}
