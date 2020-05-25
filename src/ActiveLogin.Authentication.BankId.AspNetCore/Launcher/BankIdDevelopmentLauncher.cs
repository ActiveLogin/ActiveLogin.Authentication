using Microsoft.AspNetCore.Http;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Launcher
{
    internal class BankIdDevelopmentLauncher : IBankIdLauncher
    {
        public BankIdLaunchInfo GetLaunchInfo(LaunchUrlRequest request, HttpContext httpContext)
        {
            // Always stay on same page, without reloading, in simulated mode
            return new BankIdLaunchInfo("#", false, false);
        }
    }
}
