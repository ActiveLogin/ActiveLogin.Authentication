using ActiveLogin.Authentication.BankId.AspNetCore.Launcher;
using ActiveLogin.Authentication.BankId.AspNetCore.SupportedDevice;

using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.ServerSample.Controllers;

[AllowAnonymous]
public class DebuggerController : Controller
{
    private const string SampleSchemeUrl = "bankid:///?autostarttoken=6951595e-2a2c-4a65-a276-f24723284712&redirect=null";
    private const string SampleAppLinkUrl = "https://app.bankid.com/?autostarttoken=6951595e-2a2c-4a65-a276-f24723284712&redirect=null";
    private const string UserAgentHttpHeaderName = "User-Agent";

    private readonly IBankIdSupportedDeviceDetector _bankIdDeviceDetector;
    private readonly IBankIdLauncher _bankIdLauncher;

    public DebuggerController(IBankIdSupportedDeviceDetector bankIdDeviceDetector, IBankIdLauncher bankIdLauncher)
    {
        _bankIdDeviceDetector = bankIdDeviceDetector;
        _bankIdLauncher = bankIdLauncher;
    }

    public IActionResult Index()
    {
        return RedirectToAction("Launch");
    }

    public IActionResult Info(string userAgent, string redirectUrl, string autoStartToken, string? relyingPartyReference)
    {
        if (string.IsNullOrEmpty(userAgent))
        {
            userAgent = HttpContext.Request.Headers[UserAgentHttpHeaderName];
        }

        if (string.IsNullOrEmpty(redirectUrl))
        {
            redirectUrl = HttpContext.Request.GetUri().ToString();
        }

        if (string.IsNullOrEmpty(autoStartToken))
        {
            autoStartToken = Guid.NewGuid().ToString();
        }

        var launchRequest = new LaunchUrlRequest(redirectUrl, autoStartToken);
        if (!string.IsNullOrEmpty(relyingPartyReference))
        {
            launchRequest = new LaunchUrlRequest(redirectUrl, autoStartToken, relyingPartyReference);
        }


        var httpContext = HttpContext;
        httpContext.Request.Headers[UserAgentHttpHeaderName] = userAgent;

        var detectedDevice = _bankIdDeviceDetector.Detect(userAgent);
        var launchInfo = _bankIdLauncher.GetLaunchInfo(launchRequest, httpContext);

        var viewModel = new DebuggerInfoViewModel
        {
            UserAgent = userAgent,

            LaunchUrlRequest = launchRequest,

            DetectedDevice = detectedDevice,
            DetectedDeviceImplementation = _bankIdDeviceDetector.GetType().FullName ?? string.Empty,

            DetectedDeviceLaunchInfo = launchInfo,
            DetectedDeviceLaunchInfoImplementation = _bankIdLauncher.GetType().FullName ?? string.Empty
        };

        return View(viewModel);
    }

    public IActionResult Launch()
    {
        var userAgent = HttpContext.Request.Headers[UserAgentHttpHeaderName];
        var detectedDevice = _bankIdDeviceDetector.Detect(userAgent);
        var viewModel = new DebuggerLaunchViewModel
        {
            UserAgent = userAgent,
            DetectedDevice = detectedDevice,

            SchemeUrl = SampleSchemeUrl,
            AppLinkUrl = SampleAppLinkUrl
        };

        return View(viewModel);
    }

    public IActionResult AutoLaunch(string type, bool withConfirm)
    {
        if (type == "AppLink")
        {
            return View(new DebuggerAutoLaunchViewModel
            {
                Type = "AppLink",
                Url = SampleAppLinkUrl,
                WithConfirm = withConfirm
            });
        }

        return View(new DebuggerAutoLaunchViewModel
        {
            Type = "Scheme",
            Url = SampleSchemeUrl,
            WithConfirm = withConfirm
        });
    }

    public class DebuggerInfoViewModel
    {
        public string UserAgent { get; set; } = string.Empty;
        public LaunchUrlRequest LaunchUrlRequest { get; set; } = new LaunchUrlRequest("", "");


        public BankIdSupportedDevice? DetectedDevice { get; set; } = null;
        public string DetectedDeviceImplementation { get; set; } = "";

        public BankIdLaunchInfo? DetectedDeviceLaunchInfo { get; set; } = null;
        public string DetectedDeviceLaunchInfoImplementation { get; set; } = "";
    }

    public class DebuggerLaunchViewModel
    {
        public string UserAgent { get; set; } = string.Empty;
        public BankIdSupportedDevice? DetectedDevice { get; set; } = null;

        public string SchemeUrl { get; set; } = "";
        public string AppLinkUrl { get; set; } = "";
    }

    public class DebuggerAutoLaunchViewModel
    {
        public string Type { get; set; } = "";
        public string Url { get; set; } = "";
        public bool WithConfirm { get; set; } = false;
    }
}
