using ActiveLogin.Authentication.BankId.AspNetCore.DataProtection.Serialization;
using ActiveLogin.Authentication.BankId.AspNetCore.Models;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;

namespace ActiveLogin.Authentication.BankId.AspNetCore.DataProtection;

internal class BankIdUiAuthResultProtector : IBankIdUiAuthResultProtector
{
    private const string ProtectorVersion = "v1";

    private readonly ISecureDataFormat<BankIdUiAuthResult> _secureDataFormat;

    public BankIdUiAuthResultProtector(IDataProtectionProvider dataProtectionProvider)
    {
        var dataProtector = dataProtectionProvider.CreateProtector(
            typeof(BankIdUiAuthResultProtector).FullName ?? nameof(BankIdUiAuthResultProtector),
            ProtectorVersion
        );

        _secureDataFormat = new SecureDataFormat<BankIdUiAuthResult>(
            new BankIdUiAuthResultSerializer(),
            dataProtector
        );
    }

    public string Protect(BankIdUiAuthResult uiAuthResult)
    {
        return _secureDataFormat.Protect(uiAuthResult);
    }

    public BankIdUiAuthResult Unprotect(string protectedUiAuthResult)
    {
        var unprotected = _secureDataFormat.Unprotect(protectedUiAuthResult);

        if (unprotected == null)
        {
            throw new Exception(BankIdConstants.ErrorMessages.CouldNotUnprotect(nameof(BankIdUiAuthResult)));
        }

        return unprotected;
    }
}
