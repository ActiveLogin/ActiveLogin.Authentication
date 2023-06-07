using System.Globalization;

using ActiveLogin.Authentication.BankId.AspNetCore;
using ActiveLogin.Authentication.BankId.AspNetCore.Auth;
using ActiveLogin.Authentication.BankId.AspNetCore.Sign;
using ActiveLogin.Authentication.BankId.AzureKeyVault;
using ActiveLogin.Authentication.BankId.AzureMonitor;
using ActiveLogin.Authentication.BankId.Core;
using ActiveLogin.Authentication.BankId.QrCoder;
using ActiveLogin.Authentication.BankId.UaParser;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Azure;


//
// DISCLAIMER
//
// These are samples on how to use Active Login in different situations
// and might not represent optimal way of setting up
// ASP.NET MVC, IdentityServer or other components.
//
// Please see this as inspiration, not a complete template.
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

// Add Active Login - BankID
services
    .AddBankId(bankId =>
    {
        bankId.AddDebugEventListener();
        bankId.AddApplicationInsightsEventListener(options =>
        {
            options.LogUserPersonalIdentityNumber = false;
            options.LogUserPersonalIdentityNumberHints = true;

            options.LogUserNames = false;

            options.LogUserBankIdIssueDate = false;

            options.LogDeviceIpAddress = false;
            options.LogDeviceUniqueHardwareId = false;
        });

        bankId.UseQrCoderQrCodeGenerator();
        bankId.UseUaParserDeviceDetection();

        bankId.AddCustomAppCallbackByUserAgent(userAgent => userAgent.Contains("Instagram"), "instagram://");
        bankId.AddCustomAppCallbackByUserAgent(userAgent => userAgent.Contains("FBAN") || userAgent.Contains("FBAV"), "fb://");

        if (configuration.GetValue("ActiveLogin:BankId:UseSimulatedEnvironment", false))
        {
            bankId.UseSimulatedEnvironment();
        }
        else if (configuration.GetValue("ActiveLogin:BankId:UseTestEnvironment", false))
        {
            bankId.UseTestEnvironment();
        }
        else
        {
            bankId.UseProductionEnvironment()
                .UseClientCertificateFromAzureKeyVault(configuration.GetSection("ActiveLogin:BankId:ClientCertificate"));
        }
    });

// Add Active Login - Auth
services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie()
    .AddBankIdAuth(bankId =>
    {
        bankId.AddSameDevice(BankIdAuthDefaults.SameDeviceAuthenticationScheme, "BankID (SameDevice)", options => { });
        bankId.AddOtherDevice(BankIdAuthDefaults.OtherDeviceAuthenticationScheme, "BankID (OtherDevice)", options => { });
    });

// Add Active Login - Sign
services.AddBankIdSign(bankId =>
{
    bankId.AddSameDevice(BankIdSignDefaults.SameDeviceConfigKey, "BankID (SameDevice)", options => { });
    bankId.AddOtherDevice(BankIdSignDefaults.OtherDeviceConfigKey, "BankID (OtherDevice)", options => { });
});

// Add Authorization
builder.Services.AddAuthorization(options =>
{
    // By default, all incoming requests will be authorized according to the default policy.
    options.FallbackPolicy = options.DefaultPolicy;
});

// Add MVC
services.AddControllersWithViews();


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
