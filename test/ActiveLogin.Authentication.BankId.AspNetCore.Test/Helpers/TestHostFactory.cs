using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

using ActiveLogin.Authentication.BankId.AspNetCore.Auth;
using ActiveLogin.Authentication.BankId.AspNetCore.Payment;
using ActiveLogin.Authentication.BankId.AspNetCore.Sign;
using ActiveLogin.Authentication.BankId.Core;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Test.Helpers;

internal static class TestHostFactory
{

    public static IHost CreateHost()
    {
        return new HostBuilder()
            .ConfigureWebHost(webHostBuilder =>
            {
                webHostBuilder
                    .UseTestServer()
                    .UseSolutionRelativeContentRoot(Path.Combine("test", "ActiveLogin.Authentication.BankId.AspNetCore.Test"))
                    .Configure(app => DefaultAppConfiguration(_ => Task.CompletedTask))
                    .ConfigureServices(services =>
                    {
                        services.AddMvc();
                    });
            })
            .Build();
    }

    public static IHost CreateHostWithAuthentication()
    {
        return new HostBuilder()
            .ConfigureWebHost(webHostBuilder =>
            {
                webHostBuilder
                    .UseTestServer()
                    .UseSolutionRelativeContentRoot(Path.Combine("test", "ActiveLogin.Authentication.BankId.AspNetCore.Test"))
                    .Configure(app => DefaultAppConfiguration(_ => Task.CompletedTask))
                    .ConfigureServices(services =>
                    {
                        services.AddAuthentication();
                        services.AddMvc();
                    });
            })
            .Build();
    }

    public static TestServer CreateSignTestServer(
        Action<IBankIdBuilder> configureBankId,
        Action<IBankIdSignBuilder> configureBankIdSign,
        Action<IApplicationBuilder> configureApplication,
        Action<IServiceCollection> configureServices = null)
    {

        var host = new HostBuilder()
                .ConfigureWebHost(webHostBuilder =>
                {
                    webHostBuilder
                        .UseTestServer()
                        .UseSolutionRelativeContentRoot(Path.Combine("test", "ActiveLogin.Authentication.BankId.AspNetCore.Test"))
                        .Configure(app => configureApplication.Invoke(app))
                        .ConfigureServices(services =>
                        {
                            services.AddBankId(configureBankId);
                            services.AddBankIdSign(configureBankIdSign);
                            services.AddMvc();
                            configureServices?.Invoke(services);
                        });
                })
                .Build();

        return host.GetTestServer();
    }

    public static TestServer CreateAuthTestServer(
        Action<IBankIdBuilder> configureBankId,
        Action<IBankIdAuthBuilder> configureBankIdAuth,
        Action<IApplicationBuilder> configureApplication,
        Action<IServiceCollection> configureServices = null)
    {

        var host = new HostBuilder()
                .ConfigureWebHost(webHostBuilder =>
                {
                    webHostBuilder
                        .UseTestServer()
                        .UseSolutionRelativeContentRoot(Path.Combine("test", "ActiveLogin.Authentication.BankId.AspNetCore.Test"))
                        .Configure(app => configureApplication.Invoke(app))
                        .ConfigureServices(services =>
                        {
                            services.AddBankId(configureBankId);
                            services.AddAuthentication()
                                .AddCookie()
                                .AddBankIdAuth(configureBankIdAuth);
                            services.AddMvc();
                            configureServices?.Invoke(services);
                        });
                })
                .Build();

        return host.GetTestServer();
    }

    public static TestServer CreatePaymentTestServer(
        Action<IBankIdBuilder> configureBankId,
        Action<IBankIdPaymentBuilder> configureBankIdPayment,
        Action<IApplicationBuilder> configureApplication,
        Action<IServiceCollection> configureServices = null)
    {

        var host = new HostBuilder()
                .ConfigureWebHost(webHostBuilder =>
                {
                    webHostBuilder
                        .UseTestServer()
                        .UseSolutionRelativeContentRoot(Path.Combine("test", "ActiveLogin.Authentication.BankId.AspNetCore.Test"))
                        .Configure(app => configureApplication.Invoke(app))
                        .ConfigureServices(services =>
                        {
                            services.AddBankId(configureBankId);
                            services.AddBankIdPayment(configureBankIdPayment);
                            services.AddMvc();
                            configureServices?.Invoke(services);
                        });
                })
                .Build();

        return host.GetTestServer();
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
}
