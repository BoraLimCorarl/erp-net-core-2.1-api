using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class RemoveDeliveryScheduleDocumentType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"delete from ""CarlErpAutoSequences"" where ""DocumentType""=22");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
