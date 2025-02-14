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

public class BankIdAppApiClient_Tests
{
    private readonly Mock<HttpMessageHandler> _messageHandlerMock;
    private readonly BankIdAppApiClient _bankIdAppApiClient;

    public BankIdAppApiClient_Tests()
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
        _bankIdAppApiClient = new BankIdAppApiClient(httpClient);
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
        var bankIdApiClient = new BankIdAppApiClient(httpClient);

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
        await _bankIdAppApiClient.AuthAsync(new AuthRequest("1.1.1.1"));

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
        await _bankIdAppApiClient.AuthAsync(new AuthRequest("1.1.1.1"));

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
    public async Task AuthAsync_WithRequirements__ShouldPostJsonPayload_WithRequirements()
    {
        // Arrange

        // Act
        await _bankIdAppApiClient.AuthAsync(new AuthRequest("1.1.1.1", new Requirement(new List<string> { "req1", "req2" }, "low", true, true, "190001010101")));

        // Assert
        var request = _messageHandlerMock.GetFirstArgumentOfFirstInvocation<HttpMessageHandler, HttpRequestMessage>();
        var contentString = await request.Content.ReadAsStringAsync();

        JsonTests.AssertProperty(contentString, "endUserIp", "1.1.1.1");
        JsonTests.AssertSubProperty(contentString, "requirement", "certificatePolicies", new List<string> { "req1", "req2" });
        JsonTests.AssertSubProperty(contentString, "requirement", "pinCode", true);
        JsonTests.AssertSubProperty(contentString, "requirement", "mrtd", true);
        JsonTests.AssertSubProperty(contentString, "requirement", "personalNumber", "190001010101");
    }

