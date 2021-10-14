using MediatR;

namespace Store.Catalogue.Application.Product.Command.Create
{
    public record ProductCreateCommand(string Name, decimal Price, string Description = null) : IRequest;
}