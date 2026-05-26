using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClearPathPayroll.Migrations
{
    /// <inheritdoc />
    public partial class AddPrototypeDirectDepositAndAuditEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EmployeeId",
                table: "EmployerTaxLines",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AuditLogEntries",
                columns: table => new
                {
                    AuditLogEntryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PayrollRunId = table.Column<int>(type: "int", nullable: true),
                    EventType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogEntries", x => x.AuditLogEntryId);
                    table.ForeignKey(
                        name: "FK_AuditLogEntries_PayrollRuns_PayrollRunId",
                        column: x => x.PayrollRunId,
                        principalTable: "PayrollRuns",
                        principalColumn: "PayrollRunId");
                });

            migrationBuilder.CreateTable(
                name: "CompanyFundingAccounts",
                columns: table => new
                {
                    CompanyFundingAccountId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    BankName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AccountType = table.Column<int>(type: "int", nullable: false),
                    RoutingNumberToken = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    AccountNumberToken = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Last4 = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyFundingAccounts", x => x.CompanyFundingAccountId);
                    table.ForeignKey(
                        name: "FK_CompanyFundingAccounts_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeBankAccounts",
                columns: table => new
                {
                    EmployeeBankAccountId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    BankName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AccountType = table.Column<int>(type: "int", nullable: false),
                    RoutingNumberToken = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    AccountNumberToken = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Last4 = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    VerificationStatus = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeBankAccounts", x => x.EmployeeBankAccountId);
                    table.ForeignKey(
                        name: "FK_EmployeeBankAccounts_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "DirectDepositBatches",
                columns: table => new
                {
                    BatchId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PayrollRunId = table.Column<int>(type: "int", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    CompanyFundingAccountId = table.Column<int>(type: "int", nullable: false),
                    PayDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SettledAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExternalBatchReference = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ErrorMessage = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SubmittedByUserId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DirectDepositBatches", x => x.BatchId);
                    table.ForeignKey(
                        name: "FK_DirectDepositBatches_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_DirectDepositBatches_CompanyFundingAccounts_CompanyFundingAccountId",
                        column: x => x.CompanyFundingAccountId,
                        principalTable: "CompanyFundingAccounts",
                        principalColumn: "CompanyFundingAccountId",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_DirectDepositBatches_PayrollRuns_PayrollRunId",
                        column: x => x.PayrollRunId,
                        principalTable: "PayrollRuns",
                        principalColumn: "PayrollRunId",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "DirectDepositItems",
                columns: table => new
                {
                    BatchItemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BatchId = table.Column<int>(type: "int", nullable: false),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    EmployeeBankAccountId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ExternalItemReference = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ReturnCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    ReturnDescription = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TransmittedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SettledAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DirectDepositItems", x => x.BatchItemId);
                    table.ForeignKey(
                        name: "FK_DirectDepositItems_DirectDepositBatches_BatchId",
                        column: x => x.BatchId,
                        principalTable: "DirectDepositBatches",
                        principalColumn: "BatchId",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_DirectDepositItems_EmployeeBankAccounts_EmployeeBankAccountId",
                        column: x => x.EmployeeBankAccountId,
                        principalTable: "EmployeeBankAccounts",
                        principalColumn: "EmployeeBankAccountId",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_DirectDepositItems_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmployerTaxLines_EmployeeId",
                table: "EmployerTaxLines",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogEntries_PayrollRunId",
                table: "AuditLogEntries",
                column: "PayrollRunId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyFundingAccounts_CompanyId",
                table: "CompanyFundingAccounts",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_DirectDepositBatches_CompanyFundingAccountId",
                table: "DirectDepositBatches",
                column: "CompanyFundingAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_DirectDepositBatches_CompanyId",
                table: "DirectDepositBatches",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_DirectDepositBatches_PayrollRunId",
                table: "DirectDepositBatches",
                column: "PayrollRunId");

            migrationBuilder.CreateIndex(
                name: "IX_DirectDepositItems_BatchId",
                table: "DirectDepositItems",
                column: "BatchId");

            migrationBuilder.CreateIndex(
                name: "IX_DirectDepositItems_EmployeeBankAccountId",
                table: "DirectDepositItems",
                column: "EmployeeBankAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_DirectDepositItems_EmployeeId",
                table: "DirectDepositItems",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeBankAccounts_EmployeeId",
                table: "EmployeeBankAccounts",
                column: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployerTaxLines_Employees_EmployeeId",
                table: "EmployerTaxLines",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "EmployeeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployerTaxLines_Employees_EmployeeId",
                table: "EmployerTaxLines");

            migrationBuilder.DropTable(
                name: "AuditLogEntries");

            migrationBuilder.DropTable(
                name: "DirectDepositItems");

            migrationBuilder.DropTable(
                name: "DirectDepositBatches");

            migrationBuilder.DropTable(
                name: "EmployeeBankAccounts");

            migrationBuilder.DropTable(
                name: "CompanyFundingAccounts");

            migrationBuilder.DropIndex(
                name: "IX_EmployerTaxLines_EmployeeId",
                table: "EmployerTaxLines");

            migrationBuilder.DropColumn(
                name: "EmployeeId",
                table: "EmployerTaxLines");
        }
    }
}
