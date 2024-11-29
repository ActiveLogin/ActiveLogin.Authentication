using ActiveLogin.Authentication.BankId.AspNetCore.Auth;
using ActiveLogin.Authentication.BankId.AspNetCore.Models;
using ActiveLogin.Authentication.BankId.AspNetCore.Sign;
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
        if(model is BankIdUiAuthState authState)
        {
            PropertiesSerializer.Default.Write(writer, authState.AuthenticationProperties);
        }
        else if(model is BankIdUiSignState signState)
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

            writer.Write(signState.BankIdSignProperties.Items.Count);
            foreach (var item in signState.BankIdSignProperties.Items)
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
                Items = items
            };
            return new BankIdUiSignState(configKey, bankIdSignProperties);
        }
        else
        {
            throw new ArgumentException("Invalid enum value for BankIdStateType", nameof(type));
        }
    }
}
