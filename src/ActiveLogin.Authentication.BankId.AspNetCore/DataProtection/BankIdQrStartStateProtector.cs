using ActiveLogin.Authentication.BankId.AspNetCore.DataProtection.Serialization;
using ActiveLogin.Authentication.BankId.Core.Models;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;

namespace ActiveLogin.Authentication.BankId.AspNetCore.DataProtection;

internal class BankIdQrStartStateProtector : IBankIdQrStartStateProtector
{
    private readonly ISecureDataFormat<BankIdQrStartState> _secureDataFormat;

    public BankIdQrStartStateProtector(IDataProtectionProvider dataProtectionProvider)
    {
        var dataProtector = dataProtectionProvider.CreateProtector(
            typeof(BankIdQrStartStateProtector).FullName ?? nameof(BankIdQrStartStateProtector),
            "v1"
        );

        _secureDataFormat = new SecureDataFormat<BankIdQrStartState>(
            new BankIdQrStartStateSerializer(),
            dataProtector
        );
    }

    public string Protect(BankIdQrStartState qrStartState)
    {
        return _secureDataFormat.Protect(qrStartState);
    }

    public BankIdQrStartState Unprotect(string protectedQrStartState)
    {
        var unprotected = _secureDataFormat.Unprotect(protectedQrStartState);

        if (unprotected == null)
        {
            throw new Exception("Could not unprotect BankIdQrStartState");
        }

        return unprotected;
    }
}
