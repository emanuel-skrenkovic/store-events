using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Store.Catalogue.Domain.Product;
using Store.Core.Domain.ErrorHandling;
using Unit = Store.Core.Domain.Functional.Unit;

namespace Store.Catalogue.Application.Product.Command.Availability;

public class ProductSetAvailabilityCommandHandler : IRequestHandler<ProductSetAvailabilityCommand, Result<Unit>>
{
    private readonly IProductRepository _productRepository;

    public ProductSetAvailabilityCommandHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
    }
        
    public async Task<Result<Unit>> Handle(ProductSetAvailabilityCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Domain.Product.Product product = await _productRepository.GetProductAsync(request.ProductId);

        if (product == null)
        {
            return new NotFoundError($"Entity with id {request.ProductId} not found.");
        }

        if (request.ProductAvailable)
        {
            product.MarkAvailable();
        }
        else
        {
            product.MarkUnavailable();
        }

        await _productRepository.SaveProductAsync(product);

        return Unit.Value;
    }
}