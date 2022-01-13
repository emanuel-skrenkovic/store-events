using MediatR;
using Store.Core.Domain;
using Store.Core.Domain.ErrorHandling;
using Store.Shopping.Domain.Orders;
using Store.Shopping.Domain.Orders.ValueObjects;

namespace Store.Shopping.Application.Orders.Commands.SetShippingInformation;

public class OrderSetShippingInformationCommandHandler : IRequestHandler<OrderSetShippingInformationCommand, Result>
{
    private readonly IAggregateRepository _repository;
    
    public OrderSetShippingInformationCommandHandler(IAggregateRepository repository)
        => _repository = Ensure.NotNull(repository);
    
    public Task<Result> Handle(OrderSetShippingInformationCommand request, CancellationToken cancellationToken) =>
        _repository.GetAsync<Order, Guid>(request.OrderId)
            .Then(order => ValidateShippingInfo(request.ShippingInfo)
                .Then(si =>
                {
                    order.SetShippingInformation(si);
                    return _repository.SaveAsync<Order, Guid>(order);
                }));

    private Result<ShippingInformation> ValidateShippingInfo(ShippingInfo shippingInfo) 
        => ShippingInformation.Create(
            shippingInfo.CountryCode, 
            shippingInfo.FullName, 
            shippingInfo.StreetAddress,
            shippingInfo.City,
            shippingInfo.StateProvince, 
            shippingInfo.Postcode, 
            shippingInfo.PhoneNumber);
}