using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Payment;

public class BankIdInitiatePaymentResult : ActionResult
{
    private readonly BankIdPaymentProperties _properties;
    private readonly string _callbackPath;
    private readonly string _configKey;

    public BankIdInitiatePaymentResult(BankIdPaymentProperties properties, string callbackPath, string configKey)
    {
        _properties = properties;
        _callbackPath = callbackPath;
        _configKey = configKey;
    }

    public override async Task ExecuteResultAsync(ActionContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var httpContext = context.HttpContext;
        var bankIdPaymentService = httpContext.RequestServices.GetRequiredService<IBankIdPaymentService>();
        await bankIdPaymentService.InitiatePaymentAsync(_properties, _callbackPath, _configKey);
    }
}
