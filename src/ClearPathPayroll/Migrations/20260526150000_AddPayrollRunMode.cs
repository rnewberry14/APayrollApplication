using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClearPathPayroll.Migrations
{
    public partial class AddPayrollRunMode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PayrollMode",
                table: "PayrollRuns",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "Regular payroll");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PayrollMode",
                table: "PayrollRuns");
        }
    }
}
