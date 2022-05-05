namespace ActiveLogin.Authentication.BankId.Core.Launcher;

public class BankIdDevelopmentLauncher : IBankIdLauncher
{
    public BankIdLaunchInfo GetLaunchInfo(LaunchUrlRequest request)
    {
        // Always stay on same page, without reloading, in simulated mode
        return new BankIdLaunchInfo("#", false, false);
    }
}
