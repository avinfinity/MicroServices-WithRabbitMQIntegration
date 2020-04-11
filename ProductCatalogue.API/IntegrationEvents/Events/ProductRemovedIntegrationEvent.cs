using EventBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductCatalogue.API.IntegrationEvents
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
