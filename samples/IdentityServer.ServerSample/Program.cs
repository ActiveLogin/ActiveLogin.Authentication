using System.Collections.Generic;
using System.Globalization;
using System.IO;

using ActiveLogin.Authentication.BankId.AspNetCore;

using IdentityServer.ServerSample;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;



//
// DISCLAIMER
//
// These are samples on how to use Active Login in different situations
// and might not represent optimal way of setting up
// ASP.NET MVC, Identity Server or other components.
//
// Please see this as inspiration, not a complete template.

//
// Please note that Duende IdentityServer might require you to have a license, see:
// https://duendesoftware.com/products/identityserver
//



var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
var configuration = builder.Configuration;
var environment = builder.Environment;


// Add telemetry
services.AddApplicationInsightsTelemetry(configuration);

// Configure cookie policy
services.Configure<CookiePolicyOptions>(options =>
{
    options.MinimumSameSitePolicy = SameSiteMode.None;
    options.HttpOnly = HttpOnlyPolicy.Always;
    options.Secure = CookieSecurePolicy.Always;
});

// Add IdentityServer
services.AddIdentityServer(options =>
{
    options.Authentication.CookieLifetime = BankIdDefaults.MaximumSessionLifespan;
})
    .AddDeveloperSigningCredential()
    .AddInMemoryIdentityResources(IdentityServerConfig.GetIdentityResources())
    .AddInMemoryClients(IdentityServerConfig.GetClients(configuration.GetSection("ActiveLogin:Clients")));

// Add authentication and Active Login

// # Sample: Using BankID with in memory dev environment
//services.AddAuthentication()
//        .AddBankId(builder =>
//    {
//        builder
//            .UseSimulatedEnvironment()
//            .AddSameDevice()
//            .AddOtherDevice();
//    });

// # Sample: Using BankID with production environment
//services.AddAuthentication()
//        .AddBankId(builder =>
//        {
//            builder
//                .UseProductionEnvironment()
//                .UseClientCertificateFromAzureKeyVault(configuration.GetSection("ActiveLogin:BankId:ClientCertificate"))
//                .UseRootCaCertificate(Path.Combine(environment.ContentRootPath, configuration.GetValue<string>("ActiveLogin:BankId:CaCertificate:FilePath")))
//                .AddSameDevice()
//                .AddOtherDevice();
//        });

// # Sample: BankID with production environment, custom display name and multiple environment support
services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddBankId(builder =>
    {
        builder.AddDebugEventListener();
        builder.AddApplicationInsightsEventListener(options =>
        {
            options.LogUserPersonalIdentityNumber = false;
            options.LogUserPersonalIdentityNumberHints = true;

            options.LogUserNames = false;

            options.LogDeviceIpAddress = false;
            options.LogCertificateDates = true;
        });

        builder.UseQrCoderQrCodeGenerator();
        builder.UseUaParserDeviceDetection();

        builder.Configure(options =>
        {
            options.IssueBirthdateClaim = true;
            options.IssueGenderClaim = true;
        })
        .AddSameDevice(BankIdDefaults.SameDeviceAuthenticationScheme, "BankID (SameDevice)", options => { })
        .AddOtherDevice(BankIdDefaults.OtherDeviceAuthenticationScheme, "BankID (OtherDevice)", options => { });

        if (configuration.GetValue("ActiveLogin:BankId:UseSimulatedEnvironment", false))
        {
            builder.UseSimulatedEnvironment();
        }
        else
        {
            builder.UseProductionEnvironment()
                .UseRootCaCertificate(Path.Combine(environment.ContentRootPath, configuration.GetValue<string>("ActiveLogin:BankId:CaCertificate:FilePath")))
                .UseClientCertificateFromAzureKeyVault(configuration.GetSection("ActiveLogin:BankId:ClientCertificate"));
        }
    });

// Add MVC
services.AddControllersWithViews()
        .AddRazorRuntimeCompilation();




// Build app
var app = builder.Build();

if (environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

if (!environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRequestLocalization(options =>
{
    var supportedCultures = new List<CultureInfo>
                {
                    new CultureInfo("en-US"),
                    new CultureInfo("en"),
                    new CultureInfo("sv-SE"),
                    new CultureInfo("sv")
                };

    options.DefaultRequestCulture = new RequestCulture("en-US");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

app.UseRouting();

app.UseIdentityServer();
app.UseAuthorization();

app.MapControllers();
app.MapDefaultControllerRoute();

app.Run();
