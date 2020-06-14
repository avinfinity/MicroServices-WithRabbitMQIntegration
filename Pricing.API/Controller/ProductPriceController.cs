using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pricing.API
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    public class ProductPriceController : ControllerBase
    {
        private readonly IProductPriceQuery _productPriceQuery;

        public ProductPriceController(IProductPriceQuery productPriceQuery)
        {
            _productPriceQuery = productPriceQuery;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductPriceDTO>>> GetAllPricesAsync()
        {
            var result = await _productPriceQuery.GetAllPricesAsync();
            return Ok(result);
        }

        [HttpGet]
        [Route("{productId:Guid}")]
        public async Task<ActionResult<ProductPriceDTO>> GetProductPriceAsync(Guid productId)
        {
            try
            {
                var result = await _productPriceQuery.GetProductPriceAsync(productId);
                return Ok(result);
            }
            catch
            {
                return NotFound();
            }
            
        }
    }
}