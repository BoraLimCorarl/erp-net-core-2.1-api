using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class Update_Save_Tamplete_Report : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PermissionReadWrite",
                table: "CarlErpReportTemplate",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "CarlErpGroupMemberItemTamplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    ReportTemplateId = table.Column<long>(nullable: false),
                    UserGroupId = table.Column<Guid>(nullable: true),
                    MemberUserId = table.Column<long>(nullable: true),
                    PermissionReadWrite = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpGroupMemberItemTamplates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpGroupMemberItemTamplates_AbpUsers_MemberUserId",
                        column: x => x.MemberUserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpGroupMemberItemTamplates_CarlErpReportTemplate_ReportTemplateId",
                        column: x => x.ReportTemplateId,
                        principalTable: "CarlErpReportTemplate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpGroupMemberItemTamplates_CarlErpUserGroups_UserGroupId",
                        column: x => x.UserGroupId,
                        principalTable: "CarlErpUserGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpGroupMemberItemTamplates_MemberUserId",
                table: "CarlErpGroupMemberItemTamplates",
                column: "MemberUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpGroupMemberItemTamplates_ReportTemplateId",
                table: "CarlErpGroupMemberItemTamplates",
                column: "ReportTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpGroupMemberItemTamplates_UserGroupId",
                table: "CarlErpGroupMemberItemTamplates",
                column: "UserGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpGroupMemberItemTamplates_CreatorUserId_TenantId",
                table: "CarlErpGroupMemberItemTamplates",
                columns: new[] { "CreatorUserId", "TenantId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CarlErpGroupMemberItemTamplates");

            migrationBuilder.DropColumn(
                name: "PermissionReadWrite",
                table: "CarlErpReportTemplate");
        }
    }
}
