using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class Update_Table_Lock_Transaction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ExpiredTime",
                table: "CarlErpLocks",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "GenerateDate",
                table: "CarlErpLocks",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "CarlErpLocks",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpiredTime",
                table: "CarlErpLocks");

            migrationBuilder.DropColumn(
                name: "GenerateDate",
                table: "CarlErpLocks");

            migrationBuilder.DropColumn(
                name: "Password",
                table: "CarlErpLocks");
        }
    }
}
