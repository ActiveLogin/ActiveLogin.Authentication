using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using ActiveLogin.Authentication.BankId.AspNetCore.DataProtection;
using ActiveLogin.Authentication.BankId.AspNetCore.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
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
                .Returns(new BankIdLoginOptions(new List<string>(), null, false, false, false));
        }

        [NoLinuxFact("Issues with layout pages from unit tests on Linux")]
        public async Task Challange_Redirects_To_BankIdAuthentication_Login()
        {
            // Arrange
            HttpClient client = CreateServer(o =>
                    {
                        o.UseSimulatedEnvironment()
                            .AddSameDevice();
                    },
                    async context => { await context.ChallengeAsync(BankIdAuthenticationDefaults.SameDeviceAuthenticationScheme); })
                .CreateClient();

            // Act
            HttpResponseMessage transaction = await client.GetAsync("/");

            // Assert
            Assert.Equal(HttpStatusCode.Redirect, transaction.StatusCode);
            Assert.StartsWith("/BankIdAuthentication/Login", transaction.Headers.Location.OriginalString);
        }


        [NoLinuxFact("Issues with layout pages from unit tests on Linux")]
        public async Task Authentication_UI_Should_Be_Accessible_Even_When_Site_Requires_Auth()
        {
            // Arrange
            HttpClient client = CreateServer(o =>
                    {
                        o.UseSimulatedEnvironment()
                            .AddSameDevice();
                    },
                    context => Task.CompletedTask,
                    services =>
                    {
                        services.AddMvc(config =>
                            {
                                AuthorizationPolicy policy = new AuthorizationPolicyBuilder()
                                    .RequireAuthenticatedUser()
                                    .Build();
                                config.Filters.Add(new AuthorizeFilter(policy));
                            })
                            .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

                        services.AddTransient(s => _bankIdLoginOptionsProtector.Object);
                    })
                .CreateClient();

            // Act
            HttpResponseMessage transaction =
                await client.GetAsync("/BankIdAuthentication/Login?returnUrl=%2F&loginOptions=X&orderRef=Y");

            // Assert
            Assert.Equal(HttpStatusCode.OK, transaction.StatusCode);
        }

        [NoLinuxFact("Issues with layout pages from unit tests on Linux")]
        public async Task BankIdAuthentication_Login_Returns_Form()
        {
            // Arrange
            HttpClient client = CreateServer(o =>
                    {
                        o.UseSimulatedEnvironment()
                            .AddSameDevice();
                    },
                    async context => { await context.ChallengeAsync(BankIdAuthenticationDefaults.SameDeviceAuthenticationScheme); },
                    services => { services.AddTransient(s => _bankIdLoginOptionsProtector.Object); })
                .CreateClient();

            // Act
            HttpResponseMessage transaction =
                await client.GetAsync("/BankIdAuthentication/Login?returnUrl=%2F&loginOptions=X&orderRef=Y");

            // Assert
            Assert.Equal(HttpStatusCode.OK, transaction.StatusCode);
            string content = await transaction.Content.ReadAsStringAsync();
            Assert.Contains("<form id=\"bankIdLoginForm\">", content);
        }


        private TestServer CreateServer(Action<IBankIdAuthenticationBuilder> builder, Func<HttpContext, Task> testpath,
            Action<IServiceCollection> configureServices = null)
        {
            IWebHostBuilder webHostBuilder = new WebHostBuilder()
                .UseSolutionRelativeContentRoot(Path.Combine("test",
                    "ActiveLogin.Authentication.BankId.AspNetCore.Test"))
                .Configure(app =>
                {
                    app.UseAuthentication();
                    app.UseMvcWithDefaultRoute();
                    app.Use(async (context, next) =>
                    {
                        await testpath(context);
                        await next();
                    });
                    app.Run(context => context.Response.WriteAsync(""));
                })
                .ConfigureServices(services =>
                {
                    services.AddAuthentication()
                        .AddBankId(builder);
                    services.AddMvc();
                    configureServices?.Invoke(services);
                });

            return new TestServer(webHostBuilder);
        }
    }
}
