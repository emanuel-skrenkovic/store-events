using Xunit;

namespace Store.Shopping.Tests;

[CollectionDefinition(nameof(StoreShoppingCombinedFixtureCollection))]
public abstract class StoreShoppingCombinedFixtureCollection : ICollectionFixture<StoreShoppingCombinedFixture>
{
}