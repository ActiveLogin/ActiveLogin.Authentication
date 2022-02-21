using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Serialization
{
    internal static class JsonSerializer
    {
        public static string Serialize<T>(T value)
        {
            if (value == null)
            {
                return string.Empty;
            }

            using var stream = new MemoryStream();

            var serializer = new DataContractJsonSerializer(typeof(T));
            serializer.WriteObject(stream, value);
            var json = stream.ToArray();

            return Encoding.UTF8.GetString(json, 0, json.Length);
        }
    }
}
