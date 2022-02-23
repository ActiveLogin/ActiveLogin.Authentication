using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Nodes;

using Xunit;

namespace ActiveLogin.Authentication.BankId.Api.Test
{
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
            string expectedAsString = "[";
            for (int i = 0; i < expectedValue.Count; i++)
            {
                expectedAsString += $"\"{expectedValue[i]}\"";
                if (i != expectedValue.Count - 1)
                    expectedAsString += ",";
                else
                    expectedAsString += "]";
            }
            Assert.Equal(expectedAsString, jsonContentNode[expectedParentProperty][expectedProperty].ToJsonString());
        }

        public static void AssertPropertyIsEmptyObject(string jsonContent, string expectedProperty)
        {
            var jsonContentNode = JsonNode.Parse(jsonContent);
            Assert.Equal("{}", jsonContentNode[expectedProperty].ToJsonString());
        }
    }
}
