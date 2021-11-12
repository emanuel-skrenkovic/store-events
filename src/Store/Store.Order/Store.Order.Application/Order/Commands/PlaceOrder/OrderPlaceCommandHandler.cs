using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Store.Core.Domain;
using Store.Core.Domain.Result;
using Store.Core.Domain.Result.Extensions;
using Store.Order.Domain;
using Store.Order.Domain.Buyers;
using Store.Order.Domain.Orders;
using Unit = Store.Core.Domain.Functional.Unit;

namespace Store.Order.Application.Order.Commands.PlaceOrder
{
    public class OrderPlaceCommandHandler : IRequestHandler<OrderPlaceCommand, Result<Unit>>
    {
        private readonly IBuyerRepository _buyerRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly ICustomerOrderService _customerOrderService;

        public OrderPlaceCommandHandler(
            IBuyerRepository buyerRepository, 
            IOrderRepository orderRepository,
            ICustomerOrderService customerOrderService)
        {
            _buyerRepository = buyerRepository ?? throw new ArgumentNullException(nameof(buyerRepository));
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _customerOrderService = customerOrderService ?? throw new ArgumentNullException(nameof(customerOrderService));
        }
        
        public async Task<Result<Unit>> Handle(OrderPlaceCommand request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Domain.Buyers.Buyer buyer = await _buyerRepository.GetBuyerAsync(request.CustomerNumber);
            if (buyer == null) return new Error($"Customer with customer number {request.CustomerNumber} not found.");

            Result<Domain.Orders.Order> placeOrderResult = _customerOrderService.PlaceOrder(buyer);

            return await placeOrderResult.Bind<Domain.Orders.Order, Unit>(async order =>
            {
                await _orderRepository.SaveOrderAsync(order);
                return Unit.Value; 
            });
        }
    }
}