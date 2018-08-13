using System.IO;
using ActiveLogin.Authentication.GrandId.AspNetCore.Models;
using Microsoft.AspNetCore.Authentication;

namespace ActiveLogin.Authentication.GrandId.AspNetCore.Serialization
{
    public class GrandIdOrderRefSerializer : IDataSerializer<GrandIdOrderRef>
    {
        private const int FormatVersion = 1;

        public byte[] Serialize(GrandIdOrderRef model)
        {
            using (var memory = new MemoryStream())
            {
                using (var writer = new BinaryWriter(memory))
                {
                    writer.Write(FormatVersion);
                    writer.Write(model.OrderRef);
                    writer.Flush();
                    return memory.ToArray();
                }
            }
        }

        public GrandIdOrderRef Deserialize(byte[] data)
        {
            using (var memory = new MemoryStream(data))
            {
                using (var reader = new BinaryReader(memory))
                {
                    if (reader.ReadInt32() != FormatVersion)
                    {
                        return null;
                    }

                    return new GrandIdOrderRef
                    {
                        OrderRef = reader.ReadString()
                    };
                }
            }
        }
    }
}