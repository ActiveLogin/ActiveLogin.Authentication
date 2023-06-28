using System.Threading.Tasks;

using ActiveLogin.Authentication.BankId.Api.Models;
using ActiveLogin.Authentication.BankId.Api.Test.TestHelpers;

using Moq;

using Xunit;

namespace ActiveLogin.Authentication.BankId.Api.Test;

public class BankIdVerifyApiClientExtensions_Tests
{
    [Fact]
    public async Task VerifyAsync_WithQrCode_ShouldMap_ToVerifyRequest_WithQrCode()
    {
        // Arrange
        var bankIdVerifyApiClientMock = new Mock<IBankIdVerifyApiClient>(MockBehavior.Strict);
        bankIdVerifyApiClientMock.Setup(client => client.VerifyAsync(It.IsAny<VerifyRequest>()))
            .ReturnsAsync(It.IsAny<VerifyResponse>());

        // Act
        await BankIdVerifyApiClientExtensions.VerifyAsync(bankIdVerifyApiClientMock.Object, "QR");

        // Assert
        var request = bankIdVerifyApiClientMock.GetFirstArgumentOfFirstInvocation<IBankIdVerifyApiClient, VerifyRequest>();
        Assert.Equal("QR", request.QrCode);
    }
}
