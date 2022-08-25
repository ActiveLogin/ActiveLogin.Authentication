namespace ActiveLogin.Authentication.BankId.Core.Launcher;

internal class BankIdDevelopmentLauncher : IBankIdLauncher
{
    private const string DevelopmentLaunchUrl = "#";

    public BankIdLaunchInfo GetLaunchInfo(LaunchUrlRequest request)
    {
        // Always stay on same page, without reloading, in simulated mode
        return new BankIdLaunchInfo(DevelopmentLaunchUrl, false, false);
    }
}
