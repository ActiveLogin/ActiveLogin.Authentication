using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

using ActiveLogin.Authentication.BankId.Api;
using ActiveLogin.Authentication.BankId.AspNetCore.Areas.ActiveLogin.Models;
using ActiveLogin.Authentication.BankId.AspNetCore.Auth;
using ActiveLogin.Authentication.BankId.AspNetCore.DataProtection;
using ActiveLogin.Authentication.BankId.AspNetCore.Models;
using ActiveLogin.Authentication.BankId.AspNetCore.Test.Helpers;
using ActiveLogin.Authentication.BankId.Core;
using ActiveLogin.Authentication.BankId.Core.Launcher;

using AngleSharp.Html.Dom;

using Microsoft.AspNetCore.Authentication;
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

public class BankId_UiAuth_Tests : BankId_Ui_Tests_Base
{

    [Fact]
    public async Task BankIdUiAuthController_Returns_404_If_BankId_Is_Not_Registered()
    {
        // Arrange
        var webHostBuilder = new WebHostBuilder()
            .UseSolutionRelativeContentRoot(Path.Combine("test", "ActiveLogin.Authentication.BankId.AspNetCore.Test"))
            .Configure(app => DefaultAppConfiguration(x => Task.CompletedTask))
            .ConfigureServices(services =>
            {
                services.AddAuthentication();
                services.AddMvc();
            });
        using var client = new TestServer(webHostBuilder).CreateClient();

        // Act
        var transaction = await client.GetAsync("/ActiveLogin/BankId/Auth");

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
                services.AddAuthentication();
                services.AddMvc();
            });
        using var client = new TestServer(webHostBuilder).CreateClient();

        // Act
        var transaction = await client.PostAsync("/ActiveLogin/BankId/Auth/Api/Initialize", null);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, transaction.StatusCode);
    }

    [Fact]
    public async Task Challenge_Redirects_To_Login()
    {
        // Arrange
        using var client = CreateServer(o =>
            {
                o.UseSimulatedEnvironment();
            },
            o =>
            {
                o.AddSameDevice();
            },
            DefaultAppConfiguration(async context =>
            {
                await context.ChallengeAsync(BankIdAuthDefaults.SameDeviceAuthenticationScheme);
            })).CreateClient();

        // Act
        var transaction = await client.GetAsync("/");

        // Assert
        Assert.Equal(HttpStatusCode.Redirect, transaction.StatusCode);
        Assert.StartsWith("/ActiveLogin/BankId/Auth", transaction.Headers.Location.OriginalString);
    }

    [Fact]
    public async Task Challenge_Redirects_To_Login_With_Path_Base()
    {
        // Arrange
        using var client = CreateServer(o =>
            {
                o.UseSimulatedEnvironment();
            },
            o =>
            {
                o.AddSameDevice();
            },
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
                        await context.ChallengeAsync(
                            BankIdAuthDefaults.SameDeviceAuthenticationScheme);
                        await next();
                    });
                    appBuilder.Run(context => context.Response.WriteAsync(""));
                });
            }
            ,
            services =>
            {
                services.AddTransient<RemoteAuthenticationOptions, BankIdAuthOptions>();

            }).CreateClient();

        // Act
        var transaction = await client.GetAsync("/PathBase");

        // Assert
        Assert.Equal(HttpStatusCode.Redirect, transaction.StatusCode);

        var redirectUrl = transaction.Headers.Location.OriginalString;
        Assert.StartsWith("/PathBase/ActiveLogin/BankId/Auth", redirectUrl);

        var callbackUrl = UrlEncoder.Default.Encode("/PathBase/al-bankid-auth-samedevice-callback");
        var callbackParameter = $"returnUrl={callbackUrl}";
        Assert.Contains(callbackParameter, redirectUrl);
    }

    [Fact]
    public async Task Authentication_UI_Should_Be_Accessible_Even_When_Site_Requires_Auth()
    {
        // Arrange
        using var server = CreateServer(o =>
            {
                o.UseSimulatedEnvironment();
            },
            o =>
            {
                o.AddSameDevice();
            },
            DefaultAppConfiguration(context => Task.CompletedTask),
            services =>
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
        var transaction = await CreateBankIdUiRequest(server, "/ActiveLogin/BankId/Auth", AuthStateKeyCookieName, new BankIdUiAuthState(new AuthenticationProperties()));

        // Assert
        Assert.Equal(HttpStatusCode.OK, transaction.StatusCode);
    }

    [Fact]
    public async Task Init_Returns_Ui_With_Resolved_Cancel_Url()
    {
        // Arrange
        var uiOptions = CreateUiOptions(AuthStateKeyCookieName, cancelReturnUrl: "~/cru", sameDevice: true);
        using var server = CreateServer(o =>
            {
                o.UseSimulatedEnvironment();
            },
            o =>
            {
                o.AddSameDevice();
            },
            DefaultAppConfiguration(async context =>
            {
                await context.ChallengeAsync(BankIdAuthDefaults.SameDeviceAuthenticationScheme);
            }));

        // Act
        var transaction = await CreateBankIdUiRequest(server, "/ActiveLogin/BankId/Auth", AuthStateKeyCookieName, new BankIdUiAuthState(new AuthenticationProperties()), uiOptions);

        // Assert
        Assert.Equal(HttpStatusCode.OK, transaction.StatusCode);

        var transactionContent = await transaction.Content.ReadAsStringAsync();
        Assert.Equal("/cru", GetInlineJsonValue(transactionContent, "cancelReturnUrl"));
    }

    [Fact]
    public async Task Init_Returns_Ui_With_Script()
    {
        // Arrange
        using var server = CreateServer(o =>
            {
                o.UseSimulatedEnvironment();
            },
            o =>
            {
                o.AddSameDevice();
            },
            DefaultAppConfiguration(async context =>
            {
                await context.ChallengeAsync(BankIdAuthDefaults.SameDeviceAuthenticationScheme);
            }));

        // Act
        var transaction = await CreateBankIdUiRequest(server, "/ActiveLogin/BankId/Auth", AuthStateKeyCookieName, new BankIdUiAuthState(new AuthenticationProperties()));

        // Assert
        Assert.Equal(HttpStatusCode.OK, transaction.StatusCode);

        var transactionContent = await transaction.Content.ReadAsStringAsync();
        var document = await HtmlDocumentHelper.FromContent(transactionContent);

        Assert.NotNull(document.GetElement<IHtmlDivElement>("div.activelogin-bankid-ui--wrapper"));
        Assert.NotNull(document.GetElement<IHtmlDivElement>("div.activelogin-bankid-ui--status-wrapper"));
        Assert.NotNull(document.GetElement<IHtmlImageElement>("img.activelogin-bankid-ui--qr-code-image"));

        Assert.Equal("/", GetInlineJsonValue(transactionContent, "returnUrl"));
        Assert.Equal("/", GetInlineJsonValue(transactionContent, "cancelReturnUrl"));
        Assert.NotEmpty(GetInlineJsonValue(transactionContent, "uiOptionsGuid")); // Verify real protected options are present
    }

    [Fact]
    public async Task Init_Preserves_UI_Options()
    {
        // Arrange
        using var server = CreateServer(o =>
            {
                o.UseSimulatedEnvironment();
            },
            o =>
            {
                o.AddSameDevice();
            },
            DefaultAppConfiguration(async context =>
            {
                await context.ChallengeAsync(BankIdAuthDefaults.SameDeviceAuthenticationScheme);
            }));

        // Act
        var transaction = await CreateBankIdUiRequest(server, "/ActiveLogin/BankId/Auth", AuthStateKeyCookieName, new BankIdUiAuthState(new AuthenticationProperties()));

        // Assert
        Assert.Equal(HttpStatusCode.OK, transaction.StatusCode);

        var transactionContent = await transaction.Content.ReadAsStringAsync();

        Assert.NotEmpty(GetInlineJsonValue(transactionContent, "uiOptionsGuid")); // Verify real protected options are present
    }

    [Fact]
    public async Task Init_Requires_State_Cookie_To_Be_Present()
    {
        // Arrange
        var uiOptions = CreateUiOptions(AuthStateKeyCookieName, sameDevice: true);

        // Setup mock protector to handle "dummy" string
        var mockProtector = new Mock<IBankIdDataStateProtector<BankIdUiOptions>>();
        mockProtector
            .Setup(protector => protector.Unprotect("dummy"))
            .Returns(uiOptions);
        mockProtector
            .Setup(protector => protector.Protect(It.IsAny<BankIdUiOptions>()))
            .Returns("dummy");

        using var server = CreateServer(o =>
            {
                o.UseSimulatedEnvironment();
            },
            o =>
            {
                o.AddSameDevice();
            },
            DefaultAppConfiguration(async context =>
            {
                await context.ChallengeAsync(BankIdAuthDefaults.SameDeviceAuthenticationScheme);
            }),
            services =>
            {
                services.AddTransient(s => mockProtector.Object);
            });

        // Act - Try to access UI without state cookie (should redirect)
        var request = server.CreateRequest("/ActiveLogin/BankId/Auth?returnUrl=%2F&uiOptions=dummy&orderRef=Y");
        request.AddHeader("Cookie", "");
        var transaction = await request.GetAsync();

        // Assert
        Assert.Equal(HttpStatusCode.Redirect, transaction.StatusCode);
        Assert.Equal("/", transaction.Headers.Location.ToString());
    }

    [Fact]
    public async Task AutoLaunch_Sets_Correct_RedirectUri()
    {
        // Arrange
        var stateKey = StateKey.New();
        var autoLaunchOptions = CreateUiOptions(AuthStateKeyCookieName, cancelReturnUrl: string.Empty, sameDevice: true);

        // Setup mock protector to handle "TestOptions" and "X" strings
        var mockProtector = new Mock<IBankIdDataStateProtector<BankIdUiOptions>>();
        mockProtector
            .Setup(protector => protector.Unprotect("TestOptions"))
            .Returns(autoLaunchOptions);
        mockProtector
            .Setup(protector => protector.Unprotect("X"))
            .Returns(autoLaunchOptions);
        mockProtector
            .Setup(protector => protector.Protect(It.IsAny<BankIdUiOptions>()))
            .Returns("TestOptions");

        // Setup mock order ref protector for "Y" string
        var mockOrderRefProtector = new Mock<IBankIdDataStateProtector<BankIdUiOrderRef>>();
        mockOrderRefProtector
            .Setup(protector => protector.Unprotect(It.IsAny<string>()))
            .Returns(new BankIdUiOrderRef("ANY"));
        mockOrderRefProtector
            .Setup(protector => protector.Protect(It.IsAny<BankIdUiOrderRef>()))
            .Returns("Y");

        using var server = CreateServer(
            o =>
            {
                o.UseSimulatedEnvironment();
                o.Services.AddTransient<IBankIdLauncher, TestBankIdLauncher>();
            },
            o =>
            {
                o.AddSameDevice();
            },
            DefaultAppConfiguration(async context =>
            {
                await context.ChallengeAsync(BankIdAuthDefaults.SameDeviceAuthenticationScheme);
            }),
            services =>
            {
                services.AddTransient(s => mockProtector.Object);
                services.AddTransient(s => mockOrderRefProtector.Object);
            });

        // Arrange acting request
        var testReturnUrl = "/TestReturnUrl";
        var testOptions = "TestOptions";
        var initializeRequestBody = new { returnUrl = testReturnUrl, uiOptions = testOptions };
        var stateCookies = CreateStateCookies(AuthStateKeyCookieName, stateKey, new BankIdUiAuthState(new AuthenticationProperties()), server.Services);

        // Act
        var initializeTransaction = await GetInitializeResponse("Auth", server, initializeRequestBody, stateCookies);

        // Assert
        Assert.Equal(HttpStatusCode.OK, initializeTransaction.StatusCode);

        var responseContent = await initializeTransaction.Content.ReadAsStringAsync();
        var responseObject = JsonConvert.DeserializeAnonymousType(responseContent,
            new { RedirectUri = "", OrderRef = "", IsAutoLaunch = false });
        Assert.True(responseObject.IsAutoLaunch);

        var encodedReturnParam = UrlEncoder.Default.Encode(testReturnUrl);
        var expectedUrl = $"http://localhost/ActiveLogin/BankId/Auth?returnUrl={encodedReturnParam}&uiOptions={testOptions}";
        Assert.Equal(expectedUrl, responseObject.RedirectUri);
    }

    [Fact]
    public async Task Api_Always_Returns_CamelCase_Json_For_Http200Ok()
    {
        // Arrange
        var stateKey = StateKey.New();
        var autoLaunchOptions = CreateUiOptions(AuthStateKeyCookieName, cancelReturnUrl: string.Empty, sameDevice: false);

        // Setup mock protector to handle "TestOptions" and "X" strings
        var mockProtector = new Mock<IBankIdDataStateProtector<BankIdUiOptions>>();
        mockProtector
            .Setup(protector => protector.Unprotect("TestOptions"))
            .Returns(autoLaunchOptions);
        mockProtector
            .Setup(protector => protector.Unprotect("X"))
            .Returns(autoLaunchOptions);
        mockProtector
            .Setup(protector => protector.Protect(It.IsAny<BankIdUiOptions>()))
            .Returns("TestOptions");

        // Setup mock order ref protector for "Y" string
        var mockOrderRefProtector = new Mock<IBankIdDataStateProtector<BankIdUiOrderRef>>();
        mockOrderRefProtector
            .Setup(protector => protector.Unprotect(It.IsAny<string>()))
            .Returns(new BankIdUiOrderRef("ANY"));
        mockOrderRefProtector
            .Setup(protector => protector.Protect(It.IsAny<BankIdUiOrderRef>()))
            .Returns("Y");

        using var server = CreateServer(
            o =>
            {
                o.UseSimulatedEnvironment();
                o.Services.AddTransient<IBankIdLauncher, TestBankIdLauncher>();
            },
            o =>
            {
                o.AddSameDevice();
            },
            DefaultAppConfiguration(async context =>
            {
                await context.ChallengeAsync(BankIdAuthDefaults.SameDeviceAuthenticationScheme);
            }),
            services =>
            {
                services.AddTransient(s => mockProtector.Object);
                services.AddTransient(s => mockOrderRefProtector.Object);
                services.AddMvc().AddJsonOptions(configure =>
                {
                    configure.JsonSerializerOptions.PropertyNamingPolicy = null;
                });
            });

        // Arrange acting request
        var testReturnUrl = "/TestReturnUrl";
        var testOptions = "TestOptions";
        var initializeRequestBody = new { returnUrl = testReturnUrl, uiOptions = testOptions };
        var stateCookies = CreateStateCookies(AuthStateKeyCookieName, stateKey, new BankIdUiAuthState(new AuthenticationProperties()), server.Services);

        //Act
        var initializeTransaction = await GetInitializeResponse("Auth", server, initializeRequestBody, stateCookies);

        // Assert
        Assert.Equal(HttpStatusCode.OK, initializeTransaction.StatusCode);

        var responseContent = await initializeTransaction.Content.ReadAsStringAsync();
        Assert.Contains("redirectUri", responseContent);
        Assert.Contains("orderRef", responseContent);
        Assert.Contains("isAutoLaunch", responseContent);
    }

    [Fact]
    public async Task Api_Always_Returns_CamelCase_Json_For_Http400BadRequest()
    {
        // Arrange
        var stateKey = StateKey.New();
        var autoLaunchOptions = CreateUiOptions(AuthStateKeyCookieName, cancelReturnUrl: string.Empty, sameDevice: false);

        // Setup mock protector to handle both "X" for UI call and return "TestOptions"
        var mockProtector = new Mock<IBankIdDataStateProtector<BankIdUiOptions>>();
        mockProtector
            .Setup(protector => protector.Unprotect("X"))
            .Returns(autoLaunchOptions);
        mockProtector
            .Setup(protector => protector.Protect(It.IsAny<BankIdUiOptions>()))
            .Returns("TestOptions");

        // Setup mock order ref protector for "Y" string
        var mockOrderRefProtector = new Mock<IBankIdDataStateProtector<BankIdUiOrderRef>>();
        mockOrderRefProtector
            .Setup(protector => protector.Unprotect(It.IsAny<string>()))
            .Returns(new BankIdUiOrderRef("ANY"));
        mockOrderRefProtector
            .Setup(protector => protector.Protect(It.IsAny<BankIdUiOrderRef>()))
            .Returns("Y");

        using var server = CreateServer(
            o =>
            {
                o.UseSimulatedEnvironment();
                o.Services.AddTransient<IBankIdLauncher, TestBankIdLauncher>();
            },
            o =>
            {
                o.AddSameDevice();
            },
            DefaultAppConfiguration(async context =>
            {
                await context.ChallengeAsync(BankIdAuthDefaults.SameDeviceAuthenticationScheme);
            }),
            services =>
            {
                services.AddTransient(s => mockProtector.Object);
                services.AddTransient(s => mockOrderRefProtector.Object);
            });

        // Arrange acting request
        var initializeRequestBody = new { };
        var stateCookies = CreateStateCookies(AuthStateKeyCookieName, stateKey, new BankIdUiAuthState(new AuthenticationProperties()), server.Services);

        //Act
        var initializeTransaction = await GetInitializeResponse("Auth", server, initializeRequestBody, stateCookies);

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
        // Arrange
        var autoLaunchOptions = CreateUiOptions(AuthStateKeyCookieName, cancelReturnUrl: string.Empty, sameDevice: false);

        // Setup mock protectors
        var mockOptionsProtector = new Mock<IBankIdDataStateProtector<BankIdUiOptions>>();
        mockOptionsProtector
            .Setup(protector => protector.Unprotect(It.IsAny<string>()))
            .Returns(autoLaunchOptions);
        mockOptionsProtector
            .Setup(protector => protector.Protect(It.IsAny<BankIdUiOptions>()))
            .Returns("TestOptions");
        var mockOrderRefProtector = new Mock<IBankIdDataStateProtector<BankIdUiOrderRef>>();
        mockOrderRefProtector
            .Setup(protector => protector.Unprotect(It.IsAny<string>()))
            .Returns(new BankIdUiOrderRef("ANY"));
        mockOrderRefProtector
            .Setup(protector => protector.Protect(It.IsAny<BankIdUiOrderRef>()))
            .Returns("MockedProtectedOrderRef");

        var testBankIdApi = new TestBankIdAppApi(new BankIdSimulatedAppApiClient());
        var stateKey = StateKey.New();

        using var server = CreateServer(
            o =>
            {
                o.UseSimulatedEnvironment();
                o.Services.AddTransient<IBankIdLauncher, TestBankIdLauncher>();
            },
            o =>
            {
                o.AddSameDevice();
            },
            DefaultAppConfiguration(async context =>
            {
                await context.ChallengeAsync(BankIdAuthDefaults.SameDeviceAuthenticationScheme);
            }),
            services =>
            {
                services.AddSingleton(s => mockOptionsProtector.Object);
                services.AddSingleton(s => mockOrderRefProtector.Object);
                services.AddSingleton<IBankIdAppApiClient>(s => testBankIdApi);
            });

        // Arrange csrf info using helper
        var loginResponse = await CreateBankIdUiRequest(server, "/ActiveLogin/BankId/Auth", AuthStateKeyCookieName, new BankIdUiAuthState(new AuthenticationProperties()));
        var loginCookies = loginResponse.Headers.GetValues("set-cookie");
        var loginContent = await loginResponse.Content.ReadAsStringAsync();
        var csrfToken = GetRequestVerificationToken(loginContent);

        // Arrange acting request
        var testReturnUrl = "/TestReturnUrl";
        var testOptions = "TestOptions";
        var initializeRequest = new JsonContent(new { returnUrl = testReturnUrl, uiOptions = testOptions });
        initializeRequest.Headers.Add("Cookie", loginCookies);
        initializeRequest.Headers.Add("RequestVerificationToken", csrfToken);

        // Act
        var client = server.CreateClient();
        var initializeTransaction = await client.PostAsync("/ActiveLogin/BankId/Auth/Api/Initialize", initializeRequest);
        var initializeResponseContent = await initializeTransaction.Content.ReadAsStringAsync();
        var initializeObject = JsonConvert.DeserializeAnonymousType(initializeResponseContent, new { RedirectUri = "", OrderRef = "", IsAutoLaunch = false });

        var cancelRequest = new JsonContent(new
        {
            orderRef = initializeObject.OrderRef,
            uiOptions = "TestOptions",
            cancelReturnUrl = "/"
        });
        cancelRequest.Headers.Add("Cookie", loginCookies);
        cancelRequest.Headers.Add("RequestVerificationToken", csrfToken);

        // Act
        var cancelTransaction = await client.PostAsync("/ActiveLogin/BankId/Auth/Api/Cancel", cancelRequest);

        // Assert
        Assert.Equal(HttpStatusCode.OK, cancelTransaction.StatusCode);
        Assert.True(testBankIdApi.CancelAsyncIsCalled);
    }

    private TestServer CreateServer(
        Action<IBankIdBuilder> configureBankId,
        Action<IBankIdAuthBuilder> configureBankIdAuth,
        Action<IApplicationBuilder> configureApplication,
        Action<IServiceCollection> configureServices = null)
    {
        var webHostBuilder = new WebHostBuilder()
            .UseSolutionRelativeContentRoot(Path.Combine("test", "ActiveLogin.Authentication.BankId.AspNetCore.Test"))
            .Configure(app =>
            {
                configureApplication.Invoke(app);
            })
            .ConfigureServices(services =>
            {
                services.AddBankId(configureBankId);
                services.AddAuthentication()
                    .AddCookie()
                    .AddBankIdAuth(configureBankIdAuth);
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
            app.UseMiddleware<FakeReferrerMiddleware>("https://localhost:3000");
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

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

    private Task<HttpResponseMessage> GetInitializeResponse(TestServer server, object initializeRequestBody, params (string, string)[] cookies)
    {
        return GetInitializeResponse("Auth", server, initializeRequestBody, cookies);
    }
}
