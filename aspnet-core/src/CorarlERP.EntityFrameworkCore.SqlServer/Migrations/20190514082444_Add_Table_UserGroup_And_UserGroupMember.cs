using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class Add_Table_UserGroup_And_UserGroupMember : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CarlErpUserGroups",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpUserGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpUserGroupMembers",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    MemberId = table.Column<long>(nullable: false),
                    UserGroupId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpUserGroupMembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpUserGroupMembers_AbpUsers_MemberId",
                        column: x => x.MemberId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpUserGroupMembers_CarlErpUserGroups_UserGroupId",
                        column: x => x.UserGroupId,
                        principalTable: "CarlErpUserGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpUserGroupMembers_MemberId",
                table: "CarlErpUserGroupMembers",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpUserGroupMembers_UserGroupId",
                table: "CarlErpUserGroupMembers",
                column: "UserGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpUserGroupMembers_TenantId_CreatorUserId",
                table: "CarlErpUserGroupMembers",
                columns: new[] { "TenantId", "CreatorUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpUserGroups_TenantId_CreatorUserId_Name",
                table: "CarlErpUserGroups",
                columns: new[] { "TenantId", "CreatorUserId", "Name" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CarlErpUserGroupMembers");

            migrationBuilder.DropTable(
                name: "CarlErpUserGroups");
        }
    }
}
