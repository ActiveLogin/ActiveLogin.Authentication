namespace ActiveLogin.Authentication.BankId.AspNetCore.Helpers;

internal static class BankIdHandlerHelper
{
    public static string GetCancelReturnUrl(IDictionary<string, string?> items)
    {
        // Default to root if no return url is set
        var cancelReturnUrl = items.ContainsKey(BankIdConstants.QueryStringParameters.ReturnUrl)
            ? items[BankIdConstants.QueryStringParameters.ReturnUrl]
            : BankIdConstants.DefaultCancelUrl;

        // If cancel url is set, it overrides the regular return url
        if (items.TryGetValue(BankIdConstants.AuthenticationPropertiesKeys.CancelReturnUrl, out var cancelUrl))
        {
            cancelReturnUrl = cancelUrl;
        }

        return cancelReturnUrl ?? BankIdConstants.DefaultCancelUrl;
    }
}
