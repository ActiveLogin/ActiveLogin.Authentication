using System.Threading.Tasks;

using ActiveLogin.Authentication.BankId.Core.Launcher;
using ActiveLogin.Authentication.BankId.Core.SupportedDevice;

using Xunit;

namespace ActiveLogin.Authentication.BankId.Core.Test;

public class BankIdLauncher_Tests
{
    [Fact]
    public async Task BankIdLauncherReloadPageOnReturnFromBankIdApp_Should_Be_Ignored_When_Null()
    {
        var launcher = new BankIdLauncher(
            new TestBankIdSupportedDeviceDetector(),
            System.Array.Empty<IBankIdLauncherCustomAppCallback>(),
            reloadPageOnReturnFromBankIdApp: null); // Default behaviour on return from BankID app

        var info = await launcher.GetLaunchInfoAsync(new LaunchUrlRequest("", ""));

        Assert.False(info.DeviceWillReloadPageOnReturnFromBankIdApp);
    }

    [Fact]
    public async Task BankIdLauncherReloadPageOnReturnFromBankIdApp_Should_Use_Override()
    {
        var launcher = new BankIdLauncher(
            new TestBankIdSupportedDeviceDetector(),
            System.Array.Empty<IBankIdLauncherCustomAppCallback>(),
            new TestReloadPageOnReturnFromBankIdApp()); // Override behaviour on return from BankID app

        var info = await launcher.GetLaunchInfoAsync(new LaunchUrlRequest(string.Empty, string.Empty));

        Assert.True(info.DeviceWillReloadPageOnReturnFromBankIdApp);
    }

    private class TestReloadPageOnReturnFromBankIdApp : IReloadPageOnReturnFromBankIdApp
    {
        public bool DeviceWillReloadPageOnReturn(BankIdSupportedDevice detectedDevice) => true;
    }

    private class TestBankIdSupportedDeviceDetector : IBankIdSupportedDeviceDetector
    {
        public BankIdSupportedDevice Detect()
        {
            // A device that will reload the page on return from BankID app (Desktop Windows)
            return new BankIdSupportedDevice(
                BankIdSupportedDeviceType.Desktop,
                BankIdSupportedDeviceOs.Windows,
                BankIdSupportedDeviceBrowser.Chrome,
                BankIdSupportedDeviceOsVersion.Empty);
        }
    }
}
