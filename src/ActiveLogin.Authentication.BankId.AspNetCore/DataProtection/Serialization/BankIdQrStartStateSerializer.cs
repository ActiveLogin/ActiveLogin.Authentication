using System;
using System.IO;

using ActiveLogin.Authentication.BankId.Core.Models;

using Microsoft.AspNetCore.Authentication;

namespace ActiveLogin.Authentication.BankId.AspNetCore.DataProtection.Serialization;

internal class BankIdQrStartStateSerializer : IDataSerializer<BankIdQrStartState>
{
    private const int FormatVersion = 1;

    public byte[] Serialize(BankIdQrStartState model)
    {
        using var memory = new MemoryStream();
        using var writer = new BinaryWriter(memory);

        writer.Write(FormatVersion);
        writer.Write(model.QrStartTime.ToUnixTimeMilliseconds());
        writer.Write(model.QrStartToken);
        writer.Write(model.QrStartSecret);
        writer.Flush();

        return memory.ToArray();
    }

    public BankIdQrStartState Deserialize(byte[] data)
    {
        using var memory = new MemoryStream(data);
        using var reader = new BinaryReader(memory);

        if (reader.ReadInt32() != FormatVersion)
        {
            throw new IncompatibleSerializationVersion(nameof(BankIdQrStartState));
        }

        return new BankIdQrStartState(
            DateTimeOffset.FromUnixTimeMilliseconds(reader.ReadInt64()),
            reader.ReadString(),
            reader.ReadString()
        );
    }
}
