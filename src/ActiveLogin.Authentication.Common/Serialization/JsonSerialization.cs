using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace ActiveLogin.Authentication.Common.Serialization
{
    public class JsonSerialization
    {
        public static T Deserialize<T>(string json)
        {
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                var serializer = new DataContractJsonSerializer(typeof(T));
                return (T)serializer.ReadObject(stream);
            }
        }

        public static T Deserialize<T>(Stream json)
        {
            var serializer = new DataContractJsonSerializer(typeof(T));
            return (T)serializer.ReadObject(json);
        }

        public static string Serialize<T>(T value)
        {
            if (value == null)
            {
                return string.Empty;
            }

            using (var stream = new MemoryStream())
            {
                var serializer = new DataContractJsonSerializer(typeof(T));
                serializer.WriteObject(stream, value);
                var json = stream.ToArray();
                return Encoding.UTF8.GetString(json, 0, json.Length);
            }
        }
    }
}