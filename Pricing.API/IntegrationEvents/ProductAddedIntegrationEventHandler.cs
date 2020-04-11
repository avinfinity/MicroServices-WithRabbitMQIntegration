using EventBus;
using Microsoft.Extensions.Logging;
using Pricing.API.Events;
using Pricing.Domain;
using Serilog.Context;
using System.Threading.Tasks;

namespace Pricing.API.IntegrationEvents
{
    public class ProductAddedIntegrationEventHandler : IIntegrationEventHandler<ProductAddedIntegrationEvent>
    {
        private readonly IProductPriceRepository _productPriceRepo;
        private readonly ILogger<ProductAddedIntegrationEventHandler> _logger;

        public ProductAddedIntegrationEventHandler(IProductPriceRepository productPriceRepository, 
            ILogger<ProductAddedIntegrationEventHandler> logger)
        {
            _productPriceRepo = productPriceRepository;
            _logger = logger;
        }

        public async Task Handle(ProductAddedIntegrationEvent @event)
        {
            using (LogContext.PushProperty("IntegrationEvent", $"{@event.Id}"))
            {
                _logger.LogInformation("Started handling integration event: {IntegrationEventId} at Pricing.API - ({@IntegrationEvent})", @event.Id, @event);

                await _productPriceRepo.AddProductPriceAsync(new ProductPrice(@event.ProductId, @event.UnitPrice));
                await _productPriceRepo.UnitOfWork.SaveEntitiesAsync();

                _logger.LogInformation("Completed handling integration event: {IntegrationEventId} at Pricing.API - ({@IntegrationEvent})", @event.Id, @event);
            }
        }
    }
}