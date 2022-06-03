namespace ActiveLogin.Authentication.BankId.AspNetCore.DataProtection;

internal interface IBankIdDataStateProtector<TModel>
{
    string Protect(TModel model);
    TModel Unprotect(string protectedModel);
}
