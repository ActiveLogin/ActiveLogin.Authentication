using ActiveLogin.Authentication.BankId.AspNetCore.Models;
using ActiveLogin.Authentication.BankId.Core.CertificatePolicies;
using ActiveLogin.Authentication.BankId.Core.Risk;

namespace ActiveLogin.Authentication.BankId.AspNetCore.DataProtection.Serialization;

internal class BankIdUiOptionsSerializer : BankIdDataSerializer<BankIdUiOptions>
{
    private const char CertificatePoliciesSeparator = ';';

    protected override void Write(BinaryWriter writer, BankIdUiOptions model)
    {
        writer.Write(string.Join(CertificatePoliciesSeparator.ToString(), model.CertificatePolicies));
        writer.Write(model.AllowedRiskLevel.ToString());
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

        var riskLevel = Enum.TryParse<BankIdAllowedRiskLevel>(reader.ReadString(), out var allowedRiskLevel) ? allowedRiskLevel : BankIdAllowedRiskLevel.Low;

        return new BankIdUiOptions(
            certificatePolicies,
            riskLevel,
            reader.ReadBoolean(),
            reader.ReadBoolean(),
            reader.ReadBoolean(),
            reader.ReadString(),
            reader.ReadString()
        );
    }
}
