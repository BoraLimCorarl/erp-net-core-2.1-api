using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class UpdatePromotionCampaignAndSubscription : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CarlErpPackageEditionPromotions");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpPackageEditions_EditionId",
                table: "CarlErpPackageEditions");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpPackageEditions_PackageId",
                table: "CarlErpPackageEditions");

            migrationBuilder.AlterColumn<Guid>(
                name: "PromotionId",
                table: "CarlErpSubscriptionPromotions",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AddColumn<Guid>(
                name: "CampaignId",
                table: "CarlErpSubscriptionPromotions",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsEligibleWithOther",
                table: "CarlErpSubscriptionPromotions",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsRenewable",
                table: "CarlErpSubscriptionPromotions",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSpecificPackage",
                table: "CarlErpSubscriptionPromotions",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndDate",
                table: "CarlErpPromotionCampaigns",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AddColumn<int>(
                name: "CampaignType",
                table: "CarlErpPromotionCampaigns",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "CarlErpPromotionCampaigns",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsEligibleWithOther",
                table: "CarlErpPromotionCampaigns",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsRenewable",
                table: "CarlErpPromotionCampaigns",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "CarlErpPromotionCampaigns",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "NeverEnd",
                table: "CarlErpPromotionCampaigns",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<Guid>(
                name: "PromotionId",
                table: "CarlErpPromotionCampaignEditions",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AddColumn<int>(
                name: "SortOrder",
                table: "CarlErpPromotionCampaignEditions",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "CarlErpPackages",
                maxLength: 512,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "AnnualPrice",
                table: "CarlErpPackageEditions",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "CarlErpSubscriptionCampaignPromotions",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    SubscriptionPromotionId = table.Column<Guid>(nullable: false),
                    CampaignId = table.Column<Guid>(nullable: false),
                    PromotionId = table.Column<Guid>(nullable: true),
                    EditionId = table.Column<int>(nullable: false),
                    SortOrder = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpSubscriptionCampaignPromotions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpSubscriptionCampaignPromotions_CarlErpPromotionCampa~",
                        column: x => x.CampaignId,
                        principalTable: "CarlErpPromotionCampaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpSubscriptionCampaignPromotions_AbpEditions_EditionId",
                        column: x => x.EditionId,
                        principalTable: "AbpEditions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpSubscriptionCampaignPromotions_CarlErpPromotions_Pro~",
                        column: x => x.PromotionId,
                        principalTable: "CarlErpPromotions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpSubscriptionCampaignPromotions_CarlErpSubscriptionPr~",
                        column: x => x.SubscriptionPromotionId,
                        principalTable: "CarlErpSubscriptionPromotions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpSubscriptionPromotions_CampaignId",
                table: "CarlErpSubscriptionPromotions",
                column: "CampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPromotionCampaigns_CampaignType",
                table: "CarlErpPromotionCampaigns",
                column: "CampaignType");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPackageEditions_EditionId",
                table: "CarlErpPackageEditions",
                column: "EditionId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPackageEditions_PackageId_EditionId",
                table: "CarlErpPackageEditions",
                columns: new[] { "PackageId", "EditionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpSubscriptionCampaignPromotions_CampaignId",
                table: "CarlErpSubscriptionCampaignPromotions",
                column: "CampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpSubscriptionCampaignPromotions_EditionId",
                table: "CarlErpSubscriptionCampaignPromotions",
                column: "EditionId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpSubscriptionCampaignPromotions_PromotionId",
                table: "CarlErpSubscriptionCampaignPromotions",
                column: "PromotionId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpSubscriptionCampaignPromotions_SubscriptionPromotion~",
                table: "CarlErpSubscriptionCampaignPromotions",
                column: "SubscriptionPromotionId");

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpSubscriptionPromotions_CarlErpPromotionCampaigns_Cam~",
                table: "CarlErpSubscriptionPromotions",
                column: "CampaignId",
                principalTable: "CarlErpPromotionCampaigns",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpSubscriptionPromotions_CarlErpPromotionCampaigns_Cam~",
                table: "CarlErpSubscriptionPromotions");

            migrationBuilder.DropTable(
                name: "CarlErpSubscriptionCampaignPromotions");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpSubscriptionPromotions_CampaignId",
                table: "CarlErpSubscriptionPromotions");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpPromotionCampaigns_CampaignType",
                table: "CarlErpPromotionCampaigns");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpPackageEditions_EditionId",
                table: "CarlErpPackageEditions");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpPackageEditions_PackageId_EditionId",
                table: "CarlErpPackageEditions");

            migrationBuilder.DropColumn(
                name: "CampaignId",
                table: "CarlErpSubscriptionPromotions");

            migrationBuilder.DropColumn(
                name: "IsEligibleWithOther",
                table: "CarlErpSubscriptionPromotions");

            migrationBuilder.DropColumn(
                name: "IsRenewable",
                table: "CarlErpSubscriptionPromotions");

            migrationBuilder.DropColumn(
                name: "IsSpecificPackage",
                table: "CarlErpSubscriptionPromotions");

            migrationBuilder.DropColumn(
                name: "CampaignType",
                table: "CarlErpPromotionCampaigns");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "CarlErpPromotionCampaigns");

            migrationBuilder.DropColumn(
                name: "IsEligibleWithOther",
                table: "CarlErpPromotionCampaigns");

            migrationBuilder.DropColumn(
                name: "IsRenewable",
                table: "CarlErpPromotionCampaigns");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "CarlErpPromotionCampaigns");

            migrationBuilder.DropColumn(
                name: "NeverEnd",
                table: "CarlErpPromotionCampaigns");

            migrationBuilder.DropColumn(
                name: "SortOrder",
                table: "CarlErpPromotionCampaignEditions");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "CarlErpPackages");

            migrationBuilder.DropColumn(
                name: "AnnualPrice",
                table: "CarlErpPackageEditions");

            //migrationBuilder.AlterColumn<Guid>(
            //    name: "PromotionId",
            //    table: "CarlErpSubscriptionPromotions",
            //    nullable: false,
            //    oldClrType: typeof(Guid),
            //    oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndDate",
                table: "CarlErpPromotionCampaigns",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            //migrationBuilder.AlterColumn<Guid>(
            //    name: "PromotionId",
            //    table: "CarlErpPromotionCampaignEditions",
            //    nullable: false,
            //    oldClrType: typeof(Guid),
            //    oldNullable: true);

            migrationBuilder.CreateTable(
                name: "CarlErpPackageEditionPromotions",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    EditionId = table.Column<int>(nullable: false),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    PackageId = table.Column<Guid>(nullable: false),
                    PromotionId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpPackageEditionPromotions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpPackageEditionPromotions_AbpEditions_EditionId",
                        column: x => x.EditionId,
                        principalTable: "AbpEditions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpPackageEditionPromotions_CarlErpPackages_PackageId",
                        column: x => x.PackageId,
                        principalTable: "CarlErpPackages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpPackageEditionPromotions_CarlErpPromotions_Promotion~",
                        column: x => x.PromotionId,
                        principalTable: "CarlErpPromotions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPackageEditions_EditionId",
                table: "CarlErpPackageEditions",
                column: "EditionId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPackageEditions_PackageId",
                table: "CarlErpPackageEditions",
                column: "PackageId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPackageEditionPromotions_EditionId",
                table: "CarlErpPackageEditionPromotions",
                column: "EditionId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPackageEditionPromotions_PromotionId",
                table: "CarlErpPackageEditionPromotions",
                column: "PromotionId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPackageEditionPromotions_PackageId_EditionId_Promoti~",
                table: "CarlErpPackageEditionPromotions",
                columns: new[] { "PackageId", "EditionId", "PromotionId" },
                unique: true);
        }
    }
}
