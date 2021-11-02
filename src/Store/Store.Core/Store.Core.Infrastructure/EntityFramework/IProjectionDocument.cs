using System;
using Store.Core.Domain;
using Store.Core.Domain.Projection;

namespace Store.Core.Infrastructure.EntityFramework
{
    public interface IProjectionDocument : IReadModel
    {
        DateTime CreatedAt { get; set; }
        
        DateTime UpdatedAt { get; set; }
        
        string Data { get; set; }

        void SerializeData(ISerializer serializer);

        void DeserializeData(ISerializer serializer);
    }
}