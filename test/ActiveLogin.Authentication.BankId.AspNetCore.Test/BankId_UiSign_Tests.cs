using System;
using System.Collections.Generic;
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

public class BankId_UiSign_Tests : BankId_Ui_Tests_Base
{
    private const string DefaultStateCookieName = "__ActiveLogin.BankIdUiState";

    private readonly Mock<IBankIdUiOrderRefProtector> _bankIdUiOrderRefProtector;
    private readonly Mock<IBankIdUiOptionsProtector> _bankIdUiOptionsProtector;
    private readonly Mock<IBankIdUiStateProtector> _bankIdUiStateProtector;

    public BankId_UiSign_Tests()
    {
        _bankIdUiOptionsProtector = new Mock<IBankIdUiOptionsProtector>();
        _bankIdUiOptionsProtector
            .Setup(protector => protector.Unprotect(It.IsAny<string>()))
            .Returns(new BankIdUiOptions(new List<string>(), false, false, "/", DefaultStateCookieName));
        _bankIdUiOptionsProtector
            .Setup(protector => protector.Protect(It.IsAny<BankIdUiOptions>()))
            .Returns("Ignored");

        _bankIdUiOrderRefProtector = new Mock<IBankIdUiOrderRefProtector>();
        _bankIdUiOrderRefProtector
            .Setup(protector => protector.Unprotect(It.IsAny<string>()))
            .Returns(new BankIdUiOrderRef("Any"));
        _bankIdUiOrderRefProtector
            .Setup(protector => protector.Protect(It.IsAny<BankIdUiOrderRef>()))
            .Returns("Any");

        var signState = new BankIdUiSignState("configKey", new BankIdSignProperties("userVisibleData"));
        _bankIdUiStateProtector = new Mock<IBankIdUiStateProtector>();
        _bankIdUiStateProtector
            .Setup(protector => protector.Unprotect(It.IsAny<string>()))
            .Returns(signState);
        _bankIdUiStateProtector
            .Setup(protector => protector.Protect(It.IsAny<BankIdUiSignState>()))
            .Returns("Ignored");
    }

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

