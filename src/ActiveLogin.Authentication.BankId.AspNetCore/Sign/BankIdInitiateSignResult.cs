using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Sign;

public class BankIdInitiateSignResult : ActionResult
{
    private readonly BankIdSignProperties _properties;
    private readonly string _callbackPath;
    private readonly string _configKey;

    public BankIdInitiateSignResult(BankIdSignProperties properties, string callbackPath, string configKey)
    {
        _properties = properties;
        _callbackPath = callbackPath;
        _configKey = configKey;
    }

    public override async Task ExecuteResultAsync(ActionContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var httpContext = context.HttpContext;
        var bankIdSignService = httpContext.RequestServices.GetRequiredService<IBankIdSignService>();
        await bankIdSignService.InitiateSignAsync(_properties, _callbackPath, _configKey);
    }
}
