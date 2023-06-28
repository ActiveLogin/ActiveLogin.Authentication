using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using ActiveLogin.Authentication.BankId.Api.Models;
using ActiveLogin.Authentication.BankId.Api.Test.TestHelpers;

using Moq;
using Moq.Protected;

using Xunit;

namespace ActiveLogin.Authentication.BankId.Api.Test;

public class BankIdVerifyApiClient_Tests
{
    private readonly Mock<HttpMessageHandler> _messageHandlerMock;
    private readonly BankIdVerifyApiClient _bankIdVerifyApiClient;

    public BankIdVerifyApiClient_Tests()
    {
        _messageHandlerMock = GetHttpClientMessageHandlerMock(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent("{ }", Encoding.Default, "application/json"),
        });

        var httpClient = new HttpClient(_messageHandlerMock.Object)
        {
            BaseAddress = new Uri("https://bankid/")
        };
        _bankIdVerifyApiClient = new BankIdVerifyApiClient(httpClient);
    }

    [Fact]
    public async Task ErrorResponse__ShouldThrowException_WithErrorCode_AndDetails()
    {
        // Arrange
        var messageHandlerMock = GetHttpClientMessageHandlerMock(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.BadRequest,
            Content = new StringContent("{ \"errorCode\": \"verificationFailed\", \"details\": \"d\" }", Encoding.Default, "application/json"),
        });

        var httpClient = new HttpClient(messageHandlerMock.Object)
        {
            BaseAddress = new Uri("https://bankid/")
        };
        var bankIdVerifyApiClient = new BankIdVerifyApiClient(httpClient);

        // Act
        var exception = await Assert.ThrowsAsync<BankIdApiException>(() => bankIdVerifyApiClient.VerifyAsync(new VerifyRequest("QR")));

        // Assert
        Assert.Equal(ErrorCode.VerificationFailed, exception.ErrorCode);
        Assert.Equal("d", exception.ErrorDetails);
    }

    [Fact]
    public async Task VerifyAsync_WithVerifyRequest__ShouldPostToBankIdVerify_WithJsonPayload()
    {
        // Arrange

        // Act
        await _bankIdVerifyApiClient.VerifyAsync(new VerifyRequest("QR"));

        // Assert
        Assert.Single(_messageHandlerMock.Invocations);
        var request = _messageHandlerMock.GetFirstArgumentOfFirstInvocation<HttpMessageHandler, HttpRequestMessage>();
        Assert.NotNull(request);
        var contentString = await request.Content.ReadAsStringAsync();

        Assert.Equal(HttpMethod.Post, request.Method);
        Assert.Equal(new Uri("https://bankid/verify"), request.RequestUri);
        Assert.Equal(new MediaTypeHeaderValue("application/json"), request.Content.Headers.ContentType);
        JsonTests.AssertProperty(contentString, "qrCode", "QR");
    }

    private static Mock<HttpMessageHandler> GetHttpClientMessageHandlerMock(HttpResponseMessage responseMessage)
    {
        var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(responseMessage)
            .Verifiable();

        return handlerMock;
    }
}
