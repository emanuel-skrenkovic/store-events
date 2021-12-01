using System;
using MediatR;
using Store.Core.Domain;
using Store.Core.Domain.ErrorHandling;
using Unit = Store.Core.Domain.Functional.Unit;

namespace Store.Catalogue.Application.Product.Command.Rename
{
    public record ProductRenameCommand : IRequest<Result<Unit>>
    {
        public Guid ProductId { get; set; }
        
        public string Name { get; set;  }
    }
}