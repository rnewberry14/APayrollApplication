using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClearPathPayroll.Migrations
{
    public partial class AddW2PdfImportModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "W2ImportBatches",
                columns: table => new
                {
                    W2ImportBatchId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(type: "int", nullable: true),
                    FileName = table.Column<string>(type: "nvarchar(260)", maxLength: 260, nullable: false),
                    ImportedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ImportedByUserId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ImportStatus = table.Column<int>(type: "int", nullable: false),
                    RecordCount = table.Column<int>(type: "int", nullable: false),
                    ErrorCount = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_W2ImportBatches", x => x.W2ImportBatchId);
                    table.ForeignKey(
                        name: "FK_W2ImportBatches_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "W2ImportRecords",
                columns: table => new
                {
                    W2ImportRecordId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    W2ImportBatchId = table.Column<int>(type: "int", nullable: false),
                    TaxYear = table.Column<int>(type: "int", nullable: true),
                    EmployerName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    EmployerEINLast4Only = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    EmployerAddress = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    EmployeeFirstName = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    EmployeeMiddleInitial = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: true),
                    EmployeeLastName = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    EmployeeSSNLast4Only = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    EmployeeAddress = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Box1WagesTipsOtherCompensation = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Box2FederalIncomeTaxWithheld = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Box3SocialSecurityWages = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Box4SocialSecurityTaxWithheld = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Box5MedicareWagesAndTips = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Box6MedicareTaxWithheld = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Box7SocialSecurityTips = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Box8AllocatedTips = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Box10DependentCareBenefits = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Box11NonqualifiedPlans = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Box12CodeA = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: true),
                    Box12AmountA = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Box12CodeB = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: true),
                    Box12AmountB = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Box12CodeC = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: true),
                    Box12AmountC = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Box12CodeD = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: true),
                    Box12AmountD = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Box13StatutoryEmployee = table.Column<bool>(type: "bit", nullable: false),
                    Box13RetirementPlan = table.Column<bool>(type: "bit", nullable: false),
                    Box13ThirdPartySickPay = table.Column<bool>(type: "bit", nullable: false),
                    Box14OtherDescription1 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Box14OtherAmount1 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Box14OtherDescription2 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Box14OtherAmount2 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Box14OtherDescription3 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Box14OtherAmount3 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    State1 = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: true),
                    EmployerStateId1Masked = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    StateWages1 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    StateIncomeTax1 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    LocalityName1 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    LocalWages1 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    LocalIncomeTax1 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    State2 = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: true),
                    EmployerStateId2Masked = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    StateWages2 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    StateIncomeTax2 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    LocalityName2 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    LocalWages2 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    LocalIncomeTax2 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    RawExtractedTextPreview = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    ConfidenceScore = table.Column<decimal>(type: "decimal(5,4)", nullable: false),
                    NeedsManualReview = table.Column<bool>(type: "bit", nullable: false),
                    UserConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    ReviewNotes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_W2ImportRecords", x => x.W2ImportRecordId);
                    table.ForeignKey(
                        name: "FK_W2ImportRecords_W2ImportBatches_W2ImportBatchId",
                        column: x => x.W2ImportBatchId,
                        principalTable: "W2ImportBatches",
                        principalColumn: "W2ImportBatchId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "W2ImportErrors",
                columns: table => new
                {
                    W2ImportErrorId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    W2ImportBatchId = table.Column<int>(type: "int", nullable: false),
                    W2ImportRecordId = table.Column<int>(type: "int", nullable: true),
                    Severity = table.Column<int>(type: "int", nullable: false),
                    FieldName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_W2ImportErrors", x => x.W2ImportErrorId);
                    table.ForeignKey(
                        name: "FK_W2ImportErrors_W2ImportBatches_W2ImportBatchId",
                        column: x => x.W2ImportBatchId,
                        principalTable: "W2ImportBatches",
                        principalColumn: "W2ImportBatchId");
                    table.ForeignKey(
                        name: "FK_W2ImportErrors_W2ImportRecords_W2ImportRecordId",
                        column: x => x.W2ImportRecordId,
                        principalTable: "W2ImportRecords",
                        principalColumn: "W2ImportRecordId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_W2ImportBatches_CompanyId",
                table: "W2ImportBatches",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_W2ImportErrors_W2ImportBatchId",
                table: "W2ImportErrors",
                column: "W2ImportBatchId");

            migrationBuilder.CreateIndex(
                name: "IX_W2ImportErrors_W2ImportRecordId",
                table: "W2ImportErrors",
                column: "W2ImportRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_W2ImportRecords_W2ImportBatchId",
                table: "W2ImportRecords",
                column: "W2ImportBatchId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "W2ImportErrors");
            migrationBuilder.DropTable(name: "W2ImportRecords");
            migrationBuilder.DropTable(name: "W2ImportBatches");
        }
    }
}
