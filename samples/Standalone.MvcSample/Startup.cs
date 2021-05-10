using System.Collections.Generic;
using System.Globalization;
using System.IO;
using ActiveLogin.Authentication.BankId.AspNetCore;
using ActiveLogin.Authentication.GrandId.AspNetCore;
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

using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;

namespace Standalone.MvcSample
{
    //
    // DISCLAIMER
    //
    // These are samples on how to use Active Login in different situations
    // and might not represent optimal way of setting up
    // ASP.NET MVC, Identity Server or other components.
    //
    // Please see this as inspiration, not a complete template.
    //

    public class Startup
    {
        private readonly IWebHostEnvironment _environment;

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            _environment = environment;
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<TelemetryClient>(new TelemetryClient(TelemetryConfiguration.CreateDefault()));

            services.Configure<CookiePolicyOptions>(options =>
            {
                options.MinimumSameSitePolicy = SameSiteMode.None;
                options.HttpOnly = HttpOnlyPolicy.Always;
                options.Secure = CookieSecurePolicy.Always;
            });

            services.AddControllersWithViews(config =>
            {
                config.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
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

                    if (Configuration.GetValue("ActiveLogin:BankId:UseSimulatedEnvironment", false))
                    {
                        builder.UseSimulatedEnvironment();
                    }
                    else if (Configuration.GetValue("ActiveLogin:BankId:UseTestEnvironment", false))
                    {
                        builder.UseTestEnvironment()
                            .UseRootCaCertificate(Path.Combine(_environment.ContentRootPath, Configuration.GetValue<string>("ActiveLogin:BankId:CaCertificate:FilePath")))
                            .UseClientCertificateFromAzureKeyVault(Configuration.GetSection("ActiveLogin:BankId:ClientCertificate"));
                    }
                    else
                    {
                        builder.UseProductionEnvironment()
                            .UseRootCaCertificate(Path.Combine(_environment.ContentRootPath, Configuration.GetValue<string>("ActiveLogin:BankId:CaCertificate:FilePath")))
                            .UseClientCertificateFromAzureKeyVault(Configuration.GetSection("ActiveLogin:BankId:ClientCertificate"));
                    }
                })
                .AddGrandId(builder =>
                {
                    builder.AddBankIdSameDevice(GrandIdDefaults.BankIdSameDeviceAuthenticationScheme, "GrandID (SameDevice)", options => { })
                           .AddBankIdOtherDevice(GrandIdDefaults.BankIdOtherDeviceAuthenticationScheme, "GrandID (OtherDevice)", options => { })
                           .AddBankIdChooseDevice(GrandIdDefaults.BankIdChooseDeviceAuthenticationScheme, "GrandID (ChooseDevice)", options => { });

                    if (Configuration.GetValue("ActiveLogin:GrandId:UseSimulatedEnvironment", false))
                    {
                        builder.UseSimulatedEnvironment();
                    }
                    else if (Configuration.GetValue("ActiveLogin:GrandId:UseTestEnvironment", false))
                    {
                        builder.UseTestEnvironment(ConfigureEnvironment);
                    }
                    else
                    {
                        builder.UseProductionEnvironment(ConfigureEnvironment);
                    }

                    void ConfigureEnvironment(IGrandIdEnvironmentConfiguration config)
                    {
                        config.ApiKey = Configuration.GetValue<string>("ActiveLogin:GrandId:ApiKey");
                        config.BankIdServiceKey = Configuration.GetValue<string>("ActiveLogin:GrandId:BankIdServiceKey");
                    }
                });

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseHttpsRedirection();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
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

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
