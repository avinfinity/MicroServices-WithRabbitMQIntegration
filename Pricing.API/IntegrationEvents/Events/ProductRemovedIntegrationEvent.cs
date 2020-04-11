using EventBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pricing.API.Events
{
    public class ProductRemovedIntegrationEvent : IntegrationEvent
    {
        public ProductRemovedIntegrationEvent(int productId)
        {
            ProductId = productId;
        }

        public int ProductId { get; }
    }
}
