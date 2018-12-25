using ActiveLogin.Authentication.GrandId.Api.Models;
using ActiveLogin.Authentication.GrandId.Api.Test.TestHelpers;
using Moq;
using Xunit;
// ReSharper disable InvokeAsExtensionMethod

namespace ActiveLogin.Authentication.GrandId.Api.Test
{
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
            await GrandIdApiClientExtensions.BankIdFederatedLoginAsync(grandIdApiClientMock.Object, "ask", "https://cb/");

            // Assert
            var request = grandIdApiClientMock.GetFirstArgumentOfFirstInvocation<IGrandIdApiClient, BankIdFederatedLoginRequest>();
            Assert.Equal("ask", request.AuthenticateServiceKey);
            Assert.Equal("https://cb/", request.CallbackUrl);
            Assert.Null(request.PersonalIdentityNumber);
        }

        [Fact]
        public async void BankIdFederatedLoginAsync_WithSeviceKey_AndCallbackUrl_AndPin_ShouldMap_ToBankIdFederatedLoginRequest_WithSeviceKey_AndCallbackUrl_AndPin()
        {
            // Arrange
            var grandIdApiClientMock = new Mock<IGrandIdApiClient>(MockBehavior.Strict);
            grandIdApiClientMock.Setup(client => client.BankIdFederatedLoginAsync(It.IsAny<BankIdFederatedLoginRequest>()))
                .ReturnsAsync(It.IsAny<BankIdFederatedLoginResponse>());

            // Act
            await GrandIdApiClientExtensions.BankIdFederatedLoginAsync(grandIdApiClientMock.Object, "ask", "https://cb/", "199908072391");

            // Assert
            var request = grandIdApiClientMock.GetFirstArgumentOfFirstInvocation<IGrandIdApiClient, BankIdFederatedLoginRequest>();
            Assert.Equal("ask", request.AuthenticateServiceKey);
            Assert.Equal("https://cb/", request.CallbackUrl);
            Assert.Equal("199908072391", request.PersonalIdentityNumber);
        }

        [Fact]
        public async void BankIdGetSessionAsync_WithServiceKey_AndSessionId_ShouldMap_ToBankIdGetSessionRequest_WithServiceKey_AndSessionId()
        {
            // Arrange
            var grandIdApiClientMock = new Mock<IGrandIdApiClient>(MockBehavior.Strict);
            grandIdApiClientMock.Setup(client => client.BankIdGetSessionAsync(It.IsAny<BankIdSessionStateRequest>()))
                .ReturnsAsync(It.IsAny<BankIdSessionStateResponse>());

            // Act
            await GrandIdApiClientExtensions.BankIdGetSessionAsync(grandIdApiClientMock.Object, "ask", "s");

            // Assert
            var request = grandIdApiClientMock.GetFirstArgumentOfFirstInvocation<IGrandIdApiClient, BankIdSessionStateRequest>();
            Assert.Equal("ask", request.AuthenticateServiceKey);
            Assert.Equal("s", request.SessionId);
        }

        [Fact]
        public async void FederatedDirectLoginAsync_WithServiceKey_AndUsername_AndPassword_ShouldMap_ToFederatedDirectLoginRequest_WithServiceKey_AndUsername_AndPassword()
        {
            // Arrange
            var grandIdApiClientMock = new Mock<IGrandIdApiClient>(MockBehavior.Strict);
            grandIdApiClientMock.Setup(client => client.FederatedDirectLoginAsync(It.IsAny<FederatedDirectLoginRequest>()))
                .ReturnsAsync(It.IsAny<FederatedDirectLoginResponse>());

            // Act
            await GrandIdApiClientExtensions.FederatedDirectLoginAsync(grandIdApiClientMock.Object, "ask", "u", "p");

            // Assert
            var request = grandIdApiClientMock.GetFirstArgumentOfFirstInvocation<IGrandIdApiClient, FederatedDirectLoginRequest>();
            Assert.Equal("ask", request.AuthenticateServiceKey);
            Assert.Equal("u", request.Username);
            Assert.Equal("p", request.Password);
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
}
