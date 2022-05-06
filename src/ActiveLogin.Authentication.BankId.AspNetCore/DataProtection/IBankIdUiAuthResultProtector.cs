using ActiveLogin.Authentication.BankId.AspNetCore.Models;

namespace ActiveLogin.Authentication.BankId.AspNetCore.DataProtection;

public interface IBankIdUiAuthResultProtector
{
    string Protect(BankIdUiAuthResult uiAuthResult);
    BankIdUiAuthResult Unprotect(string protectedUiAuthResult);
}
