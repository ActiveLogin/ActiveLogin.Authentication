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

    public static TestServer CreateAuthTestServer(
        Action<IBankIdBuilder> configureBankId,
        Action<IBankIdAuthBuilder> configureBankIdAuth,
        Action<IApplicationBuilder> configureApplication,
        Action<IServiceCollection> configureServices = null)
    {
        var solutionRoot = Path.GetFullPath(
            Path.Combine(AppContext.BaseDirectory, "../../../../.."));

        var contentRoot = Path.Combine(
            solutionRoot,
            "test",
            "ActiveLogin.Authentication.BankId.AspNetCore.Test");

        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            ContentRootPath = contentRoot
        });

        builder.WebHost.UseTestServer();

        builder.Services.AddBankId(configureBankId);

        builder.Services
            .AddAuthentication()
            .AddCookie()
            .AddBankIdAuth(configureBankIdAuth);

        builder.Services
            .AddControllersWithViews()
            .AddApplicationPart(
                typeof(ActiveLogin.Authentication.BankId.AspNetCore.BankIdConstants).Assembly)
            .AddRazorRuntimeCompilation();

        configureServices?.Invoke(builder.Services);

        var app = builder.Build();

        configureApplication(app);

        app.Start();

        return app.GetTestServer();
    }

    public static TestServer CreateSignTestServer(
    Action<IBankIdBuilder> configureBankId,
    Action<IBankIdSignBuilder> configureBankIdSign,
    Action<IApplicationBuilder> configureApplication,
    Action<IServiceCollection> configureServices = null)
    {
        var solutionRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../.."));

        var contentRoot = Path.Combine(
            solutionRoot,
            "test",
            "ActiveLogin.Authentication.BankId.AspNetCore.Test");

        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            ContentRootPath = contentRoot
        });

        builder.WebHost.UseTestServer();

        builder.Services.AddBankId(configureBankId);
        builder.Services.AddBankIdSign(configureBankIdSign);

        builder.Services
            .AddControllersWithViews()
            .AddApplicationPart(
                typeof(ActiveLogin.Authentication.BankId.AspNetCore.BankIdConstants).Assembly)
            .AddRazorRuntimeCompilation();

        configureServices?.Invoke(builder.Services);

        var app = builder.Build();

        configureApplication(app);

        app.Start();

        return app.GetTestServer();
    }

    public static TestServer CreatePaymentTestServer(
        Action<IBankIdBuilder> configureBankId,
        Action<IBankIdPaymentBuilder> configureBankIdPayment,
        Action<IApplicationBuilder> configureApplication,
        Action<IServiceCollection> configureServices = null)
    {

        var solutionRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../.."));

        var contentRoot = Path.Combine(
            solutionRoot,
            "test",
            "ActiveLogin.Authentication.BankId.AspNetCore.Test");

        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            ContentRootPath = contentRoot
        });

        builder.WebHost.UseTestServer();

        builder.Services.AddBankId(configureBankId);
        builder.Services.AddBankIdPayment(configureBankIdPayment);

        builder.Services
            .AddControllersWithViews()
            .AddApplicationPart(
                typeof(ActiveLogin.Authentication.BankId.AspNetCore.BankIdConstants).Assembly)
            .AddRazorRuntimeCompilation();

        configureServices?.Invoke(builder.Services);

        var app = builder.Build();

        configureApplication(app);

        app.Start();

        return app.GetTestServer();
    }


    public static TestServer CreateTestServer()
    {
        var solutionRoot = Path.GetFullPath(
            Path.Combine(AppContext.BaseDirectory, "../../../../.."));

        var contentRoot = Path.Combine(
            solutionRoot,
            "test",
            "ActiveLogin.Authentication.BankId.AspNetCore.Test");

        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            ContentRootPath = contentRoot
        });

        builder.WebHost.UseTestServer();

        builder.Services.AddAuthentication();
        builder.Services.AddAuthorization();

        var app = builder.Build();

        DefaultAppConfiguration(_ => Task.CompletedTask)(app);

        app.Start();

        return app.GetTestServer();
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

            app.Use(async (context, next) =>
            {
                await testpath(context);
                await next();
            });
        };
    }
}
