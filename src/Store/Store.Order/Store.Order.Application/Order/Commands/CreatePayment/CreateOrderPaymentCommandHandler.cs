using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Store.Core.Domain.Result;
using Store.Core.Domain.Result.Extensions;
using Store.Order.Domain;
using Store.Order.Domain.Orders;
using Store.Order.Domain.Payment;
using Unit = Store.Core.Domain.Functional.Unit;

namespace Store.Order.Application.Order.Commands.CreatePayment
{
    public class CreateOrderPaymentCommandHandler : IRequestHandler<CreateOrderPaymentCommand, Result<Unit>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IPaymentRepository _paymentRepository;

        private readonly IOrderPaymentService _orderPaymentService;

        public CreateOrderPaymentCommandHandler(
            IOrderRepository orderRepository,
            IPaymentRepository paymentRepository,
            IOrderPaymentService orderPaymentService)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _paymentRepository = paymentRepository ?? throw new ArgumentNullException(nameof(paymentRepository));
            _orderPaymentService = orderPaymentService ?? throw new ArgumentNullException(nameof(orderPaymentService));
        }
        
        public async Task<Result<Unit>> Handle(CreateOrderPaymentCommand request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Domain.Orders.Order order = await _orderRepository.GetOrderAsync(request.OrderNumber);
            if (order == null) return new NotFoundError($"Order with order number {request.OrderNumber} could not be found.");

            Result<Payment> createPaymentResult = _orderPaymentService.CreateOrderPayment(order);

            // Really don't like this. Might just revert to exceptions
            // even though I don't like exceptions for business logic.
            return await createPaymentResult.Bind<Payment, Unit>(async payment =>
            {
                await _paymentRepository.SavePaymentAsync(payment);
                return Unit.Value;
            });
        }
    }
}