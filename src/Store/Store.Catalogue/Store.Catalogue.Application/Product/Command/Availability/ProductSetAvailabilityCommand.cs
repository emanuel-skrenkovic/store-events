using System;
using MediatR;
using Store.Core.Domain.Result;
using Unit = Store.Core.Domain.Functional.Unit;

namespace Store.Catalogue.Application.Product.Command.Availability
{
    public record ProductSetAvailabilityCommand(Guid ProductId, bool ProductAvailable) : IRequest<Result<Unit>>;
}