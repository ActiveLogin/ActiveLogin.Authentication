using System.Collections.Generic;
using System.Globalization;
using System.IO;

using ActiveLogin.Authentication.BankId.AspNetCore;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
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



var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
var configuration = builder.Configuration;
var environment = builder.Environment;

// Add telemetry
services.AddApplicationInsightsTelemetry(configuration);

// Add authentication
services.Configure<CookiePolicyOptions>(options =>
{
    options.MinimumSameSitePolicy = SameSiteMode.None;
    options.HttpOnly = HttpOnlyPolicy.Always;
    options.Secure = CookieSecurePolicy.Always;
});

services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie()
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

        builder.AddSameDevice(BankIdDefaults.SameDeviceAuthenticationScheme, "BankID (SameDevice)", options => { })
               .AddOtherDevice(BankIdDefaults.OtherDeviceAuthenticationScheme, "BankID (OtherDevice)", options => { });

        if (configuration.GetValue("ActiveLogin:BankId:UseSimulatedEnvironment", false))
        {
            builder.UseSimulatedEnvironment();
        }
        else if (configuration.GetValue("ActiveLogin:BankId:UseTestEnvironment", false))
        {
            builder.UseTestEnvironment()
                .UseRootCaCertificate(Path.Combine(environment.ContentRootPath, configuration.GetValue<string>("ActiveLogin:BankId:CaCertificate:FilePath")))
                .UseClientCertificateFromAzureKeyVault(configuration.GetSection("ActiveLogin:BankId:ClientCertificate"));
        }
        else
        {
            builder.UseProductionEnvironment()
                .UseRootCaCertificate(Path.Combine(environment.ContentRootPath, configuration.GetValue<string>("ActiveLogin:BankId:CaCertificate:FilePath")))
                .UseClientCertificateFromAzureKeyVault(configuration.GetSection("ActiveLogin:BankId:ClientCertificate"));
        }
    });

// Add MVC
services.AddControllersWithViews(config =>
{
    config.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
});




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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapDefaultControllerRoute();

app.Run();
