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
        
    public Task<Result> Handle(CreateOrderPaymentCommand request, CancellationToken cancellationToken)
         => _orderRepository.GetOrderAsync(request.OrderId)
            .Then(order => _orderPaymentService.CreateOrderPayment(order))
            .Then(payment => _paymentRepository.SavePaymentAsync(payment));
}