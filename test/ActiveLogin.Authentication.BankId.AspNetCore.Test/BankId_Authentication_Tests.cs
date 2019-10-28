using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using ActiveLogin.Authentication.BankId.AspNetCore.DataProtection;
using ActiveLogin.Authentication.BankId.AspNetCore.Launcher;
using ActiveLogin.Authentication.BankId.AspNetCore.Models;
using ActiveLogin.Authentication.BankId.AspNetCore.Test.Helpers;
using ActiveLogin.Identity.Swedish;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Test
{
    public class BankId_Authentication_Tests
    {
        private readonly Mock<IBankIdLoginOptionsProtector> _bankIdLoginOptionsProtector;

        public BankId_Authentication_Tests()
        {
            _bankIdLoginOptionsProtector = new Mock<IBankIdLoginOptionsProtector>();
            _bankIdLoginOptionsProtector
                .Setup(protector => protector.Unprotect(It.IsAny<string>()))
                .Returns(new BankIdLoginOptions(new List<string>(), null, false, false, false, false));
        }

        [NoLinuxFact("Issues with layout pages from unit tests on Linux")]
        public async Task Challenge_Redirects_To_BankIdAuthentication_Login()
        {
            // Arrange
            var client = CreateServer(o =>
                {
                    o.UseSimulatedEnvironment()
                     .AddSameDevice();
                },
                DefaultAppConfiguration(async context =>
                {
                    await context.ChallengeAsync(BankIdAuthenticationDefaults.SameDeviceAuthenticationScheme);
                })).CreateClient();

            // Act
            var transaction = await client.GetAsync("/");

            // Assert
            Assert.Equal(HttpStatusCode.Redirect, transaction.StatusCode);
            Assert.StartsWith("/BankIdAuthentication/Login", transaction.Headers.Location.OriginalString);
        }

        [NoLinuxFact("Issues with layout pages from unit tests on Linux")]
        public async Task Challenge_Redirects_To_BankIdAuthentication_Login_With_Path_Base()
        {
            // Arrange
            var client = CreateServer(o =>
                {
                    o.UseSimulatedEnvironment()
                        .AddSameDevice();
                },
                app =>
                {
                    app.Map("/PathBase", appBuilder =>
                    {
                        app.UseRouting();
                        app.UseEndpoints(endpoints =>
                        {
                            endpoints.MapControllers();
                            endpoints.MapDefaultControllerRoute();
                        });
                        appBuilder.Use(async (context, next) =>
                        {
                            await context.ChallengeAsync(
                                BankIdAuthenticationDefaults.SameDeviceAuthenticationScheme);
                            await next();
                        });
                        appBuilder.Run(context => context.Response.WriteAsync(""));
                    });
                }
                ,
                services =>
                {
                    services.AddTransient<RemoteAuthenticationOptions, BankIdAuthenticationOptions>();

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

        [NoLinuxFact("Issues with layout pages from unit tests on Linux")]
        public async Task Authentication_UI_Should_Be_Accessible_Even_When_Site_Requires_Auth()
        {
            // Arrange
            var client = CreateServer(o =>
                {
                    o.UseSimulatedEnvironment()
                     .AddSameDevice();
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
                    })
                    .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

                    services.AddTransient(s => _bankIdLoginOptionsProtector.Object);
                }).CreateClient();

            // Act
            var transaction = await client.GetAsync("/BankIdAuthentication/Login?returnUrl=%2F&loginOptions=X&orderRef=Y");

            // Assert
            Assert.Equal(HttpStatusCode.OK, transaction.StatusCode);
        }

        [NoLinuxFact("Issues with layout pages from unit tests on Linux")]
        public async Task BankIdAuthentication_Login_Returns_Form_With_Qr()
        {
            // Arrange
            var client = CreateServer(o =>
                {
                    o.UseSimulatedEnvironment()
                        .AddSameDevice();
                },
                DefaultAppConfiguration(async context =>
                {
                    await context.ChallengeAsync(BankIdAuthenticationDefaults.SameDeviceAuthenticationScheme);
                }),
                services =>
                {
                    services.AddTransient(s => _bankIdLoginOptionsProtector.Object);
                }).CreateClient();

            // Act
            var transaction = await client.GetAsync("/BankIdAuthentication/Login?returnUrl=%2F&loginOptions=X&orderRef=Y");

            // Assert
            Assert.Equal(HttpStatusCode.OK, transaction.StatusCode);
            var content = await transaction.Content.ReadAsStringAsync();
            Assert.Contains("<form id=\"bankIdLoginForm\">", content);
            Assert.Contains("<img src=\"#\" title=\"qr-code\" alt=\"qr-code\" id=\"qr-code\"", content);
        }

        [Fact]
        public async Task AutoLaunch_Sets_Correct_RedirectUri()
        {
	        // Arrange mocks
	        var identityNumber = "195008212226";
	        var autoLaunchOptions = new BankIdLoginOptions(new List<string>(), SwedishPersonalIdentityNumber.Parse(identityNumber), false, true, false);
	        var mockProtector =  new Mock<IBankIdLoginOptionsProtector>();
	        mockProtector
		        .Setup(protector => protector.Unprotect(It.IsAny<string>()))
		        .Returns(autoLaunchOptions);

	        var client = CreateServer(
			        o =>
			        {
				        o.AuthenticationBuilder.Services.TryAddTransient<IBankIdLauncher, TestBankIdLauncher>();
				        o.UseSimulatedEnvironment().AddSameDevice();
			        },
			        DefaultAppConfiguration(async context =>
			        {
				        await context.ChallengeAsync(BankIdAuthenticationDefaults.SameDeviceAuthenticationScheme);
			        }),
			        services =>
			        {
				        services.AddTransient(s => mockProtector.Object);
			        })
		        .CreateClient();

	        // Arrange csrf info
	        var loginResponse = await client.GetAsync("/BankIdAuthentication/Login?returnUrl=%2F&loginOptions=X&orderRef=Y");
	        var loginCookies = loginResponse.Headers.GetValues("set-cookie");
	        var loginContent = await loginResponse.Content.ReadAsStringAsync();
	        var csrfToken = TokenExtractor.ExtractRequestVerificationTokenFromForm(loginContent);

	        // Arrange acting request
	        var testReturnUrl = "/TestReturnUrl";
	        var testOptions = "TestOptions";
	        var initializeRequest = new JsonContent(new  { returnUrl = testReturnUrl, loginOptions = testOptions });
	        initializeRequest.Headers.Add("Cookie", loginCookies);
	        initializeRequest.Headers.Add("RequestVerificationToken", csrfToken);

	        // Act
	        var transaction = await client.PostAsync("/BankIdAuthentication/Api/Initialize", initializeRequest);

	        // Assert
	        Assert.Equal(HttpStatusCode.OK, transaction.StatusCode);

	        var responseContent = await transaction.Content.ReadAsStringAsync();
	        var responseObject = JsonConvert.DeserializeAnonymousType(responseContent, new { RedirectUri = "", OrderRef = "", IsAutoLaunch = false });
	        Assert.True(responseObject.IsAutoLaunch);

	        var encodedReturnParam = UrlEncoder.Default.Encode(testReturnUrl);
	        var expectedUrl = $"http://localhost/BankIdAuthentication/Login?returnUrl={encodedReturnParam}&loginOptions={testOptions}";
	        Assert.Equal(expectedUrl, responseObject.RedirectUri);
        }

        private TestServer CreateServer(
            Action<IBankIdAuthenticationBuilder> configureBankId,
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
                    services.AddAuthentication().AddBankId(configureBankId);
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
                app.UseAuthentication();
                app.UseRouting();
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                    endpoints.MapDefaultControllerRoute();
                });
                app.Use(async (context, next) =>
                {
                    await testpath(context);
                    await next();
                });
                app.Run(context => context.Response.WriteAsync(""));
            };
        }
    }
}
