using System;
using System.Collections.Generic;
using System.Globalization;
using ActiveLogin.Authentication.BankId.AspNetCore;
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
            //                .UseDevelopmentEnvironment("Alice", "Smith")
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
            //                    options.AuthenticateServiceKey = Configuration.GetValue<string>("ActiveLogin:GrandId:ChooseDeviceServiceKey");
            //                });
            //        });

            // Sample of using BankID with in memory dev environment
            services.AddAuthentication()
                    .AddBankId(builder =>
                    {
                        builder
                            .UseDevelopmentEnvironment("Alice", "Smith")
                            .AddCustom(options =>
                            {
                                options.BankIdAllowChangingPersonalIdentityNumber = false;
                                options.BankIdCertificatePolicies = "BankIdCertificatePolicies";
                            });
                    });

            // Sample of using BankID with production environment
            //services.AddAuthentication()
            //        .AddBankId(builder =>
            //        {
            //            builder
            //                .UseProductionEnvironment()
            //                .UseBankIdClientCertificateFromAzureKeyVault(Configuration.GetSection("ActiveLogin:BankId:ClientCertificate"))
            //                .UseBankIdRootCaCertificate(Path.Combine(_environment.ContentRootPath, Configuration.GetValue<string>("ActiveLogin:BankId:CaCertificate:FilePath")))
            //                .AddCustom(options =>
            //                {
            //                    // ...
            //                });
            //        });
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
