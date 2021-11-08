using System.Collections.Generic;

namespace Store.Order.Domain.Buyers
{
    public record Cart(Dictionary<Item, uint> Items);
}