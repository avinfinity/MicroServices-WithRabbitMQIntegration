using System;
using System.Collections.Generic;
using System.Text;

namespace Pricing.Domain
{
    public class ProductPrice : Entity
    {
        public ProductPrice(Guid productId, decimal price)
        {
            ProductId = productId;
            Price = price;
        }

        public decimal Price { get; private set; }

        public Guid ProductId { get; private set; }

        //Dummy methods to calcuate discounted prices
        public decimal GetDiscountedPrice()//IDiscountedPriceResolver resolver
        { 
            //return resolver.ResolvePriceFor(this);

            return Price; 
        }
    }
}