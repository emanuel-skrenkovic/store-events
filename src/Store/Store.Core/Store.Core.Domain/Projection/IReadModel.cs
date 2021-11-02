using System;

namespace Store.Core.Domain.Projection
{
    public interface IReadModel
    {
        Guid Id { get; set; }
    }
}