using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class AddNewTable_ItemUserGroup_WithItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Member",
                table: "CarlErpItems",
                nullable: false,
                defaultValue: 0);

            if (ActiveProvider == "Microsoft.EntityFrameworkCore.SqlServer")
            {
                // update set default of member of CarlErpItems to 1 which mean all 
                migrationBuilder.Sql(
                @"
                    UPDATE CarlErpItems
                    SET Member = 1
                ");
            }

            migrationBuilder.CreateTable(
                name: "CarlErpItemUserGroups",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: false),
                    ItemId = table.Column<Guid>(nullable: false),
                    UserGroupId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpItemUserGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpItemUserGroups_CarlErpItems_ItemId",
                        column: x => x.ItemId,
                        principalTable: "CarlErpItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CarlErpItemUserGroups_CarlErpUserGroups_UserGroupId",
                        column: x => x.UserGroupId,
                        principalTable: "CarlErpUserGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemUserGroups_ItemId",
                table: "CarlErpItemUserGroups",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemUserGroups_UserGroupId",
                table: "CarlErpItemUserGroups",
                column: "UserGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemUserGroups_CreatorUserId_TenantId_UserGroupId_ItemId",
                table: "CarlErpItemUserGroups",
                columns: new[] { "CreatorUserId", "TenantId", "UserGroupId", "ItemId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CarlErpItemUserGroups");

            migrationBuilder.DropColumn(
                name: "Member",
                table: "CarlErpItems");
        }
    }
}
