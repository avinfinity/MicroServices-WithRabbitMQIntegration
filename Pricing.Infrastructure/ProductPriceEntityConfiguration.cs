using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pricing.Domain;
using System;

namespace Pricing.Infrastructure
{
    class ProductPriceEntityConfiguration : IEntityTypeConfiguration<ProductPrice>
    {
        public void Configure(EntityTypeBuilder<ProductPrice> productConfig)
        {
            productConfig.ToTable("Prices", ProductPriceDbContext.DEFAULT_SCHEMA);

            productConfig.HasKey(o => o.Id);

            productConfig.Property(o => o.Id).ValueGeneratedOnAdd();

            productConfig
                .Property<decimal>("Price")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("Price")
                .IsRequired(true);

            productConfig
                .Property<Guid>("ProductId")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("ProductId")
                .IsRequired(true);
        }
    }
}