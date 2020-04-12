using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Pricing.Infrastructure
{
    internal class ProductPriceDbDesignFactory : IDesignTimeDbContextFactory<ProductPriceDbContext>
    {
        public ProductPriceDbContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
               .SetBasePath(Path.Combine(Directory.GetCurrentDirectory()))
               .AddJsonFile("appsettings.json")
               .Build();

            var optionsBuilder = new DbContextOptionsBuilder<ProductPriceDbContext>()
                .UseSqlServer(config["ConnectionString"]);

            return new ProductPriceDbContext(optionsBuilder.Options);
        }
    }
}