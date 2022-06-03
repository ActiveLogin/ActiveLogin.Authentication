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

namespace ActiveLogin.Authentication.BankId.Api.Test;

public class BankIdApiClient_Tests
{
    private readonly Mock<HttpMessageHandler> _messageHandlerMock;
    private readonly BankIdApiClient _bankIdApiClient;

    public BankIdApiClient_Tests()
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
        _bankIdApiClient = new BankIdApiClient(httpClient);
    }

    [Fact]
    public async Task ErrorResponse__ShouldThrowException_WithErrorCode_AndDetails()
    {
        // Arrange
        var messageHandlerMock = GetHttpClientMessageHandlerMock(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.BadRequest,
            Content = new StringContent("{ \"errorCode\": \"AlreadyInProgress\", \"details\": \"d\" }", Encoding.Default, "application/json"),
        });

        var httpClient = new HttpClient(messageHandlerMock.Object)
        {
            BaseAddress = new Uri("https://bankid/")
        };
        var bankIdApiClient = new BankIdApiClient(httpClient);

        // Act
        var exception = await Assert.ThrowsAsync<BankIdApiException>(() => bankIdApiClient.AuthAsync(new AuthRequest("1.1.1.1")));

        // Assert
        Assert.Equal(ErrorCode.AlreadyInProgress, exception.ErrorCode);
        Assert.Equal("d", exception.ErrorDetails);
    }

    [Fact]
    public async Task AuthAsync_WithAuthRequest__ShouldPostToBankIdAuth_WithJsonPayload()
    {
        // Arrange

        // Act
        await _bankIdApiClient.AuthAsync(new AuthRequest("1.1.1.1"));

        // Assert
        Assert.Single(_messageHandlerMock.Invocations);
        var request = _messageHandlerMock.GetFirstArgumentOfFirstInvocation<HttpMessageHandler, HttpRequestMessage>();
        Assert.NotNull(request);

        Assert.Equal(HttpMethod.Post, request.Method);
        Assert.Equal(new Uri("https://bankid/auth"), request.RequestUri);
        Assert.Equal(new MediaTypeHeaderValue("application/json"), request.Content.Headers.ContentType);
    }

    [Fact]
    public async Task AuthAsync_WithEndUserIp__ShouldPostJsonPayload_WithEndUserIp_AndNoPersonalNumber_AndRequirementAsEmptyObject()
    {
        // Arrange

        // Act
        await _bankIdApiClient.AuthAsync(new AuthRequest("1.1.1.1"));

        // Assert
        var request = _messageHandlerMock.GetFirstArgumentOfFirstInvocation<HttpMessageHandler, HttpRequestMessage>();
        var contentString = await request.Content.ReadAsStringAsync();

        JsonTests.AssertProperty(contentString, "endUserIp", "1.1.1.1");
        JsonTests.AssertPropertyIsEmptyObject(contentString, "requirement");
        JsonTests.AssertOnlyProperties(contentString, new[]
        {
            "endUserIp",
            "requirement"
        });
    }


    [Fact]
    public async Task AuthAsync_WithEndUserIp_AndPin__ShouldPostJsonPayload_WithEndUserIp_AndPersonalNumber_AndNoRequirements()
    {
        // Arrange

        // Act
        await _bankIdApiClient.AuthAsync(new AuthRequest("1.1.1.1", "201801012392"));

        // Assert
        var request = _messageHandlerMock.GetFirstArgumentOfFirstInvocation<HttpMessageHandler, HttpRequestMessage>();
        var contentString = await request.Content.ReadAsStringAsync();

        JsonTests.AssertProperty(contentString, "endUserIp", "1.1.1.1");
        JsonTests.AssertProperty(contentString, "personalNumber", "201801012392");
        JsonTests.AssertPropertyIsEmptyObject(contentString, "requirement");
        JsonTests.AssertOnlyProperties(contentString, new[]
        {
            "endUserIp",
            "personalNumber",
            "requirement"
        });
    }


    [Fact]
    public async Task AuthAsync_WithRequirements__ShouldPostJsonPayload_WithRequirements()
    {
        // Arrange

        // Act
        await _bankIdApiClient.AuthAsync(new AuthRequest("1.1.1.1", "201801012392", new Requirement(new List<string> { "req1", "req2" }, true, true)));

        // Assert
        var request = _messageHandlerMock.GetFirstArgumentOfFirstInvocation<HttpMessageHandler, HttpRequestMessage>();
        var contentString = await request.Content.ReadAsStringAsync();

        JsonTests.AssertProperty(contentString, "endUserIp", "1.1.1.1");
        JsonTests.AssertProperty(contentString, "personalNumber", "201801012392");
        JsonTests.AssertSubProperty(contentString, "requirement", "allowFingerprint", true);
        JsonTests.AssertSubProperty(contentString, "requirement", "certificatePolicies", new List<string> { "req1", "req2" });
        JsonTests.AssertSubProperty(contentString, "requirement", "tokenStartRequired", true);
    }

    [Fact]
    public async Task AuthAsync_WithAuthRequest__ShouldParseAndReturnOrderRef_AndTokens()
    {
        // Arrange
        var httpClient = GetHttpClientMockWithOkResponse("{ \"orderRef\": \"abc123\", \"autoStartToken\": \"def456\", \"qrStartSecret\": \"ghi790\", \"qrStartToken\": \"jkl123\" }");
        var bankIdClient = new BankIdApiClient(httpClient);

        // Act
        var result = await bankIdClient.AuthAsync(new AuthRequest("1.1.1.1"));

        // Assert
        Assert.NotNull(result);
        Assert.Equal("abc123", result.OrderRef);
        Assert.Equal("def456", result.AutoStartToken);
        Assert.Equal("ghi790", result.QrStartSecret);
        Assert.Equal("jkl123", result.QrStartToken);
    }

    [Fact]
    public async Task AuthAsync_WithAuthRequest__ShouldHaveUserData()
    {
        //Arrange
        byte[] userNonVisibleData = Encoding.ASCII.GetBytes("Hello");
        string asBase64 = Convert.ToBase64String(userNonVisibleData);

        //Act
        await _bankIdApiClient.AuthAsync(new AuthRequest("1.1.1.1", null, null, "Hello", userNonVisibleData, "simpleMarkdownV1"));

        //Assert
        var request = _messageHandlerMock.GetFirstArgumentOfFirstInvocation<HttpMessageHandler, HttpRequestMessage>();
        var contentString = await request.Content.ReadAsStringAsync();

        JsonTests.AssertProperty(contentString, "endUserIp", "1.1.1.1");
        JsonTests.AssertPropertyIsEmptyObject(contentString, "requirement");
        JsonTests.AssertProperty(contentString, "userVisibleData", asBase64);
        JsonTests.AssertProperty(contentString, "userNonVisibleData", asBase64);
        JsonTests.AssertProperty(contentString, "userVisibleDataFormat", "simpleMarkdownV1");
        JsonTests.AssertOnlyProperties(contentString, new[]
        {
            "endUserIp",
            "requirement",
            "userVisibleData",
            "userVisibleDataFormat",
            "userNonVisibleData"
        });
    }

    [Fact]
    public async Task SignAsync_WithSignRequest__ShouldPostToBankIdSign_WithJsonPayload()
    {
        // Arrange

        // Act
        await _bankIdApiClient.SignAsync(new SignRequest("1.1.1.1", "userVisibleData"));

        // Assert
        Assert.Single(_messageHandlerMock.Invocations);
        var request = _messageHandlerMock.GetFirstArgumentOfFirstInvocation<HttpMessageHandler, HttpRequestMessage>();
        Assert.NotNull(request);

        Assert.Equal(HttpMethod.Post, request.Method);
        Assert.Equal(new Uri("https://bankid/sign"), request.RequestUri);
        Assert.Equal(new MediaTypeHeaderValue("application/json"), request.Content.Headers.ContentType);
    }

    [Fact]
    public async Task SignAsync_WithUserVisibleDataFormat__ShouldPostToBankIdSign_WithJsonPayload()
    {
        // Arrange

        // Act
        await _bankIdApiClient.SignAsync(new SignRequest(
            "1.1.1.1",
            "userVisibleData",
            userVisibleDataFormat: "userVisibleDataFormat",
            userNonVisibleData: new byte[1]));

        // Assert
        var request = _messageHandlerMock.GetFirstArgumentOfFirstInvocation<HttpMessageHandler, HttpRequestMessage>();
        var contentString = await request.Content.ReadAsStringAsync();

        JsonTests.AssertProperty(contentString, "endUserIp", "1.1.1.1");
        JsonTests.AssertPropertyIsEmptyObject(contentString, "requirement");
        JsonTests.AssertProperty(contentString, "userVisibleData", "dXNlclZpc2libGVEYXRh");
        JsonTests.AssertProperty(contentString, "userVisibleDataFormat", "userVisibleDataFormat");
        JsonTests.AssertProperty(contentString, "userNonVisibleData", "AA==");
        JsonTests.AssertOnlyProperties(contentString, new[]
        {
            "endUserIp",
            "requirement",
            "userVisibleData",
            "userVisibleDataFormat",
            "userNonVisibleData"
        });
    }

    [Fact]
    public async Task SignAsync_WithEndUserIp__ShouldPostJsonPayload_WithEndUserIp_AndUserVisibleData_AndNoPersonalNumber_AndRequirementAsEmptyObject()
    {
        // Arrange

        // Act
        await _bankIdApiClient.SignAsync(new SignRequest("1.1.1.1", "userVisibleData"));

        // Assert
        var request = _messageHandlerMock.GetFirstArgumentOfFirstInvocation<HttpMessageHandler, HttpRequestMessage>();
        var contentString = await request.Content.ReadAsStringAsync();

        JsonTests.AssertProperty(contentString, "endUserIp", "1.1.1.1");
        JsonTests.AssertPropertyIsEmptyObject(contentString, "requirement");
        JsonTests.AssertProperty(contentString, "userVisibleData", "dXNlclZpc2libGVEYXRh");
        JsonTests.AssertOnlyProperties(contentString, new[]
        {
            "endUserIp",
            "requirement",
            "userVisibleData"
        });
    }

    [Fact]
    public async Task SignAsync_WithEndUserIp_AndPin__ShouldPostJsonPayload_WithEndUserIp_AndPersonalNumber_AndUserVisibleData_AndNoRequirements()
    {
        // Arrange

        // Act
        await _bankIdApiClient.SignAsync(new SignRequest("1.1.1.1", "userVisibleData", null, "201801012392"));

        // Assert
        var request = _messageHandlerMock.GetFirstArgumentOfFirstInvocation<HttpMessageHandler, HttpRequestMessage>();
        var contentString = await request.Content.ReadAsStringAsync();

        JsonTests.AssertProperty(contentString, "endUserIp", "1.1.1.1");
        JsonTests.AssertProperty(contentString, "personalNumber", "201801012392");
        JsonTests.AssertPropertyIsEmptyObject(contentString, "requirement");
        JsonTests.AssertProperty(contentString, "userVisibleData", "dXNlclZpc2libGVEYXRh");
        JsonTests.AssertOnlyProperties(contentString, new[]
        {
            "endUserIp",
            "personalNumber",
            "requirement",
            "userVisibleData"
        });
    }

    [Fact]
    public async Task SignAsync_WithUserNonVisibleData__ShouldPostJsonPayload_WithUserNonVisibleData()
    {
        // Arrange

        // Act
        await _bankIdApiClient.SignAsync(new SignRequest("1.1.1.1", "userVisibleData", userNonVisibleData: new byte[1]));

        // Assert
        var request = _messageHandlerMock.GetFirstArgumentOfFirstInvocation<HttpMessageHandler, HttpRequestMessage>();
        var contentString = await request.Content.ReadAsStringAsync();

        JsonTests.AssertProperty(contentString, "endUserIp", "1.1.1.1");
        JsonTests.AssertPropertyIsEmptyObject(contentString, "requirement");
        JsonTests.AssertProperty(contentString, "userVisibleData", "dXNlclZpc2libGVEYXRh");
        JsonTests.AssertProperty(contentString, "userNonVisibleData", "AA==");
        JsonTests.AssertOnlyProperties(contentString, new[]
        {
            "endUserIp",
            "requirement",
            "userVisibleData",
            "userNonVisibleData"
        });
    }

    [Fact]
    public async Task SignAsync_WithRequirements__ShouldPostJsonPayload_WithRequirements()
    {
        // Arrange

        // Act
        await _bankIdApiClient.SignAsync(new SignRequest("1.1.1.1", "userVisibleData", Encoding.UTF8.GetBytes("userNonVisibleData"), "201801012392", new Requirement(new List<string> { "req1", "req2" }, true, true)));

        // Assert
        var request = _messageHandlerMock.GetFirstArgumentOfFirstInvocation<HttpMessageHandler, HttpRequestMessage>();
        var contentString = await request.Content.ReadAsStringAsync();

        JsonTests.AssertProperty(contentString, "endUserIp", "1.1.1.1");
        JsonTests.AssertProperty(contentString, "personalNumber", "201801012392");
        JsonTests.AssertSubProperty(contentString, "requirement", "allowFingerprint", true);
        JsonTests.AssertSubProperty(contentString, "requirement", "certificatePolicies", new List<string> { "req1", "req2" });
        JsonTests.AssertSubProperty(contentString, "requirement", "tokenStartRequired", true);
        JsonTests.AssertProperty(contentString, "userVisibleData", "dXNlclZpc2libGVEYXRh");
        JsonTests.AssertProperty(contentString, "userNonVisibleData", "dXNlck5vblZpc2libGVEYXRh");
        JsonTests.AssertOnlyProperties(contentString, new[]
        {
            "endUserIp",
            "personalNumber",
            "requirement",
            "userVisibleData",
            "userNonVisibleData"
        });
    }

    [Fact]
    public async Task SignAsync_WithSignRequest__ShouldParseAndReturnOrderRef_AndTokens()
    {
        // Arrange
        var httpClient = GetHttpClientMockWithOkResponse("{ \"orderRef\": \"abc123\", \"autoStartToken\": \"def456\", \"qrStartSecret\": \"ghi790\", \"qrStartToken\": \"jkl123\" }");
        var bankIdClient = new BankIdApiClient(httpClient);

        // Act
        var result = await bankIdClient.SignAsync(new SignRequest("1.1.1.1", "userVisibleData"));

        // Assert
        Assert.NotNull(result);
        Assert.Equal("abc123", result.OrderRef);
        Assert.Equal("def456", result.AutoStartToken);
        Assert.Equal("ghi790", result.QrStartSecret);
        Assert.Equal("jkl123", result.QrStartToken);
    }

    [Fact]
    public async Task CollectAsync_WithCollectRequest__ShouldPostToBankIdCollect_WithJsonPayload()
    {
        // Arrange

        // Act
        await _bankIdApiClient.CollectAsync(new CollectRequest("abc123"));

        // Assert
        Assert.Single(_messageHandlerMock.Invocations);
        var request = _messageHandlerMock.GetFirstArgumentOfFirstInvocation<HttpMessageHandler, HttpRequestMessage>();
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
        var request = _messageHandlerMock.GetFirstArgumentOfFirstInvocation<HttpMessageHandler, HttpRequestMessage>();
        var contentString = await request.Content.ReadAsStringAsync();

        JsonTests.AssertProperty(contentString, "orderRef", "abc123");
        JsonTests.AssertOnlyProperties(contentString, new[]
        {
            "orderRef"
        });    
    }

    [Fact]
    public async Task CollectAsync_WithCollectRequest__ShouldParseAndReturnHintCode()
    {
        // Arrange
        var httpClient = GetHttpClientMockWithOkResponse("{ \"hintCode\":\"OutstandingTransaction\" }");
        var bankIdClient = new BankIdApiClient(httpClient);

        // Act
        var result = await bankIdClient.CollectAsync(new CollectRequest("x"));

        // Assert
        Assert.NotNull(result);
        Assert.Equal("OutstandingTransaction", result.HintCode);
        Assert.Equal(CollectHintCode.OutstandingTransaction, result.GetCollectHintCode());
    }

    [Fact]
    public async Task CollectAsync_WithCollectRequest__ShouldParseAndReturnStatus()
    {
        // Arrange
        var httpClient = GetHttpClientMockWithOkResponse("{ \"status\":\"Pending\" }");
        var bankIdClient = new BankIdApiClient(httpClient);

        // Act
        var result = await bankIdClient.CollectAsync(new CollectRequest("x"));

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Pending", result.Status);
        Assert.Equal(CollectStatus.Pending, result.GetCollectStatus());
    }

    [Fact]
    public async Task CollectAsync_WithCollectRequest__ShouldParseAndReturnOrderRef()
    {
        // Arrange
        var httpClient = GetHttpClientMockWithOkResponse("{ \"orderRef\":\"abc123\" }");
        var bankIdClient = new BankIdApiClient(httpClient);

        // Act
        var result = await bankIdClient.CollectAsync(new CollectRequest("x"));

        // Assert
        Assert.NotNull(result);
        Assert.Equal("abc123", result.OrderRef);
    }

    [Fact]
    public async Task CollectAsync_WithCollectRequest__ShouldParseAndReturnCompletionDataSignature_AndOcspResponse()
    {
        // Arrange
        var httpClient = GetHttpClientMockWithOkResponse("{ \"completionData\": {  \"signature\": \"s\", \"ocspResponse\": \"or\" } }");
        var bankIdClient = new BankIdApiClient(httpClient);

        // Act
        var result = await bankIdClient.CollectAsync(new CollectRequest("x"));

        // Assert
        Assert.NotNull(result);
        Assert.Equal("s", result.CompletionData.Signature);
        Assert.Equal("or", result.CompletionData.OcspResponse);
    }

    [Fact]
    public async Task CollectAsync_WithCollectRequest__ShouldParseAndReturnCompletionDataSignatureXml()
    {
        // Arrange
        var httpClient = GetHttpClientMockWithOkResponse("{ \"completionData\": { \"signature\": \"PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0iVVRGLTgiIHN0YW5kYWxvbmU9Im5vIj8+PHNhbXBsZT48dmFsdWU+SGk8L3ZhbHVlPjxjb250ZW50PkJ5ZTwvY29uZW50Pjwvc2FtcGxlPg==\" } }");
        var bankIdClient = new BankIdApiClient(httpClient);

        // Act
        var result = await bankIdClient.CollectAsync(new CollectRequest("x"));

        // Assert
        Assert.NotNull(result);
        Assert.Equal("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"no\"?><sample><value>Hi</value><content>Bye</conent></sample>", result.CompletionData.GetSignatureXml());
    }

    [Fact]
    public async Task CollectAsync_WithCollectRequest__ShouldParseAndReturnCompletionDataUser()
    {
        // Arrange
        var httpClient = GetHttpClientMockWithOkResponse("{ \"completionData\": { \"user\": { \"personalNumber\": \"201801012392\", \"name\": \"n\", \"givenName\": \"gn\", \"surname\": \"sn\" } } }");
        var bankIdClient = new BankIdApiClient(httpClient);

        // Act
        var result = await bankIdClient.CollectAsync(new CollectRequest("x"));

        // Assert
        Assert.NotNull(result);
        Assert.Equal("201801012392", result.CompletionData.User.PersonalIdentityNumber);
        Assert.Equal("n", result.CompletionData.User.Name);
        Assert.Equal("gn", result.CompletionData.User.GivenName);
        Assert.Equal("sn", result.CompletionData.User.Surname);
    }

    [Fact]
    public async Task CollectAsync_WithCollectRequest__ShouldParseAndReturnCompletionDataDevice()
    {
        // Arrange
        var httpClient = GetHttpClientMockWithOkResponse("{ \"completionData\": { \"device\": { \"ipAddress\": \"1.1.1.1\" } } }");
        var bankIdClient = new BankIdApiClient(httpClient);

        // Act
        var result = await bankIdClient.CollectAsync(new CollectRequest("x"));

        // Assert
        Assert.NotNull(result);
        Assert.Equal("1.1.1.1", result.CompletionData.Device.IpAddress);
    }

    [Fact]
    public async Task CollectAsync_WithCollectRequest__ShouldParseAndReturnCompletionDataCertDates_ConvetedFromUnixEpochMillisecondsToDateTime()
    {
        // Arrange
        var httpClient = GetHttpClientMockWithOkResponse("{ \"completionData\": { \"cert\": { \"notBefore\": \"671630400000\", \"notAfter\": \"671659200000\" } } }");
        var bankIdClient = new BankIdApiClient(httpClient);

        // Act
        var result = await bankIdClient.CollectAsync(new CollectRequest("x"));

        // Assert
        Assert.NotNull(result);
        Assert.Equal("671630400000", result.CompletionData.Cert.NotBefore);
        Assert.Equal(new DateTime(1991, 4, 14, 12, 00, 00), result.CompletionData.Cert.GetNotBeforeDateTime());
        Assert.Equal("671659200000", result.CompletionData.Cert.NotAfter);
        Assert.Equal(new DateTime(1991, 4, 14, 20, 00, 00), result.CompletionData.Cert.GetNotAfterDateTime());
    }

    [Fact]
    public async Task CancelAsync_WithCancelRequest__ShouldPostToBankIdCancel_WithJsonPayload()
    {
        // Arrange

        // Act
        await _bankIdApiClient.CancelAsync(new CancelRequest("x"));

        // Assert
        Assert.Single(_messageHandlerMock.Invocations);
        var request = _messageHandlerMock.GetFirstArgumentOfFirstInvocation<HttpMessageHandler, HttpRequestMessage>();
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
        var request = _messageHandlerMock.GetFirstArgumentOfFirstInvocation<HttpMessageHandler, HttpRequestMessage>();
        var contentString = await request.Content.ReadAsStringAsync();

        JsonTests.AssertProperty(contentString, "orderRef", "abc123");
        JsonTests.AssertOnlyProperties(contentString, new[]
        {
            "orderRef"
        });
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
