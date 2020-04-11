using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pricing.API
{
    public interface IProductPriceQuery
    {
        Task<ProductPriceDTO> GetProductPriceAsync(int productId);
    }
}