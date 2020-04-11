using EventBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pricing.API.Events
{
    public class ProductAddedIntegrationEvent : IntegrationEvent
    {
        public ProductAddedIntegrationEvent(Guid productId, int quantity, decimal unitPrice)
        {
            ProductId = productId;
            Quantity = quantity;
            UnitPrice = unitPrice;
        }

        public Guid ProductId { get; }

        public int Quantity { get; }

        public decimal UnitPrice { get; }
    }
}