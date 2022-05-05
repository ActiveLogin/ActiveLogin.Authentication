using System.Text.Json;

namespace ActiveLogin.Authentication.BankId.AspNetCore;

internal static class BankIdConstants
{
    public const string ProductName = "ActiveLogin-BankId-AspNetCore";

    public static readonly TimeSpan StatusRefreshInterval = TimeSpan.FromSeconds(2);
    public static readonly TimeSpan QrCodeRefreshInterval = TimeSpan.FromSeconds(1);

    public const string DefaultCancelUrl = "/";

    public const string LocalizationResourcesPath = "Resources";

    internal static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
    public static class AuthenticationPropertiesKeys
    {
        public const string CancelReturnUrl = "cancelReturnUrl";
    }

    public static class QueryStringParameters
    {
        public const string LoginResult = "loginResult";
        public const string LoginOptions = "loginOptions";
        public const string ReturnUrl = "returnUrl";
    }

    public static class ErrorMessages
    {
        public const string InvalidReturnUrl = "Invalid returnUrl. Needs to be a local url.";

        public const string InvalidStateCookie = "Invalid state cookie";
        public const string InvalidLoginResult = "Invalid login result";

        public const string UnknownFlowCollectResultType = "Unknown collect result type";
        public const string UnknownFlowLaunchType = "Unknown launch type";

        private const string CouldNotUnprotectPrefix = "Could not unprotect";
        public static string CouldNotUnprotect(string classType) => $"{CouldNotUnprotectPrefix} {classType}";

        private const string CouldNotDeserializePrefix = "Could not deserialize";
        public static string CouldNotDeserialize(string classType) => $"{CouldNotDeserializePrefix} {classType}";

        private const string CouldNotGetUrlForPrefix = "Could not get URL for";
        public static string CouldNotGetUrlFor(string controller, string action) => $"{CouldNotDeserializePrefix} {controller}.{action}";
    }

    public static class LocalizationKeys
    {
        public const string UnsupportedBrowserErrorMessage = "UnsupportedBrowser_ErrorMessage";
    }

    public static class Routes
    {
        public const string BankIdAreaName = "BankIdAuthentication";

        public const string BankIdControllerName = "BankId";
        public const string BankIdLoginActionName = "Login";

        public const string BankIdApiControllerName = "BankIdApi";
        public const string BankIdApiInitializeActionName = "Initialize";
        public const string BankIdApiStatusActionName = "Status";
        public const string BankIdApiQrCodeActionName = "QrCode";
        public const string BankIdApiCancelActionName = "Cancel";
    }
}
