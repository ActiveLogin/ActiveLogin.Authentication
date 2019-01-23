using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Xunit;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Test
{
    public class BankId_Authentication_Tests
    {
        private TestServer CreateServer(Action<IBankIdAuthenticationBuilder> builder, Func<HttpContext, Task> testpath = null)
        {
            var webHostBuilder = new WebHostBuilder()
                .Configure(app =>
                {
                    app.UseAuthentication();
                    app.Use(async (context, next) =>
                    {
                        if (testpath != null)
                        {
                            await testpath(context);
                        }
                        await next();
                    });
                    app.UseMvc(routes =>
                    {
                        routes.MapRoute("areas", "{area}/{controller=Home}/{action=Index}/{id?}");
                        routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
                    });
                    app.Run(context => context.Response.WriteAsync(""));
                })
                .ConfigureServices(services =>
                {
                    services.AddAuthentication().AddBankId(builder);
                    services.AddMvc();
                });
            return new TestServer(webHostBuilder);
        }

        private HttpClient CreateClientForServer(Action<IBankIdAuthenticationBuilder> builder, Func<HttpContext, Task> testpath = null)
        {
            var testServer = CreateServer(builder, testpath);
            return testServer.CreateClient();
        }

        [Fact]
        public async Task Challange_Redirects_To_BankIdAuthentication_Login()
        {
            // Arrange
            var client = CreateClientForServer(o =>
                {
                    o.UseDevelopmentEnvironment()
                     .AddSameDevice();
                },
                async context =>
                {
                    await context.ChallengeAsync(BankIdAuthenticationDefaults.SameDeviceAuthenticationScheme);
                });

            // Act
            var transaction = await client.GetAsync("/");

            // Assert
            Assert.Equal(HttpStatusCode.Redirect, transaction.StatusCode);
            Assert.StartsWith("/BankIdAuthentication/Login", transaction.Headers.Location.OriginalString);
        }
    }
}
