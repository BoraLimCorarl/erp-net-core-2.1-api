using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class AddPaymentMethodUserGroup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Member",
                table: "CarlErpPaymentMethods",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "PaymentMethodId",
                table: "CarlErpPayBills",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CarlErpPaymentMethodUserGroups",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: false),
                    PaymentMethodId = table.Column<Guid>(nullable: false),
                    UserGroupId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpPaymentMethodUserGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpPaymentMethodUserGroups_CarlErpPaymentMethods_PaymentMethodId",
                        column: x => x.PaymentMethodId,
                        principalTable: "CarlErpPaymentMethods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpPaymentMethodUserGroups_CarlErpUserGroups_UserGroupId",
                        column: x => x.UserGroupId,
                        principalTable: "CarlErpUserGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPayBills_PaymentMethodId",
                table: "CarlErpPayBills",
                column: "PaymentMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPaymentMethodUserGroups_PaymentMethodId",
                table: "CarlErpPaymentMethodUserGroups",
                column: "PaymentMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPaymentMethodUserGroups_UserGroupId",
                table: "CarlErpPaymentMethodUserGroups",
                column: "UserGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpPayBills_CarlErpPaymentMethods_PaymentMethodId",
                table: "CarlErpPayBills",
                column: "PaymentMethodId",
                principalTable: "CarlErpPaymentMethods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpPayBills_CarlErpPaymentMethods_PaymentMethodId",
                table: "CarlErpPayBills");

            migrationBuilder.DropTable(
                name: "CarlErpPaymentMethodUserGroups");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpPayBills_PaymentMethodId",
                table: "CarlErpPayBills");

            migrationBuilder.DropColumn(
                name: "Member",
                table: "CarlErpPaymentMethods");

            migrationBuilder.DropColumn(
                name: "PaymentMethodId",
                table: "CarlErpPayBills");
        }
    }
}
