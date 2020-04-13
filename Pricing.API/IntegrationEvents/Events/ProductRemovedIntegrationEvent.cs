using EventBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pricing.API.Events
{
    public class ProductRemovedIntegrationEvent : IntegrationEvent
    {
        public ProductRemovedIntegrationEvent(Guid productId)
        {
            ProductId = productId;
        }

        public Guid ProductId { get; }
    }
}
