using System.IO;
using ActiveLogin.Authentication.GrandId.AspNetCore.Models;
using Microsoft.AspNetCore.Authentication;

namespace ActiveLogin.Authentication.GrandId.AspNetCore.Serialization
{
    public class GrandIdSessionIdSerializer : IDataSerializer<GrandIdSessionId>
    {
        private const int FormatVersion = 1;

        public byte[] Serialize(GrandIdSessionId model)
        {
            using (var memory = new MemoryStream())
            {
                using (var writer = new BinaryWriter(memory))
                {
                    writer.Write(FormatVersion);
                    writer.Write(model.SessionId);
                    writer.Flush();
                    return memory.ToArray();
                }
            }
        }

        public GrandIdSessionId Deserialize(byte[] data)
        {
            using (var memory = new MemoryStream(data))
            {
                using (var reader = new BinaryReader(memory))
                {
                    if (reader.ReadInt32() != FormatVersion)
                    {
                        return null;
                    }

                    return new GrandIdSessionId
                    {
                        SessionId = reader.ReadString()
                    };
                }
            }
        }
    }
}