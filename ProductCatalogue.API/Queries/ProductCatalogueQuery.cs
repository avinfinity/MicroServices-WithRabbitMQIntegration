using AutoMapper;
using ProductCatalogue.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductCatalogue.API.Queries
{
    public class ProductCatalogueQuery : IProductCatalogueQuery
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public ProductCatalogueQuery(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductDTO>> GetAllProductsAsync()
        {
            var products = await _productRepository.GetAllProductsAsync();
            return _mapper.Map<IEnumerable<ProductDTO>>(products);
        }

        public async Task<ProductDTO> GetProductByIdAsync(int id)
        {
            var product = await _productRepository.GetProductByIdAsync(id);
            return _mapper.Map<ProductDTO>(product);
        }

        public async Task<IEnumerable<ProductDTO>> GetProductsFromStoreAsync(int storeid)
        {
            var products = await _productRepository.GetProductByStoreAsync(storeid);
            return _mapper.Map<IEnumerable<ProductDTO>>(products);
        }
    }
}
