using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pricing.Domain
{
    public interface IProductPriceRepository : IRepository
    {
        Task<decimal> GetPriceForProductAsync(int productId);

        Task<bool> AddProductPriceAsync(ProductPrice price);

        Task<bool> RemoveProductPriceAsync(int productId);
    }
}