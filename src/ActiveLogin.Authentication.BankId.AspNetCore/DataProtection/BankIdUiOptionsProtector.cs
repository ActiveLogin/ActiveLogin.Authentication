using ActiveLogin.Authentication.BankId.AspNetCore.DataProtection.Serialization;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;

namespace ActiveLogin.Authentication.BankId.AspNetCore.DataProtection;

internal class BankIdUiOptionsProtector : IBankIdUiOptionsProtector
{
    private const string ProtectorVersion = "v1";

    private readonly ISecureDataFormat<BankIdUiOptions> _secureDataFormat;

    public BankIdUiOptionsProtector(IDataProtectionProvider dataProtectionProvider)
    {
        var dataProtector = dataProtectionProvider.CreateProtector(
            typeof(BankIdUiOptionsProtector).FullName ?? nameof(BankIdUiAuthResultProtector),
            ProtectorVersion
        );

        _secureDataFormat = new SecureDataFormat<BankIdUiOptions>(
            new BankIdUiOptionsSerializer(),
            dataProtector
        );
    }

    public string Protect(BankIdUiOptions uiOptions)
    {
        return _secureDataFormat.Protect(uiOptions);
    }

    public BankIdUiOptions Unprotect(string protectedUiOptions)
    {
        var unprotected = _secureDataFormat.Unprotect(protectedUiOptions);

        if (unprotected == null)
        {
            throw new Exception(BankIdConstants.ErrorMessages.CouldNotUnprotect(nameof(BankIdUiOptions)));
        }

        return unprotected;
    }
}
