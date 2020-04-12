using IntegrationEventLogService;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ProductCatalogue.Infrastructure;
using Serilog;
using System;
using System.IO;

namespace ProductCatalogue.API
{
    public class Program
    {
        public static int Main(string[] args)
        {
            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            Log.Logger = CreateSerilogLogger(configuration);

            try
            {
                Log.Information("Configuring web host (ProductCatalogue.API)...");
                var host = CreateHost(configuration, args);

                Log.Information("Applying migrations (ProductCatalogue.API)...");
                host.MigrateDbContext<ProductsDbContext>((context, services) =>
                {
                    context.Database.Migrate();
                })
                .MigrateDbContext<IntegrationLogDbContext>((context, services) => 
                {
                    context.Database.Migrate();
                });

                Log.Information("Starting web host (ProductCatalogue.API)...");
                host.Run();

                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Program terminated unexpectedly (ProductCatalogue.API)!");
                return 1;
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
    }
}