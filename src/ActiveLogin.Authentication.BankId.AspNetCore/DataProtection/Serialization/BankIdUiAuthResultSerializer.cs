using ActiveLogin.Authentication.BankId.AspNetCore.Models;

using Microsoft.AspNetCore.Authentication;

namespace ActiveLogin.Authentication.BankId.AspNetCore.DataProtection.Serialization;

internal class BankIdUiAuthResultSerializer : IDataSerializer<BankIdUiAuthResult>
{
    private const int FormatVersion = 6;

    public byte[] Serialize(BankIdUiAuthResult model)
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

    public BankIdUiAuthResult Deserialize(byte[] data)
    {
        using var memory = new MemoryStream(data);
        using var reader = new BinaryReader(memory);

        if (reader.ReadInt32() != FormatVersion)
        {
            throw new IncompatibleSerializationVersion(nameof(BankIdUiAuthResult));
        }

        return new BankIdUiAuthResult(
            reader.ReadBoolean(),
            reader.ReadString(),
            reader.ReadString(),
            reader.ReadString(),
            reader.ReadString(),
            reader.ReadString()
        );
    }
}
