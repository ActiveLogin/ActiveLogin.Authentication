using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

using ActiveLogin.Authentication.BankId.Api;
using ActiveLogin.Authentication.BankId.Api.Models;
using ActiveLogin.Authentication.BankId.AspNetCore.Areas.ActiveLogin.Models;
using ActiveLogin.Authentication.BankId.AspNetCore.Cookies;
using ActiveLogin.Authentication.BankId.AspNetCore.DataProtection;
using ActiveLogin.Authentication.BankId.AspNetCore.Models;
using ActiveLogin.Authentication.BankId.AspNetCore.Payment;
using ActiveLogin.Authentication.BankId.AspNetCore.Test.Helpers;
using ActiveLogin.Authentication.BankId.Core;
using ActiveLogin.Authentication.BankId.Core.CertificatePolicies;
using ActiveLogin.Authentication.BankId.Core.Launcher;
using ActiveLogin.Authentication.BankId.Core.Payment;

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

public class BankId_UiPayment_Tests : BankId_Ui_Tests_Base
{
    private const string DefaultStateCookieName = "__ActiveLogin.BankIdUiState";

    private readonly Mock<IBankIdUiOrderRefProtector> _bankIdUiOrderRefProtector;
    private readonly Mock<IBankIdUiOptionsProtector> _bankIdUiOptionsProtector;
    private readonly Mock<IBankIdUiStateProtector> _bankIdUiStateProtector;
    private readonly Mock<IBankIdUiOptionsCookieManager> _bankIdUiOptionsCookieManager;

    public BankId_UiPayment_Tests()
    {
        _bankIdUiOptionsProtector = new Mock<IBankIdUiOptionsProtector>();
        _bankIdUiOptionsProtector
            .Setup(protector => protector.Unprotect(It.IsAny<string>()))
            .Returns(new BankIdUiOptions(new List<BankIdCertificatePolicy>(), false, false, false, false, "/", DefaultStateCookieName, Api.Models.CardReader.class1));
        _bankIdUiOptionsProtector
            .Setup(protector => protector.Protect(It.IsAny<BankIdUiOptions>()))
            .Returns("Ignored");

        _bankIdUiOptionsCookieManager = new Mock<IBankIdUiOptionsCookieManager>();
        _bankIdUiOptionsCookieManager
            .Setup(protector => protector.Store(It.IsAny<BankIdUiOptions>(), It.IsAny<DateTimeOffset>()));
        _bankIdUiOptionsCookieManager
            .Setup(protector => protector.Retrieve())
            .Returns(new BankIdUiOptions(new List<BankIdCertificatePolicy>(), false, false, false, false, "/", DefaultStateCookieName, Api.Models.CardReader.class1));


        _bankIdUiOrderRefProtector = new Mock<IBankIdUiOrderRefProtector>();
        _bankIdUiOrderRefProtector
            .Setup(protector => protector.Unprotect(It.IsAny<string>()))
            .Returns(new BankIdUiOrderRef("Any"));
        _bankIdUiOrderRefProtector
            .Setup(protector => protector.Protect(It.IsAny<BankIdUiOrderRef>()))
            .Returns("Any");

        var paymentState = new BankIdUiPaymentState("configKey", new BankIdPaymentProperties(TransactionType.npa, "Test Merchant"));
        _bankIdUiStateProtector = new Mock<IBankIdUiStateProtector>();
        _bankIdUiStateProtector
            .Setup(protector => protector.Unprotect(It.IsAny<string>()))
            .Returns(paymentState);
        _bankIdUiStateProtector
            .Setup(protector => protector.Protect(It.IsAny<BankIdUiPaymentState>()))
            .Returns("Ignored");
    }

