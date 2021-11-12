using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Store.Core.Domain.Result;
using Store.Order.Domain.Buyers;
using Unit = Store.Core.Domain.Functional.Unit;

namespace Store.Order.Application.Buyer.Commands.RemoveItemFromCart
{
    public class BuyerRemoveItemFromCartCommandHandler : IRequestHandler<BuyerRemoveItemFromCartCommand, Result<Unit>>
    {
        private readonly IBuyerRepository _buyerRepository;

        public BuyerRemoveItemFromCartCommandHandler(IBuyerRepository buyerRepository)
        {
            _buyerRepository = buyerRepository ?? throw new ArgumentNullException(nameof(buyerRepository));
        }
        
        public async Task<Result<Unit>> Handle(BuyerRemoveItemFromCartCommand request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Domain.Buyers.Buyer buyer = await _buyerRepository.GetBuyerAsync(request.CustomerNumber);
            if (buyer == null) return new NotFoundError($"Entity with id {request.CustomerNumber} not found.");
            
            buyer.RemoveCartItem(request.Item);

            await _buyerRepository.SaveBuyerAsync(buyer);

            return Unit.Value;
        }
    }
}