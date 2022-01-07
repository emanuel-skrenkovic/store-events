using System;
using Store.Core.Domain;
using Xunit;

namespace Store.Core.Tests;

public class GuidUtilityTests
{
    [Fact]
    public void DeterministicGuid_ShouldNot_BeEmpty()
    {
        const string name = "name";
        const string @namespace = "namespace";
        Guid guid = GuidUtility.NewDeterministicGuid(@namespace, name);
        
        Assert.NotEqual(default, guid);
    }
    
    [Fact]
    public void DeterministicGuids_Should_BeEqual_When_CreatedWithSameNameAndNamespace()
    {
        const string name = "name";
        const string @namespace = "namespace";
        
        Guid guid1 = GuidUtility.NewDeterministicGuid(@namespace, name);
        Guid guid2 = GuidUtility.NewDeterministicGuid(@namespace, name);
        
        Assert.Equal(guid1, guid2);
    }
    
    [Fact]
    public void DeterministicGuids_ShouldNot_BetEqual_When_CreatedWithSameNameAndDifferentNamespace() 
    {
        const string name = "name";
        const string @namespace = "namespace";
        
        Guid guid1 = GuidUtility.NewDeterministicGuid(@namespace, name);

        const string @namespace2 = "namespace2";
        Guid guid2 = GuidUtility.NewDeterministicGuid(@namespace2, name);
        
        Assert.NotEqual(guid1, guid2); 
    }
}