using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClearPathPayroll.Migrations
{
    public partial class AddUserDefinedPayrollFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserDefinedFieldDefinitions",
                columns: table => new
                {
                    UserDefinedFieldDefinitionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    FieldName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FieldCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AppliesTo = table.Column<int>(type: "int", nullable: false),
                    DataType = table.Column<int>(type: "int", nullable: false),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false),
                    DefaultValue = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ListOptions = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    IncludeInPayrollCalculation = table.Column<bool>(type: "bit", nullable: false),
                    CalculationRole = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDefinedFieldDefinitions", x => x.UserDefinedFieldDefinitionId);
                    table.ForeignKey(
                        name: "FK_UserDefinedFieldDefinitions_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserDefinedFieldValues",
                columns: table => new
                {
                    UserDefinedFieldValueId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FieldDefinitionId = table.Column<int>(type: "int", nullable: false),
                    EntityType = table.Column<int>(type: "int", nullable: false),
                    EntityId = table.Column<int>(type: "int", nullable: false),
                    TextValue = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    NumberValue = table.Column<int>(type: "int", nullable: true),
                    DecimalValue = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    DateValue = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BooleanValue = table.Column<bool>(type: "bit", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDefinedFieldValues", x => x.UserDefinedFieldValueId);
                    table.ForeignKey(
                        name: "FK_UserDefinedFieldValues_UserDefinedFieldDefinitions_FieldDefinitionId",
                        column: x => x.FieldDefinitionId,
                        principalTable: "UserDefinedFieldDefinitions",
                        principalColumn: "UserDefinedFieldDefinitionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserDefinedFieldDefinitions_CompanyId_FieldCode",
                table: "UserDefinedFieldDefinitions",
                columns: new[] { "CompanyId", "FieldCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserDefinedFieldValues_FieldDefinitionId_EntityType_EntityId",
                table: "UserDefinedFieldValues",
                columns: new[] { "FieldDefinitionId", "EntityType", "EntityId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "UserDefinedFieldValues");
            migrationBuilder.DropTable(name: "UserDefinedFieldDefinitions");
        }
    }
}
