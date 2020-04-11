using AutoMapper;
using MediatR;
using ProductCatalogue.API.IntegrationEvents;
using ProductCatalogue.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ProductCatalogue.API.Commands
{
    public class RemoveProductCommandHandler : IRequestHandler<RemoveProductCommand,bool>
    {
        private readonly IProductRepository _productRepository;
        private readonly IProductCatalogueIntegrationEventService _eventService;

        public RemoveProductCommandHandler(IProductRepository productRepository,
            IProductCatalogueIntegrationEventService eventService)
        {
            _productRepository = productRepository;
            _eventService = eventService;
        }

        public async Task<bool> Handle(RemoveProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetProductByIdAsync(request.Id);
            var productRemovedIntegrationEvent = new ProductRemovedIntegrationEvent(product.ProductId);

            await _eventService.AddAndSaveEventAsync(productRemovedIntegrationEvent);

            await _productRepository.RemoveProductAsync(product);

            return await _productRepository.UnitOfWork
                .SaveEntitiesAsync(cancellationToken);
        }
    }
}