    [Fact]
    public async Task AuthAsync_WithAuthRequest__ShouldParseAndReturnOrderRef_AndTokens()
    {
        // Arrange
        var httpClient = GetHttpClientMockWithOkResponse("{ \"orderRef\": \"abc123\", \"autoStartToken\": \"def456\", \"qrStartSecret\": \"ghi790\", \"qrStartToken\": \"jkl123\" }");
        var bankIdClient = new BankIdAppApiClient(httpClient);

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
        await _bankIdAppApiClient.AuthAsync(new AuthRequest("1.1.1.1", null, "Hello", userNonVisibleData, "simpleMarkdownV1"));

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
    public async Task AuthAsync_WithAuthRequest__ShouldHaveAppDeviceParametersPayload()
    {
        // Arrange

        var authRequest = new AuthRequest(endUserIp: "1.1.1.1",
            requirement: null,
            userVisibleData: null,
            userNonVisibleData: null,
            userVisibleDataFormat: null,
            returnUrl: null,
            returnRisk: null,
            web: null,
            app: new DeviceDataApp(
                "appIdentifier",
                "deviceOs",
                "deviceModelName",
                "deviceIdentifier"));

        //Act
        await _bankIdAppApiClient.AuthAsync(authRequest);

        //Assert
        var request = _messageHandlerMock.GetFirstArgumentOfFirstInvocation<HttpMessageHandler, HttpRequestMessage>();
        var contentString = await request.Content.ReadAsStringAsync();

        JsonTests.AssertPropertyIsNull(contentString, "web");
        JsonTests.AssertPropertyIsNotNull(contentString, "app");

        JsonTests.AssertProperties(contentString,
            new Dictionary<string, string>
            {
                { "app.appIdentifier", "appIdentifier" },
                
                // note the casing from JsonPropertyName attribute in the <see cref="BankIdEndUserAppDeviceParameters"/> class.
                { "app.deviceOS", "deviceOs" },

                { "app.deviceModelName", "deviceModelName" },
                { "app.deviceIdentifier", "deviceIdentifier" }
            });

    }

    [Fact]
    public async Task AuthAsync_WithAuthRequest__ShouldHaveWebDeviceParametersPayload()
    {
        // Arrange

        var authRequest = new AuthRequest(endUserIp: "1.1.1.1",
            requirement: null,
            userVisibleData: null,
            userNonVisibleData: null,
            userVisibleDataFormat: null,
            returnUrl: null,
            returnRisk: null,
            web: new DeviceDataWeb(
                "referringDomain",
                "userAgent",
                "deviceIdentifier"),
            app: null);

        //Act
        await _bankIdAppApiClient.AuthAsync(authRequest);

        //Assert
        var request = _messageHandlerMock.GetFirstArgumentOfFirstInvocation<HttpMessageHandler, HttpRequestMessage>();
        var contentString = await request.Content.ReadAsStringAsync();

        JsonTests.AssertPropertyIsNull(contentString, "app");
        JsonTests.AssertPropertyIsNotNull(contentString, "web");

        JsonTests.AssertProperties(contentString,
            new Dictionary<string, string>
            {
                { "web.referringDomain", "referringDomain" },
                { "web.userAgent", "userAgent" },
                { "web.deviceIdentifier", "deviceIdentifier" }
            });

    }

    [Fact]
    public async Task AuthAsync_WithAuthRequest__ShouldHaveReturnRisk()
    {
        //Arrange
        byte[] userNonVisibleData = Encoding.ASCII.GetBytes("Hello");
        string asBase64 = Convert.ToBase64String(userNonVisibleData);

        //Act
        await _bankIdAppApiClient.AuthAsync(new AuthRequest("1.1.1.1", null, null, null, null, null, true));

        //Assert
        var request = _messageHandlerMock.GetFirstArgumentOfFirstInvocation<HttpMessageHandler, HttpRequestMessage>();
        var contentString = await request.Content.ReadAsStringAsync();

        JsonTests.AssertProperty(contentString, "endUserIp", "1.1.1.1");
        JsonTests.AssertPropertyIsEmptyObject(contentString, "requirement");
        JsonTests.AssertProperty(contentString, "returnRisk", true);
        JsonTests.AssertOnlyProperties(contentString, new[]
        {
            "endUserIp",
            "requirement",
            "returnRisk",
        });
    }

    [Fact]
    public async Task AuthAsync_WithAuthRequest__ShouldHaveReturnUrl()
    {
        //Arrange
        byte[] userNonVisibleData = Encoding.ASCII.GetBytes("Hello");
        string asBase64 = Convert.ToBase64String(userNonVisibleData);

        //Act
        await _bankIdAppApiClient.AuthAsync(new AuthRequest("1.1.1.1", null, null, null, null, "http://mywebpage.com", null));

        //Assert
        var request = _messageHandlerMock.GetFirstArgumentOfFirstInvocation<HttpMessageHandler, HttpRequestMessage>();
        var contentString = await request.Content.ReadAsStringAsync();

        JsonTests.AssertProperty(contentString, "endUserIp", "1.1.1.1");
        JsonTests.AssertPropertyIsEmptyObject(contentString, "requirement");
        JsonTests.AssertProperty(contentString, "returnUrl", "http://mywebpage.com");
        JsonTests.AssertOnlyProperties(contentString, new[]
        {
            "endUserIp",
            "requirement",
            "returnUrl",
        });
    }

    [Fact]
    public async Task SignAsync_WithSignRequest__ShouldPostToBankIdSign_WithJsonPayload()
    {
        // Arrange

        // Act
        await _bankIdAppApiClient.SignAsync(new SignRequest("1.1.1.1", "userVisibleData"));

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
        await _bankIdAppApiClient.SignAsync(new SignRequest(
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
    public async Task SignAsync_WithEndUserIp__ShouldPostJsonPayload_WithEndUserIp_AndUserVisibleData_AndRequirementAsEmptyObject()
    {
        // Arrange

        // Act
        await _bankIdAppApiClient.SignAsync(new SignRequest("1.1.1.1", "userVisibleData"));

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
    public async Task SignAsync_WithEndUserIp__ShouldPostJsonPayload_WithEndUserIp_AndUserVisibleData_AndNoRequirements()
    {
        // Arrange

        // Act
        await _bankIdAppApiClient.SignAsync(new SignRequest("1.1.1.1", "userVisibleData", null));

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
    public async Task SignAsync_WithUserNonVisibleData__ShouldPostJsonPayload_WithUserNonVisibleData()
    {
        // Arrange

        // Act
        await _bankIdAppApiClient.SignAsync(new SignRequest("1.1.1.1", "userVisibleData", userNonVisibleData: new byte[1]));

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
        await _bankIdAppApiClient.SignAsync(new SignRequest("1.1.1.1", "userVisibleData", Encoding.UTF8.GetBytes("userNonVisibleData"), new Requirement(new List<string> { "req1", "req2" }, "low", true, true, "190001010101")));

        // Assert
        var request = _messageHandlerMock.GetFirstArgumentOfFirstInvocation<HttpMessageHandler, HttpRequestMessage>();
        var contentString = await request.Content.ReadAsStringAsync();

        JsonTests.AssertProperty(contentString, "endUserIp", "1.1.1.1");
        JsonTests.AssertSubProperty(contentString, "requirement", "certificatePolicies", new List<string> { "req1", "req2" });
        JsonTests.AssertSubProperty(contentString, "requirement", "pinCode", true);
        JsonTests.AssertSubProperty(contentString, "requirement", "mrtd", true);
        JsonTests.AssertSubProperty(contentString, "requirement", "personalNumber", "190001010101");
        JsonTests.AssertProperty(contentString, "userVisibleData", "dXNlclZpc2libGVEYXRh");
        JsonTests.AssertProperty(contentString, "userNonVisibleData", "dXNlck5vblZpc2libGVEYXRh");
        JsonTests.AssertOnlyProperties(contentString, new[]
        {
            "endUserIp",
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
        var bankIdClient = new BankIdAppApiClient(httpClient);

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
    public async Task SignAsync_WithAuthRequest__ShouldHaveAppDeviceParametersPayload()
    {
        // Arrange

        var signRequest = new SignRequest(endUserIp: "1.1.1.1",
            requirement: null,
            userVisibleData: "userVisibleData",
            userNonVisibleData: null,
            userVisibleDataFormat: null,
            returnUrl: null,
            returnRisk: null,
            web: null,
            app: new DeviceDataApp(
                "appIdentifier",
                "deviceOs",
                "deviceModelName",
                "deviceIdentifier"));

        //Act
        await _bankIdAppApiClient.SignAsync(signRequest);

        //Assert
        var request = _messageHandlerMock.GetFirstArgumentOfFirstInvocation<HttpMessageHandler, HttpRequestMessage>();
        var contentString = await request.Content.ReadAsStringAsync();

        JsonTests.AssertPropertyIsNull(contentString, "web");
        JsonTests.AssertPropertyIsNotNull(contentString, "app");

        JsonTests.AssertProperties(contentString,
            new Dictionary<string, string>
            {
                { "app.appIdentifier", "appIdentifier" },
                
                // note the casing from JsonPropertyName attribute in the <see cref="BankIdEndUserAppDeviceParameters"/> class.
                { "app.deviceOS", "deviceOs" },

                { "app.deviceModelName", "deviceModelName" },
                { "app.deviceIdentifier", "deviceIdentifier" }
            });

    }

    [Fact]
    public async Task SignAsync_WithAuthRequest__ShouldHaveWebDeviceParametersPayload()
    {
        // Arrange

        var authRequest = new SignRequest(endUserIp: "1.1.1.1",
            requirement: null,
            userVisibleData: "userVisibleData",
            userNonVisibleData: null,
            userVisibleDataFormat: null,
            returnUrl: null,
            returnRisk: null,
            web: new DeviceDataWeb(
                "referringDomain",
                "userAgent",
                "deviceIdentifier"),
            app: null);

        //Act
        await _bankIdAppApiClient.SignAsync(authRequest);

        //Assert
        var request = _messageHandlerMock.GetFirstArgumentOfFirstInvocation<HttpMessageHandler, HttpRequestMessage>();
        var contentString = await request.Content.ReadAsStringAsync();

        JsonTests.AssertPropertyIsNull(contentString, "app");
        JsonTests.AssertPropertyIsNotNull(contentString, "web");

        JsonTests.AssertProperties(contentString,
            new Dictionary<string, string>
            {
                { "web.referringDomain", "referringDomain" },
                { "web.userAgent", "userAgent" },
                { "web.deviceIdentifier", "deviceIdentifier" }
            });

    }

    [Fact]
    public async Task PaymentAsync_WithPaymentnRequest__ShouldPostToBankIdPayment_WithJsonPayload()
    {
        // Arrange

        // Act
        await _bankIdAppApiClient.PaymentAsync(new PaymentRequest("1.1.1.1", new UserVisibleTransaction("npa", new Recipient("merchant"))));

        // Assert
        Assert.Single(_messageHandlerMock.Invocations);
        var request = _messageHandlerMock.GetFirstArgumentOfFirstInvocation<HttpMessageHandler, HttpRequestMessage>();
        Assert.NotNull(request);

        Assert.Equal(HttpMethod.Post, request.Method);
        Assert.Equal(new Uri("https://bankid/payment"), request.RequestUri);
        Assert.Equal(new MediaTypeHeaderValue("application/json"), request.Content.Headers.ContentType);
    }



    [Fact]
    public async Task PaymentAsync_WithUserVisibleDataFormat__ShouldPostToBankIdPayment_WithJsonPayload()
    {
        // Arrange

        // Act
        await _bankIdAppApiClient.PaymentAsync(new PaymentRequest(
            "1.1.1.1",
            new UserVisibleTransaction("npa", new Recipient("merchant")),
            userVisibleData: "userVisibleData",
            userVisibleDataFormat: "userVisibleDataFormat"));

        // Assert
        var request = _messageHandlerMock.GetFirstArgumentOfFirstInvocation<HttpMessageHandler, HttpRequestMessage>();
        var contentString = await request.Content.ReadAsStringAsync();

        JsonTests.AssertProperty(contentString, "endUserIp", "1.1.1.1");
        JsonTests.AssertPropertyIsEmptyObject(contentString, "requirement");
        JsonTests.AssertProperty(contentString, "userVisibleData", "dXNlclZpc2libGVEYXRh");
        JsonTests.AssertProperty(contentString, "userVisibleDataFormat", "userVisibleDataFormat");
        JsonTests.AssertSubProperty(contentString, "userVisibleTransaction", "transactionType", "npa");
        JsonTests.AssertPropertyHierarchy(contentString, "merchant", "userVisibleTransaction", "recipient", "name");
        JsonTests.AssertOnlyProperties(contentString, new[]
        {
            "endUserIp",
            "requirement",
            "userVisibleData",
            "userVisibleDataFormat",
            "userVisibleTransaction"
        });
    }

    [Fact]
    public async Task PaymentAsync_WithEndUserIp__ShouldPostJsonPayload_WithEndUserIp_AndUserVisibleTransaction_AndRequirementAsEmptyObject()
    {
        // Arrange

        // Act
        await _bankIdAppApiClient.PaymentAsync(new PaymentRequest("1.1.1.1", new UserVisibleTransaction("npa", new Recipient("merchant"))));

        // Assert
        var request = _messageHandlerMock.GetFirstArgumentOfFirstInvocation<HttpMessageHandler, HttpRequestMessage>();
        var contentString = await request.Content.ReadAsStringAsync();

        JsonTests.AssertProperty(contentString, "endUserIp", "1.1.1.1");
        JsonTests.AssertSubProperty(contentString, "userVisibleTransaction", "transactionType", "npa");
        JsonTests.AssertPropertyHierarchy(contentString, "merchant", "userVisibleTransaction", "recipient", "name");
        JsonTests.AssertPropertyIsEmptyObject(contentString, "requirement");
        JsonTests.AssertOnlyProperties(contentString, new[]
        {
        "endUserIp",
        "requirement",
        "userVisibleTransaction"
        });
    }

    [Fact]
    public async Task PaymentAsync_WithEndUserIp__ShouldPostJsonPayload_WithEndUserIp_AndUserVisibleTransaction_AndNoRequirements()
    {
        // Arrange

        // Act
        await _bankIdAppApiClient.PaymentAsync(new PaymentRequest("1.1.1.1", new UserVisibleTransaction("npa", new Recipient("merchant")), null));

        // Assert
        var request = _messageHandlerMock.GetFirstArgumentOfFirstInvocation<HttpMessageHandler, HttpRequestMessage>();
        var contentString = await request.Content.ReadAsStringAsync();

        JsonTests.AssertProperty(contentString, "endUserIp", "1.1.1.1");
        JsonTests.AssertSubProperty(contentString, "userVisibleTransaction", "transactionType", "npa");
        JsonTests.AssertPropertyHierarchy(contentString, "merchant", "userVisibleTransaction", "recipient", "name");

        JsonTests.AssertPropertyIsEmptyObject(contentString, "requirement");
        JsonTests.AssertOnlyProperties(contentString, new[]
        {
        "endUserIp",
        "requirement",
        "userVisibleTransaction"
        });
    }

    [Fact]
    public async Task PaymentAsync_WithUserNonVisibleData__ShouldPostJsonPayload_WithUserNonVisibleData()
    {
        // Arrange

        // Act
        await _bankIdAppApiClient.PaymentAsync(new PaymentRequest("1.1.1.1", new UserVisibleTransaction("npa", new Recipient("merchant")), userNonVisibleData: new byte[1]));

        // Assert
        var request = _messageHandlerMock.GetFirstArgumentOfFirstInvocation<HttpMessageHandler, HttpRequestMessage>();
        var contentString = await request.Content.ReadAsStringAsync();

        JsonTests.AssertProperty(contentString, "endUserIp", "1.1.1.1");
        JsonTests.AssertPropertyIsEmptyObject(contentString, "requirement");
        JsonTests.AssertSubProperty(contentString, "userVisibleTransaction", "transactionType", "npa");
        JsonTests.AssertPropertyHierarchy(contentString, "merchant", "userVisibleTransaction", "recipient", "name");
        JsonTests.AssertProperty(contentString, "userNonVisibleData", "AA==");

        JsonTests.AssertOnlyProperties(contentString, new[]
        {
        "endUserIp",
        "requirement",
        "userNonVisibleData",
        "userVisibleTransaction",
        });
    }

    [Fact]
    public async Task PaymentAsync_WithRequirements__ShouldPostJsonPayload_WithRequirements()
    {
        // Arrange

        // Act
        await _bankIdAppApiClient.PaymentAsync(new PaymentRequest("1.1.1.1", new UserVisibleTransaction("npa", new Recipient("merchant")), new Requirement(new List<string> { "req1", "req2" }, "low", true, true, "190001010101")));

        // Assert
        var request = _messageHandlerMock.GetFirstArgumentOfFirstInvocation<HttpMessageHandler, HttpRequestMessage>();
        var contentString = await request.Content.ReadAsStringAsync();

        JsonTests.AssertProperty(contentString, "endUserIp", "1.1.1.1");
        JsonTests.AssertSubProperty(contentString, "userVisibleTransaction", "transactionType", "npa");
        JsonTests.AssertPropertyHierarchy(contentString, "merchant", "userVisibleTransaction", "recipient", "name");
        JsonTests.AssertSubProperty(contentString, "requirement", "certificatePolicies", new List<string> { "req1", "req2" });
        JsonTests.AssertSubProperty(contentString, "requirement", "pinCode", true);
        JsonTests.AssertSubProperty(contentString, "requirement", "mrtd", true);
        JsonTests.AssertSubProperty(contentString, "requirement", "personalNumber", "190001010101");
        JsonTests.AssertOnlyProperties(contentString, new[]
        {
        "endUserIp",
        "requirement",
        "userVisibleTransaction",
        });
    }

    [Fact]
    public async Task PaymentAsync_WithPaymentRequest__ShouldParseAndReturnOrderRef_AndTokens()
    {
        // Arrange
        var httpClient = GetHttpClientMockWithOkResponse("{ \"orderRef\": \"abc123\", \"autoStartToken\": \"def456\", \"qrStartSecret\": \"ghi790\", \"qrStartToken\": \"jkl123\" }");
        var bankIdClient = new BankIdAppApiClient(httpClient);

        // Act
        var result = await bankIdClient.PaymentAsync(new PaymentRequest("1.1.1.1", new UserVisibleTransaction("npa", new Recipient("merchant"))));

        // Assert
        Assert.NotNull(result);
        Assert.Equal("abc123", result.OrderRef);
        Assert.Equal("def456", result.AutoStartToken);
        Assert.Equal("ghi790", result.QrStartSecret);
        Assert.Equal("jkl123", result.QrStartToken);
    }

    [Fact]
    public async Task PaymentAsync_WithPaymentRequest__ShouldHaveWebDeviceParametersPayload()
    {
        // Arrange

        var paymentRequest = new PaymentRequest(endUserIp: "1.1.1.1",
            userVisibleTransaction: new UserVisibleTransaction("npa", new Recipient("merchant")),
            requirement: null,
            userVisibleData: "userVisibleData",
            userNonVisibleData: null,
            userVisibleDataFormat: null,
            returnUrl: null,
            returnRisk: null,
            web: new DeviceDataWeb(
                "referringDomain",
                "userAgent",
                "deviceIdentifier"),
            app: null);

        //Act
        await _bankIdAppApiClient.PaymentAsync(paymentRequest);

        //Assert
        var request = _messageHandlerMock.GetFirstArgumentOfFirstInvocation<HttpMessageHandler, HttpRequestMessage>();
        var contentString = await request.Content.ReadAsStringAsync();

        JsonTests.AssertPropertyIsNull(contentString, "app");
        JsonTests.AssertPropertyIsNotNull(contentString, "web");

        JsonTests.AssertProperties(contentString,
            new Dictionary<string, string>
            {
            { "web.referringDomain", "referringDomain" },
            { "web.userAgent", "userAgent" },
            { "web.deviceIdentifier", "deviceIdentifier" }
            });

    }

    [Fact]
    public async Task PhoneAuthAsync_WithPhoneAuthRequest__ShouldPostToBankIdPhoneAuth_WithJsonPayload()
    {
        // Arrange

        // Act
        await _bankIdAppApiClient.PhoneAuthAsync(new PhoneAuthRequest("201801012392", CallInitiator.User));

        // Assert
        Assert.Single(_messageHandlerMock.Invocations);
        var request = _messageHandlerMock.GetFirstArgumentOfFirstInvocation<HttpMessageHandler, HttpRequestMessage>();
        Assert.NotNull(request);

        Assert.Equal(HttpMethod.Post, request.Method);
        Assert.Equal(new Uri("https://bankid/phone/auth"), request.RequestUri);
        Assert.Equal(new MediaTypeHeaderValue("application/json"), request.Content.Headers.ContentType);
    }

    [Fact]
    public async Task PhoneAuthAsync_WithPersonalNumberAndCallInitiator__ShouldPostJsonPayload_WithPeronalNumberAndCallInitiator_AndRequirementAsEmptyObject()
    {
        // Arrange

        // Act
        await _bankIdAppApiClient.PhoneAuthAsync(new PhoneAuthRequest("201801012392", CallInitiator.User));

        // Assert
        var request = _messageHandlerMock.GetFirstArgumentOfFirstInvocation<HttpMessageHandler, HttpRequestMessage>();
        var contentString = await request.Content.ReadAsStringAsync();

        JsonTests.AssertProperty(contentString, "personalNumber", "201801012392");
        JsonTests.AssertProperty(contentString, "callInitiator", "user");
        JsonTests.AssertPropertyIsEmptyObject(contentString, "requirement");
        JsonTests.AssertOnlyProperties(contentString, new[]
        {
            "personalNumber",
            "callInitiator",
            "requirement"
        });
    }

    [Fact]
    public async Task PhoneAuthAsync_WithRequirements__ShouldPostJsonPayload_WithRequirements()
    {
        // Arrange

        // Act
        await _bankIdAppApiClient.PhoneAuthAsync(new PhoneAuthRequest("201801012392", CallInitiator.User,
            new PhoneRequirement(new List<string> { "req1", "req2" }, true)));

        // Assert
        var request = _messageHandlerMock.GetFirstArgumentOfFirstInvocation<HttpMessageHandler, HttpRequestMessage>();
        var contentString = await request.Content.ReadAsStringAsync();

        JsonTests.AssertProperty(contentString, "personalNumber", "201801012392");
        JsonTests.AssertProperty(contentString, "callInitiator", "user");
        JsonTests.AssertSubProperty(contentString, "requirement", "certificatePolicies", new List<string> { "req1", "req2" });
        JsonTests.AssertSubProperty(contentString, "requirement", "pinCode", true);
    }

    [Fact]
    public async Task PhoneAuthAsync_WithPhoneAuthRequest__ShouldParseAndReturnOrderRef()
    {
        // Arrange
        var httpClient = GetHttpClientMockWithOkResponse("{ \"orderRef\": \"abc123\" }");
        var bankIdClient = new BankIdAppApiClient(httpClient);

        // Act
        var result = await bankIdClient.PhoneAuthAsync(new PhoneAuthRequest("201801012392", CallInitiator.User));

        // Assert
        Assert.NotNull(result);
        Assert.Equal("abc123", result.OrderRef);
    }

    [Fact]
    public async Task PhoneAuthAsync_WithPhoneAuthRequest__ShouldHaveUserData()
    {
        //Arrange
        byte[] userNonVisibleData = Encoding.ASCII.GetBytes("Hello");
        string asBase64 = Convert.ToBase64String(userNonVisibleData);

        //Act
        await _bankIdAppApiClient.PhoneAuthAsync(new PhoneAuthRequest("201801012392", CallInitiator.User, null, "Hello",
            userNonVisibleData, "simpleMarkdownV1"));

        //Assert
        var request = _messageHandlerMock.GetFirstArgumentOfFirstInvocation<HttpMessageHandler, HttpRequestMessage>();
        var contentString = await request.Content.ReadAsStringAsync();

        JsonTests.AssertProperty(contentString, "personalNumber", "201801012392");
        JsonTests.AssertProperty(contentString, "callInitiator", "user");
        JsonTests.AssertPropertyIsEmptyObject(contentString, "requirement");
        JsonTests.AssertProperty(contentString, "userVisibleData", asBase64);
        JsonTests.AssertProperty(contentString, "userNonVisibleData", asBase64);
        JsonTests.AssertProperty(contentString, "userVisibleDataFormat", "simpleMarkdownV1");
        JsonTests.AssertOnlyProperties(contentString, new[]
        {
            "personalNumber",
            "callInitiator",
            "requirement",
            "userVisibleData",
            "userVisibleDataFormat",
            "userNonVisibleData"
        });
    }

    [Fact]
    public async Task PhoneSignAsync_WithPhoneSignRequest__ShouldPostToBankIdPhoneSign_WithJsonPayload()
    {
        // Arrange

        // Act
        await _bankIdAppApiClient.PhoneSignAsync(new PhoneSignRequest("201801012392", CallInitiator.User,
            "userVisibleData"));

        // Assert
        Assert.Single(_messageHandlerMock.Invocations);
        var request = _messageHandlerMock.GetFirstArgumentOfFirstInvocation<HttpMessageHandler, HttpRequestMessage>();
        Assert.NotNull(request);

        Assert.Equal(HttpMethod.Post, request.Method);
        Assert.Equal(new Uri("https://bankid/phone/sign"), request.RequestUri);
        Assert.Equal(new MediaTypeHeaderValue("application/json"), request.Content.Headers.ContentType);
    }

    [Fact]
    public async Task PhoneSignAsync_WithUserVisibleDataFormat__ShouldPostToBankIdPhoneSign_WithJsonPayload()
    {
        // Arrange

        // Act
        await _bankIdAppApiClient.PhoneSignAsync(new PhoneSignRequest(
            "201801012392",
            CallInitiator.User,
            "userVisibleData",
            userVisibleDataFormat: "userVisibleDataFormat",
            userNonVisibleData: new byte[1]));

        // Assert
        var request = _messageHandlerMock.GetFirstArgumentOfFirstInvocation<HttpMessageHandler, HttpRequestMessage>();
        var contentString = await request.Content.ReadAsStringAsync();

        JsonTests.AssertProperty(contentString, "personalNumber", "201801012392");
        JsonTests.AssertProperty(contentString, "callInitiator", "user");
        JsonTests.AssertPropertyIsEmptyObject(contentString, "requirement");
        JsonTests.AssertProperty(contentString, "userVisibleData", "dXNlclZpc2libGVEYXRh");
        JsonTests.AssertProperty(contentString, "userVisibleDataFormat", "userVisibleDataFormat");
        JsonTests.AssertProperty(contentString, "userNonVisibleData", "AA==");
        JsonTests.AssertOnlyProperties(contentString, new[]
        {
            "personalNumber",
            "callInitiator",
            "requirement",
            "userVisibleData",
            "userVisibleDataFormat",
            "userNonVisibleData"
        });
    }

    [Fact]
    public async Task PhoneSignAsync_WithPersonalNumberAndCallInitiator__ShouldPostJsonPayload_WithPersonalNumberAndCallInitiator_AndUserVisibleData_AndRequirementAsEmptyObject()
    {
        // Arrange

        // Act
        await _bankIdAppApiClient.PhoneSignAsync(new PhoneSignRequest("201801012392", CallInitiator.User,
            "userVisibleData"));

        // Assert
        var request = _messageHandlerMock.GetFirstArgumentOfFirstInvocation<HttpMessageHandler, HttpRequestMessage>();
        var contentString = await request.Content.ReadAsStringAsync();

        JsonTests.AssertProperty(contentString, "personalNumber", "201801012392");
        JsonTests.AssertProperty(contentString, "callInitiator", "user");
        JsonTests.AssertPropertyIsEmptyObject(contentString, "requirement");
        JsonTests.AssertProperty(contentString, "userVisibleData", "dXNlclZpc2libGVEYXRh");
        JsonTests.AssertOnlyProperties(contentString, new[]
        {
            "personalNumber",
            "callInitiator",
            "requirement",
            "userVisibleData"
        });
    }

    [Fact]
    public async Task PhoneSignAsync_WithPersonalNumberAndCallInitiator__ShouldPostJsonPayload_WithPersonalNumberAndCallInitiator_AndUserVisibleData_AndNoRequirements()
    {
        // Arrange

        // Act
        await _bankIdAppApiClient.PhoneSignAsync(new PhoneSignRequest("201801012392", CallInitiator.User,
            "userVisibleData", null));

        // Assert
        var request = _messageHandlerMock.GetFirstArgumentOfFirstInvocation<HttpMessageHandler, HttpRequestMessage>();
        var contentString = await request.Content.ReadAsStringAsync();

        JsonTests.AssertProperty(contentString, "personalNumber", "201801012392");
        JsonTests.AssertProperty(contentString, "callInitiator", "user");
        JsonTests.AssertPropertyIsEmptyObject(contentString, "requirement");
        JsonTests.AssertProperty(contentString, "userVisibleData", "dXNlclZpc2libGVEYXRh");
        JsonTests.AssertOnlyProperties(contentString, new[]
        {
            "personalNumber",
            "callInitiator",
            "requirement",
            "userVisibleData"
        });
    }

    [Fact]
    public async Task PhoneSignAsync_WithUserNonVisibleData__ShouldPostJsonPayload_WithUserNonVisibleData()
    {
        // Arrange

        // Act
        await _bankIdAppApiClient.PhoneSignAsync(new PhoneSignRequest("201801012392", CallInitiator.User,
            "userVisibleData", userNonVisibleData: new byte[1]));

        // Assert
        var request = _messageHandlerMock.GetFirstArgumentOfFirstInvocation<HttpMessageHandler, HttpRequestMessage>();
        var contentString = await request.Content.ReadAsStringAsync();

        JsonTests.AssertProperty(contentString, "personalNumber", "201801012392");
        JsonTests.AssertProperty(contentString, "callInitiator", "user");
        JsonTests.AssertPropertyIsEmptyObject(contentString, "requirement");
        JsonTests.AssertProperty(contentString, "userVisibleData", "dXNlclZpc2libGVEYXRh");
        JsonTests.AssertProperty(contentString, "userNonVisibleData", "AA==");
        JsonTests.AssertOnlyProperties(contentString, new[]
        {
            "personalNumber",
            "callInitiator",
            "requirement",
            "userVisibleData",
            "userNonVisibleData"
        });
    }

    [Fact]
    public async Task PhoneSignAsync_WithRequirements__ShouldPostJsonPayload_WithRequirements()
    {
        // Arrange

        // Act
        await _bankIdAppApiClient.PhoneSignAsync(new PhoneSignRequest("201801012392", CallInitiator.User,
            "userVisibleData", Encoding.UTF8.GetBytes("userNonVisibleData"),
            new PhoneRequirement(new List<string> { "req1", "req2" }, true)));

        // Assert
        var request = _messageHandlerMock.GetFirstArgumentOfFirstInvocation<HttpMessageHandler, HttpRequestMessage>();
        var contentString = await request.Content.ReadAsStringAsync();

        JsonTests.AssertProperty(contentString, "personalNumber", "201801012392");
        JsonTests.AssertProperty(contentString, "callInitiator", "user");
        JsonTests.AssertSubProperty(contentString, "requirement", "certificatePolicies", new List<string> { "req1", "req2" });
        JsonTests.AssertSubProperty(contentString, "requirement", "pinCode", true);
        JsonTests.AssertProperty(contentString, "userVisibleData", "dXNlclZpc2libGVEYXRh");
        JsonTests.AssertProperty(contentString, "userNonVisibleData", "dXNlck5vblZpc2libGVEYXRh");
        JsonTests.AssertOnlyProperties(contentString, new[]
        {
            "personalNumber",
            "callInitiator",
            "requirement",
            "userVisibleData",
            "userNonVisibleData"
        });
    }

    [Fact]
    public async Task PhoneSignAsync_WithPhoneSignRequest__ShouldParseAndReturnOrderRef()
    {
        // Arrange
        var httpClient = GetHttpClientMockWithOkResponse("{ \"orderRef\": \"abc123\" }");
        var bankIdClient = new BankIdAppApiClient(httpClient);

        // Act
        var result =
            await bankIdClient.PhoneSignAsync(new PhoneSignRequest("201801012392", CallInitiator.User,
                "userVisibleData"));

        // Assert
        Assert.NotNull(result);
        Assert.Equal("abc123", result.OrderRef);
    }

    [Fact]
    public async Task CollectAsync_WithCollectRequest__ShouldPostToBankIdCollect_WithJsonPayload()
    {
        // Arrange

        // Act
        await _bankIdAppApiClient.CollectAsync(new CollectRequest("abc123"));

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
        await _bankIdAppApiClient.CollectAsync(new CollectRequest("abc123"));

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
        var bankIdClient = new BankIdAppApiClient(httpClient);

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
        var bankIdClient = new BankIdAppApiClient(httpClient);

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
        var bankIdClient = new BankIdAppApiClient(httpClient);

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
        var bankIdClient = new BankIdAppApiClient(httpClient);

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
        var bankIdClient = new BankIdAppApiClient(httpClient);

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
        var bankIdClient = new BankIdAppApiClient(httpClient);

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
        var httpClient = GetHttpClientMockWithOkResponse("{ \"completionData\": { \"device\": { \"ipAddress\": \"1.1.1.1\", \"uhi\": \"OZvYM9VvyiAmG7NA5jU5zqGcVpo=\" } } }");
        var bankIdClient = new BankIdAppApiClient(httpClient);

        // Act
        var result = await bankIdClient.CollectAsync(new CollectRequest("x"));

        // Assert
        Assert.NotNull(result);
        Assert.Equal("1.1.1.1", result.CompletionData.Device.IpAddress);
        Assert.Equal("OZvYM9VvyiAmG7NA5jU5zqGcVpo=", result.CompletionData.Device.Uhi);
    }

    [Fact]
    public async Task CollectAsync_WithCollectRequest__ShouldParseAndReturnCompletionDataBankIdIssueDate()
    {
        // Arrange
        var httpClient = GetHttpClientMockWithOkResponse("{ \"completionData\": { \"bankIdIssueDate\": \"2023-01-01\" } }");
        var bankIdClient = new BankIdAppApiClient(httpClient);

        // Act
        var result = await bankIdClient.CollectAsync(new CollectRequest("x"));

        // Assert
        Assert.NotNull(result);
        Assert.Equal("2023-01-01", result.CompletionData.BankIdIssueDate);
    }

    [Fact]
    public async Task CollectAsync_WithCollectRequest__ShouldParseAndReturnCompletionDataStepUp()
    {
        // Arrange
        var httpClient = GetHttpClientMockWithOkResponse("{ \"completionData\": { \"stepUp\": { \"mrtd\": true } } }");
        var bankIdClient = new BankIdAppApiClient(httpClient);

        // Act
        var result = await bankIdClient.CollectAsync(new CollectRequest("x"));

        // Assert
        Assert.NotNull(result);
        Assert.True(result.CompletionData.StepUp.Mrtd);
    }

    [Fact]
    public async Task CancelAsync_WithCancelRequest__ShouldPostToBankIdCancel_WithJsonPayload()
    {
        // Arrange

        // Act
        await _bankIdAppApiClient.CancelAsync(new CancelRequest("x"));

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
        await _bankIdAppApiClient.CancelAsync(new CancelRequest("abc123"));

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
