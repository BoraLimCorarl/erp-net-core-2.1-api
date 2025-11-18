using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class UpdateItemPropertyLink : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "ItemId",
                table: "CarlErpItemProperties",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "ItemId",
                table: "CarlErpItemProperties",
                nullable: true,
                oldClrType: typeof(Guid));
        }
    }
}
