using MediatR;
using Store.Core.Domain.ErrorHandling;
using Store.Shopping.Domain.Orders;

namespace Store.Shopping.Application.Orders.Commands.AddShippingInformation;

public class OrderAddShippingInformationCommandHandler : IRequestHandler<OrderAddShippingInformationCommand, Result>
{
    private readonly IOrderRepository _orderRepository;

    public OrderAddShippingInformationCommandHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
    }
        
    public async Task<Result> Handle(OrderAddShippingInformationCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Result<Order> getOrderResult = await _orderRepository.GetOrderAsync(request.OrderId);
        return await getOrderResult.Then(order =>
        {
            order.SetShippingInformation(request.ShippingInformation); 
            return _orderRepository.SaveOrderAsync(order);
        });
    }
}