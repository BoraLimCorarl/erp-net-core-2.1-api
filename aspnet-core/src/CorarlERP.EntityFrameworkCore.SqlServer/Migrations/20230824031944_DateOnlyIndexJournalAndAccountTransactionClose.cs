using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class DateOnlyIndexJournalAndAccountTransactionClose : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateOnly",
                table: "CarlErpAccountTransactionCloses",
                type: "Date",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpAccountTransactionCloses_DateOnly",
                table: "CarlErpAccountTransactionCloses",
                column: "DateOnly");

            migrationBuilder.Sql(@"
                 Update CarlErpJournals Set DateOnly = Convert(Date,Date);
                 Update CarlErpAccountTransactionCloses Set DateOnly = Convert(Date,Date);
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CarlErpAccountTransactionCloses_DateOnly",
                table: "CarlErpAccountTransactionCloses");

            migrationBuilder.DropColumn(
                name: "DateOnly",
                table: "CarlErpAccountTransactionCloses");
        }
    }
}
