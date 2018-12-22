using System;
using System.Collections.Generic;
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
        public async void AuthAsync_WithAuthRequest__ShouldPostToBankIdAuth_WithJsonPayload()
        {
            // Arrange
            var messageHandlerMock = GetHttpClientMessageHandlerMock(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{ \"orderRef\": \"x\", \"autoStartToken\": \"y\" }", Encoding.Default, "application/json"),
            });
            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = new Uri("https://bankid/")
            };
            var bankIdClient = new BankIdApiClient(httpClient);

            // Act
            await bankIdClient.AuthAsync(new AuthRequest("1.1.1.1"));

            // Assert
            Assert.Single(messageHandlerMock.Invocations);

            var request = messageHandlerMock.Invocations[0].Arguments[0] as HttpRequestMessage;
            Assert.NotNull(request);

            Assert.Equal(HttpMethod.Post, request.Method);
            Assert.Equal(new Uri("https://bankid/auth"), request.RequestUri);
            Assert.Equal(new MediaTypeHeaderValue("application/json"), request.Content.Headers.ContentType);
        }

        [Fact]
        public async void AuthAsync_WithEndUserIp__ShouldPostJsonPayload_WithEndUserIp_AndNoPersonalNumber_AndRequirementAsEmptyObject()
        {
            // Arrange
            var messageHandlerMock = GetHttpClientMessageHandlerMock(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{ \"orderRef\": \"x\", \"autoStartToken\": \"y\" }", Encoding.Default, "application/json"),
            });
            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = new Uri("https://bankid/")
            };
            var bankIdClient = new BankIdApiClient(httpClient);

            // Act
            await bankIdClient.AuthAsync(new AuthRequest("1.1.1.1"));

            // Assert
            var request = messageHandlerMock.Invocations[0].Arguments[0] as HttpRequestMessage;
            var contentString = await request.Content.ReadAsStringAsync();

            Assert.Equal("{\"endUserIp\":\"1.1.1.1\",\"requirement\":{}}", contentString);
        }

        [Fact]
        public async void AuthAsync_WithEndUserIp_AndPin__ShouldPostJsonPayload_WithEndUserIp_AndPersonalNumber_AndNoRequirements()
        {
            // Arrange
            var messageHandlerMock = GetHttpClientMessageHandlerMock(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{ \"orderRef\": \"x\", \"autoStartToken\": \"y\" }", Encoding.Default, "application/json"),
            });
            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = new Uri("https://bankid/")
            };
            var bankIdClient = new BankIdApiClient(httpClient);

            // Act
            await bankIdClient.AuthAsync(new AuthRequest("1.1.1.1", "199908072391"));

            // Assert
            var request = messageHandlerMock.Invocations[0].Arguments[0] as HttpRequestMessage;
            var contentString = await request.Content.ReadAsStringAsync();

            Assert.Equal("{\"endUserIp\":\"1.1.1.1\",\"personalNumber\":\"199908072391\",\"requirement\":{}}", contentString);
        }

        [Fact]
        public async void AuthAsync_WithRequirements__ShouldPostJsonPayload_WithReqirements()
        {
            // Arrange
            var messageHandlerMock = GetHttpClientMessageHandlerMock(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{ \"orderRef\": \"x\", \"autoStartToken\": \"y\" }", Encoding.Default, "application/json"),
            });
            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = new Uri("https://bankid/")
            };
            var bankIdClient = new BankIdApiClient(httpClient);

            // Act
            await bankIdClient.AuthAsync(new AuthRequest("1.1.1.1", "199908072391", new Requirement()
            {
                CertificatePolicies = new List<string> { "req1", "req2" },
                AllowFingerprint = true,
                AutoStartTokenRequired = true
            }));

            // Assert
            var request = messageHandlerMock.Invocations[0].Arguments[0] as HttpRequestMessage;
            var contentString = await request.Content.ReadAsStringAsync();

            Assert.Equal("{\"endUserIp\":\"1.1.1.1\",\"personalNumber\":\"199908072391\",\"requirement\":{\"allowFingerprint\":true,\"autoStartTokenRequired\":true,\"certificatePolicies\":[\"req1\",\"req2\"]}}", contentString);
        }
        
        [Fact]
        public async void AuthAsync_WithAuthRequest__ShouldParseAndReturnOrderRef_AndAutoStatToken()
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
            var result = await bankIdClient.AuthAsync(new AuthRequest("1.1.1.1"));

            // Assert
            Assert.NotNull(result);
            Assert.Equal("abc123", result.OrderRef);
            Assert.Equal("def456", result.AutoStartToken);
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
