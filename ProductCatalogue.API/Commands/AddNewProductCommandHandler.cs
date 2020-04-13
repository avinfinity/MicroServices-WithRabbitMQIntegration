using AutoMapper;
using MediatR;
using ProductCatalogue.API.Events;
using ProductCatalogue.API.IntegrationEvents;
using ProductCatalogue.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ProductCatalogue.API.Commands
{
    public class AddNewProductCommandHandler : IRequestHandler<AddNewProductCommand, bool>
    {
        private readonly IProductRepository _productRepository;
        private readonly IProductCatalogueIntegrationEventService _eventService;

        public AddNewProductCommandHandler(IProductRepository productRepository, 
            IProductCatalogueIntegrationEventService eventService)
        {
            _productRepository = productRepository;
            _eventService = eventService;
        }

        public async Task<bool> Handle(AddNewProductCommand request, CancellationToken cancellationToken)
        {
            var product = new Product(request.ProductId, request.StoreId, request.Name, request.Description,
               request.ProductCategory, request.Units, request.UnitPrice, request.PictureUrl);

            var productAddedIntegrationEvent = new ProductAddedIntegrationEvent(product.ProductId,request.Units,request.UnitPrice);
            await _eventService.AddAndSaveEventAsync(productAddedIntegrationEvent);

            await _productRepository.AddNewProductAsync(product);

            return await _productRepository.UnitOfWork
                .SaveEntitiesAsync(cancellationToken);
        }
    }
}