using System.Collections.Generic;
using Store.Core.Domain;
using Xunit;

namespace Store.Core.Tests;

public class ValueObjectTests
{
    private class SimpleTestObject : ValueObject<SimpleTestObject>
    {
        public string StringValue { get; }
            
        public int IntValue { get; }

        public SimpleTestObject(string stringValue, int intValue)
        {
            StringValue = stringValue;
            IntValue = intValue;
        }
            
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return StringValue;
            yield return IntValue;
        }
    }

    private class ComplexTestObject : ValueObject<ComplexTestObject>
    {
        public string StringValue { get; }
            
        public int IntValue { get; }

        public SimpleTestObject ObjectValue { get; }

        public ComplexTestObject(string stringValue, int intValue, SimpleTestObject objectValue)
        {
            StringValue = stringValue;
            IntValue = intValue;
            ObjectValue = objectValue;
        }
            
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return StringValue;
            yield return IntValue;
            yield return ObjectValue;
        }
    }
        
    [Fact]
    public void Should_Equal_WithEqualValues()
    {
        string stringValue = "test";
        int intValue = 2;
            
        SimpleTestObject obj1 = new SimpleTestObject(stringValue, intValue);
        SimpleTestObject obj2 = new SimpleTestObject(stringValue, intValue);
            
        Assert.Equal(obj1, obj2);
    }
        
    [Fact]
    public void Should_EqualHashCodes_WithEqualValues()
    {
        string stringValue = "test";
        int intValue = 2;
            
        SimpleTestObject obj1 = new SimpleTestObject(stringValue, intValue);
        SimpleTestObject obj2 = new SimpleTestObject(stringValue, intValue);
            
        Assert.Equal(obj1, obj2);
        Assert.Equal(obj1.GetHashCode(), obj2.GetHashCode());
    }
        
    [Fact]
    public void Should_NotEqual_WithDifferentValues()
    {
        string stringValue = "test";
        int intValue = 2;
            
        SimpleTestObject obj1 = new SimpleTestObject(stringValue, intValue);
        SimpleTestObject obj2 = new SimpleTestObject("test2", intValue);
            
        Assert.NotEqual(obj1, obj2);
    }
        
    [Fact]
    public void Should_NotEqualHashCodes_WithDifferentValues()
    {
        string stringValue = "test";
        int intValue = 2;
            
        SimpleTestObject obj1 = new SimpleTestObject(stringValue, intValue);
        SimpleTestObject obj2 = new SimpleTestObject("test2", intValue);
            
        Assert.NotEqual(obj1, obj2);
        Assert.NotEqual(obj1.GetHashCode(), obj2.GetHashCode());
    }
        
    [Fact]
    public void ComplexValueObject_Should_Equal_WithEqualValues()
    {
        string stringValue = "test";
        int intValue = 2;
            
        SimpleTestObject obj1 = new SimpleTestObject(stringValue, intValue);
        SimpleTestObject obj2 = new SimpleTestObject(stringValue, intValue);

        ComplexTestObject cObj1 = new ComplexTestObject(stringValue, intValue, obj1);
        ComplexTestObject cObj2 = new ComplexTestObject(stringValue, intValue, obj2);
            
        Assert.Equal(cObj1, cObj2);
    }
        
    [Fact]
    public void ComplexValueObject_Should_EqualHashCodes_WithEqualValues()
    {
        string stringValue = "test";
        int intValue = 2;
            
        SimpleTestObject obj1 = new SimpleTestObject(stringValue, intValue);
        SimpleTestObject obj2 = new SimpleTestObject(stringValue, intValue);

        ComplexTestObject cObj1 = new ComplexTestObject(stringValue, intValue, obj1);
        ComplexTestObject cObj2 = new ComplexTestObject(stringValue, intValue, obj2);
            
        Assert.Equal(cObj1, cObj2);
        Assert.Equal(cObj1.GetHashCode(), cObj2.GetHashCode());
    }
}