using ActiveLogin.Authentication.BankId.Core.Launcher;
using ActiveLogin.Authentication.BankId.Core.SupportedDevice;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Launcher;

public class BankIdCustomBrowserResolver(
    IBankIdSupportedDeviceDetector deviceDetector,
    IEnumerable<IBankIdLauncherCustomBrowser> customBrowsers
) : ICustomBrowserResolver
{
    public async Task<BankIdLauncherCustomBrowserConfig?> GetConfig(LaunchUrlRequest launchUrlRequest)
    {
        var device = deviceDetector.Detect();
        var context = new BankIdLauncherCustomBrowserContext(device, launchUrlRequest);
        foreach (var browser in customBrowsers)
        {
            if (await browser.IsApplicable(context))
            {
                return await browser.GetCustomAppCallbackResult(context);
            }
        }
        return null;
    }
}
