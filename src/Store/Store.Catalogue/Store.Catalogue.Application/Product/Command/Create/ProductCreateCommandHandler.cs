using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Store.Catalogue.Domain.Product;
using Store.Core.Domain.Result;

namespace Store.Catalogue.Application.Product.Command.Create
{
    public class ProductCreateCommandHandler : IRequestHandler<ProductCreateCommand, Result<Guid>>
    {
        private readonly IProductRepository _productRepository;

        public ProductCreateCommandHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
        }
        
        public async Task<Result<Guid>> Handle(ProductCreateCommand request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Guid newProductId = Guid.NewGuid(); // TODO
            await _productRepository.CreateProductAsync(Domain.Product.Product.Create(
                newProductId,
                request.Name, 
                request.Price, 
                request.Description));

            return newProductId;
        }
    }
}