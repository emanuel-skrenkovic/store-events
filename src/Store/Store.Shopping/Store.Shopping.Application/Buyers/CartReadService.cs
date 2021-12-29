using System.Data;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Store.Core.Domain;
using Store.Shopping.Domain.Buyers.ValueObjects;
using Store.Shopping.Infrastructure;
using Store.Shopping.Infrastructure.Entity;

namespace Store.Shopping.Application.Buyers;

public class CartReadService
{
    private readonly ISerializer _serializer;
    private readonly IDbConnection _db;

    public CartReadService(ISerializer serializer, StoreShoppingDbContext context)
    {
        _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        _db = context?.Database.GetDbConnection() ?? throw new ArgumentNullException(nameof(context));  
    } 
        
    public async Task<Cart> GetCartAsync(BuyerIdentifier buyerId)
    {
        string query = @"SELECT *
                         FROM public.cart
                         WHERE customer_number = @CustomerNumber 
                         AND session_id = @SessionId;";

        CartEntity cartEntity = await _db.QuerySingleOrDefaultAsync<CartEntity>(query, new { buyerId.CustomerNumber, buyerId.SessionId });
        if (cartEntity == null) return null;

        return _serializer.Deserialize<Cart>(cartEntity.Data);
    }
}