    [Fact]
    public async Task BankIdUiPaymentController_Returns_404_If_BankId_Is_Not_Registered()
    {
        // Arrange
        var server = TestHostFactory.CreateTestServer();
        using var client = server.CreateClient();

        // Act
        var transaction = await client.GetAsync("/ActiveLogin/BankId/Payment");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, transaction.StatusCode);
    }

    [Fact]
    public async Task BankIdUiPaymentApiController_Returns_404_If_BankId_Is_Not_Registered()
    {
        // Arrange
        var server = TestHostFactory.CreateTestServer();
        using var client = server.CreateClient();

        // Act
        var transaction = await client.PostAsync("/ActiveLogin/BankId/Payment/Api/Initialize", null);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, transaction.StatusCode);
    }

    [Fact]
    public async Task InitiatePayment_Redirects_To_Payment()
    {
        // Arrange
        using var client = TestHostFactory.CreatePaymentTestServer(o =>
            {
                o.UseSimulatedEnvironment();
            },
            o =>
            {
                o.AddSameDevice();
            },
            DefaultAppConfiguration(async context =>
            {
                var bankIdPaymentService = context.RequestServices.GetRequiredService<IBankIdPaymentService>();
                await bankIdPaymentService.InitiatePaymentAsync(new BankIdPaymentProperties(TransactionType.npa, "Test Merchant"), "/al-payment-cb", BankIdPaymentDefaults.OtherDeviceConfigKey);
            })).CreateClient();

        // Act
        var transaction = await client.GetAsync("/");

        // Assert
        Assert.Equal(HttpStatusCode.Redirect, transaction.StatusCode);
        Assert.StartsWith("/ActiveLogin/BankId/Payment", transaction.Headers.Location.OriginalString);
    }

    [Fact]
    public async Task InitiatePaymentAsync_Redirects_To_Payment_Without_Path_Base()
    {
        // Arrange
        using var client = TestHostFactory.CreatePaymentTestServer(o =>
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
                        await InitiatePayment(context);
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
        Assert.StartsWith("/PathBase/ActiveLogin/BankId/Payment", redirectUrl);

        var callbackUrl = UrlEncoder.Default.Encode("/al-payment-cb");
        var callbackParameter = $"returnUrl={callbackUrl}";
        Assert.Contains(callbackParameter, redirectUrl);
    }

    [Fact]
    public async Task Payment_UI_Should_Be_Accessible_Even_When_Site_Requires_Auth()
    {
        // Arrange
        using var server = TestHostFactory.CreatePaymentTestServer(o =>
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
        var request = CreateRequestWithFakeStateAndUiOptionsCookie( server, "/ActiveLogin/BankId/Payment?returnUrl=%2F");
        var transaction = await request.GetAsync();


        // Assert
        Assert.Equal(HttpStatusCode.OK, transaction.StatusCode);
    }

    [Fact]
    public async Task PaymentInit_Returns_Ui_With_Resolved_Cancel_Url()
    {
        // Arrange
        var options = new BankIdUiOptions(new List<BankIdCertificatePolicy>(), true, false, false, false, "~/cru", DefaultStateCookieName, CardReader.class1);
        _bankIdUiOptionsProtector
            .Setup(protector => protector.Unprotect(It.IsAny<string>()))
            .Returns(options);

        using var server = TestHostFactory.CreatePaymentTestServer(o =>
            {
                o.UseSimulatedEnvironment();
            },
            o =>
            {
                o.AddSameDevice();
            },
            DefaultAppConfiguration(async context =>
            {
                await context.ChallengeAsync(BankIdPaymentDefaults.SameDeviceConfigKey);
            }),
            services =>
            {
                services.AddTransient(s => _bankIdUiOptionsProtector.Object);
                services.AddTransient(s => _bankIdUiStateProtector.Object);
            });

        // Act
        var request = CreateRequestWithFakeStateAndUiOptionsCookie(server, "/ActiveLogin/BankId/Payment?returnUrl=%2F");
        var transaction = await request.GetAsync();

        // Assert
        Assert.Equal(HttpStatusCode.OK, transaction.StatusCode);

        var transactionContent = await transaction.Content.ReadAsStringAsync();
        Assert.Equal("/cru", GetInlineJsonValue(transactionContent, "cancelReturnUrl"));
    }

    [Fact]
    public async Task PaymentInit_Returns_Ui_With_Script()
    {
        // Arrange
        using var server = TestHostFactory.CreatePaymentTestServer(o =>
            {
                o.UseSimulatedEnvironment();
            },
            o =>
            {
                o.AddSameDevice();
            },
            DefaultAppConfiguration(async context =>
            {
                await context.ChallengeAsync(BankIdPaymentDefaults.SameDeviceConfigKey);
            }),
            services =>
            {
                services.AddTransient(s => _bankIdUiOptionsProtector.Object);
                services.AddTransient(s => _bankIdUiStateProtector.Object);
            });

        // Act
        var request = CreateRequestWithFakeStateAndUiOptionsCookie(server, "/ActiveLogin/BankId/Payment?returnUrl=%2F");
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
    }

    [Fact]
    public async Task PaymentInit_Requires_State_Cookie_To_Be_Present()
    {
        // Arrange
        using var server = TestHostFactory.CreatePaymentTestServer(o =>
            {
                o.UseSimulatedEnvironment();
            },
            o =>
            {
                o.AddSameDevice();
            },
            DefaultAppConfiguration(async context =>
            {
                await context.ChallengeAsync(BankIdPaymentDefaults.SameDeviceConfigKey);
            }),
            services =>
            {
                services.AddTransient(s => _bankIdUiOptionsProtector.Object);
            });

        // Act
        var request = server.CreateRequest("/ActiveLogin/BankId/Payment?returnUrl=%2F");
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
        var autoLaunchOptions = new BankIdUiOptions(new List<BankIdCertificatePolicy>(), true, false, false, false, string.Empty, DefaultStateCookieName, CardReader.class1);

        _bankIdUiOptionsCookieManager
            .Setup(protector => protector.Retrieve())
            .Returns(autoLaunchOptions);

        using var server = TestHostFactory.CreatePaymentTestServer(
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
                await InitiatePayment(context);
            }),
            services =>
            {
                services.AddTransient(s => _bankIdUiOptionsCookieManager.Object);
                services.AddTransient(s => _bankIdUiStateProtector.Object);
            });

        // Arrange acting request
        var testReturnUrl = "/TestReturnUrl";
        var initializeRequestBody = new { returnUrl = testReturnUrl};

        // Act
        var initializeTransaction = await GetInitializeResponse(server, initializeRequestBody);

        // Assert
        Assert.Equal(HttpStatusCode.OK, initializeTransaction.StatusCode);

        var responseContent = await initializeTransaction.Content.ReadAsStringAsync();
        var responseObject = JsonConvert.DeserializeAnonymousType(responseContent, new { RedirectUri = "", OrderRef = "", IsAutoLaunch = false });
        Assert.True(responseObject.IsAutoLaunch);

        var encodedReturnParam = UrlEncoder.Default.Encode(testReturnUrl);
        var expectedUrl = $"http://localhost/ActiveLogin/BankId/Payment?returnUrl={encodedReturnParam}";
        Assert.Equal(expectedUrl, responseObject.RedirectUri);
    }

    [Fact]
    public async Task Api_Always_Returns_CamelCase_Json_For_Http200Ok()
    {
        // Arrange mocks
        var autoLaunchOptions = new BankIdUiOptions(new List<BankIdCertificatePolicy>(), false, false, false, false, string.Empty, DefaultStateCookieName, CardReader.class1);

        var mockProtector = new Mock<IBankIdUiOptionsProtector>();
        mockProtector
            .Setup(protector => protector.Unprotect(It.IsAny<string>()))
            .Returns(autoLaunchOptions);
        mockProtector
            .Setup(protector => protector.Protect(It.IsAny<BankIdUiOptions>()))
            .Returns("Ignored");

        using var server = TestHostFactory.CreatePaymentTestServer(
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
                await InitiatePayment(context);
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

    private static async Task InitiatePayment(HttpContext context)
    {
        var bankIdPaymentService = context.RequestServices.GetRequiredService<IBankIdPaymentService>();
        await bankIdPaymentService.InitiatePaymentAsync(new BankIdPaymentProperties(TransactionType.npa, "Test Merchant"), "/al-payment-cb",
            BankIdPaymentDefaults.OtherDeviceConfigKey);
    }

    [Fact]
    public async Task Api_Always_Returns_CamelCase_Json_For_Http400BadRequest()
    {
        // Arrange mocks
        var autoLaunchOptions = new BankIdUiOptions(new List<BankIdCertificatePolicy>(), false, false, false, false, string.Empty, DefaultStateCookieName, CardReader.class1);
        _bankIdUiOptionsProtector
            .Setup(protector => protector.Unprotect(It.IsAny<string>()))
            .Returns(autoLaunchOptions);

        using var server = TestHostFactory.CreatePaymentTestServer(
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
                await InitiatePayment(context);
            }),
            services =>
            {
                services.AddTransient(s => _bankIdUiOptionsProtector.Object);
                services.AddTransient(s => _bankIdUiStateProtector.Object);
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
        var autoLaunchOptions = new BankIdUiOptions(new List<BankIdCertificatePolicy>(), false, false, false, false, string.Empty, DefaultStateCookieName, CardReader.class1);
        _bankIdUiOptionsProtector
            .Setup(protector => protector.Unprotect(It.IsAny<string>()))
            .Returns(autoLaunchOptions);
        var testBankIdApi = new TestBankIdAppApi(new BankIdSimulatedAppApiClient());

        using var server = TestHostFactory.CreatePaymentTestServer(
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
                await InitiatePayment(context);
            }),
            services =>
            {
                services.AddTransient(s => _bankIdUiOptionsProtector.Object);
                services.AddTransient(s => _bankIdUiStateProtector.Object);
                services.AddTransient(s => _bankIdUiOrderRefProtector.Object);
                services.AddSingleton<IBankIdAppApiClient>(s => testBankIdApi);
            });

        //Act
        var cancelRequest = new JsonContent(new
        {
            orderRef = "ANY",
            uiOptions = "TestOptions",
            cancelReturnUrl = "/"
        });

        // Act
        var cancelTransaction = await MakeRequestWithRequiredContext("Payment", "/ActiveLogin/BankId/Payment/Api/Cancel", server, cancelRequest);

        // Assert
        Assert.Equal(HttpStatusCode.OK, cancelTransaction.StatusCode);
        Assert.True(testBankIdApi.CancelAsyncIsCalled);
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
    private Task<HttpResponseMessage> GetInitializeResponse(TestServer server, object initializeRequestBody)
    {
        return GetInitializeResponse("Payment", server, initializeRequestBody);
    }
}
