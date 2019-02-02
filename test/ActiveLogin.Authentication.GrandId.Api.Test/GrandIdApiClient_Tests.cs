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
        public async void BankIdFederatedLoginAsync__ShouldPostToGrandIdFederatedLogin_WithFormPayload()
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
            await grandIdApiClient.BankIdFederatedLoginAsync(new BankIdFederatedLoginRequest("y"));

            // Assert
            Assert.Single(messageHandlerMock.Invocations);
            var request = messageHandlerMock.GetFirstArgumentOfFirstInvocation<HttpMessageHandler, HttpRequestMessage>();
            Assert.NotNull(request);

            Assert.Equal(HttpMethod.Post, request.Method);
            Assert.StartsWith("https://grandid/FederatedLogin", request.RequestUri.ToString());
            Assert.Equal(new MediaTypeHeaderValue("application/x-www-form-urlencoded"), request.Content.Headers.ContentType);
        }

        [Fact]
        public async void BankIdFederatedLoginAsync_WithApiKey__ShouldPostToGrandIdFederatedLogin_WithApiKey()
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
            var request = messageHandlerMock.GetFirstArgumentOfFirstInvocation<HttpMessageHandler, HttpRequestMessage>();
            Assert.Contains("apiKey=ak", request.RequestUri.ToString());
        }

        [Fact]
        public async void BankIdFederatedLoginAsync_WithNoParams__ShouldPostToGrandIdFederatedLogin_WithNoPayload()
        {
            // Arrange

            // Act
            await _grandIdApiClient.BankIdFederatedLoginAsync(new BankIdFederatedLoginRequest("ask"));

            // Assert
            var request = _messageHandlerMock.GetFirstArgumentOfFirstInvocation<HttpMessageHandler, HttpRequestMessage>();
            var contentString = await request.Content.ReadAsStringAsync();

            Assert.Empty(contentString);
        }

        [Fact]
        public async void BankIdFederatedLoginAsync_WithCallbackUrl__ShouldPostToGrandIdFederatedLogin_WithCallbackUrl_ButNoPin()
        {
            // Arrange

            // Act
            await _grandIdApiClient.BankIdFederatedLoginAsync(new BankIdFederatedLoginRequest("ask", "https://cb/"));

            // Assert
            var request = _messageHandlerMock.GetFirstArgumentOfFirstInvocation<HttpMessageHandler, HttpRequestMessage>();
            var contentString = await request.Content.ReadAsStringAsync();

            Assert.Equal("callbackUrl=aHR0cHM6Ly9jYi8%3D", contentString);
        }

        [Fact]
        public async void BankIdFederatedLoginAsync_WithAllValues__ShouldPostToGrandIdFederatedLogin_WithAllValues()
        {
            // Arrange
            var bankIdFederatedLoginRequest = new BankIdFederatedLoginRequest(
                authenticateServiceKey: "ask",
                callbackUrl: "https://cb/",
                useChooseDevice: true,
                useSameDevice: true,
                askForPersonalIdentityNumber: true,
                personalIdentityNumber: "20180101239",
                requireMobileBankId: true,
                customerUrl: "https://cu/",
                showGui: true,
                signUserVisibleData: "uvd",
                signUserNonVisibleData: "unvd"
            );

            // Act
            await _grandIdApiClient.BankIdFederatedLoginAsync(bankIdFederatedLoginRequest);

            // Assert
            var request = _messageHandlerMock.GetFirstArgumentOfFirstInvocation<HttpMessageHandler, HttpRequestMessage>();
            var contentString = await request.Content.ReadAsStringAsync();

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
        public async void BankIdFederatedLoginAsync_WithServiceKey_AndCallbackUrl_AndPin__ShouldPostToGrandIdFederatedLogin_WithPin()
        {
            // Arrange

            // Act
            await _grandIdApiClient.BankIdFederatedLoginAsync(new BankIdFederatedLoginRequest("y", "https://c/", personalIdentityNumber: "201801012392"));

            // Assert
            var request = _messageHandlerMock.GetFirstArgumentOfFirstInvocation<HttpMessageHandler, HttpRequestMessage>();
            var contentString = await request.Content.ReadAsStringAsync();

            Assert.Contains("personalNumber=20180101239", contentString);
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
            await _grandIdApiClient.BankIdGetSessionAsync(new BankIdGetSessionRequest("ask", "s"));

            // Assert
            var request = _messageHandlerMock.GetFirstArgumentOfFirstInvocation<HttpMessageHandler, HttpRequestMessage>();
            Assert.Contains("authenticateServiceKey=ask", request.RequestUri.ToString());
            Assert.Contains("sessionid=s", request.RequestUri.ToString());
        }

        [Fact]
        public async void BankIdGetSessionAsync_WithBankIdGetSessionRequest__ShouldParseAndReturnGetSessionResponse()
        {
            // Arrange
            var httpClient = GetHttpClientMockWithOkResponse("{ \"sessionId\": \"s\", \"username\": \"u\", \"userAttributes\": { \"givenName\": \"ugn\", \"surname\": \"usn\", \"name\": \"un\", \"personalNumber\": \"upn\", \"ipAddress\": \"uip\" } }");
            var grandIdApiClient = new GrandIdApiClient(httpClient, new GrandIdApiClientConfiguration("x"));

            // Act
            var result = await grandIdApiClient.BankIdGetSessionAsync(new BankIdGetSessionRequest("x", "y"));

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
            var result = await grandIdApiClient.BankIdGetSessionAsync(new BankIdGetSessionRequest("x", "y"));

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.UserAttributes);
            Assert.Equal("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"no\"?><sample><value>Hi</value><content>Bye</conent></sample>", result.UserAttributes.GetSignatureXml());
        }

        [Fact]
        public async void BankIdGetSessionAsync_WithBankIdGetSessionRequest__ShouldParseAndReturnNotBefore_AndNotAfter()
        {
            // Arrange
            var httpClient = GetHttpClientMockWithOkResponse("{ \"userAttributes\": { \"notBefore\": \"2018-12-25T00:00:00.000+02:00\", \"notAfter\": \"2018-12-26T00:00:00.000+02:00\" } }");
            var grandIdApiClient = new GrandIdApiClient(httpClient, new GrandIdApiClientConfiguration("x"));

            // Act
            var result = await grandIdApiClient.BankIdGetSessionAsync(new BankIdGetSessionRequest("x", "y"));

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.UserAttributes);
            Assert.Equal(new DateTime(2018, 12, 24, 22, 00, 00, DateTimeKind.Local), result.UserAttributes.GetNotBeforeDateTime());
            Assert.Equal(new DateTime(2018, 12, 25, 22, 00, 00, DateTimeKind.Utc), result.UserAttributes.GetNotAfterDateTime());
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
