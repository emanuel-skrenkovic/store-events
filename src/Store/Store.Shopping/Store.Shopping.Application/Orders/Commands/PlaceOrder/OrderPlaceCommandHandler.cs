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

        (string customerNumber, string sessionId) = request;
        BuyerIdentifier buyerId = new(customerNumber, sessionId);
        
        Result<Buyer> getBuyerResult = await _buyerRepository.GetBuyerAsync(buyerId);
        
        return await getBuyerResult
            .Then(buyer => _buyerOrderService.PlaceOrderAsync(buyer))
            .Then(order => _orderRepository.SaveOrderAsync(order));
    }
}