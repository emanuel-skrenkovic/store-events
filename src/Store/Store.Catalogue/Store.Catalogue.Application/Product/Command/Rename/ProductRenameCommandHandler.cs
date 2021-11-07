using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Store.Catalogue.Domain.Product;
using Store.Core.Domain.Result;
using Unit = Store.Core.Domain.Functional.Unit;

namespace Store.Catalogue.Application.Product.Command.Rename
{
    public class ProductRenameCommandHandler : IRequestHandler<ProductRenameCommand, Result<Unit>>
    {
        private readonly IProductRepository _productRepository;

        public ProductRenameCommandHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
        }
        
        public async Task<Result<Unit>> Handle(ProductRenameCommand request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Guid productId = request.ProductId;
            
            Domain.Product.Product product = await _productRepository.GetProductAsync(productId);
            if (product == null)
            {
                return new NotFoundError($"Entity with id {productId} not found.");
            }

            product.Rename(request.Name);
            await _productRepository.SaveProductAsync(product);
            
            return Unit.Value;
        }
    }
}