using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class AddVendorType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           

            migrationBuilder.AddColumn<long>(
                name: "VendorTypeId",
                table: "CarlErpVendors",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CarlErpVendorTypes",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    VendorTypeName = table.Column<string>(maxLength: 512, nullable: false),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpVendorTypes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpVendors_VendorTypeId",
                table: "CarlErpVendors",
                column: "VendorTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpVendorTypes_TenantId_CreatorUserId_VendorTypeName",
                table: "CarlErpVendorTypes",
                columns: new[] { "TenantId", "CreatorUserId", "VendorTypeName" });

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpVendors_CarlErpVendorTypes_VendorTypeId",
                table: "CarlErpVendors",
                column: "VendorTypeId",
                principalTable: "CarlErpVendorTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            if (ActiveProvider == "Microsoft.EntityFrameworkCore.SqlServer")
            {
                migrationBuilder.Sql(
                @"                 
                    -- Insert Default vendor type by tenant
                    Insert CarlErpVendorTypes (CreationTime, CreatorUserId, TenantId, VendorTypeName, IsActive) 				   
                    select GETDATE(), Min(u.Id), max(t.Id) , N'Supplier', 1 
                    from AbpTenants t
                    join AbpUsers u on t.Id = u.TenantId
                    group by TenantId	
                    go

                    Insert CarlErpVendorTypes (CreationTime, CreatorUserId, TenantId, VendorTypeName, IsActive) 				   
                    select GETDATE(), Min(u.Id), max(t.Id) , N'Employee', 1 
                    from AbpTenants t
                    join AbpUsers u on t.Id = u.TenantId
                    group by TenantId
                    go     

                    --Update VendorType for all vendor
                    update CarlErpVendors set VendorTypeId = vt.Id
                    from CarlErpVendorTypes vt
                    join AbpTenants t on vt.TenantId = t.Id
                    where
	                CarlErpVendors.TenantId = t.Id and (
                    (CarlErpVendors.VendorType = 1 and vt.VendorTypeName = N'Supplier') or 
                    (CarlErpVendors.VendorType = 2 and vt.VendorTypeName = N'Employee'))
                    go
                ");
            }


            migrationBuilder.DropColumn(
               name: "VendorType",
               table: "CarlErpVendors");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpVendors_CarlErpVendorTypes_VendorTypeId",
                table: "CarlErpVendors");

            migrationBuilder.DropTable(
                name: "CarlErpVendorTypes");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpVendors_VendorTypeId",
                table: "CarlErpVendors");

            migrationBuilder.DropColumn(
                name: "VendorTypeId",
                table: "CarlErpVendors");

            migrationBuilder.AddColumn<int>(
                name: "VendorType",
                table: "CarlErpVendors",
                nullable: false,
                defaultValue: 0);
        }
    }
}
