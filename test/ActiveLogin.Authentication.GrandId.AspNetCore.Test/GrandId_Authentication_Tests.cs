using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Xunit;

namespace ActiveLogin.Authentication.GrandId.AspNetCore.Test
{
    public class GrandId_Authentication_Tests
    {
        [Fact]
        public async Task Challange_Redirects_To_SignIn()
        {
            // Arrange
            var client = CreateServer(o =>
                {
                    o.UseSimulatedEnvironment()
                     .AddBankIdSameDevice(options => { });
                },
                async context =>
                {
                    await context.ChallengeAsync(GrandIdDefaults.BankIdSameDeviceAuthenticationScheme);
                }).CreateClient();

            // Act
            var transaction = await client.GetAsync("/");

            // Assert
            Assert.Equal(HttpStatusCode.Redirect, transaction.StatusCode);
            Assert.Equal("/signin-grandid-bankid-samedevice", transaction.Headers.Location.LocalPath);
        }

        private TestServer CreateServer(Action<IGrandIdBuilder> builder, Func<HttpContext, Task> testpath, Action<IServiceCollection> configureServices = null)
        {
            var webHostBuilder = new WebHostBuilder()
                .Configure(app =>
                {
                    app.UseAuthentication();
                    app.Use(async (context, next) =>
                    {
                        await testpath(context);
                        await next();
                    });
                    app.Run(context => context.Response.WriteAsync(""));
                })
                .ConfigureServices(services =>
                {
                    services.AddAuthentication().AddGrandId(builder);
                    configureServices?.Invoke(services);
                });

            return new TestServer(webHostBuilder);
        }
    }
}
