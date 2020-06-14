using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pricing.Domain
{
    public interface IProductPriceRepository : IRepository
    {
        Task<IEnumerable<ProductPrice>> GetAllPrices();

        Task<ProductPrice> GetPriceForProductAsync(Guid productId);

        Task<bool> AddProductPriceAsync(ProductPrice price);

        Task<bool> RemoveProductPriceAsync(Guid productId);
    }
}