using System;
using MediatR;
using Store.Core.Domain.Result;

namespace Store.Catalogue.Application.Product.Command.Create
{
    public record ProductCreateCommand(string Name, decimal Price, string Description = null) : IRequest<Result<Guid>>;
}