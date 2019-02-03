using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using ActiveLogin.Authentication.BankId.AspNetCore;
using ActiveLogin.Authentication.BankId.AspNetCore.Azure;
using ActiveLogin.Authentication.GrandId.AspNetCore;
using Microsoft.ApplicationInsights.AspNetCore.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace IdentityServer.ServerSample
{
    public class Startup
    {
        private readonly IHostingEnvironment _environment;

        public Startup(IConfiguration configuration, IHostingEnvironment environment)
        {
            _environment = environment;
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddApplicationInsightsTelemetry(Configuration)
                .AddOptions<ApplicationInsightsLoggerOptions>()
                .Configure(options =>
                {
                    options.IncludeEventId = true;
                });

            services.Configure<CookiePolicyOptions>(options =>
            {
                options.MinimumSameSitePolicy = SameSiteMode.None;
                options.HttpOnly = HttpOnlyPolicy.Always;
                options.Secure = CookieSecurePolicy.Always;
            });

            services.AddMvc();

            services.AddIdentityServer(x => { x.Authentication.CookieLifetime = TimeSpan.FromHours(1); })
                    .AddDeveloperSigningCredential()
                    .AddInMemoryIdentityResources(Config.GetIdentityResources())
                    .AddInMemoryClients(Config.GetClients(Configuration.GetSection("ActiveLogin:Clients")));

            // Sample of using BankID with in memory dev environment
            //services.AddAuthentication()
            //        .AddBankId(builder =>
            //    {
            //        builder
            //            .UseDevelopmentEnvironment()
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


            // Sample of using BankID through GrandID (Svensk E-identitet) with in memory dev environment
            //services.AddAuthentication()
            //        .AddGrandId(builder =>
            //        {
            //            builder
            //                .UseDevelopmentEnvironment()
            //                .AddBankIdSameDevice(options => { })
            //                .AddBankIdOtherDevice(options => { });
            //        });

            // Sample of using BankID through GrandID (Svensk E-identitet) with production environment
            //services.AddAuthentication()
            //        .AddGrandId(builder =>
            //        {
            //            builder
            //                .UseProductionEnvironment(config =>
            //                {
            //                    config.ApiKey = Configuration.GetValue<string>("ActiveLogin:GrandId:ApiKey");
            //                    config.BankIdServiceKey = Configuration.GetValue<string>("ActiveLogin:GrandId:BankIdServiceKey");
            //                })
            //                .AddBankIdChooseDevice();
            //        });

            // Full sample with both BankID and GrandID with custom display name and multiple environment support
            services.AddAuthentication()
                .AddBankId(builder =>
                {
                    builder.AddSameDevice(BankIdAuthenticationDefaults.SameDeviceAuthenticationScheme, "BankID (SameDevice)", options =>
                            {
                                options.IssueBirthdateClaim = true;
                                options.IssueGenderClaim = true;
                            })
                           .AddOtherDevice(BankIdAuthenticationDefaults.OtherDeviceAuthenticationScheme, "BankID (OtherDevice)", options =>
                            {
                                options.IssueBirthdateClaim = true;
                                options.IssueGenderClaim = true;
                            });

                    if (Configuration.GetValue("ActiveLogin:BankId:UseDevelopmentEnvironment", false))
                    {
                        builder.UseDevelopmentEnvironment();
                    }
                    else
                    {
                        builder.UseProductionEnvironment()
                               .UseClientCertificateFromAzureKeyVault(Configuration.GetSection("ActiveLogin:BankId:ClientCertificate"))
                               .UseRootCaCertificate(Path.Combine(_environment.ContentRootPath, Configuration.GetValue<string>("ActiveLogin:BankId:CaCertificate:FilePath")));
                    }
                })
                .AddGrandId(builder =>
                {
                    builder.ConfigureBankId(options =>
                           {
                               options.IssueBirthdateClaim = true;
                               options.IssueGenderClaim = true;
                           })
                           .AddBankIdSameDevice(GrandIdAuthenticationDefaults.BankIdSameDeviceAuthenticationScheme, "GrandID (SameDevice)", options => { })
                           .AddBankIdOtherDevice(GrandIdAuthenticationDefaults.BankIdOtherDeviceAuthenticationScheme, "GrandID (OtherDevice)", options => { })
                           .AddBankIdChooseDevice(GrandIdAuthenticationDefaults.BankIdChooseDeviceAuthenticationScheme, "GrandID (ChooseDevice)", options => { });

                    if (Configuration.GetValue("ActiveLogin:GrandId:UseDevelopmentEnvironment", false))
                    {
                        builder.UseDevelopmentEnvironment();
                    }
                    else
                    {
                        builder.UseProductionEnvironment(config =>
                        {
                            config.ApiKey = Configuration.GetValue<string>("ActiveLogin:GrandId:ApiKey");
                            config.BankIdServiceKey = Configuration.GetValue<string>("ActiveLogin:GrandId:BankIdServiceKey");
                        });
                    }
                });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddApplicationInsights(app.ApplicationServices, LogLevel.Information);

            app.UseHttpsRedirection();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseIdentityServer();

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

            app.UseMvcWithDefaultRoute();
        }
    }
}
