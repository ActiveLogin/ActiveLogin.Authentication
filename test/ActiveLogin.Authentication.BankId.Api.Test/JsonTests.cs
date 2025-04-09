using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;

using Xunit;

namespace ActiveLogin.Authentication.BankId.Api.Test;

internal class JsonTests
{
    public static void AssertOnlyProperties(string jsonContent, string[] expectedPaths)
    {
        var jsonDocument = JsonDocument.Parse(jsonContent);
        var jsonPaths = new List<string>();
        foreach (var item in jsonDocument.RootElement.EnumerateObject())
        {
            jsonPaths.Add(item.Name);
        }

        Assert.Equal(expectedPaths, jsonPaths);
    }

    public static void AssertProperty<TValue>(string jsonContent, string expectedProperty, TValue expectedValue)
    {
        var jsonContentNode = JsonNode.Parse(jsonContent);
        Assert.Equal(expectedValue, jsonContentNode[expectedProperty].GetValue<TValue>());
    }

    public static void AssertSubProperty<TValue>(string jsonContent, string expectedParentProperty, string expectedProperty, TValue expectedValue)
    {
        var jsonContentNode = JsonNode.Parse(jsonContent);
        Assert.Equal(expectedValue, jsonContentNode[expectedParentProperty][expectedProperty].GetValue<TValue>());
    }

    public static void AssertSubProperty<TValue>(string jsonContent, string expectedParentProperty, string expectedProperty, List<TValue> expectedValue)
    {
        var jsonContentNode = JsonNode.Parse(jsonContent);
        var expectedAsString = JsonSerializer.Serialize(expectedValue);

        Assert.Equal(expectedAsString, jsonContentNode[expectedParentProperty][expectedProperty].ToJsonString());
    }

    public static void AssertPropertyHierarchy<TValue>(string jsonContent, TValue expectedValue, params string[] properties)
    {
        var jsonContentNode = JsonNode.Parse(jsonContent);
        var expectedAsString = JsonSerializer.Serialize(expectedValue);

        var value = GetSubPropertyValue(jsonContentNode, properties);

        Assert.Equal(expectedAsString, value);
    }

    public static void AssertPropertyIsEmptyObject(string jsonContent, string expectedProperty)
    {
        var jsonContentNode = JsonNode.Parse(jsonContent);
        Assert.Equal("{}", jsonContentNode[expectedProperty].ToJsonString());
    }

    public static void AssertPropertyIsNull(string jsonContent, string expectedProperty)
    {
        var jsonContentNode = JsonNode.Parse(jsonContent);
        Assert.Null(jsonContentNode[expectedProperty]);
    }

    public static void AssertPropertyIsNotNull(string jsonContent, string expectedProperty)
    {
        var jsonContentNode = JsonNode.Parse(jsonContent);
        Assert.NotNull(jsonContentNode[expectedProperty]);
    }
    
    public static void AssertProperties(string contentString, Dictionary<string, string> dictionary)
    {
        var contentObject = JsonUtils.DeserializeAndFlatten(contentString);

        foreach (var (path, expectedValue) in dictionary)
        {
            contentObject.TryGetValue(path, out var actualValue);
            Assert.Equal(expectedValue, actualValue);
        }
    }

    private static string GetSubPropertyValue(JsonNode jsonNode, IEnumerable<string> properties)
    {
        return properties.Any() ?  GetSubPropertyValue(jsonNode[properties.First()], properties.Skip(1)) : jsonNode.ToJsonString();
    }

}
