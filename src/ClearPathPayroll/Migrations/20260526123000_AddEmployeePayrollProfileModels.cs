using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClearPathPayroll.Migrations
{
    public partial class AddEmployeePayrollProfileModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>("Address1", "Employees", type: "nvarchar(200)", maxLength: 200, nullable: true);
            migrationBuilder.AddColumn<string>("Address2", "Employees", type: "nvarchar(200)", maxLength: 200, nullable: true);
            migrationBuilder.AddColumn<string>("County", "Employees", type: "nvarchar(100)", maxLength: 100, nullable: true);
            migrationBuilder.AddColumn<string>("DefaultEarningCode", "Employees", type: "nvarchar(50)", maxLength: 50, nullable: true);
            migrationBuilder.AddColumn<int>("DefaultPayFrequency", "Employees", type: "int", nullable: true);
            migrationBuilder.AddColumn<decimal>("DefaultOvertimeHours", "Employees", type: "decimal(18,2)", precision: 18, scale: 2, nullable: true);
            migrationBuilder.AddColumn<decimal>("DefaultRegularHours", "Employees", type: "decimal(18,2)", precision: 18, scale: 2, nullable: true);
            migrationBuilder.AddColumn<string>("Department", "Employees", type: "nvarchar(100)", maxLength: 100, nullable: true);
            migrationBuilder.AddColumn<decimal>("Deductions", "Employees", type: "decimal(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 0m);
            migrationBuilder.AddColumn<decimal>("DependentsAmount", "Employees", type: "decimal(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 0m);
            migrationBuilder.AddColumn<string>("EmergencyContactName", "Employees", type: "nvarchar(100)", maxLength: 100, nullable: true);
            migrationBuilder.AddColumn<string>("EmergencyContactPhone", "Employees", type: "nvarchar(25)", maxLength: 25, nullable: true);
            migrationBuilder.AddColumn<string>("EmployeeNumber", "Employees", type: "nvarchar(100)", maxLength: 100, nullable: true);
            migrationBuilder.AddColumn<bool>("ExemptFromFederalWithholding", "Employees", type: "bit", nullable: false, defaultValue: false);
            migrationBuilder.AddColumn<decimal>("ExtraWithholding", "Employees", type: "decimal(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 0m);
            migrationBuilder.AddColumn<int>("FilingStatus", "Employees", type: "int", nullable: false, defaultValue: 0);
            migrationBuilder.AddColumn<bool>("IsActive", "Employees", type: "bit", nullable: false, defaultValue: true);
            migrationBuilder.AddColumn<bool>("IsExemptFromOvertime", "Employees", type: "bit", nullable: false, defaultValue: false);
            migrationBuilder.AddColumn<bool>("IsTippedEmployee", "Employees", type: "bit", nullable: false, defaultValue: false);
            migrationBuilder.AddColumn<string>("JobTitle", "Employees", type: "nvarchar(100)", maxLength: 100, nullable: true);
            migrationBuilder.AddColumn<bool>("MinimumWageWarningEnabled", "Employees", type: "bit", nullable: false, defaultValue: true);
            migrationBuilder.AddColumn<bool>("MultipleJobsOrSpouseWorks", "Employees", type: "bit", nullable: false, defaultValue: false);
            migrationBuilder.AddColumn<decimal>("OtherIncome", "Employees", type: "decimal(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 0m);
            migrationBuilder.AddColumn<string>("PayrollNotes", "Employees", type: "nvarchar(2000)", maxLength: 2000, nullable: true);
            migrationBuilder.AddColumn<DateTime>("PayrollProfileCreatedAt", "Employees", type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()");
            migrationBuilder.AddColumn<DateTime>("PayrollProfileUpdatedAt", "Employees", type: "datetime2", nullable: true);
            migrationBuilder.AddColumn<string>("PreferredName", "Employees", type: "nvarchar(50)", maxLength: 50, nullable: true);
            migrationBuilder.AddColumn<DateTime>("RehireDate", "Employees", type: "datetime2", nullable: true);
            migrationBuilder.AddColumn<string>("StateTaxFormVersion", "Employees", type: "nvarchar(50)", maxLength: 50, nullable: true);
            migrationBuilder.AddColumn<int>("StateAllowances", "Employees", type: "int", nullable: true);
            migrationBuilder.AddColumn<decimal>("StateAdditionalWithholding", "Employees", type: "decimal(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 0m);
            migrationBuilder.AddColumn<bool>("StateExemptFromWithholding", "Employees", type: "bit", nullable: false, defaultValue: false);
            migrationBuilder.AddColumn<int>("StateFilingStatus", "Employees", type: "int", nullable: false, defaultValue: 0);
            migrationBuilder.AddColumn<DateTime>("StateTaxSignedDate", "Employees", type: "datetime2", nullable: true);
            migrationBuilder.AddColumn<string>("StateTaxNotes", "Employees", type: "nvarchar(1000)", maxLength: 1000, nullable: true);
            migrationBuilder.AddColumn<string>("StateTaxState", "Employees", type: "nvarchar(2)", maxLength: 2, nullable: true);
            migrationBuilder.AddColumn<string>("Suffix", "Employees", type: "nvarchar(20)", maxLength: 20, nullable: true);
            migrationBuilder.AddColumn<decimal>("TippedCashWageRate", "Employees", type: "decimal(18,2)", precision: 18, scale: 2, nullable: true);
            migrationBuilder.AddColumn<bool>("TipCreditAllowedPlaceholder", "Employees", type: "bit", nullable: false, defaultValue: false);
            migrationBuilder.AddColumn<DateTime>("W4SignedDate", "Employees", type: "datetime2", nullable: true);
            migrationBuilder.AddColumn<string>("W4Notes", "Employees", type: "nvarchar(1000)", maxLength: 1000, nullable: true);
            migrationBuilder.AddColumn<int>("W4Year", "Employees", type: "int", nullable: true);
            migrationBuilder.AddColumn<int>("WorkLocationId", "Employees", type: "int", nullable: true);
            migrationBuilder.AddColumn<int>("WorkerType", "Employees", type: "int", nullable: false, defaultValue: 0);

            migrationBuilder.AddColumn<int>("PriorityOrder", "EmployeeBankAccounts", type: "int", nullable: false, defaultValue: 1);
            migrationBuilder.AddColumn<int>("DepositType", "EmployeeBankAccounts", type: "int", nullable: false, defaultValue: 2);
            migrationBuilder.AddColumn<decimal>("DepositAmount", "EmployeeBankAccounts", type: "decimal(18,2)", precision: 18, scale: 2, nullable: true);
            migrationBuilder.AddColumn<decimal>("DepositPercent", "EmployeeBankAccounts", type: "decimal(5,2)", precision: 5, scale: 2, nullable: true);
            migrationBuilder.AddColumn<bool>("IsRemainderAccount", "EmployeeBankAccounts", type: "bit", nullable: false, defaultValue: true);
            migrationBuilder.AddColumn<int>("PrenoteStatus", "EmployeeBankAccounts", type: "int", nullable: false, defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "EmployeePayrollFields",
                columns: table => new
                {
                    EmployeePayrollFieldId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    FieldName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FieldValue = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeePayrollFields", x => x.EmployeePayrollFieldId);
                    table.ForeignKey(
                        name: "FK_EmployeePayrollFields_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeePayrollFields_EmployeeId",
                table: "EmployeePayrollFields",
                column: "EmployeeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "EmployeePayrollFields");

            migrationBuilder.DropColumn("Address1", "Employees");
            migrationBuilder.DropColumn("Address2", "Employees");
            migrationBuilder.DropColumn("County", "Employees");
            migrationBuilder.DropColumn("DefaultEarningCode", "Employees");
            migrationBuilder.DropColumn("DefaultPayFrequency", "Employees");
            migrationBuilder.DropColumn("DefaultOvertimeHours", "Employees");
            migrationBuilder.DropColumn("DefaultRegularHours", "Employees");
            migrationBuilder.DropColumn("Department", "Employees");
            migrationBuilder.DropColumn("Deductions", "Employees");
            migrationBuilder.DropColumn("DependentsAmount", "Employees");
            migrationBuilder.DropColumn("EmergencyContactName", "Employees");
            migrationBuilder.DropColumn("EmergencyContactPhone", "Employees");
            migrationBuilder.DropColumn("EmployeeNumber", "Employees");
            migrationBuilder.DropColumn("ExemptFromFederalWithholding", "Employees");
            migrationBuilder.DropColumn("ExtraWithholding", "Employees");
            migrationBuilder.DropColumn("FilingStatus", "Employees");
            migrationBuilder.DropColumn("IsActive", "Employees");
            migrationBuilder.DropColumn("IsExemptFromOvertime", "Employees");
            migrationBuilder.DropColumn("IsTippedEmployee", "Employees");
            migrationBuilder.DropColumn("JobTitle", "Employees");
            migrationBuilder.DropColumn("MinimumWageWarningEnabled", "Employees");
            migrationBuilder.DropColumn("MultipleJobsOrSpouseWorks", "Employees");
            migrationBuilder.DropColumn("OtherIncome", "Employees");
            migrationBuilder.DropColumn("PayrollNotes", "Employees");
            migrationBuilder.DropColumn("PayrollProfileCreatedAt", "Employees");
            migrationBuilder.DropColumn("PayrollProfileUpdatedAt", "Employees");
            migrationBuilder.DropColumn("PreferredName", "Employees");
            migrationBuilder.DropColumn("RehireDate", "Employees");
            migrationBuilder.DropColumn("StateTaxFormVersion", "Employees");
            migrationBuilder.DropColumn("StateAllowances", "Employees");
            migrationBuilder.DropColumn("StateAdditionalWithholding", "Employees");
            migrationBuilder.DropColumn("StateExemptFromWithholding", "Employees");
            migrationBuilder.DropColumn("StateFilingStatus", "Employees");
            migrationBuilder.DropColumn("StateTaxSignedDate", "Employees");
            migrationBuilder.DropColumn("StateTaxNotes", "Employees");
            migrationBuilder.DropColumn("StateTaxState", "Employees");
            migrationBuilder.DropColumn("Suffix", "Employees");
            migrationBuilder.DropColumn("TippedCashWageRate", "Employees");
            migrationBuilder.DropColumn("TipCreditAllowedPlaceholder", "Employees");
            migrationBuilder.DropColumn("W4SignedDate", "Employees");
            migrationBuilder.DropColumn("W4Notes", "Employees");
            migrationBuilder.DropColumn("W4Year", "Employees");
            migrationBuilder.DropColumn("WorkLocationId", "Employees");
            migrationBuilder.DropColumn("WorkerType", "Employees");

            migrationBuilder.DropColumn("PriorityOrder", "EmployeeBankAccounts");
            migrationBuilder.DropColumn("DepositType", "EmployeeBankAccounts");
            migrationBuilder.DropColumn("DepositAmount", "EmployeeBankAccounts");
            migrationBuilder.DropColumn("DepositPercent", "EmployeeBankAccounts");
            migrationBuilder.DropColumn("IsRemainderAccount", "EmployeeBankAccounts");
            migrationBuilder.DropColumn("PrenoteStatus", "EmployeeBankAccounts");
        }
    }
}
