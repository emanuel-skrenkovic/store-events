using Xunit;

namespace Store.Shopping.Tests;

[CollectionDefinition("Store.Shopping")]
public class StoreShoppingCollection : ICollectionFixture<StoreShoppingFixture>
{
    public const string Name = "Store.Shopping";
}