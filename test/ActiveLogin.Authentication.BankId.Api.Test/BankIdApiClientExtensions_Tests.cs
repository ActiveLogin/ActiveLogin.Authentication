using ActiveLogin.Authentication.BankId.Api.Models;
using ActiveLogin.Authentication.BankId.Api.Test.TestHelpers;
using Moq;
using Xunit;
// ReSharper disable InvokeAsExtensionMethod

namespace ActiveLogin.Authentication.BankId.Api.Test
{
    public class BankIdApiClientExtensions_Tests
    {
        [Fact]
        public async void AuthAsync_WithEndUserIp_ShouldMap_ToAuthRequest_WithEndUserIp()
        {
            // Arrange
            var bankIdApiClientMock = new Mock<IBankIdApiClient>(MockBehavior.Strict);
            bankIdApiClientMock.Setup(client => client.AuthAsync(It.IsAny<AuthRequest>()))
                               .ReturnsAsync(It.IsAny<AuthResponse>());

            // Act
            await BankIdApiClientExtensions.AuthAsync(bankIdApiClientMock.Object, "1.1.1.1");

            // Assert
            var request = bankIdApiClientMock.GetFirstArgumentOfFirstInvocation<IBankIdApiClient, AuthRequest>();
            Assert.Equal("1.1.1.1", request.EndUserIp);
            Assert.Null(request.PersonalIdentityNumber);
        }

        [Fact]
        public async void AuthAsync_WithEndUserIp_AndPin_ShouldMap_ToAuthRequest_WithEndUserIp_AndPin()
        {
            // Arrange
            var bankIdApiClientMock = new Mock<IBankIdApiClient>(MockBehavior.Strict);
            bankIdApiClientMock.Setup(client => client.AuthAsync(It.IsAny<AuthRequest>()))
                .ReturnsAsync(It.IsAny<AuthResponse>());

            // Act
            await BankIdApiClientExtensions.AuthAsync(bankIdApiClientMock.Object, "1.1.1.1", "199908072391");

            // Assert
            var request = bankIdApiClientMock.GetFirstArgumentOfFirstInvocation<IBankIdApiClient, AuthRequest>();
            Assert.Equal("1.1.1.1", request.EndUserIp);
            Assert.Equal("199908072391", request.PersonalIdentityNumber);
        }

        [Fact]
        public async void CollectAsync_WithOrderRef_ShouldMap_ToCollectRequest_WithOrderRef()
        {
            // Arrange
            var bankIdApiClientMock = new Mock<IBankIdApiClient>(MockBehavior.Strict);
            bankIdApiClientMock.Setup(client => client.CollectAsync(It.IsAny<CollectRequest>()))
                .ReturnsAsync(It.IsAny<CollectResponse>());

            // Act
            await BankIdApiClientExtensions.CollectAsync(bankIdApiClientMock.Object, "or");

            // Assert
            var request = bankIdApiClientMock.GetFirstArgumentOfFirstInvocation<IBankIdApiClient, CollectRequest>();
            Assert.Equal("or", request.OrderRef);
        }

        [Fact]
        public async void CancelAsync_WithOrderRef_ShouldMap_ToCancelRequest_WithOrderRef()
        {
            // Arrange
            var bankIdApiClientMock = new Mock<IBankIdApiClient>(MockBehavior.Strict);
            bankIdApiClientMock.Setup(client => client.CancelAsync(It.IsAny<CancelRequest>()))
                .ReturnsAsync(It.IsAny<CancelResponse>());

            // Act
            await BankIdApiClientExtensions.CancelAsync(bankIdApiClientMock.Object, "or");

            // Assert
            var request = bankIdApiClientMock.GetFirstArgumentOfFirstInvocation<IBankIdApiClient, CancelRequest>();
            Assert.Equal("or", request.OrderRef);
        }
    }
}
