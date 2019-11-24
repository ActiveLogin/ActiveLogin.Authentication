using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using ActiveLogin.Authentication.BankId.AspNetCore;
using ActiveLogin.Authentication.BankId.AspNetCore.Azure;
using ActiveLogin.Authentication.BankId.AspNetCore.QrCoder;
using ActiveLogin.Authentication.GrandId.AspNetCore;
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
            });

            services.AddIdentityServer(x => { x.Authentication.CookieLifetime = TimeSpan.FromHours(1); })
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


            // Sample of using BankID through GrandID (Svensk E-identitet) with in memory dev environment
            //services.AddAuthentication()
            //        .AddGrandId(builder =>
            //        {
            //            builder
            //                .UseSimulatedEnvironment()
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
                    builder.UseQrCoderQrCodeGenerator();
                    builder.Configure(options =>
                            {
                                options.IssueBirthdateClaim = true;
                                options.IssueGenderClaim = true;
                            })
                            .AddSameDevice(BankIdAuthenticationDefaults.SameDeviceAuthenticationScheme, "BankID (SameDevice)", options => { })
                            .AddOtherDevice(BankIdAuthenticationDefaults.OtherDeviceAuthenticationScheme, "BankID (OtherDevice)", options => { });

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

                    if (Configuration.GetValue("ActiveLogin:GrandId:UseSimulatedEnvironment", false))
                    {
                        builder.UseSimulatedEnvironment();
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
