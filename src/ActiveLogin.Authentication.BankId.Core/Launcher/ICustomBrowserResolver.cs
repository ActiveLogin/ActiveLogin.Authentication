namespace ActiveLogin.Authentication.BankId.Core.Launcher;

public interface ICustomBrowserResolver
{
    Task<BankIdLauncherCustomBrowserConfig?> GetConfig(LaunchUrlRequest launchUrlRequest);
}
