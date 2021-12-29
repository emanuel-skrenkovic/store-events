using Xunit;

namespace Store.Catalogue.Tests;

[CollectionDefinition(Name)]
public class StoreCatalogueFixtureCollection : ICollectionFixture<StoreCatalogueDatabaseFixture>
{
    public const string Name = "Store.Catalogue";
}