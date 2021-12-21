using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Store.Catalogue.Domain.Product;

namespace Store.Catalogue.Application.Product.Command.AdjustPrice;

public class ProductAdjustPriceCommandHandler : IRequestHandler<ProductAdjustPriceCommand>
{
    private readonly IProductRepository _productRepository;

    public ProductAdjustPriceCommandHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
    }
        
    public async Task<Unit> Handle(ProductAdjustPriceCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Domain.Product.Product product = await _productRepository.GetProductAsync(request.ProductId);

        if (product == null)
        {
            // TODO
        }
            
        product.ChangePrice(request.NewPrice, request.Reason);

        await _productRepository.SaveProductAsync(product);

        return Unit.Value;
    }
}