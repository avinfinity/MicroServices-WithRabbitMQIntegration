using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using Pricing.API;
using Pricing.Domain;
using Pricing.Infrastructure;
using ProductCatalogue.API.Commands;
using ProductCatalogue.API.Queries;
using ProductCatalogue.Domain;
using ProductCatalogue.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using Xunit;

namespace IntegrationUnitTests
{
    public class Servers : IDisposable
    {
        public Servers()
        {
            ProductCatalogAPIServer = ServerSetups.CreateServer<ProductsDbContext, ProductCatalogue.API.Startup, ProductDTO, Product>("TestData//ProductsTestData.json",true);
            PricingAPIServer = ServerSetups.CreateServer<ProductPriceDbContext, Pricing.API.Startup, ProductPriceDTO, ProductPrice>("TestData//PricesTestData.json");
        }

        public TestServer ProductCatalogAPIServer { get; private set; }

        public TestServer PricingAPIServer { get; private set; }

        public void Dispose()
        {
            //Delete database after unit tests
            var dbContext = ProductCatalogAPIServer.Services.GetService(typeof(ProductsDbContext)) as ProductsDbContext;
            dbContext.Database.EnsureDeleted();

            ProductCatalogAPIServer.Dispose();
            PricingAPIServer.Dispose();
        }
    }

    public class IntegrationTests : IClassFixture<Servers>
    {
        private readonly Servers _servers;

        public IntegrationTests(Servers server)
        {
            _servers = server;
        }

        [Fact]
        public async void Test_GetAllProductsFromCatalogue()
        {
            using (var client = _servers.ProductCatalogAPIServer.CreateClient())
            {
                var response = await client.GetAsync("api/v1/ProductCatalogue/");
                var items = await response.Content.ReadAsStringAsync();
                var products = JsonConvert.DeserializeObject<IEnumerable<ProductDTO>>(items);

                var testData = JsonConvert.DeserializeObject<IEnumerable<ProductDTO>>(File.ReadAllText("TestData//ProductsTestData.json"));

                Assert.True(products.Count() >= testData.Count());

                for (int i = 0; i < products.Count(); i++)
                {
                    Assert.True(products.ElementAt(i).productid == testData.ElementAt(i).productid);
                }
            }
        }

        [Fact]
        public async void Test_AddProductTo_ProductAPI_And_QueryProductPriceFrom_PricingAPI()
        {
            //Arrange
            //Both below servers and clients DO NOT talk to each other directly or via REST.
            //All communication is via EventBus - RabbitMQ over AMQP protocol
            using (var productClient = _servers.ProductCatalogAPIServer.CreateClient())
            {
                using (var priceClient = _servers.PricingAPIServer.CreateClient())
                {
                    //Act
                    var addNewProductCommand = new AddNewProductCommand(10, "Addedproduct", "", "", 100, new decimal(12.25), "");
                    var content = new StringContent(JsonConvert.SerializeObject(addNewProductCommand), Encoding.UTF8, "application/json");

                    //1. Add a new product to product catalog microservice
                    var productResponse = await productClient.PostAsync("api/v1/ProductCatalogue/new", content);
                    //2. Query for price from pricing microservice for added product
                    var priceResponse = await priceClient.GetAsync("api/v1/ProductPrice/" + addNewProductCommand.ProductId.ToString("D"));

                    //Assert
                    Assert.True(productResponse.StatusCode == System.Net.HttpStatusCode.OK);

                    var items = await priceResponse.Content.ReadAsStringAsync();
                    var price = JsonConvert.DeserializeObject<ProductPrice>(items);

                    //ProductId is equal, which shows that after adding to ProductCatalog microservice, 
                    //this product is also avaliable within Pricing microservices over integration events via RabbitMQ Event Bus
                    Assert.True(price.ProductId == addNewProductCommand.ProductId);
                    Assert.True((price.Price - addNewProductCommand.UnitPrice) < new decimal(0.0000000000001)); //Prices are equal
                }
            }
        }
    }
}