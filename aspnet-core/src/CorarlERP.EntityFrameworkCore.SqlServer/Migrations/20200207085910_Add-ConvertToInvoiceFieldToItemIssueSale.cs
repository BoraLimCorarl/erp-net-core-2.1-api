using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class AddConvertToInvoiceFieldToItemIssueSale : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ConvertToInvoice",
                table: "CarlErpItemIssues",
                nullable: false,
                defaultValue: false);


            if (ActiveProvider == "Microsoft.EntityFrameworkCore.SqlServer")
            {
                migrationBuilder.Sql(
                @" 
                    UPDATE CarlErpItemIssues
                    SET CarlErpItemIssues.ConvertToInvoice = 1                    
                    WHERE CarlErpItemIssues.TransactionType = 7
                ");
            }
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConvertToInvoice",
                table: "CarlErpItemIssues");
        }
    }
}
