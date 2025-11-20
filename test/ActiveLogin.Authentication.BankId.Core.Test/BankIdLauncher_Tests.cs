using System.Threading.Tasks;

using ActiveLogin.Authentication.BankId.Core.Launcher;
using ActiveLogin.Authentication.BankId.Core.SupportedDevice;

using Xunit;

namespace ActiveLogin.Authentication.BankId.Core.Test;


public class FakeBankIdSupportedDeviceDetector(BankIdSupportedDevice device) : IBankIdSupportedDeviceDetector
{
    public BankIdSupportedDevice Detect() => device;
}

public class FakeCustomBrowserResolver(BankIdLauncherCustomBrowserConfig config = null) : ICustomBrowserResolver
{
    public Task<BankIdLauncherCustomBrowserConfig> GetConfig(LaunchUrlRequest launchUrlRequest)
    {
        return Task.FromResult(config);
    }
}

/// <summary>
/// Tests we want to do:
/// * Ensure that BankIdLauncher generates a valid LaunchInfo
/// * Test that BankIdLauncher uses detected device to determine if user interaction is needed to launch BankID app
/// * Test that BankIdLauncher uses the CustomBrowserResolver to determine if the browser will reload on return from BankID app
/// </summary>

public class BankIdLauncher_Tests
{
    [Fact]
    public async Task BankIdLauncher_Should_GenerateValidLaunchInfo()
    {
        var launcher = new BankIdLauncher(
            new FakeBankIdSupportedDeviceDetector(BankIdTestDevices.Mobile.Ios.Chrome),
            new FakeCustomBrowserResolver()
        );

        var info = await launcher.GetLaunchInfoAsync(new LaunchUrlRequest("https://localhost:5001/ActiveLogin/Auth/Init", ""));

        Assert.NotNull(info);
    }

    [Fact]
    public async Task BankIdLauncher_Should_UseDetectedDeviceToDetermineUserInteraction()
    {
        var launcher = new BankIdLauncher(
            new FakeBankIdSupportedDeviceDetector(BankIdTestDevices.Mobile.Ios.Chrome),
            new FakeCustomBrowserResolver()
        );

        var info = await launcher.GetLaunchInfoAsync(new LaunchUrlRequest("https://localhost:5001/ActiveLogin/Auth/Init", ""));

        Assert.False(info.DeviceMightRequireUserInteractionToLaunchBankIdApp);
    }
}
