using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using IntegrationEventLogService;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Polly;
using Pricing.Infrastructure;
using Serilog;

namespace Pricing.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            Log.Logger = CreateSerilogLogger(configuration);

            try
            {
                Log.Information("Configuring web host (Pricing.API)...");
                var host = CreateHost(configuration, args);

                Log.Information("Applying migrations (Pricing.API)...");
                host.MigrateDbContext<ProductPriceDbContext>((context, services) =>
                {
                    context.Database.Migrate();
                });

                Log.Information("Starting web host (Pricing.API)...");

                host.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Program terminated unexpectedly (Pricing.API)!");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IWebHost CreateHost(IConfiguration configuration, string[] args)
        {
            return
                WebHost.CreateDefaultBuilder(args)
                .CaptureStartupErrors(false)
                .ConfigureAppConfiguration(x => x.AddConfiguration(configuration))
                .UseStartup<Startup>()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseSerilog()
                .Build();
        }

        private static Serilog.ILogger CreateSerilogLogger(IConfiguration configuration)
        {
            var seqServerUrl = configuration["Serilog:SeqServerUrl"];
            var logstashUrl = configuration["Serilog:LogstashgUrl"];
            return new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .Enrich.WithProperty("ApplicationContext", "ProductCatalogue.API")
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.Seq(seqServerUrl)
                .WriteTo.Http(logstashUrl)
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
