using Xunit;

namespace Store.Shopping.Tests;

[CollectionDefinition(Name)]
public class StoreShoppingEventStoreCollection : ICollectionFixture<StoreShoppingEventStoreFixture>
{
    public const string Name = "Store.Shopping.EventStore";
}