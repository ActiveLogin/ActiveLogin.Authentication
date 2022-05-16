using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Sign;

public interface IBankIdSignService
{
    IActionResult InitiateSign(BankIdSignProperties properties, string callbackPath, string configKey);
    Task<BankIdSignResult?> GetSignResultAsync(string provider);
}
