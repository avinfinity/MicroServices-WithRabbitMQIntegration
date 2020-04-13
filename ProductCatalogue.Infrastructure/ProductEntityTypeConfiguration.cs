using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductCatalogue.Domain;
using System;

namespace ProductCatalogue.Infrastructure
{
    internal class ProductEntityTypeConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> productConfig)
        {
            productConfig.ToTable("Products", ProductsDbContext.DEFAULT_SCHEMA);

            productConfig.HasKey(o => o.Id);

            productConfig.Property(o => o.Id).ValueGeneratedOnAdd();

            productConfig
                .Property<int>("StoreId")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("StoreId")
                .IsRequired(true);

            productConfig
                .Property<Guid>("ProductId")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("ProductId")
                .IsRequired(true);

            productConfig
                .Property<string>("Name")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("Name")
                .IsRequired();

            productConfig
                .Property<string>("Description")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("Description")
                .IsRequired(false);

            productConfig
                .Property<string>("ProductCategory")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("ProductCategory")
                .IsRequired(false);

            productConfig
               .Property<int>("Units")
               .UsePropertyAccessMode(PropertyAccessMode.Field)
               .HasColumnName("Units")
               .IsRequired();

            productConfig
              .Property<decimal>("UnitPrice")
              .UsePropertyAccessMode(PropertyAccessMode.Field)
              .HasColumnName("UnitPrice")
              .IsRequired();

            productConfig
              .Property<string>("PicturePath")
              .UsePropertyAccessMode(PropertyAccessMode.Field)
              .HasColumnName("PicturePath")
              .IsRequired(false);
        }
    }
}