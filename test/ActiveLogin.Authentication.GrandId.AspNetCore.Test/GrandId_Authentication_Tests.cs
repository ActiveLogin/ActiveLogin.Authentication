using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ActiveLogin.Authentication.GrandId.AspNetCore.Test
{
    public class GrandId_Authentication_Tests
    {
        private TestServer CreateServer(Action<IGrandIdAuthenticationBuilder> builder, Func<HttpContext, Task> testpath,
            Action<IServiceCollection> configureServices = null)
        {
            IWebHostBuilder webHostBuilder = new WebHostBuilder()
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
                    services.AddAuthentication()
                        .AddGrandId(builder);
                    configureServices?.Invoke(services);
                });

            return new TestServer(webHostBuilder);
        }

        [Fact]
        public async Task Challange_Redirects_To_SignIn()
        {
            // Arrange
            HttpClient client = CreateServer(o =>
                    {
                        o.UseSimulatedEnvironment()
                            .AddBankIdSameDevice(options => { });
                    },
                    async context =>
                    {
                        await context.ChallengeAsync(GrandIdAuthenticationDefaults
                            .BankIdSameDeviceAuthenticationScheme);
                    })
                .CreateClient();

            // Act
            HttpResponseMessage transaction = await client.GetAsync("/");

            // Assert
            Assert.Equal(HttpStatusCode.Redirect, transaction.StatusCode);
            Assert.Equal("/signin-grandid-bankid-samedevice", transaction.Headers.Location.LocalPath);
        }
    }
}
