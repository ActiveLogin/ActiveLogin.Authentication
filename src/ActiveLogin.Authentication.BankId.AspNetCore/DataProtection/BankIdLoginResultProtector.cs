using ActiveLogin.Authentication.BankId.AspNetCore.DataProtection.Serialization;
using ActiveLogin.Authentication.BankId.AspNetCore.Models;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;

namespace ActiveLogin.Authentication.BankId.AspNetCore.DataProtection;

internal class BankIdLoginResultProtector : IBankIdLoginResultProtector
{
    private readonly ISecureDataFormat<BankIdLoginResult> _secureDataFormat;

    public BankIdLoginResultProtector(IDataProtectionProvider dataProtectionProvider)
    {
        var dataProtector = dataProtectionProvider.CreateProtector(
            typeof(BankIdLoginResultProtector).FullName ?? nameof(BankIdLoginResultProtector),
            "v1"
        );

        _secureDataFormat = new SecureDataFormat<BankIdLoginResult>(
            new BankIdLoginResultSerializer(),
            dataProtector
        );
    }

    public string Protect(BankIdLoginResult loginResult)
    {
        return _secureDataFormat.Protect(loginResult);
    }

    public BankIdLoginResult Unprotect(string protectedLoginResult)
    {
        var unprotected = _secureDataFormat.Unprotect(protectedLoginResult);

        if (unprotected == null)
        {
            throw new Exception("Could not unprotect BankIdLoginResult");
        }

        return unprotected;
    }
}
