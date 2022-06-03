using ActiveLogin.Authentication.BankId.Core.Models;

namespace ActiveLogin.Authentication.BankId.AspNetCore.DataProtection.Serialization;

internal class BankIdQrStartStateSerializer : BankIdDataSerializer<BankIdQrStartState>
{
    protected override void Write(BinaryWriter writer, BankIdQrStartState model)
    {
        writer.Write(model.QrStartTime.ToUnixTimeMilliseconds());
        writer.Write(model.QrStartToken);
        writer.Write(model.QrStartSecret);
    }

    protected override BankIdQrStartState Read(BinaryReader reader)
    {
        return new BankIdQrStartState(
            DateTimeOffset.FromUnixTimeMilliseconds(reader.ReadInt64()),
            reader.ReadString(),
            reader.ReadString()
        );
    }
}
