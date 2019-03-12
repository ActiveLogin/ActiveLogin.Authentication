using System.IO;
using ActiveLogin.Authentication.GrandId.AspNetCore.Models;
using Microsoft.AspNetCore.Authentication;

namespace ActiveLogin.Authentication.GrandId.AspNetCore.Serialization
{
    internal class GrandIdStateSerializer : IDataSerializer<GrandIdState>
    {
        private const int FormatVersion = 1;

        public byte[] Serialize(GrandIdState model)
        {
            using (var memory = new MemoryStream())
            using (var writer = new BinaryWriter(memory))
            {
                writer.Write(FormatVersion);
                PropertiesSerializer.Default.Write(writer, model.AuthenticationProperties);
                writer.Flush();
                return memory.ToArray();
            }
        }

        public GrandIdState Deserialize(byte[] data)
        {
            using (var memory = new MemoryStream(data))
            using (var reader = new BinaryReader(memory))
            {
                if (reader.ReadInt32() != FormatVersion)
                {
                    return null;
                }

                AuthenticationProperties authenticationProperties = PropertiesSerializer.Default.Read(reader);

                return new GrandIdState(authenticationProperties);
            }
        }
    }
}
