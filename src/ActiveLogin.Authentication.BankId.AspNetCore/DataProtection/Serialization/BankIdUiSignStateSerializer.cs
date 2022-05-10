using ActiveLogin.Authentication.BankId.AspNetCore.Models;
using ActiveLogin.Authentication.BankId.AspNetCore.Sign;

namespace ActiveLogin.Authentication.BankId.AspNetCore.DataProtection.Serialization;

internal class BankIdUiSignStateSerializer : BankIdDataSerializer<BankIdUiSignState>
{
    protected override void Write(BinaryWriter writer, BankIdUiSignState model)
    {
        writer.Write(model.ConfigKey);

        writer.Write(model.BankIdSignProperties.UserVisibleData);

        writer.Write(model.BankIdSignProperties.UserVisibleDataFormat == null);
        writer.Write(model.BankIdSignProperties.UserVisibleDataFormat ?? string.Empty);

        writer.Write(model.BankIdSignProperties.UserNonVisibleData?.Length ?? -1);
        writer.Write(model.BankIdSignProperties.UserNonVisibleData ?? Array.Empty<byte>());

        writer.Write(model.BankIdSignProperties.Items.Count);
        foreach (var item in model.BankIdSignProperties.Items)
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

            Items = items
        };
        return new BankIdUiSignState(configKey, bankIdSignProperties);
    }
}
