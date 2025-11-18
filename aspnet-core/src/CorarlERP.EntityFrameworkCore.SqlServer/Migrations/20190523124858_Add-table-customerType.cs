using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class AddtablecustomerType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CustomerTypeId",
                table: "CarlErpCustomers",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CarlErpCustomerTypes",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    CustomerTypeName = table.Column<string>(maxLength: 512, nullable: false),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpCustomerTypes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpCustomers_CustomerTypeId",
                table: "CarlErpCustomers",
                column: "CustomerTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpCustomerTypes_TenantId_CreatorUserId_CustomerTypeName",
                table: "CarlErpCustomerTypes",
                columns: new[] { "TenantId", "CreatorUserId", "CustomerTypeName" });

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpCustomers_CarlErpCustomerTypes_CustomerTypeId",
                table: "CarlErpCustomers",
                column: "CustomerTypeId",
                principalTable: "CarlErpCustomerTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpCustomers_CarlErpCustomerTypes_CustomerTypeId",
                table: "CarlErpCustomers");

            migrationBuilder.DropTable(
                name: "CarlErpCustomerTypes");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpCustomers_CustomerTypeId",
                table: "CarlErpCustomers");

            migrationBuilder.DropColumn(
                name: "CustomerTypeId",
                table: "CarlErpCustomers");
        }
    }
}
