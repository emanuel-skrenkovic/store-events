using MediatR;
using Store.Core.Domain.ErrorHandling;
using Store.Order.Domain;
using Store.Order.Domain.Buyers;
using Store.Order.Domain.Buyers.ValueObjects;
using Store.Order.Domain.Orders;

namespace Store.Order.Application.Order.Commands.PlaceOrder;

public class OrderPlaceCommandHandler : IRequestHandler<OrderPlaceCommand, Result>
{
    private readonly IBuyerRepository _buyerRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IBuyerOrderService _buyerOrderService;

    public OrderPlaceCommandHandler(
        IBuyerRepository buyerRepository, 
        IOrderRepository orderRepository,
        IBuyerOrderService buyerOrderService)
    {
        _buyerRepository = buyerRepository ?? throw new ArgumentNullException(nameof(buyerRepository));
        _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        _buyerOrderService = buyerOrderService ?? throw new ArgumentNullException(nameof(buyerOrderService));
    }
        
    public async Task<Result> Handle(OrderPlaceCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Domain.Buyers.Buyer buyer = await _buyerRepository.GetBuyerAsync(
            new BuyerIdentifier(request.CustomerNumber, request.SessionId));
        if (buyer == null) return new Error($"Customer with customer number {request.CustomerNumber} not found.");

        Result<Domain.Orders.Order> placeOrderResult = await _buyerOrderService.PlaceOrderAsync(buyer);

        return placeOrderResult.Then(order => _orderRepository.SaveOrderAsync(order));
    }
}