using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class RemoveNoNeedUserGroupItem_Entity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpUserGroupMembers_AbpUsers_MemberId",
                table: "CarlErpUserGroupMembers");

            migrationBuilder.DropTable(
                name: "CarlErpUserGroupItems");

            migrationBuilder.AddColumn<bool>(
                name: "IsDefault",
                table: "CarlErpUserGroupMembers",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpUserGroupMembers_AbpUsers_MemberId",
                table: "CarlErpUserGroupMembers",
                column: "MemberId",
                principalTable: "AbpUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpUserGroupMembers_AbpUsers_MemberId",
                table: "CarlErpUserGroupMembers");

            migrationBuilder.DropColumn(
                name: "IsDefault",
                table: "CarlErpUserGroupMembers");

            migrationBuilder.CreateTable(
                name: "CarlErpUserGroupItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    Default = table.Column<bool>(nullable: false),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: false),
                    UserGroupId = table.Column<Guid>(nullable: false),
                    UserId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpUserGroupItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpUserGroupItems_CarlErpUserGroups_UserGroupId",
                        column: x => x.UserGroupId,
                        principalTable: "CarlErpUserGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpUserGroupItems_AbpUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpUserGroupItems_UserGroupId",
                table: "CarlErpUserGroupItems",
                column: "UserGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpUserGroupItems_UserId",
                table: "CarlErpUserGroupItems",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpUserGroupItems_TenantId_CreatorUserId_UserId_Default",
                table: "CarlErpUserGroupItems",
                columns: new[] { "TenantId", "CreatorUserId", "UserId", "Default" });

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpUserGroupMembers_AbpUsers_MemberId",
                table: "CarlErpUserGroupMembers",
                column: "MemberId",
                principalTable: "AbpUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
