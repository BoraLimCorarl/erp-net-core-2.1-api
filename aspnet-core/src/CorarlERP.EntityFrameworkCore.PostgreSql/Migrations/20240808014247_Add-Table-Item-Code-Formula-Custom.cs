using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace CorarlERP.Migrations
{
    public partial class AddTableItemCodeFormulaCustom : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
          

            migrationBuilder.AddColumn<bool>(
                name: "UseCustomGenerate",
                table: "CarlErpItemCodeFormulas",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "UseItemProperty",
                table: "CarlErpItemCodeFormulas",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "CarlErpItemCodeFormulaCustoms",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    Prefix = table.Column<string>(nullable: true),
                    ItemCode = table.Column<string>(nullable: true),
                    ItemCodeFormulaId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpItemCodeFormulaCustoms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpItemCodeFormulaCustoms_CarlErpItemCodeFormulas_ItemC~",
                        column: x => x.ItemCodeFormulaId,
                        principalTable: "CarlErpItemCodeFormulas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemCodeFormulaCustoms_ItemCode",
                table: "CarlErpItemCodeFormulaCustoms",
                column: "ItemCode");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemCodeFormulaCustoms_ItemCodeFormulaId",
                table: "CarlErpItemCodeFormulaCustoms",
                column: "ItemCodeFormulaId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemCodeFormulaCustoms_Prefix",
                table: "CarlErpItemCodeFormulaCustoms",
                column: "Prefix");

            migrationBuilder.Sql(@"delete from ""CarlErpItemCodeFormulaItemTypes""");
            migrationBuilder.Sql(@"delete from ""CarlErpItemCodeFormulaProperties""");
            migrationBuilder.Sql(@"delete from ""CarlErpItemCodeFormulas""");

            migrationBuilder.DropIndex(
                  name: "IX_CarlErpItemCodeFormulas_Name",
                  table: "CarlErpItemCodeFormulas");
            migrationBuilder.DropColumn(
                name: "Name",
                table: "CarlErpItemCodeFormulas");

            migrationBuilder.Sql(@"INSERT INTO  ""CarlErpItemCodeFormulas""(""CreationTime"",""TenantId"",""IsActive"",""UseCustomGenerate"",""UseItemProperty"") 
                                                SELECT NOW(), t.""Id"", true, true, false  FROM ""AbpTenants"" as t where  t.""ItemCodeSetting"" = 1");

            migrationBuilder.Sql(@"INSERT INTO  ""CarlErpItemCodeFormulaCustoms""(""CreationTime"",""TenantId"",""Prefix"",""ItemCode"",""ItemCodeFormulaId"") 
                                   SELECT NOW(),t.""Id"", t.""Prifix"", t.""ItemCode"", f.""Id""  FROM ""AbpTenants"" as t   join ""CarlErpItemCodeFormulas"" as f on f.""TenantId"" = t.""Id""
                                   where  t.""ItemCodeSetting"" = 1 and f.""UseCustomGenerate""= true");

            migrationBuilder.Sql(@"INSERT INTO ""CarlErpItemCodeFormulaItemTypes""(""CreationTime"",""TenantId"",""ItemCodeFormulaId"",""ItemTypeId"") 
                                    SELECT NOW(),f.""TenantId"", f.""Id"", b.""Id""  from ""CarlErpItemTypes"" as b
                                    join ""CarlErpItemCodeFormulas"" as f on 1 = 1 ");
          
          
            migrationBuilder.DropColumn(
                name: "ItemCode",
                table: "AbpTenants");

            migrationBuilder.DropColumn(
                name: "Prifix",
                table: "AbpTenants");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CarlErpItemCodeFormulaCustoms");

            migrationBuilder.DropColumn(
                name: "UseCustomGenerate",
                table: "CarlErpItemCodeFormulas");

            migrationBuilder.DropColumn(
                name: "UseItemProperty",
                table: "CarlErpItemCodeFormulas");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "CarlErpItemCodeFormulas",
                maxLength: 512,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ItemCode",
                table: "AbpTenants",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Prifix",
                table: "AbpTenants",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemCodeFormulas_Name",
                table: "CarlErpItemCodeFormulas",
                column: "Name");
        }
    }
}
