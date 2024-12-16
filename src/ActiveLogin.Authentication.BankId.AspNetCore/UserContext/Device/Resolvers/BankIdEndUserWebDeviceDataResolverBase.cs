using ActiveLogin.Authentication.BankId.AspNetCore.DataProtection;
using ActiveLogin.Authentication.BankId.AspNetCore.Helpers;
using ActiveLogin.Authentication.BankId.AspNetCore.UserContext.Device.State;
using ActiveLogin.Authentication.BankId.Core.UserContext.Device.Resolvers;

using Microsoft.AspNetCore.Http;

namespace ActiveLogin.Authentication.BankId.AspNetCore.UserContext.Device.Resolvers;

public abstract class BankIdEndUserWebDeviceDataResolverBase(
    IHttpContextAccessor httpContextAccessor,
    IBankIdDeviceDataProtector deviceDataProtector) : BankIdDeviceDataResolverBase
{
    protected HttpContext Context => httpContextAccessor.HttpContext ?? throw new InvalidOperationException("HttpContext is null");
    private const string DeviceDataCookieNameParameterName = "DeviceDataCookie.Name";

    private CookieBuilder _deviceDataCookieBuilder = new()
    {
        Name = BankIdConstants.DefaultDeviceDataCookieName,
        SecurePolicy = CookieSecurePolicy.SameAsRequest,
        HttpOnly = true,
        SameSite = SameSiteMode.Lax,
        IsEssential = true,
        Expiration = BankIdConstants.DeviceDataRefreshInterval
    };

    public CookieBuilder DeviceDataCookie
    {
        get => _deviceDataCookieBuilder;
        set => _deviceDataCookieBuilder = value ?? throw new ArgumentNullException(nameof(value));
    }

    protected void AppendDeviceDataCookie(DeviceDataState state)
    {
        Validators.ThrowIfNullOrWhitespace(DeviceDataCookie.Name, DeviceDataCookieNameParameterName);
        var cookieOptions = DeviceDataCookie.Build(Context);
        var cookieValue = deviceDataProtector.Protect(state);
        Context.Response.Cookies.Append(DeviceDataCookie.Name, cookieValue, cookieOptions);
    }

    protected DeviceDataState? GetDeviceDataFromCookie()
    {
        Validators.ThrowIfNullOrWhitespace(DeviceDataCookie.Name, DeviceDataCookieNameParameterName);

        var protectedState = Context.Request.Cookies[DeviceDataCookie.Name];
        return string.IsNullOrEmpty(protectedState)
            ? null
            : deviceDataProtector.Unprotect(protectedState);
    }

    protected void DeleteDeviceDataCookie()
    {
        Validators.ThrowIfNullOrWhitespace(DeviceDataCookie.Name, DeviceDataCookieNameParameterName);
        var cookieOptions = DeviceDataCookie.Build(Context);
        Context.Response.Cookies.Delete(DeviceDataCookie.Name, cookieOptions);
    }

}
