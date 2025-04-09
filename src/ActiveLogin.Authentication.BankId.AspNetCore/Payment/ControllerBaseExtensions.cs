using Microsoft.AspNetCore.Mvc;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Payment;

public static class ControllerBaseExtensions
{
    public static IActionResult BankIdInitiatePayment(this ControllerBase controllerBase, BankIdPaymentProperties properties, string callbackPath, string configKey)
    {
        return new BankIdInitiatePaymentResult(properties, callbackPath, configKey);
    }
}
