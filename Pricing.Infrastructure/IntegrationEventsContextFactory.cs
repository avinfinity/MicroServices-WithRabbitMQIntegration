using IntegrationEventLogService;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Pricing.Infrastructure
{

    public class IntegrationEventsContextFactory : IDesignTimeDbContextFactory<IntegrationLogDbContext>
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