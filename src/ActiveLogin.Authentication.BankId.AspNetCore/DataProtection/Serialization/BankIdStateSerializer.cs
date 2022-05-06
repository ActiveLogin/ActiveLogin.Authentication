using ActiveLogin.Authentication.BankId.AspNetCore.Models;

using Microsoft.AspNetCore.Authentication;

namespace ActiveLogin.Authentication.BankId.AspNetCore.DataProtection.Serialization;

internal class BankIdStateSerializer : IDataSerializer<BankIdUiState>
{
    private const int FormatVersion = 1;

    public byte[] Serialize(BankIdUiState model)
    {
        using var memory = new MemoryStream();
        using var writer = new BinaryWriter(memory);

        writer.Write(FormatVersion);
        PropertiesSerializer.Default.Write(writer, model.AuthenticationProperties);
        writer.Flush();

        return memory.ToArray();
    }

    public BankIdUiState Deserialize(byte[] data)
    {
        using var memory = new MemoryStream(data);
        using var reader = new BinaryReader(memory);

        if (reader.ReadInt32() != FormatVersion)
        {
            throw new IncompatibleSerializationVersion(nameof(BankIdUiState));
        }

        var authenticationProperties = PropertiesSerializer.Default.Read(reader);

        if(authenticationProperties == null)
        {
            throw new Exception(BankIdConstants.ErrorMessages.CouldNotDeserialize(nameof(AuthenticationProperties)));
        }

        return new BankIdUiState(authenticationProperties);
    }
}
