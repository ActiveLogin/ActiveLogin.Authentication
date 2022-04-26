using System.IO;
using ActiveLogin.Authentication.BankId.AspNetCore.Models;
using Microsoft.AspNetCore.Authentication;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Serialization
{
    internal class BankIdOrderRefSerializer : IDataSerializer<BankIdOrderRef>
    {
        private const int FormatVersion = 1;

        public byte[] Serialize(BankIdOrderRef model)
        {
            using var memory = new MemoryStream();
            using var writer = new BinaryWriter(memory);

            writer.Write(FormatVersion);
            writer.Write(model.OrderRef);
            writer.Flush();

            return memory.ToArray();
        }

        public BankIdOrderRef Deserialize(byte[] data)
        {
            using var memory = new MemoryStream(data);
            using var reader = new BinaryReader(memory);

            if (reader.ReadInt32() != FormatVersion)
            {
                throw new IncompatibleSerializationVersion(nameof(BankIdOrderRef));
            }

            return new BankIdOrderRef(reader.ReadString());
        }
    }
}
