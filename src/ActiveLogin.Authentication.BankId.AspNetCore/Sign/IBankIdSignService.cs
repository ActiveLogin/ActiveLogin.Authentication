using Microsoft.AspNetCore.Mvc;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Sign;

public interface IBankIdSignService
{
    Task InitiateSignAsync(BankIdSignProperties properties, string callbackPath, string configKey);
    Task<BankIdSignResult?> GetSignResultAsync(string provider);
}
