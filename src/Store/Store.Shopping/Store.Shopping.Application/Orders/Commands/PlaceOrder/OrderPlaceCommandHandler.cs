using MediatR;
using Store.Core.Domain.ErrorHandling;
using Store.Shopping.Domain;
using Store.Shopping.Domain.Buyers;
using Store.Shopping.Domain.Buyers.ValueObjects;
using Store.Shopping.Domain.Orders;

namespace Store.Shopping.Application.Orders.Commands.PlaceOrder;

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

        BuyerIdentifier buyerId = new(request.CustomerNumber, request.SessionId);
        
        Buyer buyer = await _buyerRepository.GetBuyerAsync(buyerId);
        if (buyer == null) return new NotFoundError($"Customer with customer number {request.CustomerNumber} not found.");

        Result<Order> placeOrderResult = await _buyerOrderService.PlaceOrderAsync(buyer);
        return placeOrderResult.Then(order => _orderRepository.SaveOrderAsync(order));
    }
}