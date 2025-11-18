using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class AddVendorTypeMembers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CarlErpVendorTypeMembers",
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
                    VendorTypeId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpVendorTypeMembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpVendorTypeMembers_AbpUsers_MemberId",
                        column: x => x.MemberId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpVendorTypeMembers_CarlErpVendorTypes_VendorTypeId",
                        column: x => x.VendorTypeId,
                        principalTable: "CarlErpVendorTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpVendorTypeMembers_MemberId",
                table: "CarlErpVendorTypeMembers",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpVendorTypeMembers_VendorTypeId",
                table: "CarlErpVendorTypeMembers",
                column: "VendorTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CarlErpVendorTypeMembers");
        }
    }
}
