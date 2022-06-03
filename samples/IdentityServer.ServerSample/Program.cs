using System.Globalization;
using System.Text;

using ActiveLogin.Authentication.BankId.Api;
using ActiveLogin.Authentication.BankId.AspNetCore.Auth;
using ActiveLogin.Authentication.BankId.AzureKeyVault;
using ActiveLogin.Authentication.BankId.AzureMonitor;
using ActiveLogin.Authentication.BankId.Core;
using ActiveLogin.Authentication.BankId.QrCoder;
using ActiveLogin.Authentication.BankId.UaParser;

using IdentityServer.ServerSample;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Localization;



//
// DISCLAIMER
//
// These are samples on how to use Active Login in different situations
// and might not represent optimal way of setting up
// ASP.NET MVC, IdentityServer or other components.
//
// Please see this as inspiration, not a complete template.

//
// Please note that Duende IdentityServer requires a license if you are running it in production
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
    options.Authentication.CookieLifetime = BankIdAuthDefaults.MaximumSessionLifespan;
})
    .AddDeveloperSigningCredential()
    .AddInMemoryIdentityResources(IdentityServerConfig.GetIdentityResources())
    .AddInMemoryClients(IdentityServerConfig.GetClients(configuration.GetSection("ActiveLogin:Clients")));

// Add Active Login - BankID

// # Sample: Using BankID with in memory dev environment
//services.AddBankId(bankId =>
//    {
//        bankId
//            .UseSimulatedEnvironment();
//    });

// # Sample: Using BankID with production environment
//services.AddBankId(bankId =>
//        {
//            bankId
//                .UseProductionEnvironment()
//                .UseClientCertificateFromAzureKeyVault(configuration.GetSection("ActiveLogin:BankId:ClientCertificate"))
//                .UseRootCaCertificate(Path.Combine(environment.ContentRootPath, configuration.GetValue<string>("ActiveLogin:BankId:CaCertificate:FilePath")))
//        });

// # Sample: BankID with production environment, custom display name and multiple environment support
services
    .AddBankId(bankId =>
    {
        bankId.AddDebugEventListener();
        bankId.AddApplicationInsightsEventListener(options =>
        {
            options.LogUserPersonalIdentityNumber = false;
            options.LogUserPersonalIdentityNumberHints = true;

            options.LogUserNames = false;

            options.LogDeviceIpAddress = false;
            options.LogCertificateDates = true;
        });

        bankId.UseQrCoderQrCodeGenerator();
        bankId.UseUaParserDeviceDetection();

        bankId.UseAuthRequestUserData(authUserData =>
        {
            var message = new StringBuilder();
            message.AppendLine("# Active Login");
            message.AppendLine();
            message.AppendLine("Welcome to the *Active Login* demo.");

            authUserData.UserVisibleData = message.ToString();
            authUserData.UserVisibleDataFormat = BankIdUserVisibleDataFormats.SimpleMarkdownV1;
        });

        if (configuration.GetValue("ActiveLogin:BankId:UseSimulatedEnvironment", false))
        {
            bankId.UseSimulatedEnvironment();
        }
        else
        {
            bankId.UseProductionEnvironment();
            bankId.UseRootCaCertificate(Path.Combine(environment.ContentRootPath, configuration.GetValue<string>("ActiveLogin:BankId:CaCertificate:FilePath")));
            bankId.UseClientCertificateFromAzureKeyVault(configuration.GetSection("ActiveLogin:BankId:ClientCertificate"));
        }
    });

// Add authentication
services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddBankId(bankId =>
    {
        bankId.AddSameDevice(BankIdAuthDefaults.SameDeviceAuthenticationScheme, "BankID (SameDevice)", options => { });
        bankId.AddOtherDevice(BankIdAuthDefaults.OtherDeviceAuthenticationScheme, "BankID (OtherDevice)", options => { });
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
