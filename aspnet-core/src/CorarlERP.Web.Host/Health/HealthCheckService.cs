using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using CorarlERP.Configuration;
using Npgsql;

namespace CorarlERP.Web.Health
{
    public interface IHealthCheckService
    {
        Task<HealthCheckResponse> CheckHealthAsync();
    }

    public class SqlServerHealthCheckService : IHealthCheckService
    {
        private readonly IConfigurationRoot _appConfiguration;

        public SqlServerHealthCheckService(IHostingEnvironment env)
        {
            _appConfiguration = env.GetAppConfiguration();
        }

        public async Task<HealthCheckResponse> CheckHealthAsync()
        {
            var results = new List<HealthCheckResult>();

            // Basic check
            results.Add(HealthCheckResult.Healthy("Application is running."));

            // Database check
            try
            {
                var connectionString = _appConfiguration.GetConnectionString("Default");

                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    // Simple query to ensure database is responsive
                    using (var command = new SqlCommand("SELECT 1", connection))
                    {
                        var result = await command.ExecuteScalarAsync();
                        if (result == null || Convert.ToInt32(result) != 1)
                        {
                            results.Add(HealthCheckResult.Unhealthy("Database query failed."));
                        }
                        else
                        {
                            results.Add(HealthCheckResult.Healthy("Database is accessible."));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                results.Add(HealthCheckResult.Unhealthy($"Database error: {ex.Message}"));
            }

            // Determine overall status
            var status = results.Any(r => r.Status == HealthStatus.Unhealthy) ? "Unhealthy" : "Healthy";

            return new HealthCheckResponse
            {
                Status = status,
                Checks = results
            };
        }
    }

    public class PostgreSqlHealthCheckService : IHealthCheckService
    {
        private readonly IConfigurationRoot _appConfiguration;

        public PostgreSqlHealthCheckService(IHostingEnvironment env)
        {
            _appConfiguration = env.GetAppConfiguration();
        }

        public async Task<HealthCheckResponse> CheckHealthAsync()
        {
            var results = new List<HealthCheckResult>();

            // Basic check
            results.Add(HealthCheckResult.Healthy("Application is running."));

            // Database check
            try
            {
                var connectionString = _appConfiguration.GetConnectionString("Default");

                using (var connection = new NpgsqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    // Simple query to ensure database is responsive
                    using (var command = new NpgsqlCommand("SELECT 1", connection))
                    {
                        var result = await command.ExecuteScalarAsync();
                        if (result == null || Convert.ToInt32(result) != 1)
                        {
                            results.Add(HealthCheckResult.Unhealthy("Database query failed."));
                        }
                        else
                        {
                            results.Add(HealthCheckResult.Healthy("Database is accessible."));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                results.Add(HealthCheckResult.Unhealthy($"Database error: {ex.Message}"));
            }

            // Determine overall status
            var status = results.Any(r => r.Status == HealthStatus.Unhealthy) ? "Unhealthy" : "Healthy";

            return new HealthCheckResponse
            {
                Status = status,
                Checks = results
            };
        }
    }



    public class HealthCheckResponse
    {
        public string Status { get; set; }
        public IEnumerable<HealthCheckResult> Checks { get; set; }
    }
}
