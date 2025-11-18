using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class CustomerTypeMembers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CarlErpCustomerTypeMembers",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    MemberId = table.Column<long>(nullable: false),
                    CustomerTypeId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpCustomerTypeMembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpCustomerTypeMembers_CarlErpCustomerTypes_CustomerTypeId",
                        column: x => x.CustomerTypeId,
                        principalTable: "CarlErpCustomerTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpCustomerTypeMembers_AbpUsers_MemberId",
                        column: x => x.MemberId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpCustomerTypeMembers_CustomerTypeId",
                table: "CarlErpCustomerTypeMembers",
                column: "CustomerTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpCustomerTypeMembers_MemberId",
                table: "CarlErpCustomerTypeMembers",
                column: "MemberId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CarlErpCustomerTypeMembers");
        }
    }
}
