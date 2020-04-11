using EventBus;
using Microsoft.Extensions.Logging;
using Pricing.API.Events;
using Pricing.Domain;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pricing.API.IntegrationEvents
{
    public class ProductRemovedIntegrationEventHandler : IIntegrationEventHandler<ProductRemovedIntegrationEvent>
    {
        private readonly IProductPriceRepository _productPriceRepo;
        private readonly ILogger<ProductRemovedIntegrationEventHandler> _logger;

        public ProductRemovedIntegrationEventHandler(IProductPriceRepository productPriceRepository,
            ILogger<ProductRemovedIntegrationEventHandler> logger)
        {
            _productPriceRepo = productPriceRepository;
            _logger = logger;
        }

        public async Task Handle(ProductRemovedIntegrationEvent @event)
        {
            using (LogContext.PushProperty("IntegrationEvent", $"{@event.Id}"))
            {
                _logger.LogInformation("Started handling integration event: {IntegrationEventId} at Pricing.API - ({@IntegrationEvent})", @event.Id, @event);

                await _productPriceRepo.RemoveProductPriceAsync(@event.ProductId);
                await _productPriceRepo.UnitOfWork.SaveEntitiesAsync();

                _logger.LogInformation("Completed handling integration event: {IntegrationEventId} at Pricing.API - ({@IntegrationEvent})", @event.Id, @event);
            }
        }
    }
}