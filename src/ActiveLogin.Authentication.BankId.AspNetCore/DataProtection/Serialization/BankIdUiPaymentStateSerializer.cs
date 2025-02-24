using ActiveLogin.Authentication.BankId.Api.Models;
using ActiveLogin.Authentication.BankId.AspNetCore.Payment;
using ActiveLogin.Authentication.BankId.Core.CertificatePolicies;
using ActiveLogin.Authentication.BankId.Core.Payment;
using ActiveLogin.Identity.Swedish;

namespace ActiveLogin.Authentication.BankId.AspNetCore.DataProtection.Serialization;

internal class BankIdUiPaymentStateSerializer : BankIdDataSerializer<BankIdUiPaymentState>
{
    protected override void Write(BinaryWriter writer, BankIdUiPaymentState paymentState)
    {
        writer.Write(paymentState.ConfigKey);

        writer.Write(paymentState.BankIdPaymentProperties.TransactionType.ToString());
        writer.Write(paymentState.BankIdPaymentProperties.RecipientName.ToString());

        writer.Write(paymentState.BankIdPaymentProperties.Money == null);
        writer.Write(paymentState.BankIdPaymentProperties.Money?.Amount ?? string.Empty);
        writer.Write(paymentState.BankIdPaymentProperties.Money?.Currency ?? string.Empty);

        writer.Write(paymentState.BankIdPaymentProperties.RiskWarning == null);
        writer.Write(paymentState.BankIdPaymentProperties.RiskWarning ?? string.Empty);

        writer.Write(paymentState.BankIdPaymentProperties.RiskFlags == null);
        var riskFlagsStr = paymentState.BankIdPaymentProperties.RiskFlags != null ? string.Join(",", paymentState.BankIdPaymentProperties.RiskFlags.Select(r => r.ToString())) : string.Empty;
        writer.Write(riskFlagsStr);

        writer.Write(paymentState.BankIdPaymentProperties.UserVisibleData == null);
        writer.Write(paymentState.BankIdPaymentProperties.UserVisibleData ?? string.Empty);

        writer.Write(paymentState.BankIdPaymentProperties.UserVisibleDataFormat == null);
        writer.Write(paymentState.BankIdPaymentProperties.UserVisibleDataFormat ?? string.Empty);

        writer.Write(paymentState.BankIdPaymentProperties.UserNonVisibleData?.Length ?? -1);
        writer.Write(paymentState.BankIdPaymentProperties.UserNonVisibleData ?? Array.Empty<byte>());

        writer.Write(paymentState.BankIdPaymentProperties.RequiredPersonalIdentityNumber == null);
        writer.Write(paymentState.BankIdPaymentProperties.RequiredPersonalIdentityNumber?.To12DigitString() ?? string.Empty);

        writer.Write(paymentState.BankIdPaymentProperties.RequireMrtd == null);
        writer.Write(paymentState.BankIdPaymentProperties.RequireMrtd.GetValueOrDefault());

        writer.Write(paymentState.BankIdPaymentProperties.RequirePinCode == null);
        writer.Write(paymentState.BankIdPaymentProperties.RequirePinCode.GetValueOrDefault());

        writer.Write(paymentState.BankIdPaymentProperties.BankIdCertificatePolicies?.Count ?? 0);
        if (paymentState.BankIdPaymentProperties.BankIdCertificatePolicies?.Count > 0)
        {
            foreach (var policy in paymentState.BankIdPaymentProperties.BankIdCertificatePolicies)
            {
                writer.Write(Convert.ToInt32(policy));
            }
        }

        writer.Write(paymentState.BankIdPaymentProperties.CardReader == null);
        writer.Write(paymentState.BankIdPaymentProperties.CardReader.HasValue ? Convert.ToInt32(paymentState.BankIdPaymentProperties.CardReader) : -1);

        writer.Write(paymentState.BankIdPaymentProperties.Items.Count);
        foreach (var item in paymentState.BankIdPaymentProperties.Items)
        {
            writer.Write(item.Key ?? string.Empty);
            writer.Write(item.Value ?? string.Empty);
        }
    }

    protected override BankIdUiPaymentState Read(BinaryReader reader)
    {
        var configKey = reader.ReadString();

        var transactionTypeStr = reader.ReadString();
        var transactionType = (TransactionType) Enum.Parse(typeof(TransactionType), transactionTypeStr);
        var recipientName = reader.ReadString();

        var moneyIsNull = reader.ReadBoolean();
        var moneyAmount = reader.ReadString();
        var moneyCurrency = reader.ReadString();
        var money = moneyIsNull ? null : new Money(moneyAmount, moneyCurrency);

        var riskFlagsIsNull = reader.ReadBoolean();
        var riskFlagsStr = reader.ReadString();
        var riskFlags = riskFlagsIsNull ? null : riskFlagsStr.Split(",").Select(f => (RiskFlags) Enum.Parse(typeof(RiskFlags), f));

        var riskWarningIsNull = reader.ReadBoolean();
        var riskWarning = reader.ReadString();
        if (riskWarningIsNull)
        {
            riskWarning = null;
        }

        var userVisibleDataIsNull = reader.ReadBoolean();
        var userVisibleData = reader.ReadString();
        if (userVisibleDataIsNull) {
            userVisibleData = null;
        }

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
        for (var index = 0; index < bankIdCertificatePoliciesCount; index++)
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

        var bankIdPaymentProperties = new BankIdPaymentProperties(transactionType, recipientName)
        {
            Money = money,
            RiskFlags = riskFlags,
            UserVisibleData = userVisibleData,
            UserVisibleDataFormat = userVisibleDataFormat,
            UserNonVisibleData = userNonVisibleData,
            RequiredPersonalIdentityNumber = requiredPersonalIdentityNumber != null ? PersonalIdentityNumber.Parse(requiredPersonalIdentityNumber) : null,
            RequireMrtd = requireMrtdIsNull ? null : requireMrtd,
            RequirePinCode = requirePinCodeIsNull ? null : requirePinCode,
            BankIdCertificatePolicies = bankIdCertificatePolicies,
            CardReader = cardReader,
            Items = items
        };
        return new BankIdUiPaymentState(configKey, bankIdPaymentProperties);
    }
}
