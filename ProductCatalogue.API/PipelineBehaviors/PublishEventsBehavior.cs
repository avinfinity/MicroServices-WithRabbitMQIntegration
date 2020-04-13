using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ProductCatalogue.API.IntegrationEvents;
using ProductCatalogue.Infrastructure;
using Serilog.Context;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ProductCatalogue.API.PipelineBehaviors
{

    public class PublishEventsBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ILogger<PublishEventsBehavior<TRequest, TResponse>> _logger;
        private readonly ProductsDbContext _dbContext;
        private readonly IProductCatalogueIntegrationEventService _integartionService;

        public PublishEventsBehavior(ProductsDbContext dbContext,
            IProductCatalogueIntegrationEventService integrationService,
            ILogger<PublishEventsBehavior<TRequest, TResponse>> logger)
        {
            _dbContext = dbContext;
            _integartionService = integrationService;
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var response = default(TResponse);
            var typeName = request.GetType().Name;

            try
            {
                if (_dbContext.HasActiveTransaction)
                {
                    return await next();
                }

                var strategy = _dbContext.Database.CreateExecutionStrategy();

                await strategy.ExecuteAsync(async () =>
                {
                    Guid transactionId;

                    using (var transaction = await _dbContext.BeginTransactionAsync())
                    {
                        using (LogContext.PushProperty("TransactionContext", transaction.TransactionId))
                        {
                            _logger.LogInformation("Transaction started with {TransactionId} for {CommandName} ({@Command})", 
                                transaction.TransactionId, typeName, request);

                            response = await next();

                            _logger.LogInformation("Transaction committed with {TransactionId} for {CommandName}", 
                                transaction.TransactionId, typeName);

                            await _dbContext.CommitTransactionAsync(transaction);

                            transactionId = transaction.TransactionId;
                        }
                    }

                    await _integartionService.PublishEventsThroughEventBusAsync(transactionId);
                });

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in handling transaction for {CommandName} ({@Command})", typeName, request);

                throw;
            }
        }
    }
}