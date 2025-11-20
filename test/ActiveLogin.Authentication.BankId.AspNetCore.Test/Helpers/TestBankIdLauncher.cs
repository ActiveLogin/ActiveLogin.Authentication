using System.Threading.Tasks;

using ActiveLogin.Authentication.BankId.Core.Launcher;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Test.Helpers;

internal class TestBankIdLauncher : IBankIdLauncher
{
    public Task<BankIdLaunchInfo> GetLaunchInfoAsync(LaunchUrlRequest request)
    {
        // Always redirect back without user interaction in simulated mode
        return Task.FromResult(new BankIdLaunchInfo(request.RedirectUrl, false, false));
    }
}
