using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pricing.Domain;

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
        public ActionResult<bool> GetProductPriceAsync()
        {
            //var result = await _productPriceQuery.GetProductPriceAsync(productId);
            return Ok(true);
        }

        [HttpGet]
        [Route("{productId:int}")]
        public async Task<ActionResult<ProductPrice>> GetProductPriceAsync(int productId)
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