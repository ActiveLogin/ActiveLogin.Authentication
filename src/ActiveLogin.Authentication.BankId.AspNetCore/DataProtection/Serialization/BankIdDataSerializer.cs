using ActiveLogin.Authentication.BankId.AspNetCore.Areas.ActiveLogin.Models;

using Microsoft.AspNetCore.Authentication;

namespace ActiveLogin.Authentication.BankId.AspNetCore.DataProtection.Serialization;

internal abstract class BankIdDataSerializer<TModel> : IDataSerializer<TModel>
{
    private const int FormatVersion = 11;

    public byte[] Serialize(TModel model)
    {
        using var memory = new MemoryStream();
        using var writer = new BinaryWriter(memory);

        writer.Write(FormatVersion);
        Write(writer, model);
        writer.Flush();

        return memory.ToArray();
    }

    protected abstract void Write(BinaryWriter writer, TModel model);
    protected abstract TModel Read(BinaryReader reader);

    public TModel Deserialize(byte[] data)
    {
        using var memory = new MemoryStream(data);
        using var reader = new BinaryReader(memory);

        if (reader.ReadInt32() != FormatVersion)
        {
            throw new Exception(BankIdConstants.ErrorMessages.CouldNotDeserialize(nameof(BankIdUiOrderRef)));
        }

        return Read(reader);
    }
}
