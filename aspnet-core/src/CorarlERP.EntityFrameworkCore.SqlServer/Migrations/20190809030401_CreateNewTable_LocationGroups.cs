using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class CreateNewTable_LocationGroups : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Member",
                table: "CarlErpLocations",
                nullable: false,
                defaultValue: 0);

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
                    TenantId = table.Column<int>(nullable: false),
                    LocationId = table.Column<long>(nullable: false),
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



            if (ActiveProvider == "Microsoft.EntityFrameworkCore.SqlServer")
            {
                // update set default of member of location to 1 which mean all 
                migrationBuilder.Sql(
                @"
                    UPDATE CarlErpLocations
                    SET Member = 1
                ");
            }
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CarlErpLocationGroups");

            migrationBuilder.DropColumn(
                name: "Member",
                table: "CarlErpLocations");
        }
    }
}
