using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ActiveLogin.Authentication.GrandId.Api;
using ActiveLogin.Authentication.GrandId.Api.Models;
using Moq;
using Moq.Protected;
using Xunit;

namespace ActiveLogin.Authentication.BankId.Api.Test
{
    public class GrandIdApiClient_Tests
    {
        private readonly Mock<HttpMessageHandler> _messageHandlerMock;
        private readonly GrandIdApiClient _grandIdApiClient;

        public GrandIdApiClient_Tests()
        {
            _messageHandlerMock = GetHttpClientMessageHandlerMock(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{ }", Encoding.Default, "application/json"),
            });

            var httpClient = new HttpClient(_messageHandlerMock.Object)
            {
                BaseAddress = new Uri("https://grandid/")
            };
            _grandIdApiClient = new GrandIdApiClient(httpClient, new GrandIdApiClientConfiguration("key"));
        }

        [Fact]
        public async void ErrorResponse__ShouldThrowException_WithErrorCode_AndMessage()
        {
            // Arrange
            var messageHandlerMock = GetHttpClientMessageHandlerMock(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest,
                Content = new StringContent("{ \"errorObject\": { \"code\": \"FieldsNotValid\", \"message\": \"m\" } }", Encoding.Default, "application/json"),
            });

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = new Uri("https://grandid/")
            };
            var bankIdApiClient = new GrandIdApiClient(httpClient, new GrandIdApiClientConfiguration("key"));

            // Act
            var exception = await Assert.ThrowsAsync<GrandIdApiException>(() => bankIdApiClient.BankIdFederatedLoginAsync(new BankIdFederatedLoginRequest("askey", "http://cb/")));

            // Assert
            Assert.Equal(ErrorCode.FieldsNotValid, exception.ErrorCode);
            Assert.Equal("m", exception.Details);
        }


        private static HttpClient GetHttpClientMockWithOkResponse(string jsonResponse)
        {
            var messageHandlerMock = GetHttpClientMessageHandlerMock(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonResponse, Encoding.Default, "application/json"),
            });
            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = new Uri("https://bankid/")
            };
            return httpClient;
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
}
