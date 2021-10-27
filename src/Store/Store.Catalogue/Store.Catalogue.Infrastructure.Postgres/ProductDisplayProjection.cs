using System;
using Store.Catalogue.Application.Product;
using Store.Core.Domain.Event;

namespace Store.Catalogue.Infrastructure.Postgres
{
    public class ProductDisplayProjection : IProjection<ProductDisplay>
    {
        public void Handle(object integrationEvent)
        {
            throw new NotImplementedException();
        }
    }
}