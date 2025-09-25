#nullable enable
using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;

using ActiveLogin.Authentication.BankId.Api.Models;
using ActiveLogin.Authentication.BankId.AspNetCore.DataProtection;
using ActiveLogin.Authentication.BankId.AspNetCore.UserContext.Device.Resolvers;
using ActiveLogin.Authentication.BankId.AspNetCore.UserContext.Device.State;

using AngleSharp.Io;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

using Moq;

using Xunit;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Test.UserContext.Device.Resolvers;

public class BankIdDefaultEndUserWebDeviceDataResolver_Tests
{
    private class FakeProtector : IBankIdDataStateProtector<DeviceDataState>
    {
        public string Protect(DeviceDataState deviceDataState) =>
            JsonSerializer.Serialize(deviceDataState);
        public DeviceDataState Unprotect(string protectedDeviceDataState) =>
            JsonSerializer.Deserialize<DeviceDataState>(protectedDeviceDataState)!;
    }

    private readonly string _defaultUserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/132.0.0.0 Safari/537.36 Edg/132.0.0.0";
    private readonly string _defaultReferer = "https://example.se/this/that/";
    private readonly string _defaultDeviceIdentifier =
        Guid.Parse("BAB9B309-84D3-4D7B-90C0-89CCACB41D51").ToString();

    private (BankIdDefaultEndUserWebDeviceDataResolver resolver, string identifier) CreateSut(
        string userAgent, string referer, string deviceIdentifier)
    {
        var fakeProtector = new FakeProtector();
        var mockedAccessor = new Mock<IHttpContextAccessor>();
        var mockedHttpContext = new Mock<HttpContext>();

        Cookie? cookie = null;

        var createdIdentifier = string.IsNullOrWhiteSpace(deviceIdentifier) ? Guid.NewGuid().ToString() : deviceIdentifier;

        cookie = new Cookie(BankIdConstants.DefaultDeviceDataCookieName, JsonSerializer.Serialize(new DeviceDataState(createdIdentifier)));

        var headers = new Dictionary<string, StringValues>
        {
            { HeaderNames.UserAgent, new StringValues(userAgent ?? _defaultUserAgent) },
            { HeaderNames.Referer, new StringValues(referer ?? _defaultReferer) }
        };

        mockedHttpContext.Setup(x => x.Request.Headers).Returns(new HeaderDictionary(headers));
        mockedHttpContext.Setup(x => x.Request.Cookies[BankIdConstants.DefaultDeviceDataCookieName]).Returns(cookie?.Value ?? null);
        mockedHttpContext.Setup(x =>
            x.Response.Cookies.Append(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CookieOptions>()))
            .Callback<string, string, CookieOptions>((key, value, options) =>
            {
                cookie = new Cookie(key, value);
            });

        mockedAccessor.Setup(x => x.HttpContext).Returns(mockedHttpContext.Object);

        return (new BankIdDefaultEndUserWebDeviceDataResolver(mockedAccessor.Object, fakeProtector), createdIdentifier);
    }

    [Fact]
    public void GetDeviceData_Returns_DefaultValues()
    {
        //Arrange
        var (sut, createdIdentifier) = CreateSut(_defaultUserAgent, _defaultReferer, _defaultDeviceIdentifier);

        // Act
        var data = sut.GetDeviceData() as DeviceDataWeb;

        // Assert
        Assert.NotNull(data);
        Assert.Equal("example.se", data.ReferringDomain);
        Assert.Equal(_defaultUserAgent, data.UserAgent);
        Assert.Equal(createdIdentifier, data.DeviceIdentifier);
    }

    [Fact]
    public void GetDeviceData_Returns_CreatedDeviceIdentifier()
    {
        //Arrange
        var (sut, createdIdentifier) = CreateSut(_defaultUserAgent, _defaultReferer, string.Empty);

        // Act
        var data = sut.GetDeviceData() as DeviceDataWeb;

        // Assert
        Assert.NotNull(data);
        Assert.Equal("example.se", data.ReferringDomain);
        Assert.Equal(_defaultUserAgent, data.UserAgent);
        Assert.Equal(createdIdentifier, data.DeviceIdentifier);
    }

}
