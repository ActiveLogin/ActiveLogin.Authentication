using System.IO;
using System.Security.Cryptography.X509Certificates;
using ActiveLogin.Authentication.BankId.AspNetCore;
using ActiveLogin.Authentication.BankId.AspNetCore.Azure;
using ActiveLogin.Authentication.GrandId.AspNetCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace StandaloneMvcClientSample
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

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie()
                .AddBankId(builder =>
                {
                    builder.AddSameDevice(BankIdAuthenticationDefaults.SameDeviceAuthenticationScheme, "BankID (SameDevice)", options => { })
                           .AddOtherDevice(BankIdAuthenticationDefaults.OtherDeviceAuthenticationScheme, "BankID (OtherDevice)", options => { });

                    //if (Configuration.GetValue("ActiveLogin:BankId:UseDevelopmentEnvironment", false))
                    //{
                        builder.UseDevelopmentEnvironment();
                    //}
                    //else if (Configuration.GetValue("ActiveLogin:BankId:UseTestEnvironment", false))
                    //{
                    //    builder.UseTestEnvironment()
                    //           .UseClientCertificate(() => new X509Certificate2(Path.Combine(_environment.ContentRootPath, Configuration.GetValue<string>("ActiveLogin:BankId:ClientCertificate"))))
                    //           .UseRootCaCertificate(Path.Combine(_environment.ContentRootPath, Configuration.GetValue<string>("ActiveLogin:BankId:CaCertificate:FilePath")));
                    //}
                    //else
                    //{
                    //    builder.UseProductionEnvironment()
                    //           .UseClientCertificateFromAzureKeyVault(Configuration.GetSection("ActiveLogin:BankId:ClientCertificate"))
                    //           .UseRootCaCertificate(Path.Combine(_environment.ContentRootPath, Configuration.GetValue<string>("ActiveLogin:BankId:CaCertificate:FilePath")));
                    //}
                })
                .AddGrandId(builder =>
                {
                    builder.AddBankIdSameDevice(GrandIdAuthenticationDefaults.BankIdSameDeviceAuthenticationScheme, "GrandID (SameDevice)", options =>
                    {
                        options.GrandIdAuthenticateServiceKey = Configuration.GetValue<string>("ActiveLogin:GrandId:BankIdSameDeviceServiceKey");
                    })
                            .AddBankIdOtherDevice(GrandIdAuthenticationDefaults.BankIdOtherDeviceAuthenticationScheme, "GrandID (OtherDevice)", options =>
                            {
                                options.GrandIdAuthenticateServiceKey = Configuration.GetValue<string>("ActiveLogin:GrandId:BankIdOtherDeviceServiceKey");
                            })
                            .AddBankIdChooseDevice(GrandIdAuthenticationDefaults.BankIdChooseDeviceAuthenticationScheme, "GrandID (ChooseDevice)", options =>
                            {
                                options.GrandIdAuthenticateServiceKey = Configuration.GetValue<string>("ActiveLogin:GrandId:BankIdChooseDeviceServiceKey");
                            });


                    //if (Configuration.GetValue("ActiveLogin:GrandId:UseDevelopmentEnvironment", false))
                    //{
                        builder.UseDevelopmentEnvironment();
                    //}
                    //else if (Configuration.GetValue("ActiveLogin:GrandId:UseTestEnvironment", false))
                    //{
                    //    builder.UseTestEnvironment(Configuration.GetValue<string>("ActiveLogin:GrandId:ApiKey"));
                    //}
                    //else
                    //{
                    //    builder.UseProductionEnvironment(Configuration.GetValue<string>("ActiveLogin:GrandId:ApiKey"));
                    //}
                });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
