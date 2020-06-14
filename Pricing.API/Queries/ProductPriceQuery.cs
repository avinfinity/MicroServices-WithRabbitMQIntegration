using AutoMapper;
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
        private readonly IMapper _mapper;

        public ProductPriceQuery(IProductPriceRepository productRepository, IMapper mapper)
        {
            _productRepo = productRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductPriceDTO>> GetAllPricesAsync()
        {
            var prices = await _productRepo.GetAllPrices();
            return _mapper.Map<IEnumerable<ProductPriceDTO>>(prices);
        }

        public async Task<ProductPriceDTO> GetProductPriceAsync(Guid productId)
        {
            var price = await _productRepo.GetPriceForProductAsync(productId);
            return _mapper.Map<ProductPriceDTO>(price);
        }
    }
}