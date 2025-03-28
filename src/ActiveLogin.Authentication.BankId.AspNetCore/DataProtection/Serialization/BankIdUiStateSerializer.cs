using System.Linq;

using ActiveLogin.Authentication.BankId.Api.Models;
using ActiveLogin.Authentication.BankId.AspNetCore.Auth;
using ActiveLogin.Authentication.BankId.AspNetCore.Models;
using ActiveLogin.Authentication.BankId.AspNetCore.Payment;
using ActiveLogin.Authentication.BankId.AspNetCore.Sign;
using ActiveLogin.Authentication.BankId.Core.CertificatePolicies;
using ActiveLogin.Authentication.BankId.Core.Payment;
using ActiveLogin.Identity.Swedish;

using Microsoft.AspNetCore.Authentication;

namespace ActiveLogin.Authentication.BankId.AspNetCore.DataProtection.Serialization;

internal class BankIdUiStateSerializer : BankIdDataSerializer<BankIdUiState>
{
    public BankIdUiStateSerializer()
    {
    }

    protected override void Write(BinaryWriter writer, BankIdUiState model)
    {
        writer.Write((int)model.Type);
        if (model is BankIdUiAuthState authState)
        {
            PropertiesSerializer.Default.Write(writer, authState.AuthenticationProperties);
        }
        else if (model is BankIdUiSignState signState)
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
        else if (model is BankIdUiPaymentState paymentState)
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

            writer.Write(paymentState.BankIdPaymentProperties.Items.Count);
            foreach (var item in paymentState.BankIdPaymentProperties.Items)
            {
                writer.Write(item.Key ?? string.Empty);
                writer.Write(item.Value ?? string.Empty);
            }
        }
        else
        {
            throw new ArgumentException("Invalid enum value for BankIdStateType", nameof(model.Type));
        }
    }

    protected override BankIdUiState Read(BinaryReader reader)
    {
        var type = (BankIdStateType)reader.ReadInt32();
        if (type == BankIdStateType.Auth)
        {
            var authenticationProperties = PropertiesSerializer.Default.Read(reader);
            if (authenticationProperties == null)
            {
                throw new Exception(BankIdConstants.ErrorMessages.CouldNotDeserialize(nameof(AuthenticationProperties)));
            }

            return new BankIdUiAuthState(authenticationProperties);
        }
        else if (type == BankIdStateType.Sign)
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
        else if (type == BankIdStateType.Payment)
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
                Items = items
            };
            return new BankIdUiPaymentState(configKey, bankIdPaymentProperties);
        }
        else
        {
            throw new ArgumentException("Invalid enum value for BankIdStateType", nameof(type));
        }
    }
}
