using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClearPathPayroll.Migrations
{
    public partial class AddOfficialSourceReviewTracking : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OfficialSourceDocuments",
                columns: table => new
                {
                    OfficialSourceDocumentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SourceName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PublicationTitle = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    RevisionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RetrievedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OfficialSourceUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    ExcerptText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsCurrent = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OfficialSourceDocuments", x => x.OfficialSourceDocumentId);
                });

            migrationBuilder.CreateTable(
                name: "OfficialSourceReviewLogs",
                columns: table => new
                {
                    OfficialSourceReviewLogId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OfficialSourceDocumentId = table.Column<int>(type: "int", nullable: false),
                    ReviewedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReviewedByUserId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Note = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OfficialSourceReviewLogs", x => x.OfficialSourceReviewLogId);
                    table.ForeignKey(
                        name: "FK_OfficialSourceReviewLogs_OfficialSourceDocuments_OfficialSourceDocumentId",
                        column: x => x.OfficialSourceDocumentId,
                        principalTable: "OfficialSourceDocuments",
                        principalColumn: "OfficialSourceDocumentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OfficialSourceReviewLogs_OfficialSourceDocumentId",
                table: "OfficialSourceReviewLogs",
                column: "OfficialSourceDocumentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OfficialSourceReviewLogs");

            migrationBuilder.DropTable(
                name: "OfficialSourceDocuments");
        }
    }
}
