using System;
using System.Collections.Generic;
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

namespace ActiveLogin.Authentication.BankId.Api.Test
{
    public class BankIdApiClient_Tests
    {
        public BankIdApiClient_Tests()
        {
            _messageHandlerMock = GetHttpClientMessageHandlerMock(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{ }", Encoding.Default, "application/json")
            });

            var httpClient = new HttpClient(_messageHandlerMock.Object)
            {
                BaseAddress = new Uri("https://bankid/")
            };
            _bankIdApiClient = new BankIdApiClient(httpClient);
        }

        private readonly Mock<HttpMessageHandler> _messageHandlerMock;
        private readonly BankIdApiClient _bankIdApiClient;

        private static HttpClient GetHttpClientMockWithOkResponse(string jsonResponse)
        {
            Mock<HttpMessageHandler> messageHandlerMock = GetHttpClientMessageHandlerMock(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonResponse, Encoding.Default, "application/json")
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

        [Fact]
        public async Task AuthAsync_WithAuthRequest__ShouldParseAndReturnOrderRef_AndAutoStartToken()
        {
            // Arrange
            HttpClient httpClient =
                GetHttpClientMockWithOkResponse("{ \"orderRef\": \"abc123\", \"autoStartToken\": \"def456\" }");
            var bankIdClient = new BankIdApiClient(httpClient);

            // Act
            AuthResponse result = await bankIdClient.AuthAsync(new AuthRequest("1.1.1.1"));

            // Assert
            Assert.NotNull(result);
            Assert.Equal("abc123", result.OrderRef);
            Assert.Equal("def456", result.AutoStartToken);
        }

        [Fact]
        public async Task AuthAsync_WithAuthRequest__ShouldPostToBankIdAuth_WithJsonPayload()
        {
            // Arrange

            // Act
            await _bankIdApiClient.AuthAsync(new AuthRequest("1.1.1.1"));

            // Assert
            Assert.Single(_messageHandlerMock.Invocations);
            HttpRequestMessage request = _messageHandlerMock
                .GetFirstArgumentOfFirstInvocation<HttpMessageHandler, HttpRequestMessage>();
            Assert.NotNull(request);

            Assert.Equal(HttpMethod.Post, request.Method);
            Assert.Equal(new Uri("https://bankid/auth"), request.RequestUri);
            Assert.Equal(new MediaTypeHeaderValue("application/json"), request.Content.Headers.ContentType);
        }

        [Fact]
        public async Task
            AuthAsync_WithEndUserIp__ShouldPostJsonPayload_WithEndUserIp_AndNoPersonalNumber_AndRequirementAsEmptyObject()
        {
            // Arrange

            // Act
            await _bankIdApiClient.AuthAsync(new AuthRequest("1.1.1.1"));

            // Assert
            HttpRequestMessage request = _messageHandlerMock
                .GetFirstArgumentOfFirstInvocation<HttpMessageHandler, HttpRequestMessage>();
            string contentString = await request.Content.ReadAsStringAsync();

            Assert.Equal("{\"endUserIp\":\"1.1.1.1\",\"requirement\":{}}", contentString);
        }

        [Fact]
        public async Task
            AuthAsync_WithEndUserIp_AndPin__ShouldPostJsonPayload_WithEndUserIp_AndPersonalNumber_AndNoRequirements()
        {
            // Arrange

            // Act
            await _bankIdApiClient.AuthAsync(new AuthRequest("1.1.1.1", "201801012392"));

            // Assert
            HttpRequestMessage request = _messageHandlerMock
                .GetFirstArgumentOfFirstInvocation<HttpMessageHandler, HttpRequestMessage>();
            string contentString = await request.Content.ReadAsStringAsync();

            Assert.Equal("{\"endUserIp\":\"1.1.1.1\",\"personalNumber\":\"201801012392\",\"requirement\":{}}",
                contentString);
        }

        [Fact]
        public async Task AuthAsync_WithRequirements__ShouldPostJsonPayload_WithReqirements()
        {
            // Arrange

            // Act
            await _bankIdApiClient.AuthAsync(new AuthRequest("1.1.1.1", "201801012392",
                new Requirement(new List<string> {"req1", "req2"}, true, true)));

            // Assert
            HttpRequestMessage request = _messageHandlerMock
                .GetFirstArgumentOfFirstInvocation<HttpMessageHandler, HttpRequestMessage>();
            string contentString = await request.Content.ReadAsStringAsync();

            Assert.Equal(
                "{\"endUserIp\":\"1.1.1.1\",\"personalNumber\":\"201801012392\",\"requirement\":{\"allowFingerprint\":true,\"autoStartTokenRequired\":true,\"certificatePolicies\":[\"req1\",\"req2\"]}}",
                contentString);
        }

        [Fact]
        public async Task CancelAsync_WithCancelRequest__ShouldPostToBankIdCancel_WithJsonPayload()
        {
            // Arrange

            // Act
            await _bankIdApiClient.CancelAsync(new CancelRequest("x"));

            // Assert
            Assert.Single(_messageHandlerMock.Invocations);
            HttpRequestMessage request = _messageHandlerMock
                .GetFirstArgumentOfFirstInvocation<HttpMessageHandler, HttpRequestMessage>();
            Assert.NotNull(request);

            Assert.Equal(HttpMethod.Post, request.Method);
            Assert.Equal(new Uri("https://bankid/cancel"), request.RequestUri);
            Assert.Equal(new MediaTypeHeaderValue("application/json"), request.Content.Headers.ContentType);
        }

        [Fact]
        public async Task CancelAsync_WithOrderRef__ShouldPostJsonPayload_WithOrderRef()
        {
            // Arrange

            // Act
            await _bankIdApiClient.CancelAsync(new CancelRequest("abc123"));

            // Assert
            HttpRequestMessage request = _messageHandlerMock
                .GetFirstArgumentOfFirstInvocation<HttpMessageHandler, HttpRequestMessage>();
            string contentString = await request.Content.ReadAsStringAsync();

            Assert.Equal("{\"orderRef\":\"abc123\"}", contentString);
        }

        [Fact]
        public async Task
            CollectAsync_WithCollectRequest__ShouldParseAndReturnCompletionDataCertDates_ConvetedFromUnixEpochMillisecondsToDateTime()
        {
            // Arrange
            HttpClient httpClient = GetHttpClientMockWithOkResponse(
                "{ \"completionData\": { \"cert\": { \"notBefore\": \"671630400000\", \"notAfter\": \"671659200000\" } } }");
            var bankIdClient = new BankIdApiClient(httpClient);

            // Act
            CollectResponse result = await bankIdClient.CollectAsync(new CollectRequest("x"));

            // Assert
            Assert.NotNull(result);
            Assert.Equal("671630400000", result.CompletionData.Cert.NotBefore);
            Assert.Equal(new DateTime(1991, 4, 14, 12, 00, 00), result.CompletionData.Cert.GetNotBeforeDateTime());
            Assert.Equal("671659200000", result.CompletionData.Cert.NotAfter);
            Assert.Equal(new DateTime(1991, 4, 14, 20, 00, 00), result.CompletionData.Cert.GetNotAfterDateTime());
        }

        [Fact]
        public async Task CollectAsync_WithCollectRequest__ShouldParseAndReturnCompletionDataDevice()
        {
            // Arrange
            HttpClient httpClient =
                GetHttpClientMockWithOkResponse(
                    "{ \"completionData\": { \"device\": { \"ipAddress\": \"1.1.1.1\" } } }");
            var bankIdClient = new BankIdApiClient(httpClient);

            // Act
            CollectResponse result = await bankIdClient.CollectAsync(new CollectRequest("x"));

            // Assert
            Assert.NotNull(result);
            Assert.Equal("1.1.1.1", result.CompletionData.Device.IpAddress);
        }

        [Fact]
        public async Task CollectAsync_WithCollectRequest__ShouldParseAndReturnCompletionDataSignature_AndOcspResponse()
        {
            // Arrange
            HttpClient httpClient =
                GetHttpClientMockWithOkResponse(
                    "{ \"completionData\": {  \"signature\": \"s\", \"ocspResponse\": \"or\" } }");
            var bankIdClient = new BankIdApiClient(httpClient);

            // Act
            CollectResponse result = await bankIdClient.CollectAsync(new CollectRequest("x"));

            // Assert
            Assert.NotNull(result);
            Assert.Equal("s", result.CompletionData.Signature);
            Assert.Equal("or", result.CompletionData.OcspResponse);
        }

        [Fact]
        public async Task CollectAsync_WithCollectRequest__ShouldParseAndReturnCompletionDataSignatureXml()
        {
            // Arrange
            HttpClient httpClient = GetHttpClientMockWithOkResponse(
                "{ \"completionData\": { \"signature\": \"PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0iVVRGLTgiIHN0YW5kYWxvbmU9Im5vIj8+PHNhbXBsZT48dmFsdWU+SGk8L3ZhbHVlPjxjb250ZW50PkJ5ZTwvY29uZW50Pjwvc2FtcGxlPg==\" } }");
            var bankIdClient = new BankIdApiClient(httpClient);

            // Act
            CollectResponse result = await bankIdClient.CollectAsync(new CollectRequest("x"));

            // Assert
            Assert.NotNull(result);
            Assert.Equal(
                "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"no\"?><sample><value>Hi</value><content>Bye</conent></sample>",
                result.CompletionData.GetSignatureXml());
        }

        [Fact]
        public async Task CollectAsync_WithCollectRequest__ShouldParseAndReturnCompletionDataUser()
        {
            // Arrange
            HttpClient httpClient = GetHttpClientMockWithOkResponse(
                "{ \"completionData\": { \"user\": { \"personalNumber\": \"201801012392\", \"name\": \"n\", \"givenName\": \"gn\", \"surname\": \"sn\" } } }");
            var bankIdClient = new BankIdApiClient(httpClient);

            // Act
            CollectResponse result = await bankIdClient.CollectAsync(new CollectRequest("x"));

            // Assert
            Assert.NotNull(result);
            Assert.Equal("201801012392", result.CompletionData.User.PersonalIdentityNumber);
            Assert.Equal("n", result.CompletionData.User.Name);
            Assert.Equal("gn", result.CompletionData.User.GivenName);
            Assert.Equal("sn", result.CompletionData.User.Surname);
        }

        [Fact]
        public async Task CollectAsync_WithCollectRequest__ShouldParseAndReturnHintCode()
        {
            // Arrange
            HttpClient httpClient = GetHttpClientMockWithOkResponse("{ \"hintCode\":\"OutstandingTransaction\" }");
            var bankIdClient = new BankIdApiClient(httpClient);

            // Act
            CollectResponse result = await bankIdClient.CollectAsync(new CollectRequest("x"));

            // Assert
            Assert.NotNull(result);
            Assert.Equal("OutstandingTransaction", result.HintCode);
            Assert.Equal(CollectHintCode.OutstandingTransaction, result.GetCollectHintCode());
        }

        [Fact]
        public async Task CollectAsync_WithCollectRequest__ShouldParseAndReturnOrderRef()
        {
            // Arrange
            HttpClient httpClient = GetHttpClientMockWithOkResponse("{ \"orderRef\":\"abc123\" }");
            var bankIdClient = new BankIdApiClient(httpClient);

            // Act
            CollectResponse result = await bankIdClient.CollectAsync(new CollectRequest("x"));

            // Assert
            Assert.NotNull(result);
            Assert.Equal("abc123", result.OrderRef);
        }

        [Fact]
        public async Task CollectAsync_WithCollectRequest__ShouldParseAndReturnStatus()
        {
            // Arrange
            HttpClient httpClient = GetHttpClientMockWithOkResponse("{ \"status\":\"Pending\" }");
            var bankIdClient = new BankIdApiClient(httpClient);

            // Act
            CollectResponse result = await bankIdClient.CollectAsync(new CollectRequest("x"));

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Pending", result.Status);
            Assert.Equal(CollectStatus.Pending, result.GetCollectStatus());
        }

        [Fact]
        public async Task CollectAsync_WithCollectRequest__ShouldPostToBankIdCollect_WithJsonPayload()
        {
            // Arrange

            // Act
            await _bankIdApiClient.CollectAsync(new CollectRequest("abc123"));

            // Assert
            Assert.Single(_messageHandlerMock.Invocations);
            HttpRequestMessage request = _messageHandlerMock
                .GetFirstArgumentOfFirstInvocation<HttpMessageHandler, HttpRequestMessage>();
            Assert.NotNull(request);

            Assert.Equal(HttpMethod.Post, request.Method);
            Assert.Equal(new Uri("https://bankid/collect"), request.RequestUri);
            Assert.Equal(new MediaTypeHeaderValue("application/json"), request.Content.Headers.ContentType);
        }

        [Fact]
        public async Task CollectAsync_WithOrderRef__ShouldPostJsonPayload_WithOrderRef()
        {
            // Arrange

            // Act
            await _bankIdApiClient.CollectAsync(new CollectRequest("abc123"));

            // Assert
            HttpRequestMessage request = _messageHandlerMock
                .GetFirstArgumentOfFirstInvocation<HttpMessageHandler, HttpRequestMessage>();
            string contentString = await request.Content.ReadAsStringAsync();

            Assert.Equal("{\"orderRef\":\"abc123\"}", contentString);
        }

        [Fact]
        public async Task ErrorResponse__ShouldThrowException_WithErrorCode_AndDetails()
        {
            // Arrange
            Mock<HttpMessageHandler> messageHandlerMock = GetHttpClientMessageHandlerMock(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest,
                Content = new StringContent("{ \"errorCode\": \"AlreadyInProgress\", \"details\": \"d\" }",
                    Encoding.Default, "application/json")
            });

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = new Uri("https://bankid/")
            };
            var bankIdApiClient = new BankIdApiClient(httpClient);

