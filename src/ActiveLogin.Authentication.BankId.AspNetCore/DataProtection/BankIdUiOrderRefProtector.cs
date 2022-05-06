using ActiveLogin.Authentication.BankId.AspNetCore.Areas.ActiveLogin.Models;
using ActiveLogin.Authentication.BankId.AspNetCore.DataProtection.Serialization;
using ActiveLogin.Authentication.BankId.AspNetCore.Models;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;

namespace ActiveLogin.Authentication.BankId.AspNetCore.DataProtection;

internal class BankIdUiOrderRefProtector : IBankIdUiOrderRefProtector
{
    private const string ProtectorVersion = "v1";

    private readonly ISecureDataFormat<BankIdUiOrderRef> _secureDataFormat;

    public BankIdUiOrderRefProtector(IDataProtectionProvider dataProtectionProvider)
    {
        var dataProtector = dataProtectionProvider.CreateProtector(
            typeof(BankIdUiAuthResultProtector).FullName ?? nameof(BankIdUiAuthResultProtector),
            ProtectorVersion
        );

        _secureDataFormat = new SecureDataFormat<BankIdUiOrderRef>(
            new BankIdOrderRefSerializer(),
            dataProtector
        );
    }

    public string Protect(BankIdUiOrderRef orderRef)
    {
        return _secureDataFormat.Protect(orderRef);
    }

    public BankIdUiOrderRef Unprotect(string protectedOrderRef)
    {
        var unprotected = _secureDataFormat.Unprotect(protectedOrderRef);

        if (unprotected == null)
        {
            throw new Exception(BankIdConstants.ErrorMessages.CouldNotUnprotect(nameof(BankIdUiOrderRef)));
        }

        return unprotected;
    }
}
