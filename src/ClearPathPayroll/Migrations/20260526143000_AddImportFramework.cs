using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClearPathPayroll.Migrations
{
    public partial class AddImportFramework : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ImportBatches",
                columns: table => new
                {
                    ImportBatchId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ImportType = table.Column<int>(type: "int", nullable: false),
                    FileFormat = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(260)", maxLength: 260, nullable: false),
                    RowCount = table.Column<int>(type: "int", nullable: false),
                    ValidRowCount = table.Column<int>(type: "int", nullable: false),
                    ErrorCount = table.Column<int>(type: "int", nullable: false),
                    DemoModeWarningAcknowledged = table.Column<bool>(type: "bit", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ConfirmedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImportBatches", x => x.ImportBatchId);
                });

            migrationBuilder.CreateTable(
                name: "ImportMappings",
                columns: table => new
                {
                    ImportMappingId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ImportBatchId = table.Column<int>(type: "int", nullable: false),
                    SourceColumn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TargetField = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImportMappings", x => x.ImportMappingId);
                    table.ForeignKey(
                        name: "FK_ImportMappings_ImportBatches_ImportBatchId",
                        column: x => x.ImportBatchId,
                        principalTable: "ImportBatches",
                        principalColumn: "ImportBatchId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ImportRows",
                columns: table => new
                {
                    ImportRowId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ImportBatchId = table.Column<int>(type: "int", nullable: false),
                    RowNumber = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    RowDataJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ErrorSummary = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    ImportedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImportRows", x => x.ImportRowId);
                    table.ForeignKey(
                        name: "FK_ImportRows_ImportBatches_ImportBatchId",
                        column: x => x.ImportBatchId,
                        principalTable: "ImportBatches",
                        principalColumn: "ImportBatchId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ImportErrors",
                columns: table => new
                {
                    ImportErrorId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ImportBatchId = table.Column<int>(type: "int", nullable: false),
                    ImportRowId = table.Column<int>(type: "int", nullable: true),
                    RowNumber = table.Column<int>(type: "int", nullable: true),
                    ColumnName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ErrorCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImportErrors", x => x.ImportErrorId);
                    table.ForeignKey(
                        name: "FK_ImportErrors_ImportBatches_ImportBatchId",
                        column: x => x.ImportBatchId,
                        principalTable: "ImportBatches",
                        principalColumn: "ImportBatchId");
                    table.ForeignKey(
                        name: "FK_ImportErrors_ImportRows_ImportRowId",
                        column: x => x.ImportRowId,
                        principalTable: "ImportRows",
                        principalColumn: "ImportRowId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ImportErrors_ImportBatchId",
                table: "ImportErrors",
                column: "ImportBatchId");

            migrationBuilder.CreateIndex(
                name: "IX_ImportErrors_ImportRowId",
                table: "ImportErrors",
                column: "ImportRowId");

            migrationBuilder.CreateIndex(
                name: "IX_ImportMappings_ImportBatchId",
                table: "ImportMappings",
                column: "ImportBatchId");

            migrationBuilder.CreateIndex(
                name: "IX_ImportRows_ImportBatchId",
                table: "ImportRows",
                column: "ImportBatchId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "ImportErrors");
            migrationBuilder.DropTable(name: "ImportMappings");
            migrationBuilder.DropTable(name: "ImportRows");
            migrationBuilder.DropTable(name: "ImportBatches");
        }
    }
}
