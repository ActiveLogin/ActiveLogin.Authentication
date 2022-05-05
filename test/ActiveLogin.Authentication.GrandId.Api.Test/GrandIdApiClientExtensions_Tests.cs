using ActiveLogin.Authentication.GrandId.Api.Models;
using ActiveLogin.Authentication.GrandId.Api.Test.TestHelpers;
using Moq;
using Xunit;
// ReSharper disable InvokeAsExtensionMethod

namespace ActiveLogin.Authentication.GrandId.Api.Test;

public class GrandIdApiClientExtensions_Tests
{
    [Fact]
    public async void BankIdFederatedLoginAsync_WithSeviceKey_AndCallbackUrl_ShouldMap_ToBankIdFederatedLoginRequest_WithSeviceKey_AndCallbackUrl()
    {
        // Arrange
        var grandIdApiClientMock = new Mock<IGrandIdApiClient>(MockBehavior.Strict);
        grandIdApiClientMock.Setup(client => client.BankIdFederatedLoginAsync(It.IsAny<BankIdFederatedLoginRequest>()))
            .ReturnsAsync(It.IsAny<BankIdFederatedLoginResponse>());

        // Act
        await GrandIdApiClientExtensions.BankIdFederatedLoginAsync(grandIdApiClientMock.Object, "https://cb/");

        // Assert
        var request = grandIdApiClientMock.GetFirstArgumentOfFirstInvocation<IGrandIdApiClient, BankIdFederatedLoginRequest>();
        Assert.Equal("https://cb/", request.CallbackUrl);
        Assert.Null(request.PersonalIdentityNumber);
    }

    [Fact]
    public async void BankIdFederatedLoginAsync_WithAllValues_ShouldMap_ToBankIdFederatedLoginRequest_WithAllValues()
    {
        // Arrange
        var grandIdApiClientMock = new Mock<IGrandIdApiClient>(MockBehavior.Strict);
        grandIdApiClientMock.Setup(client => client.BankIdFederatedLoginAsync(It.IsAny<BankIdFederatedLoginRequest>()))
            .ReturnsAsync(It.IsAny<BankIdFederatedLoginResponse>());

        // Act
        await GrandIdApiClientExtensions.BankIdFederatedLoginAsync(grandIdApiClientMock.Object,
            "https://cb/",
            true,
            true,
            true,
            "20180101239",
            true,
            "https://cu/",
            true,
            "uvd",
            "unvd"
        );

        // Assert
        var request = grandIdApiClientMock.GetFirstArgumentOfFirstInvocation<IGrandIdApiClient, BankIdFederatedLoginRequest>();
        Assert.Equal("https://cb/", request.CallbackUrl);
        Assert.Equal(true, request.UseChooseDevice);
        Assert.Equal(true, request.UseSameDevice);
        Assert.Equal(true, request.AskForPersonalIdentityNumber);
        Assert.Equal("20180101239", request.PersonalIdentityNumber);
        Assert.Equal(true, request.RequireMobileBankId);
        Assert.Equal("https://cu/", request.CustomerUrl);
        Assert.Equal(true, request.ShowGui);
        Assert.Equal("uvd", request.SignUserVisibleData);
        Assert.Equal("unvd", request.SignUserNonVisibleData);
    }

    [Fact]
    public async void BankIdGetSessionAsync_WithServiceKey_AndSessionId_ShouldMap_ToBankIdGetSessionRequest_WithServiceKey_AndSessionId()
    {
        // Arrange
        var grandIdApiClientMock = new Mock<IGrandIdApiClient>(MockBehavior.Strict);
        grandIdApiClientMock.Setup(client => client.BankIdGetSessionAsync(It.IsAny<BankIdGetSessionRequest>()))
            .ReturnsAsync(It.IsAny<BankIdGetSessionResponse>());

        // Act
        await GrandIdApiClientExtensions.BankIdGetSessionAsync(grandIdApiClientMock.Object, "s");

        // Assert
        var request = grandIdApiClientMock.GetFirstArgumentOfFirstInvocation<IGrandIdApiClient, BankIdGetSessionRequest>();
        Assert.Equal("s", request.SessionId);
    }

    [Fact]
    public async void LogoutAsync_WithSessionId_ShouldMap_ToLogoutRequest_WithSessionId()
    {
        // Arrange
        var grandIdApiClientMock = new Mock<IGrandIdApiClient>(MockBehavior.Strict);
        grandIdApiClientMock.Setup(client => client.LogoutAsync(It.IsAny<LogoutRequest>()))
            .ReturnsAsync(It.IsAny<LogoutResponse>());

        // Act
        await GrandIdApiClientExtensions.LogoutAsync(grandIdApiClientMock.Object, "s");

        // Assert
        var request = grandIdApiClientMock.GetFirstArgumentOfFirstInvocation<IGrandIdApiClient, LogoutRequest>();
        Assert.Equal("s", request.SessionId);
    }
}