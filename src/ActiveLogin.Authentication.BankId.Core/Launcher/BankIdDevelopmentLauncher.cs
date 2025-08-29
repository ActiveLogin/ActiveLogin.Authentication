namespace ActiveLogin.Authentication.BankId.Core.Launcher;

internal class BankIdDevelopmentLauncher : IBankIdLauncher
{
    private const string DevelopmentLaunchUrl = "#";
    private const string DevelopmentReturnUrl = "";

    public Task<BankIdLaunchInfo> GetLaunchInfoAsync(LaunchUrlRequest request)
    {
        // Always stay on same page, without reloading, in simulated mode
        return Task.FromResult(new BankIdLaunchInfo(DevelopmentLaunchUrl, false, false, DevelopmentReturnUrl));
    }
}
