using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using Abp.AspNetCore;
using Abp.AspNetCore.Configuration;
using Abp.AspNetCore.Mvc.Extensions;
using Abp.AspNetCore.Mvc.Results;
using Abp.AspNetCore.SignalR.Hubs;
using Abp.AspNetZeroCore.Web.Authentication.JwtBearer;
using Abp.Authorization;
using Abp.Castle.Logging.Log4Net;
using Abp.Dependency;
using Abp.Domain.Entities;
using Abp.Events.Bus;
using Abp.Events.Bus.Exceptions;
using Abp.Extensions;
using Abp.Hangfire;
using Abp.PlugIns;
using Abp.Reflection.Extensions;
using Abp.Runtime.Validation;
using Abp.Timing;
using Abp.Web.Models;
using AutoMapper.Internal;
using Castle.Core.Logging;
using Castle.Facilities.Logging;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Cors.Internal;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CorarlERP.Authorization;
using CorarlERP.Authorization.Roles;
using CorarlERP.Authorization.Users;
using CorarlERP.Configuration;
using CorarlERP.EntityFrameworkCore;
using CorarlERP.Identity;
using CorarlERP.Install;
using CorarlERP.MultiTenancy;
using CorarlERP.Web.Authentication.JwtBearer;
using CorarlERP.Web.Chat.SignalR;
using PaulMiami.AspNetCore.Mvc.Recaptcha;
using Swashbuckle.AspNetCore.Swagger;
using CorarlERP.Web.IdentityServer;
using ILogger = Microsoft.Extensions.Logging.ILogger;
using ILoggerFactory = Microsoft.Extensions.Logging.ILoggerFactory;
using Castle.MicroKernel.Registration;
using CorarlERP.EntityFrameworkCore.Repositories;
using Amazon.S3;
using CorarlERP.FileStorages;
using System.Threading.Tasks;
using Abp.Runtime.Session;
using Hangfire.Dashboard;
using System.IdentityModel.Tokens.Jwt;
using Abp;
using Abp.AutoMapper;
using Hangfire.PostgreSql;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Newtonsoft.Json;
using CorarlERP.Web.Health;

namespace CorarlERP.Web.Startup
{
    public class Startup
    {
        private const string DefaultCorsPolicyName = "localhost";

        private readonly IConfigurationRoot _appConfiguration;
        private readonly IHostingEnvironment _hostingEnvironment;

        public Startup(IHostingEnvironment env)
        {
            _hostingEnvironment = env;
            _appConfiguration = env.GetAppConfiguration();

            //if (bool.Parse(_appConfiguration["UTC"]))
            //{
            //    Clock.Provider = ClockProviders.Utc;

            //    if (bool.Parse(_appConfiguration["UsePostgreSql"]))
            //    {
            //        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            //        AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);
            //    }   
            //}
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            //MVC
            services.AddMvc(options =>
            {
                options.Filters.Add(new CorsAuthorizationFilterFactory(DefaultCorsPolicyName));
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_1); ;

            services.AddSignalR(options => { options.EnableDetailedErrors = true; });

            //Configure CORS for angular2 UI
            services.AddCors(options =>
            {
                options.AddPolicy(DefaultCorsPolicyName, builder =>
                {
                    //App:CorsOrigins in appsettings.json can contain more than one address with splitted by comma.
                    builder
                        .WithOrigins(
                            // App:CorsOrigins in appsettings.json can contain more than one address separated by comma.
                            _appConfiguration["App:CorsOrigins"]
                                .Split(",", StringSplitOptions.RemoveEmptyEntries)
                                .Select(o => o.RemovePostFix("/"))
                                .ToArray()
                        )
                        .SetIsOriginAllowedToAllowWildcardSubdomains()
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });

            IdentityRegistrar.Register(services);
            AuthConfigurer.Configure(services, _appConfiguration);

            //Identity server
            if (bool.Parse(_appConfiguration["IdentityServer:IsEnabled"]))
            {
                IdentityServerRegistrar.Register(services, _appConfiguration);
            }

            //Swagger - Enable this line and the related lines in Configure method to enable swagger UI
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Info { Title = "CorarlERP API", Version = "v1" });
                options.DocInclusionPredicate((docName, description) => true);

                //Note: This is just for showing Authorize button on the UI. 
                //Authorize button's behaviour is handled in wwwroot/swagger/ui/index.html
                options.AddSecurityDefinition("Bearer", new BasicAuthScheme());
            });

            //Recaptcha
            services.AddRecaptcha(new RecaptchaOptions
            {
                SiteKey = _appConfiguration["Recaptcha:SiteKey"],
                SecretKey = _appConfiguration["Recaptcha:SecretKey"]
            });

            //Hangfire (Enable to use Hangfire instead of default job manager)
            services.AddHangfire(config =>
            {
                if (bool.Parse(_appConfiguration["UsePostgreSql"]))
                {
                    config.UsePostgreSqlStorage(_appConfiguration.GetConnectionString("Default"));
                }
                else
                {
                    config.UseSqlServerStorage(_appConfiguration.GetConnectionString("Default"));
                }
            });
            //GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute { Attempts = 5, DelaysInSeconds = new int[] { 300 } });
            GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute { Attempts = 3 });


