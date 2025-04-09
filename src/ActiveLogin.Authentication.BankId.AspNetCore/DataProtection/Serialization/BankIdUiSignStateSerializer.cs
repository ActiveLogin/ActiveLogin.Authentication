using ActiveLogin.Authentication.BankId.Api.Models;
using ActiveLogin.Authentication.BankId.AspNetCore.Sign;
using ActiveLogin.Authentication.BankId.Core.CertificatePolicies;
using ActiveLogin.Identity.Swedish;

namespace ActiveLogin.Authentication.BankId.AspNetCore.DataProtection.Serialization;

internal class BankIdUiSignStateSerializer : BankIdDataSerializer<BankIdUiSignState>
{
    protected override void Write(BinaryWriter writer, BankIdUiSignState signState)
    {
        writer.Write(signState.ConfigKey);

        writer.Write(signState.BankIdSignProperties.UserVisibleData);

        writer.Write(signState.BankIdSignProperties.UserVisibleDataFormat == null);
        writer.Write(signState.BankIdSignProperties.UserVisibleDataFormat ?? string.Empty);

        writer.Write(signState.BankIdSignProperties.UserNonVisibleData?.Length ?? -1);
        writer.Write(signState.BankIdSignProperties.UserNonVisibleData ?? Array.Empty<byte>());

        writer.Write(signState.BankIdSignProperties.RequiredPersonalIdentityNumber == null);
        writer.Write(signState.BankIdSignProperties.RequiredPersonalIdentityNumber?.To12DigitString() ?? string.Empty);

        writer.Write(signState.BankIdSignProperties.RequireMrtd == null);
        writer.Write(signState.BankIdSignProperties.RequireMrtd.GetValueOrDefault());

        writer.Write(signState.BankIdSignProperties.RequirePinCode == null);
        writer.Write(signState.BankIdSignProperties.RequirePinCode.GetValueOrDefault());

        writer.Write(signState.BankIdSignProperties.BankIdCertificatePolicies?.Count ?? 0);
        if (signState.BankIdSignProperties.BankIdCertificatePolicies?.Count > 0)
        {
            foreach (var policy in signState.BankIdSignProperties.BankIdCertificatePolicies)
            {
                writer.Write(Convert.ToInt32(policy));
            }
        }

        writer.Write(signState.BankIdSignProperties.CardReader == null);
        writer.Write(signState.BankIdSignProperties.CardReader.HasValue ? Convert.ToInt32(signState.BankIdSignProperties.CardReader) : -1);

        writer.Write(signState.BankIdSignProperties.Items.Count);
        foreach (var item in signState.BankIdSignProperties.Items)
        {
            writer.Write(item.Key ?? string.Empty);
            writer.Write(item.Value ?? string.Empty);
        }
    }

    protected override BankIdUiSignState Read(BinaryReader reader)
    {
        var configKey = reader.ReadString();
        var userVisibleData = reader.ReadString();

        var userVisibleDataFormatIsNull = reader.ReadBoolean();
        var userVisibleDataFormat = reader.ReadString();
        if (userVisibleDataFormatIsNull)
        {
            userVisibleDataFormat = null;
        }

        var userNonVisibleDataLength = reader.ReadInt32();
        var userNonVisibleData = reader.ReadBytes(userNonVisibleDataLength == -1 ? 0 : userNonVisibleDataLength);
        if (userNonVisibleDataLength == -1)
        {
            userNonVisibleData = null;
        }

        var requiredPersonalIdentityNumberIsNull = reader.ReadBoolean();
        var requiredPersonalIdentityNumber = reader.ReadString();
        if (requiredPersonalIdentityNumberIsNull)
        {
            requiredPersonalIdentityNumber = null;
        }

        var requireMrtdIsNull = reader.ReadBoolean();
        var requireMrtd = reader.ReadBoolean();

        var requirePinCodeIsNull = reader.ReadBoolean();
        var requirePinCode = reader.ReadBoolean();

        var bankIdCertificatePoliciesCount = reader.ReadInt32();
        var bankIdCertificatePolicies = new List<BankIdCertificatePolicy>();
        for(var index = 0; index < bankIdCertificatePoliciesCount; index++)
        {
            bankIdCertificatePolicies.Add((BankIdCertificatePolicy)reader.ReadInt32());
        }

        var cardReaderIsNull = reader.ReadBoolean();
        var cardReaderValue = reader.ReadInt32();
        CardReader? cardReader = cardReaderIsNull ? null : (CardReader)cardReaderValue;

        var count = reader.ReadInt32();
        var items = new Dictionary<string, string?>(count);

        for (var index = 0; index != count; ++index)
        {
            var key = reader.ReadString();
            var value = reader.ReadString();
            items.Add(key, value);
        }

        var bankIdSignProperties = new BankIdSignProperties(userVisibleData)
        {
            UserVisibleDataFormat = userVisibleDataFormat,
            UserNonVisibleData = userNonVisibleData,
            RequiredPersonalIdentityNumber = requiredPersonalIdentityNumber != null ? PersonalIdentityNumber.Parse(requiredPersonalIdentityNumber) : null,
            RequireMrtd = requireMrtdIsNull ? null : requireMrtd,
            RequirePinCode = requirePinCodeIsNull ? null : requirePinCode,
            BankIdCertificatePolicies = bankIdCertificatePolicies,
            CardReader = cardReader,
            Items = items
        };
        return new BankIdUiSignState(configKey, bankIdSignProperties);
    }
}
