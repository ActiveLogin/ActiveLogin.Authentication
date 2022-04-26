using ActiveLogin.Authentication.BankId.AspNetCore.Models;

namespace ActiveLogin.Authentication.BankId.AspNetCore.DataProtection;

public interface IBankIdQrStartStateProtector
{
    string Protect(BankIdQrStartState qrStartState);
    BankIdQrStartState Unprotect(string protectedQrStartState);
}
