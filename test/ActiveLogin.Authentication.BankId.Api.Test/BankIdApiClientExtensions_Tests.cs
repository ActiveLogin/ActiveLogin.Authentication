using System.Threading.Tasks;
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
        public async Task AuthAsync_WithEndUserIp_ShouldMap_ToAuthRequest_WithEndUserIp()
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
        public async Task AuthAsync_WithEndUserIp_AndPin_ShouldMap_ToAuthRequest_WithEndUserIp_AndPin()
        {
            // Arrange
            var bankIdApiClientMock = new Mock<IBankIdApiClient>(MockBehavior.Strict);
            bankIdApiClientMock.Setup(client => client.AuthAsync(It.IsAny<AuthRequest>()))
                .ReturnsAsync(It.IsAny<AuthResponse>());

            // Act
            await BankIdApiClientExtensions.AuthAsync(bankIdApiClientMock.Object, "1.1.1.1", "201801012392");

            // Assert
            var request = bankIdApiClientMock.GetFirstArgumentOfFirstInvocation<IBankIdApiClient, AuthRequest>();
            Assert.Equal("1.1.1.1", request.EndUserIp);
            Assert.Equal("201801012392", request.PersonalIdentityNumber);
        }

        [Fact]
        public async Task SignAsync_WithEndUserIp_ShouldMap_ToSignRequest_WithEndUserIp()
        {
            // Arrange
            var bankIdApiClientMock = new Mock<IBankIdApiClient>(MockBehavior.Strict);
            bankIdApiClientMock.Setup(client => client.SignAsync(It.IsAny<SignRequest>()))
                               .ReturnsAsync(It.IsAny<SignResponse>());

            // Act
            await BankIdApiClientExtensions.SignAsync(bankIdApiClientMock.Object, "1.1.1.1", "userVisibleData");

            // Assert
            var request = bankIdApiClientMock.GetFirstArgumentOfFirstInvocation<IBankIdApiClient, SignRequest>();
            Assert.Equal("1.1.1.1", request.EndUserIp);
            Assert.Null(request.PersonalIdentityNumber);
        }

        [Fact]
        public async Task SignAsync_WithEndUserIp_AndPin_ShouldMap_ToSignRequest_WithEndUserIp_AndPin()
        {
            // Arrange
            var bankIdApiClientMock = new Mock<IBankIdApiClient>(MockBehavior.Strict);
            bankIdApiClientMock.Setup(client => client.SignAsync(It.IsAny<SignRequest>()))
                .ReturnsAsync(It.IsAny<SignResponse>());

            // Act
            await BankIdApiClientExtensions.SignAsync(bankIdApiClientMock.Object, "1.1.1.1", "userVisibleData", "201801012392");

            // Assert
            var request = bankIdApiClientMock.GetFirstArgumentOfFirstInvocation<IBankIdApiClient, SignRequest>();
            Assert.Equal("1.1.1.1", request.EndUserIp);
            Assert.Equal("201801012392", request.PersonalIdentityNumber);
        }

        [Fact]
        public async Task CollectAsync_WithOrderRef_ShouldMap_ToCollectRequest_WithOrderRef()
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
        public async Task CancelAsync_WithOrderRef_ShouldMap_ToCancelRequest_WithOrderRef()
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