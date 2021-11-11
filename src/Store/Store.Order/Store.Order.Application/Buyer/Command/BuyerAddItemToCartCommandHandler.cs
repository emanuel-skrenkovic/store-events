using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Store.Core.Domain.Result;
using Unit = Store.Core.Domain.Functional.Unit;

namespace Store.Order.Application.Buyer.Command
{
    public class BuyerAddItemToCartCommandHandler : IRequestHandler<BuyerAddItemToCartCommand, Result<Unit>>
    {
        
        public Task<Result<Unit>> Handle(BuyerAddItemToCartCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}