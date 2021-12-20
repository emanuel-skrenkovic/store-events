using System;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Store.Order.Domain.Buyers.ValueObjects;
using Store.Order.Infrastructure;
using Store.Order.Infrastructure.Entity;

namespace Store.Order.Application.Buyer
{
    public class CartReadService
    {
        private readonly IDbConnection _db;

        public CartReadService(StoreOrderDbContext context)
            => _db = context?.Database.GetDbConnection() ?? throw new ArgumentNullException(nameof(context));
        
        public Task<Cart> GetCartAsync(BuyerIdentifier buyerId)
        {
            string query = @"SELECT *
                             FROM public.cart
                             WHERE customer_number = @CustomerNumber 
                             AND session_id = @SessionId;";

            _db.QueryAsync<Cart>(query, new { buyerId.CustomerNumber, buyerId.SessionId });
            throw new NotImplementedException();
        }
    }
}