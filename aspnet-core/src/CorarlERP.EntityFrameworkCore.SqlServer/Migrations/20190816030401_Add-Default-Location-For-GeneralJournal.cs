using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class AddDefaultLocationForGeneralJournal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            if (ActiveProvider == "Microsoft.EntityFrameworkCore.SqlServer")
            {
                migrationBuilder.Sql(
                @"                 
                    Update j Set j.LocationId = t.LocationId
                    From CarlErpJournals AS j
                    inner join AbpTenants as t on j.TenantId = t.Id
					where j.JournalType = 1  And j.LocationId is null          
                ");
            }
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
