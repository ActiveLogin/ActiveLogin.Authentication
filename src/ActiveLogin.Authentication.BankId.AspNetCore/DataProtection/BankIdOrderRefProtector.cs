using ActiveLogin.Authentication.BankId.AspNetCore.DataProtection.Serialization;
using ActiveLogin.Authentication.BankId.AspNetCore.Models;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;

namespace ActiveLogin.Authentication.BankId.AspNetCore.DataProtection;

internal class BankIdOrderRefProtector : IBankIdOrderRefProtector
{
    private readonly ISecureDataFormat<BankIdOrderRef> _secureDataFormat;

    public BankIdOrderRefProtector(IDataProtectionProvider dataProtectionProvider)
    {
        var dataProtector = dataProtectionProvider.CreateProtector(
            typeof(BankIdLoginResultProtector).FullName ?? nameof(BankIdLoginResultProtector),
            "v1"
        );

        _secureDataFormat = new SecureDataFormat<BankIdOrderRef>(
            new BankIdOrderRefSerializer(),
            dataProtector
        );
    }

    public string Protect(BankIdOrderRef orderRef)
    {
        return _secureDataFormat.Protect(orderRef);
    }

    public BankIdOrderRef Unprotect(string protectedOrderRef)
    {
        var unprotected = _secureDataFormat.Unprotect(protectedOrderRef);

        if (unprotected == null)
        {
            throw new Exception("Could not unprotect BankIdOrderRef");
        }

        return unprotected;
    }
}
