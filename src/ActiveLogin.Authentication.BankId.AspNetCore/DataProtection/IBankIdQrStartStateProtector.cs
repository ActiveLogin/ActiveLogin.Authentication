using ActiveLogin.Authentication.BankId.Core.Models;

namespace ActiveLogin.Authentication.BankId.AspNetCore.DataProtection;

public interface IBankIdQrStartStateProtector
{
    string Protect(BankIdQrStartState qrStartState);
    BankIdQrStartState Unprotect(string protectedQrStartState);
}
