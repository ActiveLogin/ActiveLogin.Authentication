using Microsoft.AspNetCore.Authentication;

namespace ActiveLogin.Authentication.BankId.AspNetCore.DataProtection.Serialization;

internal class BankIdUiOptionsSerializer : IDataSerializer<BankIdUiOptions>
{
    private const int FormatVersion = 5;
    private const char CertificatePoliciesSeparator = ';';

    public byte[] Serialize(BankIdUiOptions model)
    {
        using var memory = new MemoryStream();
        using var writer = new BinaryWriter(memory);

        writer.Write(FormatVersion);

        writer.Write(string.Join(CertificatePoliciesSeparator.ToString(), model.CertificatePolicies));
        writer.Write(model.SameDevice);
        writer.Write(model.AllowBiometric);
        writer.Write(model.CancelReturnUrl);
        writer.Write(model.StateCookieName);

        writer.Flush();
        return memory.ToArray();
    }

    public BankIdUiOptions Deserialize(byte[] data)
    {
        using var memory = new MemoryStream(data);
        using var reader = new BinaryReader(memory);

        if (reader.ReadInt32() != FormatVersion)
        {
            throw new IncompatibleSerializationVersion(nameof(BankIdUiOptions));
        }

        var certificatePolicies = reader.ReadString().Split(new[] { CertificatePoliciesSeparator }, StringSplitOptions.RemoveEmptyEntries).ToList();
        var autoLaunch = reader.ReadBoolean();
        var allowBiometric = reader.ReadBoolean();
        var cancelReturnUrl = reader.ReadString();
        var stateCookieName = reader.ReadString();

        return new BankIdUiOptions(
            certificatePolicies,
            autoLaunch,
            allowBiometric,
            cancelReturnUrl,
            stateCookieName
        );
    }
}
