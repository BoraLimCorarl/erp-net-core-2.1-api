using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class AddNewTable_CustomerGroups_Items : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Member",
                table: "CarlErpCustomers",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "CarlErpCustomerGroups",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: false),
                    CustomerId = table.Column<Guid>(nullable: false),
                    UserGroupId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpCustomerGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpCustomerGroups_CarlErpCustomers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "CarlErpCustomers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CarlErpCustomerGroups_CarlErpUserGroups_UserGroupId",
                        column: x => x.UserGroupId,
                        principalTable: "CarlErpUserGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpCustomerGroups_CustomerId",
                table: "CarlErpCustomerGroups",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpCustomerGroups_UserGroupId",
                table: "CarlErpCustomerGroups",
                column: "UserGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpCustomerGroups_TenantId_CreatorUserId_CustomerId_UserGroupId",
                table: "CarlErpCustomerGroups",
                columns: new[] { "TenantId", "CreatorUserId", "CustomerId", "UserGroupId" });

            if (ActiveProvider == "Microsoft.EntityFrameworkCore.SqlServer")
            {
                // update set default of member of CarlErpVendors to 1 which mean all 
                migrationBuilder.Sql(
                @"
                    UPDATE CarlErpCustomers
                    SET Member = 1
                ");
            }
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CarlErpCustomerGroups");

            migrationBuilder.DropColumn(
                name: "Member",
                table: "CarlErpCustomers");
        }
    }
}
