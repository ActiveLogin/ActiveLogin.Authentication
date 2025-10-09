using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

using ActiveLogin.Authentication.BankId.Api;
using ActiveLogin.Authentication.BankId.AspNetCore.Areas.ActiveLogin.Models;
using ActiveLogin.Authentication.BankId.AspNetCore.DataProtection;
using ActiveLogin.Authentication.BankId.AspNetCore.Models;
using ActiveLogin.Authentication.BankId.AspNetCore.Sign;
using ActiveLogin.Authentication.BankId.AspNetCore.Test.Helpers;
using ActiveLogin.Authentication.BankId.Core;
using ActiveLogin.Authentication.BankId.Core.Launcher;

using AngleSharp.Html.Dom;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

using Moq;

using Newtonsoft.Json;

using Xunit;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Test;

using Cookie = (string Key, string Value);

public class BankId_UiSign_Tests : BankId_Ui_Tests_Base
{
    private readonly BankIdUiSignState _signState = new("configKey", new BankIdSignProperties("userVisibleData"));

    [Fact]
    public async Task BankIdUiSignController_Returns_404_If_BankId_Is_Not_Registered()
    {
        // Arrange
        var webHostBuilder = new WebHostBuilder()
            .UseSolutionRelativeContentRoot(Path.Combine("test", "ActiveLogin.Authentication.BankId.AspNetCore.Test"))
            .Configure(app => DefaultAppConfiguration(x => Task.CompletedTask))
            .ConfigureServices(services =>
            {
                services.AddMvc();
            });

        using var client = new TestServer(webHostBuilder).CreateClient();

        // Act
        var transaction = await client.GetAsync("/ActiveLogin/BankId/Sign");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, transaction.StatusCode);
    }

    [Fact]
    public async Task BankIdUiAuthApiController_Returns_404_If_BankId_Is_Not_Registered()
    {
        // Arrange
        var webHostBuilder = new WebHostBuilder()
            .UseSolutionRelativeContentRoot(Path.Combine("test", "ActiveLogin.Authentication.BankId.AspNetCore.Test"))
            .Configure(app => DefaultAppConfiguration(x => Task.CompletedTask))
            .ConfigureServices(services =>
            {
                services.AddMvc();
            });

        using var client = new TestServer(webHostBuilder).CreateClient();

        // Act
        var transaction = await client.PostAsync("/ActiveLogin/BankId/Sign/Api/Initialize", null);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, transaction.StatusCode);
    }

    [Fact]
    public async Task InitiateSign_Redirects_To_Sign()
    {
        // Arrange
        using var client = CreateServer(
            o => o.UseSimulatedEnvironment(),
            o => o.AddSameDevice(),
            DefaultAppConfiguration(async context =>
            {
                var bankIdSignService = context.RequestServices.GetRequiredService<IBankIdSignService>();
                await bankIdSignService.InitiateSignAsync(new BankIdSignProperties("UVD"), "/al-sign-cb", BankIdSignDefaults.OtherDeviceConfigKey);
            })).CreateClient();

        // Act
        var transaction = await client.GetAsync("/");

        // Assert
        Assert.Equal(HttpStatusCode.Redirect, transaction.StatusCode);
        Assert.StartsWith("/ActiveLogin/BankId/Sign", transaction.Headers.Location.OriginalString);
    }

    [Fact]
    public async Task InitiateSignAsync_Redirects_To_Sign_Without_Path_Base()
    {
        // Arrange
        using var client = CreateServer(
            o => o.UseSimulatedEnvironment(),
            o => o.AddSameDevice(),
            app =>
            {
                app.Map("/PathBase", appBuilder =>
                {
                    app.UseRouting();
                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapControllers();
                    });
                    appBuilder.Use(async (context, next) =>
                    {
                        await InitiateSign(context);
                        await next();
                    });
                    appBuilder.Run(context => context.Response.WriteAsync(""));
                });
            }).CreateClient();

        // Act
        var transaction = await client.GetAsync("/PathBase");

        // Assert
        Assert.Equal(HttpStatusCode.Redirect, transaction.StatusCode);

        var redirectUrl = transaction.Headers.Location.OriginalString;
        Assert.StartsWith("/PathBase/ActiveLogin/BankId/Sign", redirectUrl);

        var callbackUrl = UrlEncoder.Default.Encode("/al-sign-cb");
        var callbackParameter = $"returnUrl={callbackUrl}";
        Assert.Contains(callbackParameter, redirectUrl);
    }

    [Fact]
    public async Task Sign_UI_Should_Be_Accessible_Even_When_Site_Requires_Auth()
    {
        // Arrange
        using var server = CreateServer(
            o => o.UseSimulatedEnvironment(),
            o => o.AddSameDevice(),
            DefaultAppConfiguration(context => Task.CompletedTask),
            configureServices: services =>
            {
                services.AddMvc(config =>
                {
                    var policy = new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser()
                        .Build();
                    config.Filters.Add(new AuthorizeFilter(policy));
                });
            });

        // Act
        var transaction = await CreateBankIdUiRequest(server, "/ActiveLogin/BankId/Sign", SignStateKeyCookieName, _signState);

        // Assert
        Assert.Equal(HttpStatusCode.OK, transaction.StatusCode);
    }

    [Fact]
    public async Task SignInit_Returns_Ui_With_Resolved_Cancel_Url()
    {
        // Arrange
        var uiOptions = CreateUiOptions(SignStateKeyCookieName, cancelReturnUrl: "/cru", sameDevice: true);

        using var server = CreateServer(
            o => o.UseSimulatedEnvironment(),
            o => o.AddSameDevice(),
            DefaultAppConfiguration(context => Task.CompletedTask));

        // Act
        var transaction = await CreateBankIdUiRequest(server, "/ActiveLogin/BankId/Sign", SignStateKeyCookieName, _signState, uiOptions);

        // Assert
        Assert.Equal(HttpStatusCode.OK, transaction.StatusCode);

        var transactionContent = await transaction.Content.ReadAsStringAsync();
        Assert.Equal("/cru", GetInlineJsonValue(transactionContent, "cancelReturnUrl"));
    }

    [Fact]
    public async Task SignInit_Returns_Ui_With_Script()
    {
        // Arrange
        var uiOptions = CreateUiOptions(SignStateKeyCookieName, sameDevice: true);

        using var server = CreateServer(
            o => o.UseSimulatedEnvironment(),
            o => o.AddSameDevice(),
            DefaultAppConfiguration(context => Task.CompletedTask));

        // Act
        var transaction = await CreateBankIdUiRequest(server, "/ActiveLogin/BankId/Sign", SignStateKeyCookieName, _signState, uiOptions);

        // Assert
        Assert.Equal(HttpStatusCode.OK, transaction.StatusCode);

        var transactionContent = await transaction.Content.ReadAsStringAsync();
        var document = await HtmlDocumentHelper.FromContent(transactionContent);

        Assert.NotNull(document.GetElement<IHtmlDivElement>("div.activelogin-bankid-ui--wrapper"));
        Assert.NotNull(document.GetElement<IHtmlDivElement>("div.activelogin-bankid-ui--status-wrapper"));
        Assert.NotNull(document.GetElement<IHtmlImageElement>("img.activelogin-bankid-ui--qr-code-image"));

        Assert.Equal("/", GetInlineJsonValue(transactionContent, "returnUrl"));
        Assert.Equal("/", GetInlineJsonValue(transactionContent, "cancelReturnUrl"));
        Assert.NotEmpty(GetInlineJsonValue(transactionContent, "uiOptionsGuid"));
    }

    [Fact]
    public async Task SignInit_Preserves_UI_Options()
    {
        var stateKey = StateKey.New();
        // Arrange
        var uiOptions = CreateUiOptions(SignStateKeyCookieName, sameDevice: true);

        using var server = CreateServer(
            o => o.UseSimulatedEnvironment(),
            o => o.AddSameDevice(),
            DefaultAppConfiguration(context => Task.CompletedTask));

        // Use the real protector to protect the UI options
        var uiOptionsProtector = server.Services.GetRequiredService<IBankIdDataStateProtector<BankIdUiOptions>>();
        var protectedUiOptions = uiOptionsProtector.Protect(uiOptions);

        var cookies = CreateStateCookies(SignStateKeyCookieName, stateKey, _signState, server.Services);
        var encodedProtectedOptions = Uri.EscapeDataString(protectedUiOptions);
        var request = CreateRequestWithCookies(server, $"/ActiveLogin/BankId/Sign?returnUrl=%2F&uiOptions={encodedProtectedOptions}&orderRef=Y", cookies);
        var transaction = await request.GetAsync();

        // Assert
        Assert.Equal(HttpStatusCode.OK, transaction.StatusCode);

        var transactionContent = await transaction.Content.ReadAsStringAsync();
        var responseProtectedOptions = GetInlineJsonValue(transactionContent, "uiOptionsGuid");

        // Verify that protected UI options are preserved in the response
        Assert.NotEmpty(responseProtectedOptions);

        // Verify we can unprotect the response and get back the original options
        var unprotectedResponseOptions = uiOptionsProtector.Unprotect(responseProtectedOptions);
        Assert.Equal(uiOptions.SameDevice, unprotectedResponseOptions.SameDevice);
        Assert.Equal(uiOptions.StateKeyCookieName, unprotectedResponseOptions.StateKeyCookieName);
    }

    [Fact]
    public async Task SignInit_Requires_State_Cookie_To_Be_Present()
    {
        // Arrange
        var uiOptions = CreateUiOptions(SignStateKeyCookieName, sameDevice: true);

        using var server = CreateServer(
            o => o.UseSimulatedEnvironment(),
            o => o.AddSameDevice(),
            DefaultAppConfiguration(context => Task.CompletedTask));

        // Use the real protector to protect the UI options
        var uiOptionsProtector = server.Services.GetRequiredService<IBankIdDataStateProtector<BankIdUiOptions>>();
        var protectedUiOptions = uiOptionsProtector.Protect(uiOptions);

        // Act - Make request with valid UI options but NO state cookies
        var encodedProtectedOptions = Uri.EscapeDataString(protectedUiOptions);
        var request = server.CreateRequest($"/ActiveLogin/BankId/Sign?returnUrl=%2F&uiOptions={encodedProtectedOptions}&orderRef=Y");
        request.AddHeader("Cookie", ""); // Explicitly set empty cookies to ensure no state cookie is present
        var transaction = await request.GetAsync();

        // Assert - Should redirect to cancel URL (/) when state cookie is missing
        Assert.Equal(HttpStatusCode.Redirect, transaction.StatusCode);
        Assert.Equal("/", transaction.Headers.Location.ToString());
    }

    [Fact]
    public async Task AutoLaunch_Sets_Correct_RedirectUri()
    {
        // Arrange - This test focuses on redirect URI generation logic, so we mock the data protection
        // to isolate the test from data protection key management complexity
        var autoLaunchOptions = new BankIdUiOptions([], true, false, false, false, string.Empty, SignStateKeyCookieName, Api.Models.CardReader.class1);
        var mockProtector = new Mock<IBankIdDataStateProtector<BankIdUiOptions>>();
        mockProtector
            .Setup(protector => protector.Unprotect(It.IsAny<string>()))
            .Returns(autoLaunchOptions);

        mockProtector
            .Setup(protector => protector.Protect(It.IsAny<BankIdUiOptions>()))
            .Returns("TestProtectedOptions");

        using var server = CreateServer(
            o =>
            {
                o.UseSimulatedEnvironment();
                o.Services.AddTransient<IBankIdLauncher, TestBankIdLauncher>();
            },
            o => o.AddSameDevice(),
            DefaultAppConfiguration(InitiateSign),
            configureServices: services =>
            {
                services.AddTransient(s => mockProtector.Object);
            });

        var stateKey = StateKey.New();
        var stateCookies = CreateStateCookies(SignStateKeyCookieName, stateKey, _signState, server.Services);

        // Arrange acting request
        var testReturnUrl = "/TestReturnUrl";
        var testOptions = "TestProtectedOptions";
        var initializeRequestBody = new { returnUrl = testReturnUrl, uiOptions = testOptions };

        // Act
        var initializeTransaction = await GetInitializeResponse(server, initializeRequestBody, stateCookies);

        // Assert
        Assert.Equal(HttpStatusCode.OK, initializeTransaction.StatusCode);

        var responseContent = await initializeTransaction.Content.ReadAsStringAsync();
        var responseObject = JsonConvert.DeserializeAnonymousType(responseContent, new { RedirectUri = "", OrderRef = "", IsAutoLaunch = false });
        Assert.True(responseObject.IsAutoLaunch);

        var encodedReturnParam = UrlEncoder.Default.Encode(testReturnUrl);
        var expectedUrl = $"http://localhost/ActiveLogin/BankId/Sign?returnUrl={encodedReturnParam}&uiOptions={testOptions}";
        Assert.Equal(expectedUrl, responseObject.RedirectUri);
    }

    [Fact]
    public async Task Api_Always_Returns_CamelCase_Json_For_Http200Ok()
    {
        // Arrange - This test focuses on JSON serialization behavior, so we mock the data protection
        // to isolate the test from data protection complexity and focus on camelCase formatting
        var autoLaunchOptions = new BankIdUiOptions([], false, false, false, false, string.Empty, SignStateKeyCookieName, Api.Models.CardReader.class1);

        var mockProtector = new Mock<IBankIdDataStateProtector<BankIdUiOptions>>();
        mockProtector
            .Setup(protector => protector.Unprotect(It.IsAny<string>()))
            .Returns(autoLaunchOptions);

        mockProtector
            .Setup(protector => protector.Protect(It.IsAny<BankIdUiOptions>()))
            .Returns("TestProtectedOptions");

        var stateKey = StateKey.New();

        using var server = CreateServer(
            o =>
            {
                o.UseSimulatedEnvironment();
                o.Services.AddTransient<IBankIdLauncher, TestBankIdLauncher>();
            },
            o => o.AddSameDevice(),
            DefaultAppConfiguration(InitiateSign),
            configureServices: services =>
            {
                services.AddTransient(s => mockProtector.Object);
                services.AddMvc().AddJsonOptions(configure =>
                {
                    // Deliberately disable camelCase to test that the API overrides this setting
                    configure.JsonSerializerOptions.PropertyNamingPolicy = null;
                });
            });

        // Create state cookies for the sign state
        var stateCookies = CreateStateCookies(SignStateKeyCookieName, stateKey, _signState, server.Services);

        // Arrange acting request
        var testReturnUrl = "/TestReturnUrl";
        var testOptions = "TestProtectedOptions";
        var initializeRequestBody = new { returnUrl = testReturnUrl, uiOptions = testOptions };

        // Act
        var initializeTransaction = await GetInitializeResponse(server, initializeRequestBody, stateCookies);

        // Assert
        Assert.Equal(HttpStatusCode.OK, initializeTransaction.StatusCode);

        var responseContent = await initializeTransaction.Content.ReadAsStringAsync();
        // Verify that API returns camelCase JSON even when global setting disables it
        Assert.Contains("redirectUri", responseContent);
        Assert.Contains("orderRef", responseContent);
        Assert.Contains("isAutoLaunch", responseContent);
    }

    private static async Task InitiateSign(HttpContext context)
    {
        var bankIdSignService = context.RequestServices.GetRequiredService<IBankIdSignService>();
        await bankIdSignService.InitiateSignAsync(new BankIdSignProperties("UVD"), "/al-sign-cb", BankIdSignDefaults.OtherDeviceConfigKey);
    }

    [Fact]
    public async Task Api_Always_Returns_CamelCase_Json_For_Http400BadRequest()
    {
        // Arrange mocks
        var autoLaunchOptions = new BankIdUiOptions([], false, false, false, false, string.Empty, SignStateKeyCookieName, Api.Models.CardReader.class1);
        var mockProtector = new Mock<IBankIdDataStateProtector<BankIdUiOptions>>();
        mockProtector
            .Setup(protector => protector.Unprotect(It.IsAny<string>()))
            .Returns(autoLaunchOptions);

        mockProtector
            .Setup(protector => protector.Protect(It.IsAny<BankIdUiOptions>()))
            .Returns("MockedProtectedOptions");

        var stateKey = StateKey.New();

        using var server = CreateServer(
            o =>
            {
                o.UseSimulatedEnvironment();
                o.Services.AddTransient<IBankIdLauncher, TestBankIdLauncher>();
            },
            o => o.AddSameDevice(),
            DefaultAppConfiguration(InitiateSign),
            configureServices: services =>
            {
                services.AddTransient(s => mockProtector.Object);
            });

        // Create state cookies for the sign state
        var stateCookies = CreateStateCookies(SignStateKeyCookieName, stateKey, _signState, server.Services);

        // Arrange acting request
        var initializeRequestBody = new { };

        //Act
        var initializeTransaction = await GetInitializeResponse(server, initializeRequestBody, stateCookies);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, initializeTransaction.StatusCode);

        var responseContent = await initializeTransaction.Content.ReadAsStringAsync();
        Assert.Contains("title", responseContent);
        Assert.Contains("type", responseContent);
        Assert.Contains("errors", responseContent);
    }

    [Fact]
    public async Task Cancel_Calls_CancelApi()
    {
        // Arrange - This test focuses on verifying that the Cancel API endpoint calls the underlying BankID API
        // We mock the data protection components to isolate the test from data protection complexity
        var uiOptions = new BankIdUiOptions([], false, false, false, false, string.Empty, SignStateKeyCookieName, Api.Models.CardReader.class1);
        var mockOptionsProtector = new Mock<IBankIdDataStateProtector<BankIdUiOptions>>();
        mockOptionsProtector
            .Setup(protector => protector.Unprotect(It.IsAny<string>()))
            .Returns(uiOptions);

        mockOptionsProtector
            .Setup(protector => protector.Protect(It.IsAny<BankIdUiOptions>()))
            .Returns("TestProtectedOptions");

        var mockOrderRefProtector = new Mock<IBankIdDataStateProtector<BankIdUiOrderRef>>();
        mockOrderRefProtector
            .Setup(protector => protector.Unprotect(It.IsAny<string>()))
            .Returns(new BankIdUiOrderRef("TestOrderRef"));

        mockOrderRefProtector
            .Setup(protector => protector.Protect(It.IsAny<BankIdUiOrderRef>()))
            .Returns("TestProtectedOrderRef");

        // Use a test API client that tracks whether CancelAsync was called
        var testBankIdApi = new TestBankIdAppApi(new BankIdSimulatedAppApiClient());

        var stateKey = StateKey.New();

        using var server = CreateServer(
            o =>
            {
                o.UseSimulatedEnvironment();
                o.Services.AddTransient<IBankIdLauncher, TestBankIdLauncher>();
            },
            o => o.AddSameDevice(),
            DefaultAppConfiguration(InitiateSign),
            configureServices: services =>
            {
                services.AddTransient(s => mockOptionsProtector.Object);
                services.AddTransient(s => mockOrderRefProtector.Object);
                services.AddSingleton<IBankIdAppApiClient>(s => testBankIdApi);
            });

        // Create state cookies for the sign state
        var stateCookies = CreateStateCookies(SignStateKeyCookieName, stateKey, _signState, server.Services);

        // Act - Make a cancel request to the API
        var cancelRequest = new JsonContent(new
        {
            orderRef = "TestOrderRef",
            uiOptions = "TestProtectedOptions",
            cancelReturnUrl = "/"
        });

        var cancelTransaction = await MakeRequestWithRequiredContext("Sign", "/ActiveLogin/BankId/Sign/Api/Cancel", server, cancelRequest, stateCookies);

        // Assert - Verify that the API was called and the cancel method was invoked
        Assert.Equal(HttpStatusCode.OK, cancelTransaction.StatusCode);
        Assert.True(testBankIdApi.CancelAsyncIsCalled);
    }

    private static TestServer CreateServer(
        Action<IBankIdBuilder> configureBankId,
        Action<IBankIdSignBuilder> configureBankIdSign,
        Action<IApplicationBuilder> configureApplication = null,
        Action<IServiceCollection> configureServices = null)
    {
        var webHostBuilder = new WebHostBuilder()
            .UseSolutionRelativeContentRoot(Path.Combine("test", "ActiveLogin.Authentication.BankId.AspNetCore.Test"))
            .Configure(app =>
            {
                configureApplication?.Invoke(app);
            })
            .ConfigureServices(services =>
            {
                services.AddBankId(configureBankId);
                services.AddAuthentication()
                    .AddCookie();
                services.AddBankIdSign(configureBankIdSign);
                services.AddMvc();
                configureServices?.Invoke(services);
            });

        return new TestServer(webHostBuilder);
    }

    private static Action<IApplicationBuilder> DefaultAppConfiguration(Func<HttpContext, Task> testpath)
    {
        return app =>
        {
            app.UseMiddleware<FakeRemoteIpAddressMiddleware>(IPAddress.Parse("192.0.2.1"));
            app.UseMiddleware<FakeUserAgentMiddleware>(FakeUserAgentMiddleware.DefaultUserAgent);
            app.UseMiddleware<FakeReferrerMiddleware>("http://localhost");
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.Use(async (context, next) =>
            {
                await testpath(context);
                await next();
            });
            app.Run(context => context.Response.WriteAsync(""));
        };
    }
    private Task<HttpResponseMessage> GetInitializeResponse(TestServer server, object initializeRequestBody, params Cookie[] cookies)
    {
        return GetInitializeResponse("Sign", server, initializeRequestBody, cookies);
    }
}
