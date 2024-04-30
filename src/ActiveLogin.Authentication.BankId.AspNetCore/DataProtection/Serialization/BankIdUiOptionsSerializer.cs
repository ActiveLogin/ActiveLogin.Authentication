using ActiveLogin.Authentication.BankId.AspNetCore.Models;
using ActiveLogin.Authentication.BankId.Core.CertificatePolicies;

namespace ActiveLogin.Authentication.BankId.AspNetCore.DataProtection.Serialization;

internal class BankIdUiOptionsSerializer : BankIdDataSerializer<BankIdUiOptions>
{
    private const char CertificatePoliciesSeparator = ';';

    protected override void Write(BinaryWriter writer, BankIdUiOptions model)
    {
        writer.Write(string.Join(CertificatePoliciesSeparator.ToString(), model.CertificatePolicies));
        writer.Write(model.SameDevice);
        writer.Write(model.RequirePinCode);
        writer.Write(model.RequireMrtd);
        writer.Write(model.CancelReturnUrl);
        writer.Write(model.StateCookieName);
    }

    protected override BankIdUiOptions Read(BinaryReader reader)
    {
        var certificatePoliciesRaw = reader.ReadString();
        var certificatePolicies = certificatePoliciesRaw
                                    .Split(new[]
                                    {
                                        CertificatePoliciesSeparator
                                    }, StringSplitOptions.RemoveEmptyEntries)
                                    .Select(Enum.Parse<BankIdCertificatePolicy>)
                                    .ToList();

        return new BankIdUiOptions(
            certificatePolicies,
            reader.ReadBoolean(),
            reader.ReadBoolean(),
            reader.ReadBoolean(),
            reader.ReadString(),
            reader.ReadString()
        );
    }
}
