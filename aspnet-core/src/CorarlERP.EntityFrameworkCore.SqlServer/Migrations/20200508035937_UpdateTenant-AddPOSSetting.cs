using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class UpdateTenantAddPOSSetting : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "PropertyId",
                table: "AbpTenants",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AbpTenants_PropertyId",
                table: "AbpTenants",
                column: "PropertyId");

            migrationBuilder.AddForeignKey(
                name: "FK_AbpTenants_CarlErpProperties_PropertyId",
                table: "AbpTenants",
                column: "PropertyId",
                principalTable: "CarlErpProperties",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            if (ActiveProvider == "Microsoft.EntityFrameworkCore.SqlServer")
            {
                migrationBuilder.Sql(
                @"                     
                    insert into CarlErpAutoSequences 
                    select NEWID(), GETDATE(), max(CreatorUserId), null, null, TenantId, 19, N'POS', N'-', N'0001', 1, 2, null, 0 
                    from CarlErpAutoSequences 
                    group by TenantId
                    
                ");
            }


        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AbpTenants_CarlErpProperties_PropertyId",
                table: "AbpTenants");

            migrationBuilder.DropIndex(
                name: "IX_AbpTenants_PropertyId",
                table: "AbpTenants");

            migrationBuilder.DropColumn(
                name: "PropertyId",
                table: "AbpTenants");
        }
    }
}
