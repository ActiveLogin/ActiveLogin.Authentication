namespace ActiveLogin.Authentication.BankId.Core.Launcher;

/// <summary>
/// Resolves launch information to start the BankID app
/// </summary>
public interface IBankIdLauncher
{
    /// <summary>
    /// Get info on how to launch the BankID app given the current HttpContext. Use (for example) User Agent to detect device.
    /// </summary>
    /// <param name="request">Launch info</param>
    /// <returns></returns>
    Task<BankIdLaunchInfo> GetLaunchInfoAsync(LaunchUrlRequest request);
}
