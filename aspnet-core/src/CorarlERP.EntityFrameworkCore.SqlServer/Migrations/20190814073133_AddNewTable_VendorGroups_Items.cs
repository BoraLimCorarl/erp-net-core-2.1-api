using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class AddNewTable_VendorGroups_Items : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Member",
                table: "CarlErpVendors",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "CarlErpVendorGroups",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: false),
                    VendorId = table.Column<Guid>(nullable: false),
                    UserGroupId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpVendorGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpVendorGroups_CarlErpUserGroups_UserGroupId",
                        column: x => x.UserGroupId,
                        principalTable: "CarlErpUserGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CarlErpVendorGroups_CarlErpVendors_VendorId",
                        column: x => x.VendorId,
                        principalTable: "CarlErpVendors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpVendorGroups_UserGroupId",
                table: "CarlErpVendorGroups",
                column: "UserGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpVendorGroups_VendorId",
                table: "CarlErpVendorGroups",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpVendorGroups_TenantId_CreatorUserId_VendorId",
                table: "CarlErpVendorGroups",
                columns: new[] { "TenantId", "CreatorUserId", "VendorId" });

            if (ActiveProvider == "Microsoft.EntityFrameworkCore.SqlServer")
            {
                // update set default of member of CarlErpVendors to 1 which mean all 
                migrationBuilder.Sql(
                @"
                    UPDATE CarlErpVendors
                    SET Member = 1
                ");
            }
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CarlErpVendorGroups");

            migrationBuilder.DropColumn(
                name: "Member",
                table: "CarlErpVendors");
        }
    }
}
