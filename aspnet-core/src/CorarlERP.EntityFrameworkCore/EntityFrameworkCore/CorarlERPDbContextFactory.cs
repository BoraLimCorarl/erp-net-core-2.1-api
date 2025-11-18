using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using CorarlERP.Configuration;
using CorarlERP.Web;
using Abp.Dependency;

namespace CorarlERP.EntityFrameworkCore
{
    /* This class is needed to run "dotnet ef ..." commands from command line on development. Not used anywhere else */
    public class CorarlERPDbContextFactory : IDesignTimeDbContextFactory<CorarlERPDbContext>
    {
        public CorarlERPDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<CorarlERPDbContext>();
            var configuration = AppConfigurations.Get(WebContentDirectoryFinder.CalculateContentRootFolder(), addUserSecrets: true);

            if (configuration.GetValue<bool>("UsePostgreSql"))
            {
                CorarlERPDbContextConfigurer.ConfigurePostgreSql(builder, configuration.GetConnectionString(CorarlERPConsts.ConnectionStringName));
            }
            else
            {
                CorarlERPDbContextConfigurer.Configure(builder, configuration.GetConnectionString(CorarlERPConsts.ConnectionStringName));
            }

            

            return new CorarlERPDbContext(builder.Options);
        }
    }
}