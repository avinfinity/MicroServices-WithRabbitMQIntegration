using IntegrationEventLogService;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace ProductCatalogue.Infrastructure
{

    internal class IntegrationEventsContextFactory : IDesignTimeDbContextFactory<IntegrationLogDbContext>
    {
        public IntegrationLogDbContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
               .SetBasePath(Path.Combine(Directory.GetCurrentDirectory()))
               .AddJsonFile("appsettings.json")
               .Build();

            var optionsBuilder = new DbContextOptionsBuilder<IntegrationLogDbContext>()
                .UseSqlServer(config["ConnectionString"], options => options.MigrationsAssembly(GetType().Assembly.GetName().Name));

            return new IntegrationLogDbContext(optionsBuilder.Options);
        }
    }
}