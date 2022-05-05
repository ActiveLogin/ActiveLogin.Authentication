using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

using ActiveLogin.Authentication.BankId.Api;
using ActiveLogin.Authentication.BankId.Api.Models;
using ActiveLogin.Authentication.BankId.AspNetCore.DataProtection;
using ActiveLogin.Authentication.BankId.AspNetCore.Test.Helpers;
using ActiveLogin.Authentication.BankId.Core;
using ActiveLogin.Authentication.BankId.Core.Launcher;
using ActiveLogin.Authentication.BankId.Core.Models;

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

public class BankId_Authentication_Tests
{
    private const string DefaultStateCookieName = "__ActiveLogin.BankIdState";

    private readonly Mock<IBankIdLoginOptionsProtector> _bankIdLoginOptionsProtector;

    public BankId_Authentication_Tests()
    {
        _bankIdLoginOptionsProtector = new Mock<IBankIdLoginOptionsProtector>();
        _bankIdLoginOptionsProtector
            .Setup(protector => protector.Unprotect(It.IsAny<string>()))
            .Returns(new BankIdLoginOptions(new List<string>(), false, false, "/", DefaultStateCookieName));
    }

    [Fact]
    public async Task BankIdController_Returns_404_If_BankId_Is_Not_Registered()
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
        var transaction = await client.GetAsync("/BankIdAuthentication/Login");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, transaction.StatusCode);
    }

    [Fact]
    public async Task BankIdApiController_Returns_404_If_BankId_Is_Not_Registered()
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
        var transaction = await client.PostAsync("/BankIdAuthentication/Api/Initialize", null);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, transaction.StatusCode);
    }

    [Fact]
    public async Task Challenge_Redirects_To_BankIdAuthentication_Login()
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
                await context.ChallengeAsync(BankIdDefaults.SameDeviceAuthenticationScheme);
            })).CreateClient();

        // Act
        var transaction = await client.GetAsync("/");

        // Assert
        Assert.Equal(HttpStatusCode.Redirect, transaction.StatusCode);
        Assert.StartsWith("/BankIdAuthentication/Login", transaction.Headers.Location.OriginalString);
    }

    [Fact]
    public async Task Challenge_Redirects_To_BankIdAuthentication_Login_With_Path_Base()
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
                            BankIdDefaults.SameDeviceAuthenticationScheme);
                        await next();
                    });
                    appBuilder.Run(context => context.Response.WriteAsync(""));
                });
            }
            ,
            services =>
            {
                services.AddTransient<RemoteAuthenticationOptions, BankIdOptions>();

            }).CreateClient();

        // Act
        var transaction = await client.GetAsync("/PathBase");

        // Assert
        Assert.Equal(HttpStatusCode.Redirect, transaction.StatusCode);

        var redirectUrl = transaction.Headers.Location.OriginalString;
        Assert.StartsWith("/PathBase/BankIdAuthentication/Login", redirectUrl);

        var callbackUrl = UrlEncoder.Default.Encode("/PathBase/signin-bankid-samedevice");
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

                services.AddTransient(s => _bankIdLoginOptionsProtector.Object);
            });

        // Act
        var request = CreateRequestWithStateCookie( server, "/BankIdAuthentication/Login?returnUrl=%2F&loginOptions=X&orderRef=Y");
        var transaction = await request.GetAsync();


        // Assert
        Assert.Equal(HttpStatusCode.OK, transaction.StatusCode);
    }

    [Fact]
    public async Task BankIdAuthentication_Login_Returns_Form_With_Resolved_Cancel_Url()
    {
        // Arrange
        var options = new BankIdLoginOptions(new List<string>(), true, false, "~/cru", DefaultStateCookieName);
        var mockProtector = new Mock<IBankIdLoginOptionsProtector>();
        mockProtector
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
                await context.ChallengeAsync(BankIdDefaults.SameDeviceAuthenticationScheme);
            }),
            services =>
            {
                services.AddTransient(s => mockProtector.Object);
            });

        // Act
        var request = CreateRequestWithStateCookie(server, "/BankIdAuthentication/Login?returnUrl=%2F&loginOptions=X&orderRef=Y");
        var transaction = await request.GetAsync();

        // Assert
        Assert.Equal(HttpStatusCode.OK, transaction.StatusCode);

        var document = await HtmlDocumentHelper.FromContent(transaction.Content);
        Assert.Equal("/cru", document.GetInputValue("input[name='CancelReturnUrl']"));
    }


    [Fact]
    public async Task BankIdAuthentication_Login_Returns_Form_And_Status()
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
                await context.ChallengeAsync(BankIdDefaults.SameDeviceAuthenticationScheme);
            }),
            services =>
            {
                services.AddTransient(s => _bankIdLoginOptionsProtector.Object);
            });

        // Act
        var request = CreateRequestWithStateCookie(server, "/BankIdAuthentication/Login?returnUrl=%2F&loginOptions=X&orderRef=Y");
        var transaction = await request.GetAsync();

        // Assert
        Assert.Equal(HttpStatusCode.OK, transaction.StatusCode);

        var document = await HtmlDocumentHelper.FromContent(transaction.Content);

        Assert.NotNull(document.GetElement<IHtmlFormElement>("form[id='bankIdLoginForm']"));
        Assert.NotNull(document.GetElement<IHtmlDivElement>("div[id='bankIdLoginStatus']"));
        Assert.NotNull(document.GetElement<IHtmlImageElement>("img.qr-code-image"));
        Assert.Equal("/", document.GetInputValue("input[name='ReturnUrl']"));
        Assert.Equal("/", document.GetInputValue("input[name='CancelReturnUrl']"));
        Assert.Equal("X", document.GetInputValue("input[name='LoginOptions']"));
    }

    [Fact]
    public async Task BankIdAuthentication_Login_Requires_State_Cookie_To_Be_Present()
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
                await context.ChallengeAsync(BankIdDefaults.SameDeviceAuthenticationScheme);
            }),
            services =>
            {
                services.AddTransient(s => _bankIdLoginOptionsProtector.Object);
            });

        // Act
        var request = server.CreateRequest("/BankIdAuthentication/Login?returnUrl=%2F&loginOptions=X&orderRef=Y");
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
        var autoLaunchOptions = new BankIdLoginOptions(new List<string>(), true, false, string.Empty, DefaultStateCookieName);
        var mockProtector = new Mock<IBankIdLoginOptionsProtector>();
        mockProtector
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
                await context.ChallengeAsync(BankIdDefaults.SameDeviceAuthenticationScheme);
            }),
            services =>
            {
                services.AddTransient(s => mockProtector.Object);
            });

        // Arrange acting request
        var testReturnUrl = "/TestReturnUrl";
        var testOptions = "TestOptions";
        var initializeRequestBody = new { returnUrl = testReturnUrl, loginOptions = testOptions };

        // Act
        var initializeTransaction = await GetInitializeResponse(server, initializeRequestBody);

        // Assert
        Assert.Equal(HttpStatusCode.OK, initializeTransaction.StatusCode);

        var responseContent = await initializeTransaction.Content.ReadAsStringAsync();
        var responseObject = JsonConvert.DeserializeAnonymousType(responseContent, new { RedirectUri = "", OrderRef = "", IsAutoLaunch = false });
        Assert.True(responseObject.IsAutoLaunch);

        var encodedReturnParam = UrlEncoder.Default.Encode(testReturnUrl);
        var expectedUrl = $"http://localhost/BankIdAuthentication/Login?returnUrl={encodedReturnParam}&loginOptions={testOptions}";
        Assert.Equal(expectedUrl, responseObject.RedirectUri);
    }

    [Fact]
    public async Task Api_Always_Returns_CamelCase_Json_For_Http200Ok()
    {
        // Arrange mocks
        var autoLaunchOptions = new BankIdLoginOptions(new List<string>(), false, false, string.Empty, DefaultStateCookieName);
        var mockProtector = new Mock<IBankIdLoginOptionsProtector>();
        mockProtector
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
                await context.ChallengeAsync(BankIdDefaults.SameDeviceAuthenticationScheme);
            }),
            services =>
            {
                services.AddTransient(s => mockProtector.Object);
                services.AddMvc().AddJsonOptions(configure =>
                {
                    configure.JsonSerializerOptions.PropertyNamingPolicy = null;
                });
            });


        // Arrange acting request
        var testReturnUrl = "/TestReturnUrl";
        var testOptions = "TestOptions";
        var initializeRequestBody = new { returnUrl = testReturnUrl, loginOptions = testOptions };

        //Act
        var initializeTransaction = await GetInitializeResponse(server, initializeRequestBody);

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
        var autoLaunchOptions = new BankIdLoginOptions(new List<string>(), false, false, string.Empty, DefaultStateCookieName);
        var mockProtector = new Mock<IBankIdLoginOptionsProtector>();
        mockProtector
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
                o.AddOtherDevice();
            },
            DefaultAppConfiguration(async context =>
            {
                await context.ChallengeAsync(BankIdDefaults.SameDeviceAuthenticationScheme);
            }),
            services =>
            {
                services.AddTransient(s => mockProtector.Object);
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
        var autoLaunchOptions = new BankIdLoginOptions(new List<string>(), false, false, string.Empty, DefaultStateCookieName);
        var mockProtector = new Mock<IBankIdLoginOptionsProtector>();
        mockProtector
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
                await context.ChallengeAsync(BankIdDefaults.SameDeviceAuthenticationScheme);
            }),
            services =>
            {
                services.AddTransient(s => mockProtector.Object);
                services.AddSingleton<IBankIdApiClient>(s => testBankIdApi);
            });

        // Arrange csrf info
        var loginRequest = CreateRequestWithStateCookie(server, "/BankIdAuthentication/Login?returnUrl=%2F&loginOptions=X&orderRef=Y");
        var loginResponse = await loginRequest.GetAsync();
        var loginCookies = loginResponse.Headers.GetValues("set-cookie");
        var document = await HtmlDocumentHelper.FromContent(loginResponse.Content);
        var csrfToken = document.GetRequestVerificationToken();

        // Arrange acting request
        var testReturnUrl = "/TestReturnUrl";
        var testOptions = "TestOptions";
        var initializeRequest = new JsonContent(new { returnUrl = testReturnUrl, loginOptions = testOptions });
        initializeRequest.Headers.Add("Cookie", loginCookies);
        initializeRequest.Headers.Add("RequestVerificationToken", csrfToken);

        // Act
        var client = server.CreateClient();
        var initializeTransaction = await client.PostAsync("/BankIdAuthentication/Api/Initialize", initializeRequest);
        var initializeResponseContent = await initializeTransaction.Content.ReadAsStringAsync();
        var initializeObject = JsonConvert.DeserializeAnonymousType(initializeResponseContent, new { RedirectUri = "", OrderRef = "", IsAutoLaunch = false });

        var cancelRequest = new JsonContent(new
        {
            orderRef = initializeObject.OrderRef,
            loginOptions = "TestOptions",
            cancelReturnUrl = "/"
        });
        cancelRequest.Headers.Add("Cookie", loginCookies);
        cancelRequest.Headers.Add("RequestVerificationToken", csrfToken);

        // Act
        var cancelTransaction = await client.PostAsync("/BankIdAuthentication/Api/Cancel", cancelRequest);

        // Assert
        Assert.Equal(HttpStatusCode.OK, cancelTransaction.StatusCode);
        Assert.True(testBankIdApi.CancelAsyncIsCalled);
    }

    private async Task<HttpResponseMessage> GetInitializeResponse(TestServer server, object initializeRequestBody)
    {
        // Arrange csrf info
        var loginRequest = CreateRequestWithStateCookie(server, "/BankIdAuthentication/Login?returnUrl=%2F&loginOptions=X&orderRef=Y");
        var loginResponse = await loginRequest.GetAsync();
        var loginCookies = loginResponse.Headers.GetValues("set-cookie");
        var document = await HtmlDocumentHelper.FromContent(loginResponse.Content);
        var csrfToken = document.GetRequestVerificationToken();

        // Arrange acting request
        var initializeRequest = new JsonContent(initializeRequestBody);
        initializeRequest.Headers.Add("Cookie", loginCookies);
        initializeRequest.Headers.Add("RequestVerificationToken", csrfToken);

        var client = server.CreateClient();
        return await client.PostAsync("/BankIdAuthentication/Api/Initialize", initializeRequest);
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
                services.AddAuthentication().AddBankId(configureBankIdAuth);
                services.AddMvc();
                configureServices?.Invoke(services);
            });

        return new TestServer(webHostBuilder);
    }

    private static RequestBuilder CreateRequestWithStateCookie(TestServer server, string path)
    {
        var request = server.CreateRequest(path);
        request.AddHeader("Cookie", $"{DefaultStateCookieName}=TEST");
        return request;
    }

    private static Action<IApplicationBuilder> DefaultAppConfiguration(Func<HttpContext, Task> testpath)
    {
        return app =>
        {
            app.UseMiddleware<FakeRemoteIpAddressMiddleware>(IPAddress.Parse("192.0.2.1"));
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

    private class TestBankIdApi : IBankIdApiClient
    {
        private readonly IBankIdApiClient _bankIdApiClient;

        public bool CancelAsyncIsCalled { get; private set; }

        public TestBankIdApi(IBankIdApiClient bankIdApiClient)
        {
            _bankIdApiClient = bankIdApiClient;
        }

        public Task<AuthResponse> AuthAsync(AuthRequest request)
        {
            return _bankIdApiClient.AuthAsync(request);
        }

        public Task<SignResponse> SignAsync(SignRequest request)
        {
            return _bankIdApiClient.SignAsync(request);
        }

        public Task<CollectResponse> CollectAsync(CollectRequest request)
        {
            return _bankIdApiClient.CollectAsync(request);
        }

        public Task<CancelResponse> CancelAsync(CancelRequest request)
        {
            CancelAsyncIsCalled = true;
            return _bankIdApiClient.CancelAsync(request);
        }
    }
}
