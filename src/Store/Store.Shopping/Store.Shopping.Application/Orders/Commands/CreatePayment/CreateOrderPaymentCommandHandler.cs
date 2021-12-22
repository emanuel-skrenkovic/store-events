using MediatR;
using Store.Core.Domain.ErrorHandling;
using Store.Shopping.Domain;
using Store.Shopping.Domain.Orders;
using Store.Shopping.Domain.Payments;

namespace Store.Shopping.Application.Orders.Commands.CreatePayment;

public class CreateOrderPaymentCommandHandler : IRequestHandler<CreateOrderPaymentCommand, Result>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IPaymentRepository _paymentRepository;

    private readonly IOrderPaymentService _orderPaymentService;

    public CreateOrderPaymentCommandHandler(
        IOrderRepository orderRepository,
        IPaymentRepository paymentRepository,
        IOrderPaymentService orderPaymentService)
    {
        _orderRepository     = orderRepository     ?? throw new ArgumentNullException(nameof(orderRepository));
        _paymentRepository   = paymentRepository   ?? throw new ArgumentNullException(nameof(paymentRepository));
        _orderPaymentService = orderPaymentService ?? throw new ArgumentNullException(nameof(orderPaymentService));
    }
        
    public async Task<Result> Handle(CreateOrderPaymentCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Domain.Orders.Order order = await _orderRepository.GetOrderAsync(request.OrderId);
        if (order == null) return new NotFoundError($"Order with order number {request.OrderId} could not be found.");

        Result<Payment> createPaymentResult = _orderPaymentService.CreateOrderPayment(order);

        // Really don't like this. Might just revert to exceptions
        // even though I don't like exceptions for business logic.
        return createPaymentResult.Then(payment => _paymentRepository.SavePaymentAsync(payment));
    }
}