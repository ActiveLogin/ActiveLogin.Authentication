using ActiveLogin.Authentication.BankId.AspNetCore.Areas.ActiveLogin.Models;
using ActiveLogin.Authentication.BankId.AspNetCore.Models;

using Microsoft.AspNetCore.Authentication;

namespace ActiveLogin.Authentication.BankId.AspNetCore.DataProtection.Serialization;

internal class BankIdOrderRefSerializer : IDataSerializer<BankIdUiOrderRef>
{
    private const int FormatVersion = 1;

    public byte[] Serialize(BankIdUiOrderRef model)
    {
        using var memory = new MemoryStream();
        using var writer = new BinaryWriter(memory);

        writer.Write(FormatVersion);
        writer.Write(model.OrderRef);
        writer.Flush();

        return memory.ToArray();
    }

    public BankIdUiOrderRef Deserialize(byte[] data)
    {
        using var memory = new MemoryStream(data);
        using var reader = new BinaryReader(memory);

        if (reader.ReadInt32() != FormatVersion)
        {
            throw new IncompatibleSerializationVersion(nameof(BankIdUiOrderRef));
        }

        return new BankIdUiOrderRef(reader.ReadString());
    }
}
