using System.IO;
using ActiveLogin.Authentication.GrandId.AspNetCore.Models;
using Microsoft.AspNetCore.Authentication;

namespace ActiveLogin.Authentication.GrandId.AspNetCore.Serialization
{
    public class GrandIdLoginResultSerializer : IDataSerializer<GrandIdLoginResult>
    {
        private const int FormatVersion = 1;

        public byte[] Serialize(GrandIdLoginResult model)
        {
            using (var memory = new MemoryStream())
            {
                using (var writer = new BinaryWriter(memory))
                {
                    writer.Write(FormatVersion);

                    writer.Write(model.IsSuccessful);

                    writer.Write(model.PersonalIdentityNumber);

                    writer.Write(model.Name);
                    writer.Write(model.GivenName);
                    writer.Write(model.Surname);

                    writer.Flush();
                    return memory.ToArray();
                }
            }
        }

        public GrandIdLoginResult Deserialize(byte[] data)
        {
            using (var memory = new MemoryStream(data))
            {
                using (var reader = new BinaryReader(memory))
                {
                    if (reader.ReadInt32() != FormatVersion)
                    {
                        return null;
                    }

                    return new GrandIdLoginResult
                    {
                        IsSuccessful = reader.ReadBoolean(),

                        PersonalIdentityNumber = reader.ReadString(),

                        Name = reader.ReadString(),
                        GivenName = reader.ReadString(),
                        Surname = reader.ReadString()
                    };
                }
            }
        }
    }
}