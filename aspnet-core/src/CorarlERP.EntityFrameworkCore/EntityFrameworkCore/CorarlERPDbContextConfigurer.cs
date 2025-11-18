using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace CorarlERP.EntityFrameworkCore
{
    public static class CorarlERPDbContextConfigurer
    {
        public static void Configure(DbContextOptionsBuilder<CorarlERPDbContext> builder, string connectionString)
        {
            builder.UseSqlServer(connectionString, b => b.MigrationsAssembly("CorarlERP.EntityFrameworkCore.SqlServer"));
        }

        public static void Configure(DbContextOptionsBuilder<CorarlERPDbContext> builder, DbConnection connection)
        {
            builder.UseSqlServer(connection, b => b.MigrationsAssembly("CorarlERP.EntityFrameworkCore.SqlServer"));
        }

        public static void ConfigurePostgreSql(DbContextOptionsBuilder<CorarlERPDbContext> builder, string connectionString)
        {
            builder.UseNpgsql(connectionString, b => b.MigrationsAssembly("CorarlERP.EntityFrameworkCore.PostgreSql"));
        }

        public static void ConfigurePostgreSql(DbContextOptionsBuilder<CorarlERPDbContext> builder, DbConnection connection)
        {
            builder.UseNpgsql(connection, b => b.MigrationsAssembly("CorarlERP.EntityFrameworkCore.PostgreSql"));
        }
    }
}