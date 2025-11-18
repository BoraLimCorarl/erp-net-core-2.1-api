using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class UpdateLocationWithShareGroup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CarlErpLocationGroups");

            migrationBuilder.AddColumn<long>(
                name: "LocationId",
                table: "CarlErpUserGroups",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpUserGroups_LocationId",
                table: "CarlErpUserGroups",
                column: "LocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpUserGroups_CarlErpLocations_LocationId",
                table: "CarlErpUserGroups",
                column: "LocationId",
                principalTable: "CarlErpLocations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpUserGroups_CarlErpLocations_LocationId",
                table: "CarlErpUserGroups");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpUserGroups_LocationId",
                table: "CarlErpUserGroups");

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "CarlErpUserGroups");

            migrationBuilder.CreateTable(
                name: "CarlErpLocationGroups",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    LocationId = table.Column<long>(nullable: false),
                    TenantId = table.Column<int>(nullable: false),
                    UserGroupId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpLocationGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpLocationGroups_CarlErpLocations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "CarlErpLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CarlErpLocationGroups_CarlErpUserGroups_UserGroupId",
                        column: x => x.UserGroupId,
                        principalTable: "CarlErpUserGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpLocationGroups_LocationId",
                table: "CarlErpLocationGroups",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpLocationGroups_UserGroupId",
                table: "CarlErpLocationGroups",
                column: "UserGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpLocationGroups_TenantId_CreatorUserId_LocationId",
                table: "CarlErpLocationGroups",
                columns: new[] { "TenantId", "CreatorUserId", "LocationId" });
        }
    }
}
