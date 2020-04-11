using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductCatalogue.API.Queries
{
    public interface IProductCatalogueQuery
    {
        Task<IEnumerable<ProductDTO>> GetAllProductsAsync();

        Task<ProductDTO> GetProductByIdAsync(int id);

        Task<IEnumerable<ProductDTO>> GetProductsFromStoreAsync(int storeid);
    }
}