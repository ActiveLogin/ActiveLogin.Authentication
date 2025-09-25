using ActiveLogin.Authentication.BankId.AspNetCore.Auth;

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
        return authenticationProperties == null
            ? throw new Exception(BankIdConstants.ErrorMessages.CouldNotDeserialize(nameof(AuthenticationProperties)))
            : new BankIdUiAuthState(authenticationProperties);
    }
}