            //services.AddDefaultAWSOptions(_appConfiguration.GetAWSOptions());
            services.AddAWSService<IAmazonS3>();

            if (bool.Parse(_appConfiguration["StackExchange:Enable"]))
            {
                services.AddStackExchangeRedisCache(options =>
                {
                    options.InstanceName = _appConfiguration["StackExchange:RedisCache:InstanceName"];
                    options.Configuration = _appConfiguration["StackExchange:RedisCache:ConnectionString"];
                });
            }


            //if (bool.Parse(_appConfiguration["UsePostgreSql"]))
            //{
            //    // Add health checks
            //    services.AddHealthChecks()
            //    // Example: Basic health check
            //    .AddCheck("BasicCheck", () => HealthCheckResult.Healthy("Basic check passed!"))
            //    .AddNpgSql(
            //        npgsqlConnectionString: _appConfiguration["ConnectionStrings:Default"],
            //        name: "PostgreSQL",
            //        healthQuery: "SELECT \"Id\" from \"AbpUsers\";",
            //        failureStatus: HealthStatus.Unhealthy,
            //        tags: new[] { "critical" });
            //}
            //else
            //{
            //    // Add health checks
            //    services.AddHealthChecks()
            //    // Example: Basic health check
            //    .AddCheck("BasicCheck", () => HealthCheckResult.Healthy("Basic check passed!"))
            //    // Example: SQL Server health check
            //    .AddSqlServer(
            //        connectionString: _appConfiguration["ConnectionStrings:Default"],
            //        name: "SqlServer",
            //        healthQuery: "SELECT Id from AbpUsers;",
            //        failureStatus: HealthStatus.Unhealthy,
            //        tags: new[] { "critical" });
            //}

            if (bool.Parse(_appConfiguration["UsePostgreSql"]))
            {
                // Register custom health check service
                services.AddSingleton<IHealthCheckService, PostgreSqlHealthCheckService>();
            }
            else
            {
                // Register custom health check service
                services.AddSingleton<IHealthCheckService, SqlServerHealthCheckService>();
            } 

