using Xunit;

namespace Store.Catalogue.Tests;

[CollectionDefinition(Name)]
public class StoreCatalogueFixtureCollection : ICollectionFixture<StoreCatalogueFixture>
{
    public const string Name = "Store.Catalogue";
}