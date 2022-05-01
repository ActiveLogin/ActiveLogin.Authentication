using System;
using System.IO;
using System.Linq;

using ActiveLogin.Authentication.BankId.Core.Models;

using Microsoft.AspNetCore.Authentication;

namespace ActiveLogin.Authentication.BankId.AspNetCore.DataProtection.Serialization;

internal class BankIdLoginOptionsSerializer : IDataSerializer<BankIdLoginOptions>
{
    private const int FormatVersion = 5;
    private const char CertificatePoliciesSeparator = ';';

    public byte[] Serialize(BankIdLoginOptions model)
    {
        using var memory = new MemoryStream();
        using var writer = new BinaryWriter(memory);

        writer.Write(FormatVersion);

        writer.Write(string.Join(CertificatePoliciesSeparator.ToString(), model.CertificatePolicies));
        writer.Write(model.SameDevice);
        writer.Write(model.AllowBiometric);
        writer.Write(model.CancelReturnUrl ?? string.Empty);
        writer.Write(model.StateCookieName);

        writer.Flush();
        return memory.ToArray();
    }

    public BankIdLoginOptions Deserialize(byte[] data)
    {
        using var memory = new MemoryStream(data);
        using var reader = new BinaryReader(memory);

        if (reader.ReadInt32() != FormatVersion)
        {
            throw new IncompatibleSerializationVersion(nameof(BankIdLoginOptions));
        }

        var certificatePolicies = reader.ReadString().Split(new[] { CertificatePoliciesSeparator }, StringSplitOptions.RemoveEmptyEntries).ToList();
        var autoLaunch = reader.ReadBoolean();
        var allowBiometric = reader.ReadBoolean();
        var cancelReturnUrl = reader.ReadString();
        var stateCookieName = reader.ReadString();

        return new BankIdLoginOptions(
            certificatePolicies,
            autoLaunch,
            allowBiometric,
            cancelReturnUrl,
            stateCookieName
        );
    }
}