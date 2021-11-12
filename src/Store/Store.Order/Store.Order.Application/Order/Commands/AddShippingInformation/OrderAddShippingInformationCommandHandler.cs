using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Store.Core.Domain.Result;
using Store.Order.Domain.Orders;
using Unit = Store.Core.Domain.Functional.Unit;

namespace Store.Order.Application.Order.Commands.AddShippingInformation
{
    public class OrderAddShippingInformationCommandHandler : IRequestHandler<OrderAddShippingInformationCommand, Result<Unit>>
    {
        private readonly IOrderRepository _orderRepository;

        public OrderAddShippingInformationCommandHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        }
        
        public async Task<Result<Unit>> Handle(OrderAddShippingInformationCommand request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Domain.Orders.Order order = await _orderRepository.GetOrderAsync(request.OrderNumber);
            if (order == null) return new NotFoundError($"Entity with id {request.OrderNumber} not found.");
            
            order.SetShippingInformation(request.ShippingInformation);

            return Unit.Value;
        }
    }
}