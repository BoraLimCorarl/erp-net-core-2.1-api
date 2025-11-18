using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class AddTableCardAndPOSSetting : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CarlErpCards",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    CardId = table.Column<string>(maxLength: 256, nullable: true),
                    CustomerId = table.Column<Guid>(nullable: true),
                    Remark = table.Column<string>(nullable: true),
                    CardStatus = table.Column<int>(nullable: false),
                    CardNumber = table.Column<string>(maxLength: 256, nullable: true),
                    SerialNumber = table.Column<string>(maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpCards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpCards_AbpUsers_CreatorUserId",
                        column: x => x.CreatorUserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpCards_CarlErpCustomers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "CarlErpCustomers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpCards_AbpUsers_LastModifierUserId",
                        column: x => x.LastModifierUserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpPOSSettings",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    AllowSelectCustomer = table.Column<bool>(nullable: false),
                    UseMemberCard = table.Column<bool>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpPOSSettings", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpCards_CreatorUserId",
                table: "CarlErpCards",
                column: "CreatorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpCards_CustomerId",
                table: "CarlErpCards",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpCards_LastModifierUserId",
                table: "CarlErpCards",
                column: "LastModifierUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpCards_TenantId_CardId",
                table: "CarlErpCards",
                columns: new[] { "TenantId", "CardId" },
                unique: true,
                filter: "[TenantId] IS NOT NULL AND [CardId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpCards_TenantId_CardNumber",
                table: "CarlErpCards",
                columns: new[] { "TenantId", "CardNumber" },
                unique: true,
                filter: "[TenantId] IS NOT NULL AND [CardNumber] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpCards_TenantId_SerialNumber",
                table: "CarlErpCards",
                columns: new[] { "TenantId", "SerialNumber" },
                unique: true,
                filter: "[TenantId] IS NOT NULL AND [SerialNumber] IS NOT NULL");

            migrationBuilder.Sql(@"INSERT INTO CarlErpPOSSettings (CreationTime, IsActive, TenantId, AllowSelectCustomer, UseMemberCard)
                                SELECT GETDATE(), 1, t2.Id, 1, 0
                                FROM AbpTenants AS t2
                                WHERE t2.IsDeleted = 0");
    }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CarlErpCards");

            migrationBuilder.DropTable(
                name: "CarlErpPOSSettings");
        }
    }
}
