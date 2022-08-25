using ActiveLogin.Authentication.BankId.AspNetCore.Areas.ActiveLogin.Models;

namespace ActiveLogin.Authentication.BankId.AspNetCore.DataProtection.Serialization;

internal class BankIdUiOrderRefSerializer : BankIdDataSerializer<BankIdUiOrderRef>
{
    protected override void Write(BinaryWriter writer, BankIdUiOrderRef model)
    {
        writer.Write(model.OrderRef);
    }

    protected override BankIdUiOrderRef Read(BinaryReader reader)
    {
        return new BankIdUiOrderRef(reader.ReadString());
    }
}
