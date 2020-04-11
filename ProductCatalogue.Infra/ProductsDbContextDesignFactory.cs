using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ProductCatalogue.Infrastructure
{
    public class ProductsDbContextDesignFactory : IDesignTimeDbContextFactory<ProductsDbContext>
    {
        public ProductsDbContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
               .SetBasePath(Path.Combine(Directory.GetCurrentDirectory()))
               .AddJsonFile("appsettings.json")
               .Build();

            var optionsBuilder = new DbContextOptionsBuilder<ProductsDbContext>()
                .UseSqlServer(config["ConnectionString"], options => options.MigrationsAssembly(GetType().Assembly.GetName().Name));

            return new ProductsDbContext(optionsBuilder.Options);
        }
    }
}