            //Configure Abp and Dependency Injection
            return services.AddAbp<CorarlERPWebHostModule>(options =>
            {
                //Configure Log4Net logging
                options.IocManager.IocContainer.AddFacility<LoggingFacility>(
                    f => f.UseAbpLog4Net().WithConfig("log4net.config")
                );

                options.IocManager.IocContainer.Register(Component.For(typeof(ICorarlRepository<,>))
                .ImplementedBy(typeof(CorarlRepository<,>)).LifestyleTransient());

                options.IocManager.IocContainer.Register(Component.For(typeof(ICorarlRepository<>))
                .ImplementedBy(typeof(CorarlRepository<>)).LifestyleTransient());

                if (_appConfiguration["AWS:S3:Enable"].ToLower() == "true")
                {
                    options.IocManager.IocContainer.Register(Component.For(typeof(IFileStorageManager))
                    .ImplementedBy(typeof(AWSFileStorageManager)).LifestyleTransient());
                }
                else
                {
                    options.IocManager.IocContainer.Register(Component.For(typeof(IFileStorageManager))
                    .ImplementedBy(typeof(LocalFileStorageManager)).LifestyleTransient());
                }

                options.PlugInSources.AddFolder(Path.Combine(_hostingEnvironment.WebRootPath, "Plugins"), SearchOption.AllDirectories);
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {

            //IocManager.IocContainer.Register(Component.For(typeof(ICorarlRepository<,>))
            //    .ImplementedBy(typeof(CorarlRepository<,>)).LifestyleTransient());

            //IocManager.IocContainer.Register(Component.For(typeof(ICorarlRepository<>))
            // .ImplementedBy(typeof(CorarlRepository<>)).LifestyleTransient());


            //Initializes ABP framework.
            app.UseAbp(options =>
            {
                options.UseAbpRequestLocalization = false; //used below: UseAbpRequestLocalization
            });

            app.UseCors(DefaultCorsPolicyName); //Enable CORS!

            app.UseAuthentication();
            app.UseJwtTokenMiddleware();

            if (bool.Parse(_appConfiguration["IdentityServer:IsEnabled"]))
            {
                app.UseJwtTokenMiddleware("IdentityBearer");
                app.UseIdentityServer();
            }

            app.UseStaticFiles();

            using (var scope = app.ApplicationServices.CreateScope())
            {
                if (scope.ServiceProvider.GetService<DatabaseCheckHelper>().Exist(_appConfiguration["ConnectionStrings:Default"]))
                {
                    app.UseAbpRequestLocalization();
                }
            }

            app.UseSignalR(routes =>
            {
                routes.MapHub<AbpCommonHub>("/signalr");
                routes.MapHub<ChatHub>("/signalr-chat");
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "defaultWithArea",
                    template: "{area}/{controller=Home}/{action=Index}/{id?}");

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            // Enable middleware to serve generated Swagger as a JSON endpoint
            app.UseSwagger();
            // Enable middleware to serve swagger-ui assets (HTML, JS, CSS etc.)

            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "CorarlERP API V1");
                options.IndexStream = () => Assembly.GetExecutingAssembly()
                    .GetManifestResourceStream("CorarlERP.Web.wwwroot.swagger.ui.index.html");
            }); //URL: /swagger

            app.UseHangfireServer();
            app.UseHangfireDashboard("/hangfire");

            ////Hangfire dashboard &server(Enable to use Hangfire instead of default job manager)
            //app.UseHangfireDashboard("/hangfire", new DashboardOptions
            //{
            //    Authorization = new[] { new AbpHangfireAuthorizationFilter(AppPermissions.Pages_Administration_HangfireDashboard) }
            //});

            app.Use(async (context, func) =>
            {
                if (context.Request.Path.Value == "/healthz")
                {
                    context.Response.StatusCode = 200;
                    context.Response.ContentLength = 2;
                    await context.Response.WriteAsync("UP");
                }
                else
                {
                    await func.Invoke();
                }
            });

            //// Map health check endpoint
            //app.UseHealthChecks("/health", new HealthCheckOptions
            //{
            //    // Customize response
            //    ResponseWriter = async (context, report) =>
            //    {
            //        context.Response.ContentType = "application/json";
            //        var response = new
            //        {
            //            Status = report.Status.ToString(),
            //            Checks = report.Entries.Select(e => new
            //            {
            //                Name = e.Key,
            //                Status = e.Value.Status.ToString(),
            //                Description = e.Value.Description
            //            })
            //        };
            //        await context.Response.WriteAsync(JsonConvert.SerializeObject(response, Formatting.Indented));
            //    }
            //});

        }
    }

}
