using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class CreateNewTable_ReportTemplate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CarlErpReportTemplate",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    ReportType = table.Column<int>(nullable: false),
                    TemplateName = table.Column<string>(nullable: true),
                    HeaderTitle = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    IsDefault = table.Column<bool>(nullable: false),
                    Sortby = table.Column<string>(nullable: true),
                    Groupby = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpReportTemplate", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpReportColumnTemplate",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    ColumnName = table.Column<string>(nullable: true),
                    ColumnTitle = table.Column<string>(nullable: true),
                    SortOrder = table.Column<int>(nullable: false),
                    Visible = table.Column<bool>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    AllowGroupby = table.Column<bool>(nullable: false),
                    AllowFilter = table.Column<bool>(nullable: false),
                    ColumnLength = table.Column<int>(nullable: false),
                    ColumnType = table.Column<int>(nullable: false),
                    AllowFunction = table.Column<string>(nullable: true),
                    ReportTemplateId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpReportColumnTemplate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpReportColumnTemplate_CarlErpReportTemplate_ReportTemplateId",
                        column: x => x.ReportTemplateId,
                        principalTable: "CarlErpReportTemplate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpReportFilterTemplate",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    FilterName = table.Column<string>(nullable: true),
                    FilterValue = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    SortOrder = table.Column<int>(nullable: false),
                    Visible = table.Column<bool>(nullable: false),
                    ReportTemplateId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpReportFilterTemplate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpReportFilterTemplate_CarlErpReportTemplate_ReportTemplateId",
                        column: x => x.ReportTemplateId,
                        principalTable: "CarlErpReportTemplate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpReportColumnTemplate_ReportTemplateId",
                table: "CarlErpReportColumnTemplate",
                column: "ReportTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpReportFilterTemplate_ReportTemplateId",
                table: "CarlErpReportFilterTemplate",
                column: "ReportTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpReportTemplate_CreatorUserId_TenantId",
                table: "CarlErpReportTemplate",
                columns: new[] { "CreatorUserId", "TenantId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CarlErpReportColumnTemplate");

            migrationBuilder.DropTable(
                name: "CarlErpReportFilterTemplate");

            migrationBuilder.DropTable(
                name: "CarlErpReportTemplate");
        }
    }
}
