using ActiveLogin.Authentication.BankId.Core.SupportedDevice;

namespace ActiveLogin.Authentication.BankId.Core.Launcher;

public class BankIdLauncherCustomAppCallbackContext
{
    internal BankIdLauncherCustomAppCallbackContext(BankIdSupportedDevice device, LaunchUrlRequest launchUrlRequest)
    {
        Device = device;
        LaunchUrlRequest = launchUrlRequest;
    }

    public BankIdSupportedDevice Device { get; }
    public LaunchUrlRequest LaunchUrlRequest { get; }
}
