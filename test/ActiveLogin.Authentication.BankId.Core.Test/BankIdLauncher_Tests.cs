using System.Text.Encodings.Web;
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
            System.Array.Empty<IBankIdLauncherCustomBrowser>());

        var info = await launcher.GetLaunchInfoAsync(new LaunchUrlRequest("", ""));

        Assert.False(info.DeviceWillReloadPageOnReturnFromBankIdApp);
    }

    [Fact]
    public async Task BankIdLauncher_Should_UseReloadBehaviourWhenImplemented()
    {
        var launcher = new BankIdLauncher(
            new TestBankIdSupportedDeviceDetector(),
            new [] { new TestBankIdLauncherCustomBrowser() }); // Override behaviour on return from BankID app

        var info = await launcher.GetLaunchInfoAsync(new LaunchUrlRequest(string.Empty, string.Empty));

        Assert.True(info.DeviceWillReloadPageOnReturnFromBankIdApp);
    }

    [Theory]
    [InlineData(BankIdSupportedDeviceOs.Ios, BankIdSupportedDeviceBrowser.Safari)]
    [InlineData(BankIdSupportedDeviceOs.Ios, BankIdSupportedDeviceBrowser.Chrome)]
    [InlineData(BankIdSupportedDeviceOs.Android, BankIdSupportedDeviceBrowser.Chrome)]
    [InlineData(BankIdSupportedDeviceOs.Android, BankIdSupportedDeviceBrowser.Firefox)]
    public async Task BankIdLauncher_Should_UseAppLink_ForMobileDevices(BankIdSupportedDeviceOs os, BankIdSupportedDeviceBrowser browser)
    {
        var launcher = CreateLauncher(Mobile(os, browser));

        var info = await launcher.GetLaunchInfoAsync(new LaunchUrlRequest("https://example.com/return", "token"));

        Assert.StartsWith("https://app.bankid.com/", info.LaunchUrl);
    }

    [Fact]
    public async Task BankIdLauncher_Should_UseScheme_ForDesktop()
    {
        var launcher = CreateLauncher(new BankIdSupportedDevice(
            BankIdSupportedDeviceType.Desktop,
            BankIdSupportedDeviceOs.Windows,
            BankIdSupportedDeviceBrowser.Chrome,
            BankIdSupportedDeviceOsVersion.Empty));

        var info = await launcher.GetLaunchInfoAsync(new LaunchUrlRequest("https://example.com/return", "token"));

        Assert.StartsWith("bankid:///", info.LaunchUrl);
    }

    [Fact]
    public async Task BankIdLauncher_Should_KeepAutostart_ForIos()
    {
        var launcher = CreateLauncher(Mobile(BankIdSupportedDeviceOs.Ios, BankIdSupportedDeviceBrowser.Safari));

        var info = await launcher.GetLaunchInfoAsync(new LaunchUrlRequest("https://example.com/return", "token"));

        Assert.False(info.DeviceMightRequireUserInteractionToLaunchBankIdApp);
    }

    [Theory]
    [InlineData(BankIdSupportedDeviceBrowser.Chrome, true)]
    [InlineData(BankIdSupportedDeviceBrowser.Edge, true)]
    [InlineData(BankIdSupportedDeviceBrowser.SamsungBrowser, true)]
    [InlineData(BankIdSupportedDeviceBrowser.Firefox, false)]
    [InlineData(BankIdSupportedDeviceBrowser.Opera, false)]
    public async Task BankIdLauncher_Should_RequireUserInteraction_OnlyForRestrictedAndroidBrowsers(BankIdSupportedDeviceBrowser browser, bool expected)
    {
        var launcher = CreateLauncher(Mobile(BankIdSupportedDeviceOs.Android, browser));

        var info = await launcher.GetLaunchInfoAsync(new LaunchUrlRequest("https://example.com/return", "token"));

        Assert.Equal(expected, info.DeviceMightRequireUserInteractionToLaunchBankIdApp);
    }

    [Theory]
    [InlineData(BrowserMightRequireUserInteractionToLaunch.Always, true)]
    [InlineData(BrowserMightRequireUserInteractionToLaunch.Never, false)]
    public async Task BankIdLauncher_Should_HonorCustomBrowserInteractionOverride(BrowserMightRequireUserInteractionToLaunch behaviour, bool expected)
    {
        // Use an iOS device where the default would be 'false' to prove the override is applied.
        var launcher = new BankIdLauncher(
            new ConfigurableDeviceDetector(Mobile(BankIdSupportedDeviceOs.Ios, BankIdSupportedDeviceBrowser.Safari)),
            new[] { new InteractionOverrideCustomBrowser(behaviour) });

        var info = await launcher.GetLaunchInfoAsync(new LaunchUrlRequest("https://example.com/return", "token"));

        Assert.Equal(expected, info.DeviceMightRequireUserInteractionToLaunchBankIdApp);
    }

    [Fact]
    public async Task BankIdLauncher_Should_SetRedirectNull_ForAndroid()
    {
        // BankID guideline: Android app links should use redirect=null.
        var launcher = CreateLauncher(Mobile(BankIdSupportedDeviceOs.Android, BankIdSupportedDeviceBrowser.Chrome));

        var info = await launcher.GetLaunchInfoAsync(new LaunchUrlRequest("https://example.com/return", "token"));

        Assert.Contains("redirect=null", info.LaunchUrl);
    }

    [Fact]
    public async Task BankIdLauncher_Should_UseReturnUrlAsRedirect_ForIosSafari()
    {
        const string returnUrl = "https://example.com/return";
        var launcher = CreateLauncher(Mobile(BankIdSupportedDeviceOs.Ios, BankIdSupportedDeviceBrowser.Safari));

        var info = await launcher.GetLaunchInfoAsync(new LaunchUrlRequest(returnUrl, "token"));

        Assert.Contains($"redirect={UrlEncoder.Default.Encode(returnUrl)}", info.LaunchUrl);
    }

    [Theory]
    [InlineData(BankIdSupportedDeviceBrowser.Chrome, "googlechromes://")]
    [InlineData(BankIdSupportedDeviceBrowser.Firefox, "firefox://")]
    public async Task BankIdLauncher_Should_UseBrowserSchemeAsRedirect_ForIosThirdPartyBrowsers(BankIdSupportedDeviceBrowser browser, string expectedScheme)
    {
        var launcher = CreateLauncher(Mobile(BankIdSupportedDeviceOs.Ios, browser));

        var info = await launcher.GetLaunchInfoAsync(new LaunchUrlRequest("https://example.com/return", "token"));

        Assert.Contains($"redirect={UrlEncoder.Default.Encode(expectedScheme)}", info.LaunchUrl);
    }

    [Theory]
    [InlineData(BankIdSupportedDeviceBrowser.Edge)]
    [InlineData(BankIdSupportedDeviceBrowser.Opera)]
    public async Task BankIdLauncher_Should_SetRedirectNull_ForIosEdgeAndOpera(BankIdSupportedDeviceBrowser browser)
    {
        var launcher = CreateLauncher(Mobile(BankIdSupportedDeviceOs.Ios, browser));

        var info = await launcher.GetLaunchInfoAsync(new LaunchUrlRequest("https://example.com/return", "token"));

        Assert.Contains("redirect=null", info.LaunchUrl);
    }

    private static BankIdLauncher CreateLauncher(BankIdSupportedDevice device)
    {
        return new BankIdLauncher(
            new ConfigurableDeviceDetector(device),
            System.Array.Empty<IBankIdLauncherCustomBrowser>());
    }

    private static BankIdSupportedDevice Mobile(BankIdSupportedDeviceOs os, BankIdSupportedDeviceBrowser browser)
    {
        return new BankIdSupportedDevice(
            BankIdSupportedDeviceType.Mobile,
            os,
            browser,
            new BankIdSupportedDeviceOsVersion(15));
    }

    private class InteractionOverrideCustomBrowser : IBankIdLauncherCustomBrowser
    {
        private readonly BrowserMightRequireUserInteractionToLaunch _behaviour;

        public InteractionOverrideCustomBrowser(BrowserMightRequireUserInteractionToLaunch behaviour)
        {
            _behaviour = behaviour;
        }

        public Task<bool> IsApplicable(BankIdLauncherCustomBrowserContext context)
        {
            return Task.FromResult(true);
        }

        public Task<BankIdLauncherCustomBrowserConfig> GetCustomAppCallbackResult(BankIdLauncherCustomBrowserContext context)
        {
            return Task.FromResult(
                new BankIdLauncherCustomBrowserConfig(null, BrowserReloadBehaviourOnReturnFromBankIdApp.Default, _behaviour)
            );
        }
    }

    private class ConfigurableDeviceDetector : IBankIdSupportedDeviceDetector
    {
        private readonly BankIdSupportedDevice _device;

        public ConfigurableDeviceDetector(BankIdSupportedDevice device)
        {
            _device = device;
        }

        public BankIdSupportedDevice Detect()
        {
            return _device;
        }
    }

    private class TestBankIdLauncherCustomBrowser : IBankIdLauncherCustomBrowser
    {
        public Task<bool> IsApplicable(BankIdLauncherCustomBrowserContext context)
        {
            return Task.FromResult(true);
        }

        public Task<BankIdLauncherCustomBrowserConfig> GetCustomAppCallbackResult(BankIdLauncherCustomBrowserContext context)
        {
            return Task.FromResult(
                new BankIdLauncherCustomBrowserConfig("/return", BrowserReloadBehaviourOnReturnFromBankIdApp.Always)
            );
        }
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
