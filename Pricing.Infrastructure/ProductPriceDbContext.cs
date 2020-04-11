using Microsoft.EntityFrameworkCore;
using Pricing.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pricing.Infrastructure
{
    public class ProductPriceDbContext : DbContext, IUnitOfWork
    {
        public const string DEFAULT_SCHEMA = "prices";
        public DbSet<ProductPrice> ProductPrices { get; set; }

        public ProductPriceDbContext(DbContextOptions<ProductPriceDbContext> options)
           : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ProductPriceEntityConfiguration());
        }

        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var result = await base.SaveChangesAsync(cancellationToken);
            return result != 0;
        }
    }
}