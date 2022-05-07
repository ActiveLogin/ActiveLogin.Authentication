using ActiveLogin.Authentication.BankId.AspNetCore.Models;

using Microsoft.AspNetCore.Authentication;

namespace ActiveLogin.Authentication.BankId.AspNetCore.DataProtection.Serialization;

internal class BankIdUiStateSerializer : BankIdDataSerializer<BankIdUiState>
{
    protected override void Write(BinaryWriter writer, BankIdUiState model)
    {
        PropertiesSerializer.Default.Write(writer, model.AuthenticationProperties);
    }

    protected override BankIdUiState Read(BinaryReader reader)
    {
        var authenticationProperties = PropertiesSerializer.Default.Read(reader);

        if (authenticationProperties == null)
        {
            throw new Exception(BankIdConstants.ErrorMessages.CouldNotDeserialize(nameof(AuthenticationProperties)));
        }

        return new BankIdUiState(authenticationProperties);
    }
}
