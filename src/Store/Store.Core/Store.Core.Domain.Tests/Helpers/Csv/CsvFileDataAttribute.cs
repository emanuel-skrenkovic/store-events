using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using CsvHelper;
using CsvHelper.Configuration;
using Xunit.Sdk;

namespace Store.Core.Domain.Tests.Helpers.Csv;

public class CsvFileDataAttribute : DataAttribute
{
    private readonly string _relativeFilePath;
    private readonly string _delimiter;

    private readonly Type _parameterType;

    public CsvFileDataAttribute(string relativeFilePath, string delimiter, Type parameterType = null)
    {
        _relativeFilePath = relativeFilePath ?? throw new ArgumentNullException(nameof(relativeFilePath));
        _delimiter        = delimiter        ?? throw new ArgumentNullException(nameof(delimiter));
        _parameterType    = parameterType    ?? typeof(object);
    }

    public override IEnumerable<object[]> GetData(MethodInfo testMethod)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = false,
            NewLine = Environment.NewLine,
            Delimiter = _delimiter
        };
            
        using var reader = new StreamReader(_relativeFilePath);
        using var csv = new CsvReader(reader, config);

        var records = csv.GetRecords(_parameterType);

        foreach (var record in records)
        {
            if (_parameterType != typeof(object))
            {
                yield return new[] { record };
            }
            else
            {
                ExpandoObject expando = (ExpandoObject)record;
                yield return expando.Select(kv => kv.Value).ToArray(); 
            }
        }
    }
}