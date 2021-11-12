using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Store.Core.Domain.Result;
using Store.Order.Domain.Buyers;
using Unit = Store.Core.Domain.Functional.Unit;

namespace Store.Order.Application.Buyer.Commands.AddItemToCart
{
    public class BuyerAddItemToCartCommandHandler : IRequestHandler<BuyerAddItemToCartCommand, Result<Unit>>
    {
        private readonly IBuyerRepository _buyerRepository;

        public BuyerAddItemToCartCommandHandler(IBuyerRepository buyerRepository)
        {
            _buyerRepository = buyerRepository ?? throw new ArgumentNullException(nameof(buyerRepository));
        }
        
        public async Task<Result<Unit>> Handle(BuyerAddItemToCartCommand request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Domain.Buyers.Buyer buyer = await _buyerRepository.GetBuyerAsync(request.CustomerNumber)
                ?? Domain.Buyers.Buyer.Create(request.CustomerNumber);
            
            // TODO: check if warning correct.
            buyer.AddCartItem(request.Item);

            await _buyerRepository.SaveBuyerAsync(buyer);

            return Unit.Value;
        }
   }
}