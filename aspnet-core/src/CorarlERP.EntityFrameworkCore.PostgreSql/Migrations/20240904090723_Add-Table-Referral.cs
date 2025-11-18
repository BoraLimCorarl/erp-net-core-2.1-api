using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace CorarlERP.Migrations
{
    public partial class AddTableReferral : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "CarlErpSignUps",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ReferralId",
                table: "CarlErpSignUps",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "CarlErpSignUps",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "CarlErpReferrals",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    Code = table.Column<string>(maxLength: 32, nullable: false),
                    Name = table.Column<string>(maxLength: 32, nullable: false),
                    ExpirationDate = table.Column<DateTime>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Qty = table.Column<int>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpReferrals", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpSignUps_ReferralId",
                table: "CarlErpSignUps",
                column: "ReferralId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpReferrals_Code",
                table: "CarlErpReferrals",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpReferrals_Name",
                table: "CarlErpReferrals",
                column: "Name");

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpSignUps_CarlErpReferrals_ReferralId",
                table: "CarlErpSignUps",
                column: "ReferralId",
                principalTable: "CarlErpReferrals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.Sql(@"UPDATE ""CarlErpSignUps"" SET  ""Status"" = CASE  WHEN ""CarlErpSubscriptions"".""IsTrail"" = true THEN 1 ELSE 2 END
                            FROM ""AbpTenants""
                            JOIN ""CarlErpSubscriptions"" ON ""CarlErpSubscriptions"".""Id"" = ""AbpTenants"".""SubscriptionId""
                            WHERE ""AbpTenants"".""Id"" = ""CarlErpSignUps"".""TenantId""");
          
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpSignUps_CarlErpReferrals_ReferralId",
                table: "CarlErpSignUps");

            migrationBuilder.DropTable(
                name: "CarlErpReferrals");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpSignUps_ReferralId",
                table: "CarlErpSignUps");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "CarlErpSignUps");

            migrationBuilder.DropColumn(
                name: "ReferralId",
                table: "CarlErpSignUps");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "CarlErpSignUps");
        }
    }
}
