using ActiveLogin.Authentication.BankId.AspNetCore.Models;

namespace ActiveLogin.Authentication.BankId.AspNetCore.DataProtection.Serialization;

internal class BankIdUiAuthResultSerializer : BankIdDataSerializer<BankIdUiAuthResult>
{
    protected override void Write(BinaryWriter writer, BankIdUiAuthResult model)
    {
        writer.Write(model.IsSuccessful);

        writer.Write(model.BankIdOrderRef);

        writer.Write(model.PersonalIdentityNumber);

        writer.Write(model.Name);
        writer.Write(model.GivenName);
        writer.Write(model.Surname);
    }

    protected override BankIdUiAuthResult Read(BinaryReader reader)
    {
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
