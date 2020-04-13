using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using ProductCatalogue.API.Commands;
using ProductCatalogue.API.Controllers;
using ProductCatalogue.API.Queries;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ProductCatalogue.API.FunctionalTests
{
    public class TestProductCatalogue
    {
        [Fact]
        public async void Test_API_GetAllProducts()
        {
            //Arrange
            var catalogController = CreateController();

            //Act
            var products  = await catalogController.GetProductsAsync();

            //Assert
            Assert.NotNull(products);
            var result = Assert.IsType<OkObjectResult>(products.Result);
            var expectedResults = Assert.IsAssignableFrom<IEnumerable<ProductDTO>>(result.Value);
            Assert.True(expectedResults.Count() == 2);
            
            Mock.VerifyAll();
        }

        [Fact]
        public async void Test_API_GetProductById()
        {
            //Arrange
            var catalogController = CreateController();

            //Act
            var product = await catalogController.GetProductByIdAsync(1);

            //Assert
            var result = Assert.IsType<OkObjectResult>(product);
            var expectedResult = Assert.IsAssignableFrom<ProductDTO>(result.Value);
            Assert.True(expectedResult.id == 1);

            Mock.VerifyAll();
        }

        [Fact]
        public async void Test_API_AddNewProduct()
        {
            //Arrange
            var catalogController = CreateController();
            var newProductCommand = new AddNewProductCommand( 10, "Something", 
                string.Empty, string.Empty, 10, 100, string.Empty);
           
            //Act
            var result = await catalogController.AddNewProductAsync(newProductCommand);

            //Assert
            var expectedResult = Assert.IsType<OkObjectResult>(result);
            Assert.True((bool)expectedResult.Value);

            Mock.VerifyAll();
        }

        [Fact]
        public async void Test_API_RemoveProduct()
        {
            //Arrange
            var catalogController = CreateController();
            var result = await catalogController.DeleteProductAsync(1);

            //Act
            var expectedResult = Assert.IsType<OkObjectResult>(result);

            //Assert
            Assert.True((bool)expectedResult.Value);

            Mock.VerifyAll();
        }

        private ProductCatalogueController CreateController()
        {
            var products = new List<ProductDTO>()
            {
                new ProductDTO() { id = 1, name = "First" },
                 new ProductDTO() { id = 2, name = "Second" }
            };

            var queryMock = new Mock<IProductCatalogueQuery>();
            queryMock.Setup(query => query.GetAllProductsAsync()).Returns(Task.FromResult(products.AsEnumerable())).Verifiable();
            queryMock.Setup(query => query.GetProductByIdAsync(1)).Returns(Task.FromResult(products[0]));

            var loggerMock = new Mock<ILogger<ProductCatalogueController>>();
            var mediaterMock = new Mock<IMediator>();

            mediaterMock.Setup(x => x.Send(It.IsAny<IRequest<bool>>(), new System.Threading.CancellationToken())).Returns(Task.FromResult(true)).Verifiable();

            return new ProductCatalogueController(queryMock.Object, loggerMock.Object, mediaterMock.Object);
        }
    }
}