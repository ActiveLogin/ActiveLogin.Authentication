using ActiveLogin.Authentication.BankId.AspNetCore.Models;

namespace ActiveLogin.Authentication.BankId.AspNetCore.DataProtection.Serialization;

internal class BankIdUiResultSerializer : BankIdDataSerializer<BankIdUiResult>
{
    protected override void Write(BinaryWriter writer, BankIdUiResult model)
    {
        writer.Write(model.IsSuccessful);

        writer.Write(model.BankIdOrderRef);

        writer.Write(model.PersonalIdentityNumber);

        writer.Write(model.Name);
        writer.Write(model.GivenName);
        writer.Write(model.Surname);

        writer.Write(model.Signature);
        writer.Write(model.OcspResponse);

        writer.Write(model.CertNotBefore);
        writer.Write(model.CertNotAfter);

        writer.Write(model.DetectedIpAddress);
    }

    protected override BankIdUiResult Read(BinaryReader reader)
    {
        return new BankIdUiResult(
            reader.ReadBoolean(),

            reader.ReadString(),

            reader.ReadString(),

            reader.ReadString(),
            reader.ReadString(),
            reader.ReadString(),

            reader.ReadString(),
            reader.ReadString(),

            reader.ReadString(),
            reader.ReadString(),

            reader.ReadString()
        );
    }
}
