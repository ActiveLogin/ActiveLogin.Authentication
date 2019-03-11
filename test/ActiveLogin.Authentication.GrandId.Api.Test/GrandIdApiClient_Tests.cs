using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ActiveLogin.Authentication.GrandId.Api.Models;
using ActiveLogin.Authentication.GrandId.Api.Test.TestHelpers;
using Moq;
using Moq.Protected;
using Xunit;

namespace ActiveLogin.Authentication.GrandId.Api.Test
{
    public class GrandIdApiClient_Tests
    {
        public GrandIdApiClient_Tests()
        {
            _messageHandlerMock = GetHttpClientMessageHandlerMock(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{ }", Encoding.Default, "application/json")
            });

            var httpClient = new HttpClient(_messageHandlerMock.Object)
            {
                BaseAddress = new Uri("https://grandid/")
            };
            _grandIdApiClient = new GrandIdApiClient(httpClient, new GrandIdApiClientConfiguration("x", "bsk"));
        }

        private readonly Mock<HttpMessageHandler> _messageHandlerMock;
        private readonly GrandIdApiClient _grandIdApiClient;


        private static HttpClient GetHttpClientMockWithOkResponse(string jsonResponse)
        {
            Mock<HttpMessageHandler> messageHandlerMock = GetHttpClientMessageHandlerMock(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonResponse, Encoding.Default, "application/json")
            });
            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = new Uri("https://grandid/")
            };
            return httpClient;
        }

        private static HttpClient GetHttpClientMockWithBadRequestResponse(string jsonResponse)
        {
            Mock<HttpMessageHandler> messageHandlerMock = GetHttpClientMessageHandlerMock(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest,
                Content = new StringContent(jsonResponse, Encoding.Default, "application/json")
            });
            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = new Uri("https://grandid/")
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
        public async void BankIdFederatedLoginAsync__ShouldPostToGrandIdFederatedLogin_WithFormPayload()
        {
            // Arrange
            Mock<HttpMessageHandler> messageHandlerMock = GetHttpClientMessageHandlerMock(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{ }", Encoding.Default, "application/json")
            });

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = new Uri("https://grandid/")
            };
            var grandIdApiClient = new GrandIdApiClient(httpClient, new GrandIdApiClientConfiguration("ak", "bsk"));

            // Act
            await grandIdApiClient.BankIdFederatedLoginAsync(new BankIdFederatedLoginRequest("y"));

            // Assert
            Assert.Single(messageHandlerMock.Invocations);
            HttpRequestMessage request = messageHandlerMock
                .GetFirstArgumentOfFirstInvocation<HttpMessageHandler, HttpRequestMessage>();
            Assert.NotNull(request);

            Assert.Equal(HttpMethod.Post, request.Method);
            Assert.StartsWith("https://grandid/FederatedLogin", request.RequestUri.ToString());
            Assert.Equal(new MediaTypeHeaderValue("application/x-www-form-urlencoded"),
                request.Content.Headers.ContentType);
        }

        [Fact]
        public async void BankIdFederatedLoginAsync_WithAllValues__ShouldPostToGrandIdFederatedLogin_WithAllValues()
        {
            // Arrange
            var bankIdFederatedLoginRequest = new BankIdFederatedLoginRequest(
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

            // Act
            await _grandIdApiClient.BankIdFederatedLoginAsync(bankIdFederatedLoginRequest);

            // Assert
            HttpRequestMessage request = _messageHandlerMock
                .GetFirstArgumentOfFirstInvocation<HttpMessageHandler, HttpRequestMessage>();
            string contentString = await request.Content.ReadAsStringAsync();

            Assert.Contains("callbackUrl=aHR0cHM6Ly9jYi8%3D", contentString);
            Assert.Contains("deviceChoice=true", contentString);
            Assert.Contains("thisDevice=true", contentString);
            Assert.Contains("askForSSN=true", contentString);
            Assert.Contains("personalNumber=20180101239", contentString);
            Assert.Contains("mobileBankId=true", contentString);
            Assert.Contains("customerURL=aHR0cHM6Ly9jdS8%3D", contentString);
            Assert.Contains("gui=true", contentString);
            Assert.Contains("userVisibleData=dXZk", contentString);
            Assert.Contains("userNonVisibleData=dW52ZA%3D%3D", contentString);
        }

        [Fact]
        public async void BankIdFederatedLoginAsync_WithApiKey__ShouldPostToGrandIdFederatedLogin_WithApiKey()
        {
            // Arrange
            Mock<HttpMessageHandler> messageHandlerMock = GetHttpClientMessageHandlerMock(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{ }", Encoding.Default, "application/json")
            });

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = new Uri("https://grandid/")
            };
            var grandIdApiClient = new GrandIdApiClient(httpClient, new GrandIdApiClientConfiguration("ak", "bsk"));

            // Act
            await grandIdApiClient.BankIdFederatedLoginAsync(new BankIdFederatedLoginRequest("https://c/"));

            // Assert
            HttpRequestMessage request = messageHandlerMock
                .GetFirstArgumentOfFirstInvocation<HttpMessageHandler, HttpRequestMessage>();
            Assert.Contains("apiKey=ak", request.RequestUri.ToString());
        }

        [Fact]
        public async void
            BankIdFederatedLoginAsync_WithApiKey_AndBankIdServiceKey__ShouldPostToGrandIdFederatedLogin_WithApiKey_AndBankIdServiceKey()
        {
            // Arrange
            Mock<HttpMessageHandler> messageHandlerMock = GetHttpClientMessageHandlerMock(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{ }", Encoding.Default, "application/json")
            });

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = new Uri("https://grandid/")
            };
            var grandIdApiClient = new GrandIdApiClient(httpClient, new GrandIdApiClientConfiguration("ak", "bsk"));

            // Act
            await grandIdApiClient.BankIdFederatedLoginAsync(new BankIdFederatedLoginRequest("https://c/"));

            // Assert
            HttpRequestMessage request = messageHandlerMock
                .GetFirstArgumentOfFirstInvocation<HttpMessageHandler, HttpRequestMessage>();
            Assert.Contains("apiKey=ak", request.RequestUri.ToString());
            Assert.Contains("authenticateServiceKey=bsk", request.RequestUri.ToString());
        }

        [Fact]
        public async void
            BankIdFederatedLoginAsync_WithBankIdFederatedLoginRequest__ShouldParseAndReturnOrderRef_AndAutoStartToken()
        {
            // Arrange
            HttpClient httpClient =
                GetHttpClientMockWithOkResponse("{ \"sessionId\": \"s\", \"redirectUrl\": \"https://r/\" }");
            var grandIdApiClient = new GrandIdApiClient(httpClient, new GrandIdApiClientConfiguration("x", "bsk"));

            // Act
            BankIdFederatedLoginResponse result =
                await grandIdApiClient.BankIdFederatedLoginAsync(new BankIdFederatedLoginRequest("https://c/"));

            // Assert
            Assert.NotNull(result);
            Assert.Equal("s", result.SessionId);
            Assert.Equal("https://r/", result.RedirectUrl);
        }

        [Fact]
        public async void
            BankIdFederatedLoginAsync_WithCallbackUrl__ShouldPostToGrandIdFederatedLogin_WithCallbackUrl_ButNoPin()
        {
            // Arrange

            // Act
            await _grandIdApiClient.BankIdFederatedLoginAsync(new BankIdFederatedLoginRequest("https://cb/"));

            // Assert
            HttpRequestMessage request = _messageHandlerMock
                .GetFirstArgumentOfFirstInvocation<HttpMessageHandler, HttpRequestMessage>();
            string contentString = await request.Content.ReadAsStringAsync();

            Assert.Equal("callbackUrl=aHR0cHM6Ly9jYi8%3D", contentString);
        }

        [Fact]
        public async void BankIdFederatedLoginAsync_WithNoParams__ShouldPostToGrandIdFederatedLogin_WithNoPayload()
        {
            // Arrange

            // Act
            await _grandIdApiClient.BankIdFederatedLoginAsync(new BankIdFederatedLoginRequest());

            // Assert
            HttpRequestMessage request = _messageHandlerMock
                .GetFirstArgumentOfFirstInvocation<HttpMessageHandler, HttpRequestMessage>();
            string contentString = await request.Content.ReadAsStringAsync();

            Assert.Empty(contentString);
        }

        [Fact]
        public async void
            BankIdFederatedLoginAsync_WithServiceKey_AndCallbackUrl_AndPin__ShouldPostToGrandIdFederatedLogin_WithPin()
        {
            // Arrange

            // Act
            await _grandIdApiClient.BankIdFederatedLoginAsync(
                new BankIdFederatedLoginRequest("https://c/", personalIdentityNumber: "201801012392"));

            // Assert
            HttpRequestMessage request = _messageHandlerMock
                .GetFirstArgumentOfFirstInvocation<HttpMessageHandler, HttpRequestMessage>();
            string contentString = await request.Content.ReadAsStringAsync();

            Assert.Contains("personalNumber=20180101239", contentString);
        }

        [Fact]
        public async void BankIdGetSessionAsync_WithBankIdGetSessionRequest__ShouldParseAndReturnGetSessionResponse()
        {
            // Arrange
            HttpClient httpClient = GetHttpClientMockWithOkResponse(
                "{ \"sessionId\": \"s\", \"username\": \"u\", \"userAttributes\": { \"givenName\": \"ugn\", \"surname\": \"usn\", \"name\": \"un\", \"personalNumber\": \"upn\", \"ipAddress\": \"uip\" } }");
            var grandIdApiClient = new GrandIdApiClient(httpClient, new GrandIdApiClientConfiguration("x", "bsk"));

            // Act
            BankIdGetSessionResponse result =
                await grandIdApiClient.BankIdGetSessionAsync(new BankIdGetSessionRequest("y"));

            // Assert
            Assert.NotNull(result);
            Assert.Equal("s", result.SessionId);
            Assert.Equal("u", result.Username);

            Assert.NotNull(result.UserAttributes);
            Assert.Equal("ugn", result.UserAttributes.GivenName);
            Assert.Equal("usn", result.UserAttributes.Surname);
            Assert.Equal("un", result.UserAttributes.Name);
            Assert.Equal("upn", result.UserAttributes.PersonalIdentityNumber);
            Assert.Equal("uip", result.UserAttributes.IpAddress);
        }

        [Fact]
        public async void BankIdGetSessionAsync_WithBankIdGetSessionRequest__ShouldParseAndReturnNotBefore_AndNotAfter()
        {
            // Arrange
            HttpClient httpClient = GetHttpClientMockWithOkResponse(
                "{ \"userAttributes\": { \"notBefore\": \"2018-12-25T00:00:00.000+02:00\", \"notAfter\": \"2018-12-26T00:00:00.000+02:00\" } }");
            var grandIdApiClient = new GrandIdApiClient(httpClient, new GrandIdApiClientConfiguration("x", "bsk"));

            // Act
            BankIdGetSessionResponse result =
                await grandIdApiClient.BankIdGetSessionAsync(new BankIdGetSessionRequest("y"));

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.UserAttributes);
            Assert.Equal(new DateTime(2018, 12, 24, 22, 00, 00, DateTimeKind.Local),
                result.UserAttributes.GetNotBeforeDateTime());
            Assert.Equal(new DateTime(2018, 12, 25, 22, 00, 00, DateTimeKind.Utc),
                result.UserAttributes.GetNotAfterDateTime());
        }

        [Fact]
        public async void BankIdGetSessionAsync_WithBankIdGetSessionRequest__ShouldParseAndReturnSignatureXml()
        {
            // Arrange
            HttpClient httpClient = GetHttpClientMockWithOkResponse(
                "{ \"userAttributes\": { \"signature\": \"PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0iVVRGLTgiIHN0YW5kYWxvbmU9Im5vIj8+PHNhbXBsZT48dmFsdWU+SGk8L3ZhbHVlPjxjb250ZW50PkJ5ZTwvY29uZW50Pjwvc2FtcGxlPg==\" } }");
            var grandIdApiClient = new GrandIdApiClient(httpClient, new GrandIdApiClientConfiguration("x", "bsk"));

            // Act
            BankIdGetSessionResponse result =
                await grandIdApiClient.BankIdGetSessionAsync(new BankIdGetSessionRequest("y"));

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.UserAttributes);
            Assert.Equal(
                "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"no\"?><sample><value>Hi</value><content>Bye</conent></sample>",
                result.UserAttributes.GetSignatureXml());
        }


        [Fact]
        public async void
            BankIdGetSessionAsync_WithServiceKey_AndSessionId__ShouldGetToGrandIdGetSession_WithServiceKey_AndSessionId()
        {
            // Arrange

            // Act
            await _grandIdApiClient.BankIdGetSessionAsync(new BankIdGetSessionRequest("s"));

            // Assert
            HttpRequestMessage request = _messageHandlerMock
                .GetFirstArgumentOfFirstInvocation<HttpMessageHandler, HttpRequestMessage>();
            Assert.Contains("authenticateServiceKey=bsk", request.RequestUri.ToString());
            Assert.Contains("sessionid=s", request.RequestUri.ToString());
        }

        [Fact]
        public async void ErrorResponse_WithBadRequestResult__ShouldThrowException_WithErrorCode_AndMessage()
        {
            // Arrange
            HttpClient httpClient =
                GetHttpClientMockWithBadRequestResponse(
                    "{ \"errorObject\": { \"code\": \"FieldsNotValid\", \"message\": \"m\" } }");
            var bankIdApiClient = new GrandIdApiClient(httpClient, new GrandIdApiClientConfiguration("x", "bsk"));

            // Act
            GrandIdApiException exception = await Assert.ThrowsAsync<GrandIdApiException>(() =>
                bankIdApiClient.BankIdFederatedLoginAsync(new BankIdFederatedLoginRequest("http://c/")));

            // Assert
            Assert.Equal(ErrorCode.FieldsNotValid, exception.ErrorCode);
            Assert.Equal("m", exception.ErrorDetails);
        }

        [Fact]
        public async void ErrorResponse_WithOkResult__ShouldThrowException_WithErrorCode_AndMessage()
        {
            // Arrange
            HttpClient httpClient =
                GetHttpClientMockWithOkResponse(
                    "{ \"errorObject\": { \"code\": \"FieldsNotValid\", \"message\": \"m\" } }");
            var grandIdApiClient = new GrandIdApiClient(httpClient, new GrandIdApiClientConfiguration("x", "bsk"));

            // Act
            GrandIdApiException exception = await Assert.ThrowsAsync<GrandIdApiException>(() =>
                grandIdApiClient.BankIdFederatedLoginAsync(new BankIdFederatedLoginRequest("http://c/")));

            // Assert
            Assert.Equal(ErrorCode.FieldsNotValid, exception.ErrorCode);
            Assert.Equal("m", exception.ErrorDetails);
        }

        [Fact]
        public async void LogoutAsync_WithGrandIdLogoutRequest__ShouldParseAndReturnSessionDeleted()
        {
            // Arrange
            HttpClient httpClient = GetHttpClientMockWithOkResponse("{ \"sessiondeleted\": \"1\" }");
            var grandIdApiClient = new GrandIdApiClient(httpClient, new GrandIdApiClientConfiguration("x", "bsk"));

            // Act
            LogoutResponse result = await grandIdApiClient.LogoutAsync(new LogoutRequest("s"));

            // Assert
            Assert.NotNull(result);
            Assert.True(result.SessionDeleted);
        }

        [Fact]
        public async void LogoutAsync_WithSessionId__ShouldGetToGrandIdFederatedLogin_WithSessionId()
        {
            // Arrange

            // Act
            await _grandIdApiClient.LogoutAsync(new LogoutRequest("s"));

            // Assert
            HttpRequestMessage request = _messageHandlerMock
                .GetFirstArgumentOfFirstInvocation<HttpMessageHandler, HttpRequestMessage>();
            Assert.Contains("sessionid=s", request.RequestUri.ToString());
        }
    }
}
