using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace ProductCatalogue.Infrastructure
{
    internal class ProductsDbContextDesignFactory : IDesignTimeDbContextFactory<ProductsDbContext>
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