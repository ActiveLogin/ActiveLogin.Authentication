using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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

            // Sample of using BankID through GrandID (Svensk E-identitet) with in memory dev environment
            //services.AddAuthentication()
            //        .AddGrandId(builder =>
            //        {
            //            builder
            //                .UseDevelopmentEnvironment()
            //                .AddSameDevice(options => { })
            //                .AddOtherDevice(options => { });
            //        });

            // Sample of using BankID through GrandID (Svensk E-identitet) with production environment
            //services.AddAuthentication()
            //        .AddGrandId(builder =>
            //        {
            //            builder
            //                .UseProductionEnvironment(Configuration.GetValue<string>("ActiveLogin:GrandId:ApiKey"))
            //                .AddChooseDevice(options =>
            //                {
            //                    options.GrandIdAuthenticateServiceKey = Configuration.GetValue<string>("ActiveLogin:GrandId:ChooseDeviceServiceKey");
            //                });
            //        });

            // Sample of using BankID with in memory dev environment
            //services.AddAuthentication()
            //        .AddBankId(builder =>
            //    {
            //        builder
            //            .UseDevelopmentEnvironment()
            //            .AddSameDevice(options => { })
            //            .AddOtherDevice(options => { });
            //    });

            // Sample of using BankID with production environment
            //services.AddAuthentication()
            //        .AddBankId(builder =>
            //        {
            //            builder
            //                .UseProductionEnvironment()
            //                .UseClientCertificateFromAzureKeyVault(Configuration.GetSection("ActiveLogin:BankId:ClientCertificate"))
            //                .UseRootCaCertificate(Path.Combine(_environment.ContentRootPath, Configuration.GetValue<string>("ActiveLogin:BankId:CaCertificate:FilePath")))
            //                .AddSameDevice(options => { })
            //                .AddOtherDevice(options => { });
            //        });

            // Full sample with both BankID and GrandID with custom display name and multiple environment support
            services.AddAuthentication()
                .AddBankId(builder =>
                {
                    builder.AddSameDevice(BankIdAuthenticationDefaults.SameDeviceAuthenticationScheme, "BankID (SameDevice)", options => { })
                           .AddOtherDevice(BankIdAuthenticationDefaults.OtherDeviceAuthenticationScheme, "BankID (OtherDevice)", options => { });

                    if (Configuration.GetValue("ActiveLogin:BankId:UseDevelopmentEnvironment", false))
                    {
                        builder.UseDevelopmentEnvironment();
                    }
                    else if (Configuration.GetValue("ActiveLogin:BankId:UseTestEnvironment", false))
                    {
                        builder.UseTestEnvironment()
                               .UseClientCertificateFromAzureKeyVault(Configuration.GetSection("ActiveLogin:BankId:ClientCertificate"))
                               .UseRootCaCertificate(Path.Combine(_environment.ContentRootPath, Configuration.GetValue<string>("ActiveLogin:BankId:CaCertificate:FilePath")));
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
                    builder.AddSameDevice(GrandIdAuthenticationDefaults.SameDeviceAuthenticationScheme, "GrandID (SameDevice)", options =>
                            {
                                options.GrandIdAuthenticateServiceKey = Configuration.GetValue<string>("ActiveLogin:GrandId:SameDeviceServiceKey");
                            })
                            .AddOtherDevice(GrandIdAuthenticationDefaults.OtherDeviceAuthenticationScheme, "GrandID (OtherDevice)", options =>
                            {
                                options.GrandIdAuthenticateServiceKey = Configuration.GetValue<string>("ActiveLogin:GrandId:OtherDeviceServiceKey");
                            })
                            .AddChooseDevice(GrandIdAuthenticationDefaults.ChooseDeviceAuthenticationScheme, "GrandID (ChooseDevice)", options =>
                            {
                                options.GrandIdAuthenticateServiceKey = Configuration.GetValue<string>("ActiveLogin:GrandId:ChooseDeviceServiceKey");
                            });


                    if (Configuration.GetValue("ActiveLogin:GrandId:UseDevelopmentEnvironment", false))
                    {
                        builder.UseDevelopmentEnvironment();
                    }
                    else if (Configuration.GetValue("ActiveLogin:GrandId:UseTestEnvironment", false))
                    {
                        builder.UseTestEnvironment(Configuration.GetValue<string>("ActiveLogin:GrandId:ApiKey"));
                    }
                    else
                    {
                        builder.UseProductionEnvironment(Configuration.GetValue<string>("ActiveLogin:GrandId:ApiKey"));
                    }
                });
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
