using Pricing.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pricing.API
{
    public class ProductPriceQuery : IProductPriceQuery
    {
        private readonly IProductPriceRepository _productRepo;

        public ProductPriceQuery(IProductPriceRepository productRepository)
        {
            _productRepo = productRepository;
        }

        public async Task<ProductPriceDTO> GetProductPriceAsync(int productId)
        {
            var price = await _productRepo.GetPriceForProductAsync(productId);
            return new ProductPriceDTO() { Price = price };
        }
    }
}