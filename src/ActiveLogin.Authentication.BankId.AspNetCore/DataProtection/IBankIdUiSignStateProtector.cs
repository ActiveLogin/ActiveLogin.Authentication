using ActiveLogin.Authentication.BankId.AspNetCore.Sign;

namespace ActiveLogin.Authentication.BankId.AspNetCore.DataProtection;

public interface IBankIdUiSignStateProtector
{
    string Protect(BankIdUiSignState state);
    BankIdUiSignState Unprotect(string protectedState);
}
