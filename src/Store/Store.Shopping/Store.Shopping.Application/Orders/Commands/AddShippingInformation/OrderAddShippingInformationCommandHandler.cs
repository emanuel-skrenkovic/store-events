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

        Order order = await _orderRepository.GetOrderAsync(request.OrderId);
        if (order == null) return new NotFoundError($"Entity with id {request.OrderId} not found.");
            
        order.SetShippingInformation(request.ShippingInformation);

        await _orderRepository.SaveOrderAsync(order);

        return Result.Ok();
    }
}