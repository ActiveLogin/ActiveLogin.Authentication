using ActiveLogin.Authentication.BankId.Core.SupportedDevice;

using Microsoft.AspNetCore.Http;

namespace ActiveLogin.Authentication.BankId.AspNetCore.SupportedDevice;

public class BankIdSupportedDeviceDetector : IBankIdSupportedDeviceDetector
{
    private const string UserAgentHttpHeaderName = "User-Agent";
    private readonly IHttpContextAccessor _httpContextAccessor;

    public BankIdSupportedDeviceDetector(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public BankIdSupportedDevice Detect()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
        {
            return BankIdSupportedDevice.Unknown;
        }

        var userAgent = httpContext.Request.Headers[UserAgentHttpHeaderName].ToString();
        return BankIdSupportedDeviceDetectorByUserAgent.Detect(userAgent);
    }
}
