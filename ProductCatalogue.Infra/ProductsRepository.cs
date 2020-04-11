using Dapper;
using Microsoft.EntityFrameworkCore;
using ProductCatalogue.Domain;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;

namespace ProductCatalogue.Infrastructure
{
    public class ProductsRepository : IProductRepository
    {
        private readonly ProductsDbContext _productsDbContext;

        public ProductsRepository(ProductsDbContext productsDbContext)
        {
            _productsDbContext = productsDbContext;
        }

        public IUnitOfWork UnitOfWork => _productsDbContext;

        public async Task<bool> AddNewProductAsync(Product product)
        {
            await _productsDbContext.Products.AddAsync(product);
            return true;
        }

        public Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            //1. Direct query execution -- This is always faster than EF Linq to SQL queries

            //var connection = GetConnection();

            //return connection.QueryAsync<Product>("Select * FROM Products");

            //2. using product db context - EF Linq to SQL query
            return Task.FromResult(_productsDbContext.Products as IEnumerable<Product>);
        }

        public Task<Product> GetProductByIdAsync(int productId)
        {
            var connection = GetConnection();

            return connection.QueryFirstAsync<Product>("Select * FROM products.Products WHERE Id = @productId", new { productId });
        }

        public Task<IEnumerable<Product>> GetProductByStoreAsync(int storeId)
        {
            var connection = GetConnection();

            return connection.QueryAsync<Product>("Select * FROM products.Products WHERE StoreId = @storeId", new { storeId });
        }

        public Task<int> RemoveProductAsync(int productId)
        {
            var connection = GetConnection();

            return connection.ExecuteAsync("DELETE FROM products.Products WHERE Id = @productId", new { productId });
        }

        public Task<Product> RemoveProductAsync(Product product)
        {
            return Task.FromResult(_productsDbContext.Products.Remove(product).Entity);
        }

        private DbConnection GetConnection()
        {
            var connection = _productsDbContext.Database.GetDbConnection();

            if (connection.State != System.Data.ConnectionState.Open)
            {
                connection.Open();
            }

            return connection;
        }
    }
}