                services.AddTransient(s => _bankIdUiOptionsProtector.Object);
                services.AddTransient(s => _bankIdUiStateProtector.Object);
            });

        // Act
        var request = CreateRequestWithFakeStateCookie( server, "/ActiveLogin/BankId/Sign?returnUrl=%2F&uiOptions=X&orderRef=Y");
        var transaction = await request.GetAsync();


        // Assert
        Assert.Equal(HttpStatusCode.OK, transaction.StatusCode);
    }

    [Fact]
    public async Task SignInit_Returns_Ui_With_Resolved_Cancel_Url()
    {
        // Arrange
        var options = new BankIdUiOptions(new List<string>(), true, false, "~/cru", DefaultStateCookieName);
        _bankIdUiOptionsProtector
            .Setup(protector => protector.Unprotect(It.IsAny<string>()))
            .Returns(options);

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
                await context.ChallengeAsync(BankIdSignDefaults.SameDeviceConfigKey);
            }),
            services =>
            {
                services.AddTransient(s => _bankIdUiOptionsProtector.Object);
                services.AddTransient(s => _bankIdUiStateProtector.Object);
            });

        // Act
        var request = CreateRequestWithFakeStateCookie(server, "/ActiveLogin/BankId/Sign?returnUrl=%2F&uiOptions=X&orderRef=Y");
        var transaction = await request.GetAsync();

        // Assert
        Assert.Equal(HttpStatusCode.OK, transaction.StatusCode);

        var transactionContent = await transaction.Content.ReadAsStringAsync();
        Assert.Equal("/cru", GetInlineJsonValue(transactionContent, "cancelReturnUrl"));
    }

    [Fact]
    public async Task SignInit_Returns_Ui_With_Script()
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
                await context.ChallengeAsync(BankIdSignDefaults.SameDeviceConfigKey);
            }),
            services =>
            {
                services.AddTransient(s => _bankIdUiOptionsProtector.Object);
                services.AddTransient(s => _bankIdUiStateProtector.Object);
            });

        // Act
        var request = CreateRequestWithFakeStateCookie(server, "/ActiveLogin/BankId/Sign?returnUrl=%2F&uiOptions=X&orderRef=Y");
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
    public async Task SignInit_Preserves_UI_Options()
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
                await context.ChallengeAsync(BankIdSignDefaults.SameDeviceConfigKey);
            }),
            services =>
            {
                services.AddTransient(s => _bankIdUiOptionsProtector.Object);
                services.AddTransient(s => _bankIdUiStateProtector.Object);
            });

        // Act
        var request =
            CreateRequestWithFakeStateCookie(server, "/ActiveLogin/BankId/Sign?returnUrl=%2F&uiOptions=UIOPTIONS&orderRef=Y");
        var transaction = await request.GetAsync();

        // Assert
        Assert.Equal(HttpStatusCode.OK, transaction.StatusCode);

        var transactionContent = await transaction.Content.ReadAsStringAsync();

        Assert.Equal("UIOPTIONS", GetInlineJsonValue(transactionContent, "protectedUiOptions"));
    }

    [Fact]
    public async Task SignInit_Requires_State_Cookie_To_Be_Present()
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
                await context.ChallengeAsync(BankIdSignDefaults.SameDeviceConfigKey);
            }),
            services =>
            {
                services.AddTransient(s => _bankIdUiOptionsProtector.Object);
            });

        // Act
        var request = server.CreateRequest("/ActiveLogin/BankId/Sign?returnUrl=%2F&uiOptions=X&orderRef=Y");
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
        var autoLaunchOptions = new BankIdUiOptions(new List<string>(), true, false, string.Empty, DefaultStateCookieName);
        var mockProtector = new Mock<IBankIdUiOptionsProtector>();
        mockProtector
            .Setup(protector => protector.Unprotect(It.IsAny<string>()))
            .Returns(autoLaunchOptions);
        mockProtector
            .Setup(protector => protector.Protect(It.IsAny<BankIdUiOptions>()))
            .Returns("Ignored");

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
                await InitiateSign(context);
            }),
            services =>
            {
                services.AddTransient(s => mockProtector.Object);
                services.AddTransient(s => _bankIdUiStateProtector.Object);
            });

        // Arrange acting request
        var testReturnUrl = "/TestReturnUrl";
        var testOptions = "TestOptions";
        var initializeRequestBody = new { returnUrl = testReturnUrl, uiOptions = testOptions };

        // Act
        var initializeTransaction = await GetInitializeResponse(server, initializeRequestBody);

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
        // Arrange mocks
        var autoLaunchOptions = new BankIdUiOptions(new List<string>(), false, false, string.Empty, DefaultStateCookieName);
        var mockProtector = new Mock<IBankIdUiOptionsProtector>();
        mockProtector
            .Setup(protector => protector.Unprotect(It.IsAny<string>()))
            .Returns(autoLaunchOptions);
        mockProtector
            .Setup(protector => protector.Protect(It.IsAny<BankIdUiOptions>()))
            .Returns("Ignored");

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
                await InitiateSign(context);
            }),
            services =>
            {
                services.AddTransient(s => mockProtector.Object);
                services.AddTransient(s => _bankIdUiStateProtector.Object);
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
        var initializeTransaction = await GetInitializeResponse(server, initializeRequestBody);

        // Assert
        Assert.Equal(HttpStatusCode.OK, initializeTransaction.StatusCode);

        var responseContent = await initializeTransaction.Content.ReadAsStringAsync();
        Assert.Contains("redirectUri", responseContent);
        Assert.Contains("orderRef", responseContent);
        Assert.Contains("isAutoLaunch", responseContent);
    }

    private static async Task InitiateSign(HttpContext context)
    {
        var bankIdSignService = context.RequestServices.GetRequiredService<IBankIdSignService>();
        await bankIdSignService.InitiateSignAsync(new BankIdSignProperties("UVD"), "/al-sign-cb",
            BankIdSignDefaults.OtherDeviceConfigKey);
    }

    [Fact]
    public async Task Api_Always_Returns_CamelCase_Json_For_Http400BadRequest()
    {
        // Arrange mocks
        var autoLaunchOptions = new BankIdUiOptions(new List<string>(), false, false, string.Empty, DefaultStateCookieName);
        _bankIdUiOptionsProtector
            .Setup(protector => protector.Unprotect(It.IsAny<string>()))
            .Returns(autoLaunchOptions);

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
                await InitiateSign(context);
            }),
            services =>
            {
                services.AddTransient(s => _bankIdUiOptionsProtector.Object);
                services.AddTransient(s => _bankIdUiStateProtector.Object);
                services.AddMvc().AddJsonOptions(configure =>
                {
                    configure.JsonSerializerOptions.PropertyNamingPolicy = null;
                });
            });


        // Arrange acting request
        var initializeRequestBody = new { };

        //Act
        var initializeTransaction = await GetInitializeResponse(server, initializeRequestBody);

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
        var autoLaunchOptions = new BankIdUiOptions(new List<string>(), false, false, string.Empty, DefaultStateCookieName);
        _bankIdUiOptionsProtector
            .Setup(protector => protector.Unprotect(It.IsAny<string>()))
            .Returns(autoLaunchOptions);
        var testBankIdApi = new TestBankIdApi(new BankIdSimulatedApiClient());

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
                await InitiateSign(context);
            }),
            services =>
            {
                services.AddTransient(s => _bankIdUiOptionsProtector.Object);
                services.AddTransient(s => _bankIdUiStateProtector.Object);
                services.AddTransient(s => _bankIdUiOrderRefProtector.Object);
                services.AddSingleton<IBankIdApiClient>(s => testBankIdApi);
            });

        //Act
        var cancelRequest = new JsonContent(new
        {
            orderRef = "ANY",
            uiOptions = "TestOptions",
            cancelReturnUrl = "/"
        });

        // Act
        var cancelTransaction = await MakeRequestWithRequiredContext("Sign", "/ActiveLogin/BankId/Sign/Api/Cancel", server, cancelRequest);

        // Assert
        Assert.Equal(HttpStatusCode.OK, cancelTransaction.StatusCode);
        Assert.True(testBankIdApi.CancelAsyncIsCalled);
    }

    private TestServer CreateServer(
        Action<IBankIdBuilder> configureBankId,
        Action<IBankIdSignBuilder> configureBankIdSign,
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
    private Task<HttpResponseMessage> GetInitializeResponse(TestServer server, object initializeRequestBody)
    {
        return GetInitializeResponse("Sign", server, initializeRequestBody);
    }
}
