using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Store.Core.Domain;
using Store.Order.Infrastructure;

namespace Store.Order.Application.Buyer.Queries.GetCart
{
    public class BuyerCartGetQueryHandler : IRequestHandler<BuyerCartGetQuery, CartView>
    {
        private readonly ISerializer _serializer;
        private readonly StoreOrderDbContext _context;

        public BuyerCartGetQueryHandler(ISerializer serializer, StoreOrderDbContext context)
        {
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        
        public async Task<CartView> Handle(BuyerCartGetQuery request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            IDbConnection db = _context.Database.GetDbConnection();
            
            string query = 
                @"SELECT c.data 
                  FROM public.cart c
                  WHERE c.customer_number = @CustomerNumber;";

            string data = await db.QueryFirstOrDefaultAsync<string>(query, new { request.CustomerNumber });

            if (string.IsNullOrWhiteSpace(data))
            {
                return null;
            }

            return _serializer.Deserialize<CartView>(data);
        }
    }
}