using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pricing.API
{
    public interface IProductPriceQuery
    {
        Task<IEnumerable<ProductPriceDTO>> GetAllPricesAsync();
        Task<ProductPriceDTO> GetProductPriceAsync(Guid productId);
    }
}