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

namespace IdentityServer.ServerSample
{
    //
    // DISCLAIMER
    //
    // These are samples on how to use Active Login in different situations
    // and might not represent optimal way of setting up
    // ASP.NET MVC, IdentityServer or other components.
    //
    // Please see this as inspiration, not a complete template.
    //
    // Please note that Duende IdeneityServer might require you to have a license, see:
    // https://duendesoftware.com/products/identityserver
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
            services.AddApplicationInsightsTelemetry(Configuration);

            services.Configure<CookiePolicyOptions>(options =>
            {
                options.MinimumSameSitePolicy = SameSiteMode.None;
                options.HttpOnly = HttpOnlyPolicy.Always;
                options.Secure = CookieSecurePolicy.Always;
            });

            services.AddControllersWithViews(config =>
            {
                config.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
            })
            .AddRazorRuntimeCompilation();

            services.AddIdentityServer(options =>
                {
                    options.Authentication.CookieLifetime = BankIdDefaults.MaximumSessionLifespan;
                })
                .AddDeveloperSigningCredential()
                .AddInMemoryIdentityResources(Config.GetIdentityResources())
                .AddInMemoryClients(Config.GetClients(Configuration.GetSection("ActiveLogin:Clients")));

            // Sample of using BankID with in memory dev environment
            //services.AddAuthentication()
            //        .AddBankId(builder =>
            //    {
            //        builder
            //            .UseSimulatedEnvironment()
            //            .AddSameDevice()
            //            .AddOtherDevice();
            //    });

            // Sample of using BankID with production environment
            //services.AddAuthentication()
            //        .AddBankId(builder =>
            //        {
            //            builder
            //                .UseProductionEnvironment()
            //                .UseClientCertificateFromAzureKeyVault(Configuration.GetSection("ActiveLogin:BankId:ClientCertificate"))
            //                .UseRootCaCertificate(Path.Combine(_environment.ContentRootPath, Configuration.GetValue<string>("ActiveLogin:BankId:CaCertificate:FilePath")))
            //                .AddSameDevice()
            //                .AddOtherDevice();
            //        });

            // Full sample with both BankID and GrandID with custom display name and multiple environment support
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

                    if (Configuration.GetValue("ActiveLogin:BankId:UseSimulatedEnvironment", false))
                    {
                        builder.UseSimulatedEnvironment();
                    }
                    else
                    {
                        builder.UseProductionEnvironment()
                            .UseRootCaCertificate(Path.Combine(_environment.ContentRootPath, Configuration.GetValue<string>("ActiveLogin:BankId:CaCertificate:FilePath")))
                            .UseClientCertificateFromAzureKeyVault(Configuration.GetSection("ActiveLogin:BankId:ClientCertificate"));
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

            app.UseIdentityServer();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
