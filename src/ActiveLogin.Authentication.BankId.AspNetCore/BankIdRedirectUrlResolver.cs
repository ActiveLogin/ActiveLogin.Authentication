using ActiveLogin.Authentication.BankId.Core.Launcher;
using ActiveLogin.Authentication.BankId.Core.SupportedDevice;

using Microsoft.AspNetCore.Http;

namespace ActiveLogin.Authentication.BankId.AspNetCore;

public class BankIdRedirectUrlResolver(
    IBankIdSupportedDeviceDetector deviceDetector,
    ICustomBrowserResolver customBrowserResolver,
    IHttpContextAccessor httpContextAccessor
) : IBankIdRedirectUrlResolver
{
    public async Task<BankIdRedirectUrl> GetRedirectUrl(BankIdTransactionType type, string callbackPath)
    {
        var context = httpContextAccessor.HttpContext ?? throw new InvalidOperationException(BankIdConstants.ErrorMessages.CouldNotAccessHttpContext);
        var controller = type switch
        {
            BankIdTransactionType.Auth => BankIdConstants.Routes.BankIdAuthControllerName,
            BankIdTransactionType.Sign => BankIdConstants.Routes.BankIdSignControllerName,
            BankIdTransactionType.Payment => BankIdConstants.Routes.BankIdPaymentControllerName,
            _ => throw new InvalidOperationException("Unknown BankIdTransactionType")
        };
        var returnRedirectUrl = context.ResolveControllerUrl(
            BankIdConstants.Routes.ActiveLoginAreaName,
            controller,
            "Init", new { returnUrl = callbackPath }
        );

        var config = await customBrowserResolver.GetConfig(new LaunchUrlRequest(returnRedirectUrl, ""));

        var result = BankIdRedirectUrl.TryCreate(
            returnRedirectUrl,
            config,
            deviceDetector.Detect()
        );

        return result switch
        {
            Result<BankIdRedirectUrl>.Success(var value) => value,
            Result<BankIdRedirectUrl>.Failure(var error) => throw new InvalidOperationException($"Failed to create redirect URL: {error}"),
            _ => throw new InvalidOperationException("Unexpected result when creating redirect URL")
        };
    }
}

public static class UrlHelperExtensions
{
    public static string ResolveControllerUrl(
        this HttpContext context,
        string area,
        string controller,
        string action,
        object? routeValues = null
    ){
        var host = context.Request.Host.Value;
        var url = $"https://{host}/{area}/{controller}/{action}";

        if (routeValues != null)
        {
            var properties = routeValues.GetType().GetProperties();
            var query = string.Join("&", properties.Select(p =>
            {
                var value = p.GetValue(routeValues);
                return $"{Uri.EscapeDataString(p.Name)}={Uri.EscapeDataString(value?.ToString() ?? string.Empty)}";
            }));

            if (!string.IsNullOrEmpty(query))
            {
                url += "?" + query;
            }
        }

        return url;
    }
}
