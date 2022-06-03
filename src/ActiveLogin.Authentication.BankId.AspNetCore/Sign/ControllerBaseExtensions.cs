using Microsoft.AspNetCore.Mvc;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Sign;

public static class ControllerBaseExtensions
{
    public static IActionResult BankIdInitiateSign(this ControllerBase controllerBase, BankIdSignProperties properties, string callbackPath, string configKey)
    {
        return new BankIdInitiateSignResult(properties, callbackPath, configKey);
    }
}
