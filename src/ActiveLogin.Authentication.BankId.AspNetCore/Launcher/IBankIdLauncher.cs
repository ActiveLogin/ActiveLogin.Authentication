using Microsoft.AspNetCore.Http;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Launcher
{
    /// <summary>
    /// Resolves launch information to start the BankID app
    /// </summary>
    public interface IBankIdLauncher
    {
        /// <summary>
        /// Get info on how to launch the BankID app given the current HttpContext. Use (for example) User Agent to detect device.
        /// </summary>
        /// <param name="request">Launch info</param>
        /// <param name="httpContext">HttpContext to extract info from.</param>
        /// <returns></returns>
        BankIdLaunchInfo GetLaunchInfo(LaunchUrlRequest request, HttpContext httpContext);
    }
}
