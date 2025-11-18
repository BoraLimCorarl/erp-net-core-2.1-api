using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class CashFlowTemplate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CarlErpCashFlowAccountGroups",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    SortOrder = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 512, nullable: false),
                    Description = table.Column<string>(nullable: true),
                    TenantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpCashFlowAccountGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpCashFlowCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    SortOrder = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 512, nullable: false),
                    Description = table.Column<string>(nullable: true),
                    TenantId = table.Column<int>(nullable: false),
                    Type = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpCashFlowCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpCashFlowTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    Name = table.Column<string>(maxLength: 512, nullable: false),
                    Description = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    IsDefault = table.Column<bool>(nullable: false),
                    TenantId = table.Column<int>(nullable: false),
                    SplitCashInAndCashOutFlow = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpCashFlowTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpCashFlowTemplateAccounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TemplateId = table.Column<Guid>(nullable: false),
                    CategoryId = table.Column<Guid>(nullable: false),
                    AccountId = table.Column<Guid>(nullable: false),
                    AccountGroupId = table.Column<Guid>(nullable: true),
                    TenantId = table.Column<int>(nullable: false),
                    CashInFlow = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpCashFlowTemplateAccounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpCashFlowTemplateAccounts_CarlErpCashFlowAccountGroups_AccountGroupId",
                        column: x => x.AccountGroupId,
                        principalTable: "CarlErpCashFlowAccountGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpCashFlowTemplateAccounts_CarlErpChartOfAccounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "CarlErpChartOfAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpCashFlowTemplateAccounts_CarlErpCashFlowCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "CarlErpCashFlowCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpCashFlowTemplateAccounts_CarlErpCashFlowTemplates_TemplateId",
                        column: x => x.TemplateId,
                        principalTable: "CarlErpCashFlowTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpCashFlowTemplateCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TemplateId = table.Column<Guid>(nullable: false),
                    CategoryId = table.Column<Guid>(nullable: false),
                    TenantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpCashFlowTemplateCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpCashFlowTemplateCategories_CarlErpCashFlowCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "CarlErpCashFlowCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpCashFlowTemplateCategories_CarlErpCashFlowTemplates_TemplateId",
                        column: x => x.TemplateId,
                        principalTable: "CarlErpCashFlowTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpCashFlowAccountGroups_Name",
                table: "CarlErpCashFlowAccountGroups",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpCashFlowAccountGroups_SortOrder",
                table: "CarlErpCashFlowAccountGroups",
                column: "SortOrder");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpCashFlowCategories_Name",
                table: "CarlErpCashFlowCategories",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpCashFlowCategories_SortOrder",
                table: "CarlErpCashFlowCategories",
                column: "SortOrder");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpCashFlowCategories_Type",
                table: "CarlErpCashFlowCategories",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpCashFlowTemplateAccounts_AccountGroupId",
                table: "CarlErpCashFlowTemplateAccounts",
                column: "AccountGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpCashFlowTemplateAccounts_AccountId",
                table: "CarlErpCashFlowTemplateAccounts",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpCashFlowTemplateAccounts_CategoryId",
                table: "CarlErpCashFlowTemplateAccounts",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpCashFlowTemplateAccounts_TemplateId",
                table: "CarlErpCashFlowTemplateAccounts",
                column: "TemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpCashFlowTemplateCategories_CategoryId",
                table: "CarlErpCashFlowTemplateCategories",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpCashFlowTemplateCategories_TemplateId",
                table: "CarlErpCashFlowTemplateCategories",
                column: "TemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpCashFlowTemplates_IsActive",
                table: "CarlErpCashFlowTemplates",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpCashFlowTemplates_IsDefault",
                table: "CarlErpCashFlowTemplates",
                column: "IsDefault");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpCashFlowTemplates_Name",
                table: "CarlErpCashFlowTemplates",
                column: "Name");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CarlErpCashFlowTemplateAccounts");

            migrationBuilder.DropTable(
                name: "CarlErpCashFlowTemplateCategories");

            migrationBuilder.DropTable(
                name: "CarlErpCashFlowAccountGroups");

            migrationBuilder.DropTable(
                name: "CarlErpCashFlowCategories");

            migrationBuilder.DropTable(
                name: "CarlErpCashFlowTemplates");
        }
    }
}
