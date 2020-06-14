using Microsoft.EntityFrameworkCore;
using Pricing.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pricing.Infrastructure
{
    internal class ProductPriceRepository : IProductPriceRepository
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

        public Task<IEnumerable<ProductPrice>> GetAllPrices()
        {
            return Task.FromResult(_priceDbContext.ProductPrices.AsEnumerable());
        }

        public async Task<ProductPrice> GetPriceForProductAsync(Guid productId)
        {
            var price = await _priceDbContext.ProductPrices.FirstOrDefaultAsync(x => x.ProductId == productId);
            return price;
        }

        public async Task<bool> RemoveProductPriceAsync(Guid productId)
        {
            var entity = await _priceDbContext.ProductPrices.FirstOrDefaultAsync(x => x.ProductId == productId);
            if(entity != null)
            {
                _priceDbContext.ProductPrices.Remove(entity);
            }
            
            return entity != null;
        }
    }
}