using System.Globalization;
using System.Text;

using ActiveLogin.Authentication.BankId.Api;
using ActiveLogin.Authentication.BankId.AspNetCore;

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

        builder.UseAuthRequestUserData(authUserData =>
        {
            var message = new StringBuilder();
            message.AppendLine("# Active Login");
            message.AppendLine();
            message.AppendLine("Welcome to the *Active Login* demo.");

            authUserData.UserVisibleData = message.ToString();
            authUserData.UserVisibleDataFormat = BankIdUserVisibleDataFormats.SimpleMarkdownV1;
        });

        builder.AddSameDevice(BankIdDefaults.SameDeviceAuthenticationScheme, "BankID (SameDevice)", options => { });
        builder.AddOtherDevice(BankIdDefaults.OtherDeviceAuthenticationScheme, "BankID (OtherDevice)", options => { });

        if (configuration.GetValue("ActiveLogin:BankId:UseSimulatedEnvironment", false))
        {
            builder.UseSimulatedEnvironment();
        }
        else
        {
            builder.UseProductionEnvironment();
            builder.UseRootCaCertificate(Path.Combine(environment.ContentRootPath, configuration.GetValue<string>("ActiveLogin:BankId:CaCertificate:FilePath")));
            builder.UseClientCertificateFromAzureKeyVault(configuration.GetSection("ActiveLogin:BankId:ClientCertificate"));
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
