using System;

using ActiveLogin.Authentication.BankId.AspNetCore.DataProtection.Serialization;
using ActiveLogin.Authentication.BankId.Core.Models;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;

namespace ActiveLogin.Authentication.BankId.AspNetCore.DataProtection;

internal class BankIdLoginOptionsProtector : IBankIdLoginOptionsProtector
{
    private readonly ISecureDataFormat<BankIdLoginOptions> _secureDataFormat;

    public BankIdLoginOptionsProtector(IDataProtectionProvider dataProtectionProvider)
    {
        var dataProtector = dataProtectionProvider.CreateProtector(
            typeof(BankIdLoginResultProtector).FullName ?? nameof(BankIdLoginResultProtector),
            "v1"
        );

        _secureDataFormat = new SecureDataFormat<BankIdLoginOptions>(
            new BankIdLoginOptionsSerializer(),
            dataProtector
        );
    }

    public string Protect(BankIdLoginOptions loginOptions)
    {
        return _secureDataFormat.Protect(loginOptions);
    }

    public BankIdLoginOptions Unprotect(string protectedLoginOptions)
    {
        var unprotected = _secureDataFormat.Unprotect(protectedLoginOptions);

        if (unprotected == null)
        {
            throw new Exception("Could not unprotect BankIdLoginOptions");
        }

        return unprotected;
    }
}
