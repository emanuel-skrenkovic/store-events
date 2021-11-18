using Store.Core.Domain.Tests.Helpers.Csv;
using Xunit;

namespace Store.Core.Domain.Tests
{
    public class CsvFileDataAttributeTests
    {
        [Theory]
        [CsvFileData("./Helpers/Csv/simpleCsvTestData.csv", ",")]
        public void TestSimpleCsvDataAttribute(string csvValue)
        {
            Assert.NotNull(csvValue);
            Assert.NotEmpty(csvValue);
        }

        public class TestData
        {
            public string StringValue { get; set; }
            public int IntValue { get; set; }
        }

        [Theory]
        [CsvFileData("./Helpers/Csv/objectCsvTestData.csv", ",", typeof(TestData))]
        public void TestObjectCsvDataAttribute(TestData testData)
        {
            Assert.NotNull(testData);
            
            Assert.NotNull(testData.StringValue);
            Assert.NotEmpty(testData.StringValue);
        }
    }
}