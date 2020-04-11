using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProductCatalogue.API.Commands;
using ProductCatalogue.API.Queries;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace ProductCatalogue.API.Controllers
{
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class ProductCatalogueController : ControllerBase
    {
        private readonly IProductCatalogueQuery _productQuery;
        private readonly ILogger<ProductCatalogueController> _logger;
        private readonly IMediator _mediator;

        public ProductCatalogueController(IProductCatalogueQuery productQuery, 
            ILogger<ProductCatalogueController> logger, IMediator mediator)
        {
            _productQuery = productQuery;
            _logger = logger;
            _mediator = mediator;
        }

        [HttpGet]
        [Route("{productId:int}")]
        [ProducesResponseType(typeof(ProductDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult> GetProductByIdAsync(int productId)
        {
            try
            {
                var product =  await _productQuery.GetProductByIdAsync(productId);
                return Ok(product);
            }
            catch
            {
                return NotFound();
            }
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ProductDTO>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProductsAsync()
        {
            var products = await _productQuery.GetAllProductsAsync();
            return Ok(products);
        }

        [HttpGet]
        [Route("store/{storeId:int}")]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProductsByStoreAsync(int storeId)
        {
            try
            {
                var products = await _productQuery.GetProductsFromStoreAsync(storeId);
                return Ok(products);
            }
            catch
            {
                return NotFound();
            }
        }

        [HttpPost]
        [Route("new")]
        public async Task<IActionResult> AddNewProductAsync([FromBody]AddNewProductCommand command)
        {
            if(command == null)
            {
                return BadRequest();
            }

            _logger.LogInformation($"Sending command for adding product with name {command.Name}");

            var commandResult = await _mediator.Send(command);

            _logger.LogInformation($"Command handled successfuly for adding product with name {command.Name}");

            return Ok(commandResult);
        }

        [HttpDelete]
        [Route("{productId:int}")]
        public async Task<IActionResult> DeleteProductAsync(int productId)
        {
            var removeProductCommand = new RemoveProductCommand(productId);

            _logger.LogInformation($"Sending command for deleting product with id {productId}");

            var commandResult = await _mediator.Send(removeProductCommand);

            _logger.LogInformation($"Command handled successfuly for removing product with id {productId}");
            
            return Ok(commandResult);
        }
    }
}