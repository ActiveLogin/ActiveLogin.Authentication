using ActiveLogin.Authentication.BankId.AspNetCore.Launcher;
using Microsoft.AspNetCore.Http;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Test.Helpers
{
    internal class TestBankIdLauncher : IBankIdLauncher
    {
        public BankIdLaunchInfo GetLaunchInfo(LaunchUrlRequest request, HttpContext httpContext)
        {
            // Always redirect back without user interaction in simulated mode
            return new BankIdLaunchInfo(request.RedirectUrl, false, false);
        }
    }
}
