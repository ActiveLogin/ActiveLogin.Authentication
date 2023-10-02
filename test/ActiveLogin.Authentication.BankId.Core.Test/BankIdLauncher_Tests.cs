using System.Threading.Tasks;

using ActiveLogin.Authentication.BankId.Core.Launcher;
using ActiveLogin.Authentication.BankId.Core.SupportedDevice;

using Xunit;

namespace ActiveLogin.Authentication.BankId.Core.Test;

public class BankIdLauncher_Tests
{
    [Fact]
    public async Task BankIdLauncher_Should_DefaultReloadBehavior()
    {
        var launcher = new BankIdLauncher(
            new TestBankIdSupportedDeviceDetector(),
            System.Array.Empty<IBankIdLauncherCustomAppCallback>());

        var info = await launcher.GetLaunchInfoAsync(new LaunchUrlRequest("", ""));

        Assert.False(info.DeviceWillReloadPageOnReturnFromBankIdApp);
    }

    [Fact]
    public async Task BankIdLauncher_Should_UseReloadBehaviourWhenImplemented()
    {
        var launcher = new BankIdLauncher(
            new TestBankIdSupportedDeviceDetector(),
            new [] { new TestBankIdLauncherCustomAppCallback() }); // Override behaviour on return from BankID app

        var info = await launcher.GetLaunchInfoAsync(new LaunchUrlRequest(string.Empty, string.Empty));

        Assert.True(info.DeviceWillReloadPageOnReturnFromBankIdApp);
    }

    private class TestBankIdLauncherCustomAppCallback : IBankIdLauncherCustomAppCallback
    {
        public Task<bool> IsApplicable(BankIdLauncherCustomAppCallbackContext context)
        {
            return Task.FromResult(true);
        }

        public Task<string> GetCustomAppReturnUrl(BankIdLauncherCustomAppCallbackContext context)
        {
            return Task.FromResult("/return");
        }

        public ReloadBehaviourOnReturnFromBankIdApp
            ReloadPageOnReturnFromBankIdApp(BankIdSupportedDevice detectedDevice) =>
            ReloadBehaviourOnReturnFromBankIdApp.Always;
    }

    private class TestBankIdSupportedDeviceDetector : IBankIdSupportedDeviceDetector
    {
        public BankIdSupportedDevice Detect()
        {
            // A device that will not reload the page on return from BankID app (Desktop Windows)
            return new BankIdSupportedDevice(
                BankIdSupportedDeviceType.Desktop,
                BankIdSupportedDeviceOs.Windows,
                BankIdSupportedDeviceBrowser.Chrome,
                BankIdSupportedDeviceOsVersion.Empty);
        }
    }
}
