using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using ActiveLogin.Authentication.BankId.Api;
using ActiveLogin.Authentication.BankId.AspNetCore;
using ActiveLogin.Authentication.BankId.AspNetCore.Azure;
using ActiveLogin.Authentication.GrandId.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServerSample
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

            // Sample of using BankID through GrandID (Svensk E-identitet) with all configurations available
            services.AddAuthentication()
                .AddGrandId(builder =>
                {
                    if (Configuration.GetValue("ActiveLogin:GrandId:UseDevelopmentApi", false))
                    {
                        builder.UseDevelopmentEnvironment("Alice", "Smith");
                    }
                    else
                    {
                        var apiKey = Configuration.GetValue<string>("ActiveLogin:GrandId:ApiKey");
                        if (Configuration.GetValue("ActiveLogin:GrandId:UseTestApiEndpoint", false))
                        {
                            builder.UseTestEnvironment(apiKey);
                        }
                        else
                        {
                            builder.UseProdEnvironment(apiKey);
                        }
                    }

                    builder
                        .AddScheme("grandid-samedevice", "GrandID - SameDevice", options =>
                        {
                            options.CallbackPath = new PathString("/signin-grandid-samedevice");
                            options.AuthenticateServiceKey = Configuration.GetValue<string>("ActiveLogin:GrandId:SameDeviceServiceKey");
                        })
                        .AddScheme("grandid-otherdevice", "GrandID - OtherDevice", options =>
                        {
                            options.CallbackPath = new PathString("/signin-grandid-otherdevice");
                            options.AuthenticateServiceKey = Configuration.GetValue<string>("ActiveLogin:GrandId:OtherDeviceServiceKey");
                        })
                        .AddScheme("grandid-choosedevice", "GrandID - ChooseDevice", options =>
                        {
                            options.CallbackPath = new PathString("/signin-grandid-choosedevice");
                            options.AuthenticateServiceKey = Configuration.GetValue<string>("ActiveLogin:GrandId:ChooseDeviceServiceKey");
                        });
                });

            // Sample of using BankID through GrandID (Svensk E-identitet) with minimum configuration
            //services.AddAuthentication()
            //    .AddGrandId(builder =>
            //    {
            //        builder
            //            .UseProdEnvironment(Configuration.GetValue<string>("ActiveLogin:GrandId:ApiKey"))
            //            .AddScheme("grandid-choosedevice", "GrandID - ChooseDevice", options =>
            //            {
            //                options.CallbackPath = new PathString("/signin-grandid-choosedevice");
            //                options.AuthenticateServiceKey = Configuration.GetValue<string>("ActiveLogin:GrandId:ChooseDeviceServiceKey");
            //            });
            //    });

            // Sample of using BankID nativly
            services.AddAuthentication()
                    .AddBankId()
                        .AddBankIdClientCertificateFromAzureKeyVault(Configuration.GetSection("ActiveLogin:BankId:ClientCertificate"))
                        .AddBankIdRootCaCertificate(Path.Combine(_environment.ContentRootPath, Configuration.GetValue<string>("ActiveLogin:BankId:CaCertificate:FilePath")))
                        .AddBankIdEnvironmentConfiguration(configuration =>
                        {
                            if (Configuration.GetValue("ActiveLogin:BankId:UseTestApiEndpoint", false))
                            {
                                configuration.ApiBaseUrl = BankIdUrls.TestApiBaseUrl;
                            }
                        });

            // Development BankID API
            if (Configuration.GetValue("ActiveLogin:BankId:UseDevelopmentApi", false))
            {
                services.AddBankIdDevelopmentEnvironment();
            }
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
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

            // BankID Authentication needs areas to be registered for the UI to work
            app.UseMvc(routes =>
            {
                routes.MapRoute("areas", "{area}/{controller=Home}/{action=Index}/{id?}");
                routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
