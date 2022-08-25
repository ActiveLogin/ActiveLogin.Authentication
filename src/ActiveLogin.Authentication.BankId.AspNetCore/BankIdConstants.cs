using System.Text.Json;

namespace ActiveLogin.Authentication.BankId.AspNetCore;

internal static class BankIdConstants
{
    public const string ProductName = "ActiveLogin-BankId-AspNetCore";

    public static readonly TimeSpan StatusRefreshInterval = TimeSpan.FromSeconds(2);
    public static readonly TimeSpan QrCodeRefreshInterval = TimeSpan.FromSeconds(1);

    public const string DefaultCancelUrl = "/";
    public const string DefaultStateCookieName = "__ActiveLogin.BankIdUiState";

    public const string LocalizationResourcesPath = "Resources";

    public static class AuthenticationPropertiesKeys
    {
        public const string CancelReturnUrl = "cancelReturnUrl";
    }

    public static class FormParameters
    {
        public const string UiResult = "uiResult";
    }

    public static class QueryStringParameters
    {
        public const string UiOptions = "uiOptions";
        public const string ReturnUrl = "returnUrl";
    }

    public static class ErrorMessages
    {
        public const string CouldNotAccessHttpContext = "Can't access HttpContext";

        public const string InvalidReturnUrl = "Invalid returnUrl. Needs to be a local url.";

        public const string InvalidStateCookie = "Invalid state cookie";
        public const string InvalidUiResult = "Invalid BankId UI result";

        public const string UnknownFlowCollectResultType = "Unknown collect result type";
        public const string UnknownFlowLaunchType = "Unknown launch type";

        private const string CouldNotUnprotectPrefix = "Could not unprotect";
        public static string CouldNotUnprotect(string classType) => $"{CouldNotUnprotectPrefix} {classType}";

        private const string CouldNotDeserializePrefix = "Could not deserialize";
        public static string CouldNotDeserialize(string classType) => $"{CouldNotDeserializePrefix} {classType}";

        private const string CouldNotGetUrlForPrefix = "Could not get URL for";
        public static string CouldNotGetUrlFor(string controller, string action) => $"{CouldNotGetUrlForPrefix} {controller}.{action}";
    }

    public static class LocalizationKeys
    {
        public const string UnsupportedBrowserErrorMessage = "UnsupportedBrowser_ErrorMessage";
    }

    public static class Routes
    {
        public const string ActiveLoginAreaName = "ActiveLogin";
        public const string BankIdPathName = "BankId";

        public const string BankIdAuthControllerName = "BankIdUiAuth";
        public const string BankIdAuthControllerPath = "Auth";
        public const string BankIdAuthInitActionName = "Init";

        public const string BankIdSignControllerName = "BankIdUiSign";
        public const string BankIdSignControllerPath = "Sign";
        public const string BankIdSignInitActionName = "Init";

        public const string BankIdAuthApiControllerName = "BankIdUiAuthApi";
        public const string BankIdSignApiControllerName = "BankIdUiSignApi";

        public const string BankIdApiControllerPath = "Api";
        public const string BankIdApiInitializeActionName = "Initialize";
        public const string BankIdApiStatusActionName = "Status";
        public const string BankIdApiQrCodeActionName = "QrCode";
        public const string BankIdApiCancelActionName = "Cancel";
    }
}
