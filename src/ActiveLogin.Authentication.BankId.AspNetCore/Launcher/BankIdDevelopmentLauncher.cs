using ActiveLogin.Authentication.BankId.Core.Launcher;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Launcher;

internal class BankIdDevelopmentLauncher : IBankIdLauncher
{
    public BankIdLaunchInfo GetLaunchInfo(LaunchUrlRequest request)
    {
        // Always stay on same page, without reloading, in simulated mode
        return new BankIdLaunchInfo("#", false, false);
    }
}
