using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegrationEventLogService.Migrations
{

    public class IntegrationEventsContextFactory : IDesignTimeDbContextFactory<IntegrationLogDbContext>
    {
        public IntegrationLogDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<IntegrationLogDbContext>()
                .UseSqlServer("Data Source=IN05N0004L\\SQLEXPRESS;Initial Catalog=ProductPricesDb;Integrated Security=True;");

            return new IntegrationLogDbContext(optionsBuilder.Options);
        }
    }
}