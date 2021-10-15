using System;
using MediatR;

namespace Store.Catalogue.Application.Product.Command.AdjustPrice
{
    public record ProductAdjustPriceCommand(Guid ProductId, decimal NewPrice) : IRequest;
}