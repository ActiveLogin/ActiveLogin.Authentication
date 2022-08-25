using ActiveLogin.Authentication.BankId.Core.SupportedDevice;

using Microsoft.AspNetCore.Http;

namespace ActiveLogin.Authentication.BankId.AspNetCore.SupportedDevice;

public class BankIdSupportedDeviceDetector : IBankIdSupportedDeviceDetector
{
    private const string UserAgentHttpHeaderName = "User-Agent";
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IBankIdSupportedDeviceDetectorByUserAgent _bankIdSupportedDeviceDetectorByUserAgent;

    public BankIdSupportedDeviceDetector(IHttpContextAccessor httpContextAccessor, IBankIdSupportedDeviceDetectorByUserAgent bankIdSupportedDeviceDetectorByUserAgent)
    {
        _httpContextAccessor = httpContextAccessor;
        _bankIdSupportedDeviceDetectorByUserAgent = bankIdSupportedDeviceDetectorByUserAgent;
    }

    public BankIdSupportedDevice Detect()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
        {
            return BankIdSupportedDevice.Unknown;
        }

        var userAgent = httpContext.Request.Headers[UserAgentHttpHeaderName].ToString();
        return _bankIdSupportedDeviceDetectorByUserAgent.Detect(userAgent);
    }
}
