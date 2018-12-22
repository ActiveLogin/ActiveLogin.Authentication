using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ActiveLogin.Authentication.BankId.Api.Models;
using Moq;
using Moq.Protected;
using Xunit;

namespace ActiveLogin.Authentication.BankId.Api.Test
{
    public class BankIdApiClient_Tests
    {
        [Fact]
        public async void AuthAsync__WithEndUserIpAndPin__ShouldPostToBankIdAuthWithJsonPayload()
        {
            // Arrange
            var messageHandlerMock = GetHttpClientMessageHandlerMock(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{ \"orderRef\": \"abc123\", \"autoStartToken\": \"def456\" }", Encoding.Default, "application/json"),
            });
            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = new Uri("https://bankid/")
            };
            var bankIdClient = new BankIdApiClient(httpClient);

            // Act
            await bankIdClient.AuthAsync(new AuthRequest("1.1.1.1", "199908072391"));

            // Assert
            Assert.Single(messageHandlerMock.Invocations);

            var request = messageHandlerMock.Invocations[0].Arguments[0] as HttpRequestMessage;
            Assert.NotNull(request);
            var contentString = await request.Content.ReadAsStringAsync();

            Assert.Equal(HttpMethod.Post, request.Method);
            Assert.Equal(new Uri("https://bankid/auth"), request.RequestUri);
            Assert.Equal("{\"endUserIp\":\"1.1.1.1\",\"personalNumber\":\"199908072391\",\"requirement\":{}}", contentString);
            Assert.Equal(new MediaTypeHeaderValue("application/json"), request.Content.Headers.ContentType);
        }

        [Fact]
        public async void AuthAsync__WithEndUserIpAndPin__ShouldReturnAuthResponse()
        {
            // Arrange
            var messageHandlerMock = GetHttpClientMessageHandlerMock(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{ \"orderRef\": \"abc123\", \"autoStartToken\": \"def456\" }", Encoding.Default, "application/json"),
            });
            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = new Uri("https://bankid/")
            };
            var bankIdClient = new BankIdApiClient(httpClient);

            // Act
            var result = await bankIdClient.AuthAsync(new AuthRequest("1.1.1.1", "199908072391"));

            // Assert
            Assert.NotNull(result);
            Assert.Equal("abc123", result.OrderRef);
            Assert.Equal("def456", result.AutoStartToken);
        }

        private Mock<HttpMessageHandler> GetHttpClientMessageHandlerMock(HttpResponseMessage responseMessage)
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
