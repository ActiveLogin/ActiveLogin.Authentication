using ActiveLogin.Authentication.BankId.Api;
using ActiveLogin.Authentication.BankId.Api.Models;
using ActiveLogin.Authentication.BankId.AspNetCore.DataProtection;
using ActiveLogin.Authentication.BankId.AspNetCore.UserContext.Device.State;
using ActiveLogin.Authentication.BankId.Core.UserContext.Device;
using ActiveLogin.Authentication.BankId.Core.UserContext.Device.Exceptions;

using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace ActiveLogin.Authentication.BankId.AspNetCore.UserContext.Device.Resolvers;

/// <inheritdoc cref="IBankIdEndUserDeviceDataResolver"/>
public sealed class BankIdDefaultEndUserWebDeviceDataResolver(
    IHttpContextAccessor httpContextAccessor,
    IBankIdDeviceDataProtector protector)
    : BankIdEndUserWebDeviceDataResolverBase(httpContextAccessor, protector)
{
    public override BankIdEndUserDeviceType DeviceType => BankIdEndUserDeviceType.Web;

    public override Task<IBankIdEndUserDeviceData> GetDeviceDataAsync()
    {
        return Task.FromResult(GetDeviceData());
    }

    public override IBankIdEndUserDeviceData GetDeviceData()
    {
        TryGetWebDeviceParameters(out var webDeviceParameters);
        return webDeviceParameters ?? throw new DeviceDataException("Could not resolve device parameters for web device");
    }

    private bool TryGetWebDeviceParameters(out DeviceDataWeb? parameters)
    {
        parameters = null;

        var hasUserAgent = TryGetHeader(HeaderNames.UserAgent, out var userAgent);
        var hasReferrerDomain = TryCreateReferringDomain(out var referringDomain);
        var hasDeviceIdentifier = TryGetDeviceIdentifier(out var deviceIdentifier);

        if (!hasDeviceIdentifier || !hasReferrerDomain || !hasUserAgent)
        {
            return false;
        }

        userAgent = TruncateUserAgent(userAgent);

        parameters = new DeviceDataWeb(
            referringDomain: referringDomain,
            userAgent: userAgent,
            deviceIdentifier: deviceIdentifier);

        return true;
    }

    private bool TryGetDeviceIdentifier(out string deviceIdentifier)
    {
        var persistedDeviceIdentifier = GetDeviceDataFromCookie();
        if (persistedDeviceIdentifier == null)
        {
            deviceIdentifier = Guid.NewGuid().ToString();
            AppendDeviceDataCookie(new DeviceDataState(deviceIdentifier));
        }
        else
        {
            deviceIdentifier = persistedDeviceIdentifier.DeviceIdentifier;
        }

        return true;
    }

    private bool TryCreateReferringDomain(out string referringDomain)
    {
        referringDomain = string.Empty;

        var hasReferrer = TryGetHeader(HeaderNames.Referer, out var referrer);

        if (hasReferrer == false)
        {
            return false;
        }

        if (!Uri.IsWellFormedUriString(referrer, UriKind.RelativeOrAbsolute))
        {
            return false;
        }

        var parsedReferrer = new Uri(referrer);
        referringDomain = parsedReferrer.DnsSafeHost;
        return true;

    }

    private bool TryGetHeader(string headerName, out string headerValue)
    {
        headerValue = string.Empty;
        if (Context.Request.Headers.TryGetValue(headerName, out var headerValues) != true)
        {
            return false;
        }

        headerValue = headerValues.FirstOrDefault() ?? string.Empty;
        return true;
    }

    private static string TruncateUserAgent(string userAgent)
    {
        return userAgent.Length > BankIdApiLimits.UserAgentMaxLength
            ? userAgent[..BankIdApiLimits.UserAgentMaxLength]
            : userAgent;
    }

}