            // Act
            BankIdApiException exception =
                await Assert.ThrowsAsync<BankIdApiException>(
                    () => bankIdApiClient.AuthAsync(new AuthRequest("1.1.1.1")));

            // Assert
            Assert.Equal(ErrorCode.AlreadyInProgress, exception.ErrorCode);
            Assert.Equal("d", exception.ErrorDetails);
        }

        [Fact]
        public async Task
            SignAsync_WithEndUserIp__ShouldPostJsonPayload_WithEndUserIp_AndUserVisibleData_AndNoPersonalNumber_AndRequirementAsEmptyObject()
        {
            // Arrange

            // Act
            await _bankIdApiClient.SignAsync(new SignRequest("1.1.1.1", "userVisibleData"));

            // Assert
            HttpRequestMessage request = _messageHandlerMock
                .GetFirstArgumentOfFirstInvocation<HttpMessageHandler, HttpRequestMessage>();
            string contentString = await request.Content.ReadAsStringAsync();

            Assert.Equal("{\"endUserIp\":\"1.1.1.1\",\"requirement\":{},\"userVisibleData\":\"dXNlclZpc2libGVEYXRh\"}",
                contentString);
        }

        [Fact]
        public async Task
            SignAsync_WithEndUserIp_AndPin__ShouldPostJsonPayload_WithEndUserIp_AndPersonalNumber_AndUserVisibleData_AndNoRequirements()
        {
            // Arrange

            // Act
            await _bankIdApiClient.SignAsync(new SignRequest("1.1.1.1", "userVisibleData", null, "201801012392"));

            // Assert
            HttpRequestMessage request = _messageHandlerMock
                .GetFirstArgumentOfFirstInvocation<HttpMessageHandler, HttpRequestMessage>();
            string contentString = await request.Content.ReadAsStringAsync();

            Assert.Equal(
                "{\"endUserIp\":\"1.1.1.1\",\"personalNumber\":\"201801012392\",\"requirement\":{},\"userVisibleData\":\"dXNlclZpc2libGVEYXRh\"}",
                contentString);
        }

        [Fact]
        public async Task SignAsync_WithRequirements__ShouldPostJsonPayload_WithReqirements()
        {
            // Arrange

            // Act
            await _bankIdApiClient.SignAsync(new SignRequest("1.1.1.1", "userVisibleData",
                Encoding.UTF8.GetBytes("userNonVisibleData"), "201801012392",
                new Requirement(new List<string> {"req1", "req2"}, true, true)));

            // Assert
            HttpRequestMessage request = _messageHandlerMock
                .GetFirstArgumentOfFirstInvocation<HttpMessageHandler, HttpRequestMessage>();
            string contentString = await request.Content.ReadAsStringAsync();

            Assert.Equal(
                "{\"endUserIp\":\"1.1.1.1\",\"personalNumber\":\"201801012392\",\"requirement\":{\"allowFingerprint\":true,\"autoStartTokenRequired\":true,\"certificatePolicies\":[\"req1\",\"req2\"]},\"userNonVisibleData\":\"dXNlck5vblZpc2libGVEYXRh\",\"userVisibleData\":\"dXNlclZpc2libGVEYXRh\"}",
                contentString);
        }

        [Fact]
        public async Task SignAsync_WithSignRequest__ShouldParseAndReturnOrderRef_AndAutoStartToken()
        {
            // Arrange
            HttpClient httpClient =
                GetHttpClientMockWithOkResponse("{ \"orderRef\": \"abc123\", \"autoStartToken\": \"def456\" }");
            var bankIdClient = new BankIdApiClient(httpClient);

            // Act
            SignResponse result = await bankIdClient.SignAsync(new SignRequest("1.1.1.1", "userVisibleData"));

            // Assert
            Assert.NotNull(result);
            Assert.Equal("abc123", result.OrderRef);
            Assert.Equal("def456", result.AutoStartToken);
        }

        [Fact]
        public async Task SignAsync_WithSignRequest__ShouldPostToBankIdSign_WithJsonPayload()
        {
            // Arrange

            // Act
            await _bankIdApiClient.SignAsync(new SignRequest("1.1.1.1", "userVisibleData"));

            // Assert
            Assert.Single(_messageHandlerMock.Invocations);
            HttpRequestMessage request = _messageHandlerMock
                .GetFirstArgumentOfFirstInvocation<HttpMessageHandler, HttpRequestMessage>();
            Assert.NotNull(request);

            Assert.Equal(HttpMethod.Post, request.Method);
            Assert.Equal(new Uri("https://bankid/sign"), request.RequestUri);
            Assert.Equal(new MediaTypeHeaderValue("application/json"), request.Content.Headers.ContentType);
        }

        [Fact]
        public async Task SignAsync_WithUserNonVisibleData__ShouldPostJsonPayload_WithUserNonVisibleData()
        {
            // Arrange

            // Act
            await _bankIdApiClient.SignAsync(new SignRequest("1.1.1.1", "userVisibleData", new byte[1]));

            // Assert
            HttpRequestMessage request = _messageHandlerMock
                .GetFirstArgumentOfFirstInvocation<HttpMessageHandler, HttpRequestMessage>();
            string contentString = await request.Content.ReadAsStringAsync();

            Assert.Equal(
                "{\"endUserIp\":\"1.1.1.1\",\"requirement\":{},\"userNonVisibleData\":\"AA==\",\"userVisibleData\":\"dXNlclZpc2libGVEYXRh\"}",
                contentString);
        }
    }
}
