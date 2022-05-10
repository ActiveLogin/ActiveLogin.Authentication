using ActiveLogin.Authentication.BankId.AspNetCore.Auth;
using ActiveLogin.Authentication.BankId.AspNetCore.Models;

using Microsoft.AspNetCore.Authentication;

namespace ActiveLogin.Authentication.BankId.AspNetCore.DataProtection.Serialization;

internal class BankIdUiAuthStateSerializer : BankIdDataSerializer<BankIdUiAuthState>
{
    protected override void Write(BinaryWriter writer, BankIdUiAuthState model)
    {
        PropertiesSerializer.Default.Write(writer, model.AuthenticationProperties);
    }

    protected override BankIdUiAuthState Read(BinaryReader reader)
    {
        var authenticationProperties = PropertiesSerializer.Default.Read(reader);

        if (authenticationProperties == null)
        {
            throw new Exception(BankIdConstants.ErrorMessages.CouldNotDeserialize(nameof(AuthenticationProperties)));
        }

        return new BankIdUiAuthState(authenticationProperties);
    }
}
