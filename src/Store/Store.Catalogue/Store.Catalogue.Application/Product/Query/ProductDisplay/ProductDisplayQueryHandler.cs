using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Store.Catalogue.Infrastructure;
using Store.Core.Domain;

namespace Store.Catalogue.Application.Product.Query.ProductDisplay
{
    public class ProductDisplayQueryHandler : IRequestHandler<ProductDisplayQuery, ProductDto>
    {
        private readonly ISerializer _serializer;
        private readonly StoreCatalogueDbContext _context;

        public ProductDisplayQueryHandler(ISerializer serializer, StoreCatalogueDbContext context)
        {
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        
        public async Task<ProductDto> Handle(ProductDisplayQuery request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            IDbConnection db = _context.Database.GetDbConnection();
            
            string query = 
                @"SELECT pd.data 
                  FROM public.product_display pd
                  WHERE pd.id = @Id;";

            string data = await db.QueryFirstOrDefaultAsync<string>(query, new { request.Id });

            if (string.IsNullOrWhiteSpace(data))
            {
                return null;
            }

            return _serializer.Deserialize<ProductDto>(data);
        }
    }
}