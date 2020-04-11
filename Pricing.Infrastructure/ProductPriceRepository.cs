using Pricing.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pricing.Infrastructure
{
    public class ProductPriceRepository : IProductPriceRepository
    {
        private readonly ProductPriceDbContext _priceDbContext;

        public ProductPriceRepository(ProductPriceDbContext priceDbContext)
        {
            _priceDbContext = priceDbContext;
        }

        public IUnitOfWork UnitOfWork => _priceDbContext;

        public async Task<bool> AddProductPriceAsync(ProductPrice price)
        {
            await _priceDbContext.AddAsync(price);
            return true;
        }

        public async Task<decimal> GetPriceForProductAsync(int productId)
        {
            var product = await _priceDbContext.FindAsync<ProductPrice>(productId);
            return product.Price;
        }

        public async Task<bool> RemoveProductPriceAsync(int productId)
        {
            var entity = await _priceDbContext.FindAsync<ProductPrice>(productId);
            _priceDbContext.ProductPrices.Remove(entity);
            return true;
        }
    }
}