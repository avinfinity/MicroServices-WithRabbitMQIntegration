using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Pricing.API.FunctionalTests
{
    public class PricingFunctionalTests
    {
        [Fact]
        public async void Test_API_GetProductPrice_Valid_ProductId()
        {
            //Arrange
            var productId = Guid.NewGuid();
            var priceController = CreateController(productId);

            //Act
            var price = await priceController.GetProductPriceAsync(productId);

            //Assert
            var result = Assert.IsType<OkObjectResult>(price.Result);
            var expectedResults = Assert.IsAssignableFrom<ProductPriceDTO>(result.Value);
            Assert.True(expectedResults.Price == 1000);

            Mock.VerifyAll();
        }

        [Fact]
        public async void Test_API_GetProductPrice_Invalid_ProductId()
        {
            //Arrange
            var productId = Guid.NewGuid();
            var priceController = CreateController(productId);

            //Act
            var price = await priceController.GetProductPriceAsync(Guid.Empty);

            //Assert
            Assert.IsType<NotFoundResult>(price.Result);
            
            Mock.VerifyAll();
        }

        private ProductPriceController CreateController(Guid includedId)
        {
            var queryMock = new Mock<IProductPriceQuery>();
            queryMock.Setup(x => x.GetProductPriceAsync(includedId)).Returns(Task.FromResult(new ProductPriceDTO() { Price = 1000 }));
            queryMock.Setup(x => x.GetProductPriceAsync(Guid.Empty)).Throws(new Exception(new NotFoundObjectResult(10).ToString()));
            return new ProductPriceController(queryMock.Object);
        }
    }
}