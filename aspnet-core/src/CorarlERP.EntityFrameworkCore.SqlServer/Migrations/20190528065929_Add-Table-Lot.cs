using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class AddTableLot : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CarlErpLots",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    LotName = table.Column<string>(maxLength: 512, nullable: false),
                    LocationId = table.Column<long>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpLots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpLots_CarlErpLocations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "CarlErpLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpLots_LocationId",
                table: "CarlErpLots",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpLots_TenantId_CreatorUserId_LotName",
                table: "CarlErpLots",
                columns: new[] { "TenantId", "CreatorUserId", "LotName" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CarlErpLots");
        }
    }
}
