using System.IO;
using ActiveLogin.Authentication.BankId.AspNetCore.Models;
using Microsoft.AspNetCore.Authentication;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Serialization
{
    internal class BankIdLoginResultSerializer : IDataSerializer<BankIdLoginResult>
    {
        private const int FormatVersion = 2;

        public byte[] Serialize(BankIdLoginResult model)
        {
            using var memory = new MemoryStream();
            using var writer = new BinaryWriter(memory);

            writer.Write(FormatVersion);

            writer.Write(model.IsSuccessful);

            writer.Write(model.BankIdOrderRef);

            writer.Write(model.PersonalIdentityNumber);

            writer.Write(model.Name);
            writer.Write(model.GivenName);
            writer.Write(model.Surname);

            writer.Flush();

            return memory.ToArray();
        }

        public BankIdLoginResult Deserialize(byte[] data)
        {
            using var memory = new MemoryStream(data);
            using var reader = new BinaryReader(memory);

            if (reader.ReadInt32() != FormatVersion)
            {
                throw new IncompatibleSerializationVersion(nameof(BankIdLoginResult));
            }

            return new BankIdLoginResult(
                reader.ReadBoolean(),
                reader.ReadString(),
                reader.ReadString(),
                reader.ReadString(),
                reader.ReadString(),
                reader.ReadString()
            );
        }
    }
}
