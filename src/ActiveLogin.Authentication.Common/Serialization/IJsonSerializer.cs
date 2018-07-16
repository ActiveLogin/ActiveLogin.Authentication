using System.IO;

namespace ActiveLogin.Authentication.Common.Serialization
{
    public interface IJsonSerializer
    {
        T Deserialize<T>(string json);
        T Deserialize<T>(Stream json);

        string Serialize<T>(T value);
    }
}