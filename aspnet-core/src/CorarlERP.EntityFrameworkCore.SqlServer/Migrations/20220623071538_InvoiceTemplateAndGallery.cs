using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class InvoiceTemplateAndGallery : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CarlErpGalleries",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    Name = table.Column<string>(maxLength: 512, nullable: false),
                    FileSize = table.Column<long>(nullable: false),
                    ContentType = table.Column<string>(maxLength: 512, nullable: false),
                    StorageFilePath = table.Column<string>(maxLength: 256, nullable: false),
                    UploadFrom = table.Column<int>(nullable: false),
                    UploadSource = table.Column<int>(nullable: false),
                    StorageMainFolderName = table.Column<string>(maxLength: 512, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpGalleries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpInoviceTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    Name = table.Column<string>(maxLength: 512, nullable: false),
                    TenantId = table.Column<int>(nullable: true),
                    TemplateOption = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    GalleryId = table.Column<Guid>(nullable: false),
                    TemplateType = table.Column<int>(nullable: false),
                    ShowDetail = table.Column<bool>(nullable: false),
                    ShowSummary = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpInoviceTemplates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpInoviceTemplates_CarlErpGalleries_GalleryId",
                        column: x => x.GalleryId,
                        principalTable: "CarlErpGalleries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpInvoiceTemplateMaps",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    TemplateType = table.Column<int>(nullable: false),
                    SaleTypeId = table.Column<long>(nullable: true),
                    TemplateId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpInvoiceTemplateMaps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpInvoiceTemplateMaps_CarlErpTransactionTypes_SaleTypeId",
                        column: x => x.SaleTypeId,
                        principalTable: "CarlErpTransactionTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpInvoiceTemplateMaps_CarlErpInoviceTemplates_TemplateId",
                        column: x => x.TemplateId,
                        principalTable: "CarlErpInoviceTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpGalleries_TenantId_Name",
                table: "CarlErpGalleries",
                columns: new[] { "TenantId", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpGalleries_TenantId_UploadFrom",
                table: "CarlErpGalleries",
                columns: new[] { "TenantId", "UploadFrom" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpGalleries_TenantId_UploadSource",
                table: "CarlErpGalleries",
                columns: new[] { "TenantId", "UploadSource" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpInoviceTemplates_GalleryId",
                table: "CarlErpInoviceTemplates",
                column: "GalleryId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpInoviceTemplates_TenantId_IsActive",
                table: "CarlErpInoviceTemplates",
                columns: new[] { "TenantId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpInoviceTemplates_TenantId_Name",
                table: "CarlErpInoviceTemplates",
                columns: new[] { "TenantId", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpInoviceTemplates_TenantId_TemplateType",
                table: "CarlErpInoviceTemplates",
                columns: new[] { "TenantId", "TemplateType" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpInvoiceTemplateMaps_SaleTypeId",
                table: "CarlErpInvoiceTemplateMaps",
                column: "SaleTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpInvoiceTemplateMaps_TemplateId",
                table: "CarlErpInvoiceTemplateMaps",
                column: "TemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpInvoiceTemplateMaps_TenantId_TemplateType",
                table: "CarlErpInvoiceTemplateMaps",
                columns: new[] { "TenantId", "TemplateType" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CarlErpInvoiceTemplateMaps");

            migrationBuilder.DropTable(
                name: "CarlErpInoviceTemplates");

            migrationBuilder.DropTable(
                name: "CarlErpGalleries");
        }
    }
}
