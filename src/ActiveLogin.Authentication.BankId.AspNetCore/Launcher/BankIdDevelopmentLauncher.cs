using Microsoft.AspNetCore.Http;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Launcher
{
    internal class BankIdDevelopmentLauncher : IBankIdLauncher
    {
        public BankIdLaunchInfo GetLaunchInfo(LaunchUrlRequest request, HttpContext httpContext)
        {
            // Always redirect back without user interaction in simulated mode
            return new BankIdLaunchInfo(request.RedirectUrl, false, false);
        }
    }
}
