using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ActiveLogin.Authentication.GrandId.Api;
using ActiveLogin.Authentication.GrandId.Api.Models;
using ActiveLogin.Authentication.GrandId.Api.Test.TestHelpers;
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
            _grandIdApiClient = new GrandIdApiClient(httpClient, new GrandIdApiClientConfiguration("x"));
        }

        [Fact]
        public async void ErrorResponse_WithOkResult__ShouldThrowException_WithErrorCode_AndMessage()
        {
            // Arrange
            var httpClient = GetHttpClientMockWithOkResponse("{ \"errorObject\": { \"code\": \"FieldsNotValid\", \"message\": \"m\" } }");
            var grandIdApiClient = new GrandIdApiClient(httpClient, new GrandIdApiClientConfiguration("x"));

            // Act
            var exception = await Assert.ThrowsAsync<GrandIdApiException>(() => grandIdApiClient.BankIdFederatedLoginAsync(new BankIdFederatedLoginRequest("y", "http://c/")));

            // Assert
            Assert.Equal(ErrorCode.FieldsNotValid, exception.ErrorCode);
            Assert.Equal("m", exception.ErrorDetails);
        }

        [Fact]
        public async void ErrorResponse_WithBadRequestResult__ShouldThrowException_WithErrorCode_AndMessage()
        {
            // Arrange
            var httpClient = GetHttpClientMockWithBadRequestResponse("{ \"errorObject\": { \"code\": \"FieldsNotValid\", \"message\": \"m\" } }");
            var bankIdApiClient = new GrandIdApiClient(httpClient, new GrandIdApiClientConfiguration("x"));

            // Act
            var exception = await Assert.ThrowsAsync<GrandIdApiException>(() => bankIdApiClient.BankIdFederatedLoginAsync(new BankIdFederatedLoginRequest("y", "http://c/")));

            // Assert
            Assert.Equal(ErrorCode.FieldsNotValid, exception.ErrorCode);
            Assert.Equal("m", exception.ErrorDetails);
        }

        [Fact]
        public async void BankIdFederatedLoginAsync_WithApiKey__ShouldGetToGrandIdFederatedLogin_WithApiKey()
        {
            // Arrange
            var messageHandlerMock = GetHttpClientMessageHandlerMock(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{ }", Encoding.Default, "application/json"),
            });

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = new Uri("https://grandid/")
            };
            var grandIdApiClient = new GrandIdApiClient(httpClient, new GrandIdApiClientConfiguration("ak"));

            // Act
            await grandIdApiClient.BankIdFederatedLoginAsync(new BankIdFederatedLoginRequest("y", "https://c/"));

            // Assert
            Assert.Single(messageHandlerMock.Invocations);
            var request = messageHandlerMock.GetFirstArgumentOfFirstInvocation<HttpMessageHandler, HttpRequestMessage>();
            Assert.NotNull(request);

            Assert.Equal(HttpMethod.Get, request.Method);
            Assert.StartsWith("https://grandid/FederatedLogin", request.RequestUri.ToString());
            Assert.Contains("apiKey=ak", request.RequestUri.ToString());
        }

        [Fact]
        public async void BankIdFederatedLoginAsync_WithServiceKey_AndCallbackUrl__ShouldGetToGrandIdFederatedLogin_WithServiceKey_AndCallbackUrl_ButNoPin()
        {
            // Arrange

            // Act
            await _grandIdApiClient.BankIdFederatedLoginAsync(new BankIdFederatedLoginRequest("ask", "https://cb/"));

            // Assert
            var request = _messageHandlerMock.GetFirstArgumentOfFirstInvocation<HttpMessageHandler, HttpRequestMessage>();
            Assert.Contains("authenticateServiceKey=ask", request.RequestUri.ToString());
            Assert.Contains("callbackUrl=https%3A%2F%2Fcb%2F", request.RequestUri.ToString());
            Assert.DoesNotContain("pnr", request.RequestUri.ToString());
        }

        [Fact]
        public async void BankIdFederatedLoginAsync_WithServiceKey_AndCallbackUrl_AndPin__ShouldGetToGrandIdFederatedLogin_WithPin()
        {
            // Arrange

            // Act
            await _grandIdApiClient.BankIdFederatedLoginAsync(new BankIdFederatedLoginRequest("y", "https://c/", "199908072391"));

            // Assert
            var request = _messageHandlerMock.GetFirstArgumentOfFirstInvocation<HttpMessageHandler, HttpRequestMessage>();
            Assert.Contains("pnr=199908072391", request.RequestUri.ToString());
        }

        [Fact]
        public async void BankIdFederatedLoginAsync_WithBankIdFederatedLoginRequest__ShouldParseAndReturnOrderRef_AndAutoStartToken()
        {
            // Arrange
            var httpClient = GetHttpClientMockWithOkResponse("{ \"sessionId\": \"s\", \"redirectUrl\": \"https://r/\" }");
            var grandIdApiClient = new GrandIdApiClient(httpClient, new GrandIdApiClientConfiguration("x"));

            // Act
            var result = await grandIdApiClient.BankIdFederatedLoginAsync(new BankIdFederatedLoginRequest("y", "https://c/"));

            // Assert
            Assert.NotNull(result);
            Assert.Equal("s", result.SessionId);
            Assert.Equal("https://r/", result.RedirectUrl);
        }


        [Fact]
        public async void BankIdGetSessionAsync_WithServiceKey_AndSessionId__ShouldGetToGrandIdGetSession_WithServiceKey_AndSessionId()
        {
            // Arrange

            // Act
            await _grandIdApiClient.BankIdGetSessionAsync(new BankIdSessionStateRequest("ask", "s"));

            // Assert
            var request = _messageHandlerMock.GetFirstArgumentOfFirstInvocation<HttpMessageHandler, HttpRequestMessage>();
            Assert.Contains("authenticateServiceKey=ask", request.RequestUri.ToString());
            Assert.Contains("sessionid=s", request.RequestUri.ToString());
        }

        [Fact]
        public async void BankIdGetSessionAsync_WithBankIdGetSessionRequest__ShouldParseAndReturnSessionState()
        {
            // Arrange
            var httpClient = GetHttpClientMockWithOkResponse("{ \"sessionId\": \"s\", \"username\": \"u\", \"userAttributes\": { \"givenName\": \"ugn\", \"surname\": \"usn\", \"name\": \"un\", \"personalNumber\": \"upn\", \"ipAddress\": \"uip\" } }");
            var grandIdApiClient = new GrandIdApiClient(httpClient, new GrandIdApiClientConfiguration("x"));

            // Act
            var result = await grandIdApiClient.BankIdGetSessionAsync(new BankIdSessionStateRequest("x", "y"));

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
        public async void BankIdGetSessionAsync_WithBankIdGetSessionRequest__ShouldParseAndReturnSignatureXml()
        {
            // Arrange
            var httpClient = GetHttpClientMockWithOkResponse("{ \"userAttributes\": { \"signature\": \"PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0iVVRGLTgiIHN0YW5kYWxvbmU9Im5vIj8+PHNhbXBsZT48dmFsdWU+SGk8L3ZhbHVlPjxjb250ZW50PkJ5ZTwvY29uZW50Pjwvc2FtcGxlPg==\" } }");
            var grandIdApiClient = new GrandIdApiClient(httpClient, new GrandIdApiClientConfiguration("x"));

            // Act
            var result = await grandIdApiClient.BankIdGetSessionAsync(new BankIdSessionStateRequest("x", "y"));

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.UserAttributes);
            Assert.Equal("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"no\"?><sample><value>Hi</value><content>Bye</conent></sample>", result.UserAttributes.SignatureXml);
        }

        [Fact]
        public async void BankIdGetSessionAsync_WithBankIdGetSessionRequest__ShouldParseAndReturnNotBefore_AndNotAfter()
        {
            // Arrange
            var httpClient = GetHttpClientMockWithOkResponse("{ \"userAttributes\": { \"notBefore\": \"2018-12-25T00:00:00.000+02:00\", \"notAfter\": \"2018-12-26T00:00:00.000+02:00\" } }");
            var grandIdApiClient = new GrandIdApiClient(httpClient, new GrandIdApiClientConfiguration("x"));

            // Act
            var result = await grandIdApiClient.BankIdGetSessionAsync(new BankIdSessionStateRequest("x", "y"));

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.UserAttributes);
            Assert.Equal(new DateTime(2018, 12, 24, 22, 00, 00, DateTimeKind.Local), result.UserAttributes.NotBefore);
            Assert.Equal(new DateTime(2018, 12, 25, 22, 00, 00, DateTimeKind.Utc), result.UserAttributes.NotAfter);
        }

        [Fact]
        public async void FederatedDirectLoginAsync_WithServiceKey_AndUsername_AndPassword__ShouldGetToGrandIdFederatedLogin_WithServiceKey_AndUsername_AndPassword()
        {
            // Arrange

            // Act
            await _grandIdApiClient.FederatedDirectLoginAsync(new FederatedDirectLoginRequest("ask", "u", "p"));

            // Assert
            var request = _messageHandlerMock.GetFirstArgumentOfFirstInvocation<HttpMessageHandler, HttpRequestMessage>();
            Assert.Contains("authenticateServiceKey=ask", request.RequestUri.ToString());
            Assert.Contains("username=u", request.RequestUri.ToString());
            Assert.Contains("password=p", request.RequestUri.ToString());
        }

        [Fact]
        public async void FederatedDirectLoginAsync_WithFederatedDirectLoginRequest__ShouldParseAndReturnOrderRef_AndAutoStartToken()
        {
            // Arrange
            var httpClient = GetHttpClientMockWithOkResponse("{ \"sessionId\": \"s\", \"username\": \"u\", \"userAttributes\": { \"mobile\": \"0\", \"givenname\": \"ugn\", \"sn\": \"usn\", \"samaccountname\": \"usan\", \"title\": \"ut\" } }");
            var grandIdApiClient = new GrandIdApiClient(httpClient, new GrandIdApiClientConfiguration("x"));

            // Act
            var result = await grandIdApiClient.FederatedDirectLoginAsync(new FederatedDirectLoginRequest("ask", "u", "p"));

            // Assert
            Assert.NotNull(result);
            Assert.Equal("s", result.SessionId);
            Assert.Equal("u", result.Username);

            Assert.NotNull(result.UserAttributes);
            Assert.Equal("0", result.UserAttributes.MobilePhone);
            Assert.Equal("ugn", result.UserAttributes.GivenName);
            Assert.Equal("usn", result.UserAttributes.Surname);
            Assert.Equal("usan", result.UserAttributes.SameAccountName);
            Assert.Equal("ut", result.UserAttributes.Title);
        }


        [Fact]
        public async void LogoutAsync_WithSessionId__ShouldGetToGrandIdFederatedLogin_WithSessionId()
        {
            // Arrange

            // Act
            await _grandIdApiClient.LogoutAsync(new LogoutRequest("s"));

            // Assert
            var request = _messageHandlerMock.GetFirstArgumentOfFirstInvocation<HttpMessageHandler, HttpRequestMessage>();
            Assert.Contains("sessionid=s", request.RequestUri.ToString());
        }

        [Fact]
        public async void LogoutAsync_WithGrandIdLogoutRequest__ShouldParseAndReturnSessionDeleted()
        {
            // Arrange
            var httpClient = GetHttpClientMockWithOkResponse("{ \"sessiondeleted\": \"1\" }");
            var grandIdApiClient = new GrandIdApiClient(httpClient, new GrandIdApiClientConfiguration("x"));

            // Act
            var result = await grandIdApiClient.LogoutAsync(new LogoutRequest("s"));

            // Assert
            Assert.NotNull(result);
            Assert.True(result.SessionDeleted);
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
                BaseAddress = new Uri("https://grandid/")
            };
            return httpClient;
        }

        private static HttpClient GetHttpClientMockWithBadRequestResponse(string jsonResponse)
        {
            var messageHandlerMock = GetHttpClientMessageHandlerMock(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest,
                Content = new StringContent(jsonResponse, Encoding.Default, "application/json"),
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
    }
}
