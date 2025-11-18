using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class UpdatePaymentMethod : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CarlErpPaymentMethods_TenantId_CreatorUserId_Name",
                table: "CarlErpPaymentMethods");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "CarlErpPaymentMethods");

            migrationBuilder.AddColumn<Guid>(
                name: "PaymentMethodId",
                table: "CarlErpPaymentMethods",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "CarlErpPaymentMethodBases",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    Name = table.Column<string>(maxLength: 512, nullable: false),
                    Icon = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    IsDefault = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpPaymentMethodBases", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPaymentMethods_PaymentMethodId",
                table: "CarlErpPaymentMethods",
                column: "PaymentMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPaymentMethodBases_Name",
                table: "CarlErpPaymentMethodBases",
                column: "Name");

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpPaymentMethods_CarlErpPaymentMethodBases_PaymentMethodId",
                table: "CarlErpPaymentMethods",
                column: "PaymentMethodId",
                principalTable: "CarlErpPaymentMethodBases",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpPaymentMethods_CarlErpPaymentMethodBases_PaymentMethodId",
                table: "CarlErpPaymentMethods");

            migrationBuilder.DropTable(
                name: "CarlErpPaymentMethodBases");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpPaymentMethods_PaymentMethodId",
                table: "CarlErpPaymentMethods");

            migrationBuilder.DropColumn(
                name: "PaymentMethodId",
                table: "CarlErpPaymentMethods");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "CarlErpPaymentMethods",
                maxLength: 512,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPaymentMethods_TenantId_CreatorUserId_Name",
                table: "CarlErpPaymentMethods",
                columns: new[] { "TenantId", "CreatorUserId", "Name" });
        }
    }
}
