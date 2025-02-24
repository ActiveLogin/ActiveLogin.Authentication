using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

using ActiveLogin.Authentication.BankId.Api;
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
    private readonly Mock<IBankIdDataStateProtector<BankIdUiOptions>> _bankIdUiOptionsProtector;

    public BankId_UiAuth_Tests()
    {
        _bankIdUiOptionsProtector = new Mock<IBankIdDataStateProtector<BankIdUiOptions>>();
        _bankIdUiOptionsProtector
            .Setup(protector => protector.Unprotect(It.IsAny<string>()))
            .Returns(new BankIdUiOptions([], false, false, false, false, "/", AuthStateKeyCookieName, Api.Models.CardReader.class1));
        _bankIdUiOptionsProtector
            .Setup(protector => protector.Protect(It.IsAny<BankIdUiOptions>()))
            .Returns("Ignored");
    }

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
        var (stateStorage, stateKey) = await SetupStateStorage(new BankIdUiAuthState(new AuthenticationProperties()));
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

                services.AddSingleton(stateStorage);
                services.AddTransient(s => _bankIdUiOptionsProtector.Object);
            });

        // Act
        var request = CreateRequestWithCookies(server, "/ActiveLogin/BankId/Auth?returnUrl=%2F&uiOptions=X&orderRef=Y", (AuthStateKeyCookieName, stateKey));
        var transaction = await request.GetAsync();


        // Assert
        Assert.Equal(HttpStatusCode.OK, transaction.StatusCode);
    }

    [Fact]
    public async Task Init_Returns_Ui_With_Resolved_Cancel_Url()
    {
        // Arrange
        var options = new BankIdUiOptions([], true, false, false, false, "~/cru", AuthStateKeyCookieName, Api.Models.CardReader.class1);
        _bankIdUiOptionsProtector
            .Setup(protector => protector.Unprotect(It.IsAny<string>()))
            .Returns(options);

        var (stateStorage, stateKey) = await SetupStateStorage(new BankIdUiAuthState(new AuthenticationProperties()));

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
                services.AddSingleton(stateStorage);
                services.AddTransient(s => _bankIdUiOptionsProtector.Object);
            });

        // Act
        var request = CreateRequestWithCookies(server, "/ActiveLogin/BankId/Auth?returnUrl=%2F&uiOptions=X&orderRef=Y", (AuthStateKeyCookieName, stateKey));
        var transaction = await request.GetAsync();

        // Assert
        Assert.Equal(HttpStatusCode.OK, transaction.StatusCode);

        var transactionContent = await transaction.Content.ReadAsStringAsync();
        Assert.Equal("/cru", GetInlineJsonValue(transactionContent, "cancelReturnUrl"));
    }

    [Fact]
    public async Task Init_Returns_Ui_With_Script()
    {
        // Arrange

        var (stateStorage, stateKey) = await SetupStateStorage(new BankIdUiAuthState(new AuthenticationProperties()));

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
                services.AddSingleton(stateStorage);
                services.AddTransient(s => _bankIdUiOptionsProtector.Object);
            });

        // Act
        var request = CreateRequestWithCookies(server, "/ActiveLogin/BankId/Auth?returnUrl=%2F&uiOptions=X&orderRef=Y", (AuthStateKeyCookieName, stateKey));
        var transaction = await request.GetAsync();

        // Assert
        Assert.Equal(HttpStatusCode.OK, transaction.StatusCode);

        var transactionContent = await transaction.Content.ReadAsStringAsync();
        var document = await HtmlDocumentHelper.FromContent(transactionContent);

        Assert.NotNull(document.GetElement<IHtmlDivElement>("div.activelogin-bankid-ui--wrapper"));
        Assert.NotNull(document.GetElement<IHtmlDivElement>("div.activelogin-bankid-ui--status-wrapper"));
        Assert.NotNull(document.GetElement<IHtmlImageElement>("img.activelogin-bankid-ui--qr-code-image"));

        Assert.Equal("/", GetInlineJsonValue(transactionContent, "returnUrl"));
        Assert.Equal("/", GetInlineJsonValue(transactionContent, "cancelReturnUrl"));
        Assert.Equal("X", GetInlineJsonValue(transactionContent, "protectedUiOptions"));
    }

    [Fact]
    public async Task Init_Preserves_UI_Options()
    {
        var (stateStorage, stateKey) = await SetupStateStorage(new BankIdUiAuthState(new AuthenticationProperties()));
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
            }),
            services =>
            {
                services.AddSingleton(stateStorage);
                services.AddTransient(s => _bankIdUiOptionsProtector.Object);
            });

        // Act
        var request = CreateRequestWithCookies(server, "/ActiveLogin/BankId/Auth?returnUrl=%2F&uiOptions=UIOPTIONS&orderRef=Y", (AuthStateKeyCookieName, stateKey));
        var transaction = await request.GetAsync();

        // Assert
        Assert.Equal(HttpStatusCode.OK, transaction.StatusCode);

        var transactionContent = await transaction.Content.ReadAsStringAsync();

        Assert.Equal("UIOPTIONS", GetInlineJsonValue(transactionContent, "protectedUiOptions"));
    }

    [Fact]
    public async Task Init_Requires_State_Cookie_To_Be_Present()
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
            }),
            services =>
            {
                services.AddTransient(s => _bankIdUiOptionsProtector.Object);
            });

        // Act
        var request = server.CreateRequest("/ActiveLogin/BankId/Auth?returnUrl=%2F&uiOptions=X&orderRef=Y");
        request.AddHeader("Cookie", "");
        var transaction = await request.GetAsync();

        // Assert
        Assert.Equal(HttpStatusCode.Redirect, transaction.StatusCode);
        Assert.Equal("/", transaction.Headers.Location.ToString());
    }

    [Fact]
    public async Task AutoLaunch_Sets_Correct_RedirectUri()
    {
        // Arrange mocks
        var autoLaunchOptions =
            new BankIdUiOptions([], true, false, false, false, string.Empty, AuthStateKeyCookieName, Api.Models.CardReader.class1);
        var mockProtector = new Mock<IBankIdDataStateProtector<BankIdUiOptions>>();
        mockProtector
            .Setup(protector => protector.Unprotect(It.IsAny<string>()))
            .Returns(autoLaunchOptions);
        mockProtector
            .Setup(protector => protector.Protect(It.IsAny<BankIdUiOptions>()))
            .Returns("Ignored");

        var (stateStorage, stateKey) = await SetupStateStorage(new BankIdUiAuthState(new AuthenticationProperties()));

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
                services.AddSingleton(stateStorage);
                services.AddTransient(s => mockProtector.Object);
            });

        // Arrange acting request
        var testReturnUrl = "/TestReturnUrl";
        var testOptions = "TestOptions";
        var initializeRequestBody = new { returnUrl = testReturnUrl, uiOptions = testOptions };

        // Act
        var initializeTransaction = await GetInitializeResponse(server, initializeRequestBody, (AuthStateKeyCookieName, stateKey));

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
        // Arrange mocks
        var autoLaunchOptions = new BankIdUiOptions([], false, false, false, false, string.Empty, AuthStateKeyCookieName, Api.Models.CardReader.class1);

        var mockProtector = new Mock<IBankIdDataStateProtector<BankIdUiOptions>>();
        mockProtector
            .Setup(protector => protector.Unprotect(It.IsAny<string>()))
            .Returns(autoLaunchOptions);
        mockProtector
            .Setup(protector => protector.Protect(It.IsAny<BankIdUiOptions>()))
            .Returns("Ignored");

        var (stateStorage, stateKey) = await SetupStateStorage(new BankIdUiAuthState(new AuthenticationProperties()));

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
                services.AddSingleton(stateStorage);
                services.AddTransient(s => mockProtector.Object);
                services.AddMvc().AddJsonOptions(configure =>
                {
                    configure.JsonSerializerOptions.PropertyNamingPolicy = null;
                });
            });


        // Arrange acting request
        var testReturnUrl = "/TestReturnUrl";
        var testOptions = "TestOptions";
        var initializeRequestBody = new { returnUrl = testReturnUrl, uiOptions = testOptions };

        //Act
        var initializeTransaction = await GetInitializeResponse(server, initializeRequestBody, (AuthStateKeyCookieName, stateKey));

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
        // Arrange mocks
        var autoLaunchOptions =
            new BankIdUiOptions([], false, false, false, false, string.Empty, AuthStateKeyCookieName, Api.Models.CardReader.class1);
        _bankIdUiOptionsProtector
            .Setup(protector => protector.Unprotect(It.IsAny<string>()))
            .Returns(autoLaunchOptions);

        var (stateStorage, stateKey) = await SetupStateStorage(new BankIdUiAuthState(new AuthenticationProperties()));

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
                services.AddSingleton(stateStorage);
                services.AddTransient(s => _bankIdUiOptionsProtector.Object);
            });


        // Arrange acting request
        var initializeRequestBody = new { };

        //Act
        var initializeTransaction = await GetInitializeResponse(server, initializeRequestBody, (AuthStateKeyCookieName, stateKey));

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
        // Arrange mocks
        var autoLaunchOptions =
            new BankIdUiOptions([], false, false, false, false, string.Empty, AuthStateKeyCookieName, Api.Models.CardReader.class1);
        _bankIdUiOptionsProtector
            .Setup(protector => protector.Unprotect(It.IsAny<string>()))
            .Returns(autoLaunchOptions);
        var testBankIdApi = new TestBankIdAppApi(new BankIdSimulatedAppApiClient());

        var (stateStorage, stateKey) = await SetupStateStorage(new BankIdUiAuthState(new AuthenticationProperties()));

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
                services.AddSingleton(stateStorage);
                services.AddTransient(s => _bankIdUiOptionsProtector.Object);
                services.AddSingleton<IBankIdAppApiClient>(s => testBankIdApi);
            });

        // Arrange csrf info
        var loginRequest = CreateRequestWithCookies(server, "/ActiveLogin/BankId/Auth?returnUrl=%2F&uiOptions=X&orderRef=Y", (AuthStateKeyCookieName, stateKey));
        var loginResponse = await loginRequest.GetAsync();
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
