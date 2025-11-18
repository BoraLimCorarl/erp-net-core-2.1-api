using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class _SignupSubscriptionPackage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CarlErpSubscriptionPayments");

            migrationBuilder.AddColumn<decimal>(
                name: "Discount",
                table: "CarlErpSubscriptions",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "InvoiceDate",
                table: "CarlErpSubscriptions",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PackageId",
                table: "CarlErpSubscriptions",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SubscriptionType",
                table: "CarlErpSubscriptions",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "UpgradeDeduction",
                table: "CarlErpSubscriptions",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<Guid>(
                name: "UpgradeFromSubscriptionId",
                table: "CarlErpSubscriptions",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ApiClients",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    Secret = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    ApplicationType = table.Column<int>(nullable: false),
                    Active = table.Column<bool>(nullable: false),
                    AllowedOrigin = table.Column<string>(nullable: true),
                    ClientId = table.Column<string>(nullable: true),
                    OwnedByTenantId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiClients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpFeatures",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    Name = table.Column<string>(maxLength: 512, nullable: false),
                    Description = table.Column<string>(maxLength: 512, nullable: true),
                    SortOrder = table.Column<int>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    IsDefault = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpFeatures", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpPackages",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    Name = table.Column<string>(maxLength: 512, nullable: false),
                    Description = table.Column<string>(maxLength: 512, nullable: true),
                    SortOrder = table.Column<int>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpPackages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpPromotions",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    PromotionName = table.Column<string>(nullable: true),
                    PromotionType = table.Column<int>(nullable: false),
                    DiscountRate = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    ExtraMonth = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    IsTrial = table.Column<bool>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpPromotions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpSignUps",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    FirstName = table.Column<string>(maxLength: 128, nullable: false),
                    LastName = table.Column<string>(maxLength: 128, nullable: false),
                    Email = table.Column<string>(nullable: false),
                    CompanyOrStoreName = table.Column<string>(maxLength: 128, nullable: false),
                    Position = table.Column<string>(maxLength: 128, nullable: false),
                    PhoneNumber = table.Column<string>(maxLength: 128, nullable: false),
                    SignUpCode = table.Column<string>(maxLength: 128, nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    TenantId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpSignUps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpSignUps_AbpTenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "AbpTenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpPackageEditionFeatures",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    PackageId = table.Column<Guid>(nullable: false),
                    EditionId = table.Column<int>(nullable: false),
                    FeatureId = table.Column<Guid>(nullable: false),
                    Value = table.Column<string>(maxLength: 512, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpPackageEditionFeatures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpPackageEditionFeatures_AbpEditions_EditionId",
                        column: x => x.EditionId,
                        principalTable: "AbpEditions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpPackageEditionFeatures_CarlErpFeatures_FeatureId",
                        column: x => x.FeatureId,
                        principalTable: "CarlErpFeatures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpPackageEditionFeatures_CarlErpPackages_PackageId",
                        column: x => x.PackageId,
                        principalTable: "CarlErpPackages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpPackageEditions",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    PackageId = table.Column<Guid>(nullable: false),
                    EditionId = table.Column<int>(nullable: false),
                    SortOrder = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpPackageEditions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpPackageEditions_AbpEditions_EditionId",
                        column: x => x.EditionId,
                        principalTable: "AbpEditions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpPackageEditions_CarlErpPackages_PackageId",
                        column: x => x.PackageId,
                        principalTable: "CarlErpPackages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpPackageEditionPromotions",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    PackageId = table.Column<Guid>(nullable: false),
                    EditionId = table.Column<int>(nullable: false),
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

            migrationBuilder.CreateTable(
                name: "CarlErpPromotionCampaigns",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    StartDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: false),
                    IsSpecificPackage = table.Column<bool>(nullable: false),
                    PackageId = table.Column<Guid>(nullable: true),
                    PromotionId = table.Column<Guid>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpPromotionCampaigns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpPromotionCampaigns_CarlErpPackages_PackageId",
                        column: x => x.PackageId,
                        principalTable: "CarlErpPackages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpPromotionCampaigns_CarlErpPromotions_PromotionId",
                        column: x => x.PromotionId,
                        principalTable: "CarlErpPromotions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpSubscriptionPromotions",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    SubscriptionId = table.Column<Guid>(nullable: false),
                    PromotionId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpSubscriptionPromotions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpSubscriptionPromotions_CarlErpPromotions_PromotionId",
                        column: x => x.PromotionId,
                        principalTable: "CarlErpPromotions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpSubscriptionPromotions_CarlErpSubscriptions_Subscrip~",
                        column: x => x.SubscriptionId,
                        principalTable: "CarlErpSubscriptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpPromotionCampaignEditions",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    PromotionCampaignId = table.Column<Guid>(nullable: false),
                    EditionId = table.Column<int>(nullable: false),
                    PromotionId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpPromotionCampaignEditions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpPromotionCampaignEditions_AbpEditions_EditionId",
                        column: x => x.EditionId,
                        principalTable: "AbpEditions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpPromotionCampaignEditions_CarlErpPromotionCampaigns_~",
                        column: x => x.PromotionCampaignId,
                        principalTable: "CarlErpPromotionCampaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpPromotionCampaignEditions_CarlErpPromotions_Promotio~",
                        column: x => x.PromotionId,
                        principalTable: "CarlErpPromotions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpSubscriptions_PackageId",
                table: "CarlErpSubscriptions",
                column: "PackageId");

            migrationBuilder.CreateIndex(
                name: "IX_ApiClients_ClientId",
                table: "ApiClients",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ApiClients_Name",
                table: "ApiClients",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_ApiClients_Secret",
                table: "ApiClients",
                column: "Secret");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpFeatures_Name",
                table: "CarlErpFeatures",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpFeatures_SortOrder",
                table: "CarlErpFeatures",
                column: "SortOrder");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPackageEditionFeatures_EditionId",
                table: "CarlErpPackageEditionFeatures",
                column: "EditionId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPackageEditionFeatures_FeatureId",
                table: "CarlErpPackageEditionFeatures",
                column: "FeatureId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPackageEditionFeatures_PackageId_EditionId_FeatureId",
                table: "CarlErpPackageEditionFeatures",
                columns: new[] { "PackageId", "EditionId", "FeatureId" },
                unique: true);

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

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPackageEditions_EditionId",
                table: "CarlErpPackageEditions",
                column: "EditionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPackageEditions_PackageId",
                table: "CarlErpPackageEditions",
                column: "PackageId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPackageEditions_SortOrder",
                table: "CarlErpPackageEditions",
                column: "SortOrder");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPackages_Name",
                table: "CarlErpPackages",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPackages_SortOrder",
                table: "CarlErpPackages",
                column: "SortOrder");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPromotionCampaignEditions_EditionId",
                table: "CarlErpPromotionCampaignEditions",
                column: "EditionId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPromotionCampaignEditions_PromotionId",
                table: "CarlErpPromotionCampaignEditions",
                column: "PromotionId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPromotionCampaignEditions_PromotionCampaignId_Editio~",
                table: "CarlErpPromotionCampaignEditions",
                columns: new[] { "PromotionCampaignId", "EditionId", "PromotionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPromotionCampaigns_PackageId",
                table: "CarlErpPromotionCampaigns",
                column: "PackageId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPromotionCampaigns_PromotionId",
                table: "CarlErpPromotionCampaigns",
                column: "PromotionId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPromotions_PromotionName",
                table: "CarlErpPromotions",
                column: "PromotionName");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPromotions_PromotionType",
                table: "CarlErpPromotions",
                column: "PromotionType");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpSignUps_FirstName",
                table: "CarlErpSignUps",
                column: "FirstName");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpSignUps_LastName",
                table: "CarlErpSignUps",
                column: "LastName");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpSignUps_PhoneNumber",
                table: "CarlErpSignUps",
                column: "PhoneNumber");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpSignUps_Position",
                table: "CarlErpSignUps",
                column: "Position");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpSignUps_TenantId",
                table: "CarlErpSignUps",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpSubscriptionPromotions_PromotionId",
                table: "CarlErpSubscriptionPromotions",
                column: "PromotionId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpSubscriptionPromotions_SubscriptionId",
                table: "CarlErpSubscriptionPromotions",
                column: "SubscriptionId");

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpSubscriptions_CarlErpPackages_PackageId",
                table: "CarlErpSubscriptions",
                column: "PackageId",
                principalTable: "CarlErpPackages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpSubscriptions_CarlErpPackages_PackageId",
                table: "CarlErpSubscriptions");

            migrationBuilder.DropTable(
                name: "ApiClients");

            migrationBuilder.DropTable(
                name: "CarlErpPackageEditionFeatures");

            migrationBuilder.DropTable(
                name: "CarlErpPackageEditionPromotions");

            migrationBuilder.DropTable(
                name: "CarlErpPackageEditions");

            migrationBuilder.DropTable(
                name: "CarlErpPromotionCampaignEditions");

            migrationBuilder.DropTable(
                name: "CarlErpSignUps");

            migrationBuilder.DropTable(
                name: "CarlErpSubscriptionPromotions");

            migrationBuilder.DropTable(
                name: "CarlErpFeatures");

            migrationBuilder.DropTable(
                name: "CarlErpPromotionCampaigns");

            migrationBuilder.DropTable(
                name: "CarlErpPackages");

            migrationBuilder.DropTable(
                name: "CarlErpPromotions");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpSubscriptions_PackageId",
                table: "CarlErpSubscriptions");

            migrationBuilder.DropColumn(
                name: "Discount",
                table: "CarlErpSubscriptions");

            migrationBuilder.DropColumn(
                name: "InvoiceDate",
                table: "CarlErpSubscriptions");

            migrationBuilder.DropColumn(
                name: "PackageId",
                table: "CarlErpSubscriptions");

            migrationBuilder.DropColumn(
                name: "SubscriptionType",
                table: "CarlErpSubscriptions");

            migrationBuilder.DropColumn(
                name: "UpgradeDeduction",
                table: "CarlErpSubscriptions");

            migrationBuilder.DropColumn(
                name: "UpgradeFromSubscriptionId",
                table: "CarlErpSubscriptions");

            migrationBuilder.CreateTable(
                name: "CarlErpSubscriptionPayments",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AffectedDate = table.Column<DateTime>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    Duration = table.Column<int>(nullable: false),
                    DurationType = table.Column<int>(nullable: false),
                    EditionId = table.Column<int>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: false),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    PackagePrice = table.Column<decimal>(nullable: false),
                    PaymentMethod = table.Column<int>(nullable: false),
                    SubscriptionDate = table.Column<DateTime>(nullable: false),
                    SubscriptionType = table.Column<int>(nullable: false),
                    TenantId = table.Column<int>(nullable: false),
                    TotalPrice = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpSubscriptionPayments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpSubscriptionPayments_AbpEditions_EditionId",
                        column: x => x.EditionId,
                        principalTable: "AbpEditions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpSubscriptionPayments_AbpTenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "AbpTenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpSubscriptionPayments_EditionId",
                table: "CarlErpSubscriptionPayments",
                column: "EditionId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpSubscriptionPayments_SubscriptionType",
                table: "CarlErpSubscriptionPayments",
                column: "SubscriptionType");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpSubscriptionPayments_TenantId",
                table: "CarlErpSubscriptionPayments",
                column: "TenantId");
        }
    }
}
