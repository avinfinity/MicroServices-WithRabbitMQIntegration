﻿using EventBus;
using IntegrationEventLogService;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductCatalogue.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ProductCatalogue.API.IntegrationEvents
{
    public class ProductCatalogueIntegrationEventService : IProductCatalogueIntegrationEventService
    {
        private ProductsDbContext _productDbContext;
        private readonly IEventBus _eventBus;
        private readonly IIntegrationEventLogService _eventLogService;
        private readonly ILogger<ProductCatalogueIntegrationEventService> _logger;

        public ProductCatalogueIntegrationEventService(IEventBus eventBus, ProductsDbContext productContext,
            Func<DbConnection, List<Type>, IIntegrationEventLogService> integrationEventLogServiceFactory,
            ILogger<ProductCatalogueIntegrationEventService> logger)
        {
            _productDbContext = productContext;
            _eventBus = eventBus;
            var connection = _productDbContext.Database.GetDbConnection();

            var eventTypes = GetType().Assembly.GetTypes()
               .Where(t => t.Name.EndsWith(nameof(IntegrationEvent)))
               .ToList();

            _eventLogService = integrationEventLogServiceFactory(connection, eventTypes);
            _logger = logger;
        }

        public async Task PublishEventsThroughEventBusAsync(Guid transactionId)
        {
            var pendingLogEvents = await _eventLogService.RetrieveEventLogsPendingToPublishAsync(transactionId);

            foreach (var logEvt in pendingLogEvents)
            {
                _logger.LogInformation("Publishing integration event: {IntegrationEventId} from ({@IntegrationEvent})",
                    logEvt.EventId, logEvt.IntegrationEvent);

                try
                {
                    await _eventLogService.MarkEventAsInProgressAsync(logEvt.EventId);
                    _eventBus.Publish(logEvt.IntegrationEvent);
                    await _eventLogService.MarkEventAsPublishedAsync(logEvt.EventId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in publishing integration event: {IntegrationEventId} from ProductCatalogue.API",
                        logEvt.EventId);

                    await _eventLogService.MarkEventAsFailedAsync(logEvt.EventId);
                }
            }
        }

        public async Task AddAndSaveEventAsync(IntegrationEvent evt)
        {
            _logger.LogInformation("Adding integration event {IntegrationEventId} to repository ({@IntegrationEvent})", evt.Id, evt);

            await _eventLogService.SaveEventAsync(evt, _productDbContext.GetCurrentTransaction());
        }
    }
}