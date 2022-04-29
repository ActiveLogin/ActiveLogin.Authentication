using ActiveLogin.Authentication.BankId.Core.Launcher;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Test.Helpers
{
    internal class TestBankIdLauncher : IBankIdLauncher
    {
        public BankIdLaunchInfo GetLaunchInfo(LaunchUrlRequest request)
        {
            // Always redirect back without user interaction in simulated mode
            return new BankIdLaunchInfo(request.RedirectUrl, false, false);
        }
    }
}
