using System;
using System.Threading.Tasks;
using Store.Order.Domain.Buyers.ValueObjects;

namespace Store.Order.Application.Buyer
{
    public class CartViewService
    {
        public Task<CartView> GetCartAsync(BuyerIdentifier buyerId)
        {
            throw new NotImplementedException();
        }
    }
}