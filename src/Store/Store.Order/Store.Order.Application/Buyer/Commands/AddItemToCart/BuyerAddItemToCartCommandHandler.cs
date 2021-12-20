using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Store.Core.Domain.ErrorHandling;
using Store.Order.Domain;
using Store.Order.Domain.Buyers;
using Store.Order.Domain.Buyers.ValueObjects;

namespace Store.Order.Application.Buyer.Commands.AddItemToCart
{
    public class BuyerAddItemToCartCommandHandler : IRequestHandler<BuyerAddItemToCartCommand, Result>
    {
        private readonly IBuyerRepository _buyerRepository;

        public BuyerAddItemToCartCommandHandler(IBuyerRepository buyerRepository)
        {
            _buyerRepository = buyerRepository ?? throw new ArgumentNullException(nameof(buyerRepository));
        }
        
        public async Task<Result> Handle(BuyerAddItemToCartCommand request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            BuyerIdentifier buyerId = new(request.CustomerNumber, request.SessionId);
            Domain.Buyers.Buyer buyer = 
                await _buyerRepository.GetBuyerAsync(buyerId) ?? Domain.Buyers.Buyer.Create(buyerId);
            
            // TODO: check if warning correct.
            buyer.AddCartItem(new CatalogueNumber(request.ItemCatalogueNumber));

            await _buyerRepository.SaveBuyerAsync(buyer);

            return Result.Ok();
        }
   }
}