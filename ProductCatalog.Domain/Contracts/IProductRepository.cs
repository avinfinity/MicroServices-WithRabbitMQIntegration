using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ProductCatalogue.Domain
{
    public interface IProductRepository 
        : IRepository<Product>
    {
        Task<IEnumerable<Product>> GetAllProductsAsync();

        Task<Product> GetProductByIdAsync(int productId);

        Task<IEnumerable<Product>> GetProductByStoreAsync(int storeId);

        Task<bool> AddNewProductAsync(Product product);

        Task<int> RemoveProductAsync(int productId);

        Task<Product> RemoveProductAsync(Product product);
    }
}