using ActiveLogin.Authentication.BankId.AspNetCore.Models;

namespace ActiveLogin.Authentication.BankId.AspNetCore.DataProtection;

public interface IBankIdUiResultProtector
{
    string Protect(BankIdUiResult uiAuthResult);
    BankIdUiResult Unprotect(string protectedUiAuthResult);
}
