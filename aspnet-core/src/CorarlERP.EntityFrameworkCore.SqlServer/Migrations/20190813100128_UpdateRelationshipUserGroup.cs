using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class UpdateRelationshipUserGroup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpUserGroupMembers_CarlErpUserGroups_UserGroupId",
                table: "CarlErpUserGroupMembers");

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpUserGroupMembers_CarlErpUserGroups_UserGroupId",
                table: "CarlErpUserGroupMembers",
                column: "UserGroupId",
                principalTable: "CarlErpUserGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpUserGroupMembers_CarlErpUserGroups_UserGroupId",
                table: "CarlErpUserGroupMembers");

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpUserGroupMembers_CarlErpUserGroups_UserGroupId",
                table: "CarlErpUserGroupMembers",
                column: "UserGroupId",
                principalTable: "CarlErpUserGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
