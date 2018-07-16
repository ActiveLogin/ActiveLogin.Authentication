using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using ActiveLogin.Authentication.BankId.Api;
using ActiveLogin.Authentication.BankId.AspNetCore;
using IdentityServerSample.Certificates;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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

            services.AddAuthentication()
                .AddBankId()
                    .AddBankIdClientCertificate(() =>
                    {
                        var azureAdClientId = Configuration.GetValue<string>("ActiveLogin:BankId:ClientCertificate:AzureAd:ClientId");
                        var azureAdClientSecret = Configuration.GetValue<string>("ActiveLogin:BankId:ClientCertificate:AzureAd:ClientSecret");
                        var keyVaultBaseUrl = Configuration.GetValue<string>("ActiveLogin:BankId:ClientCertificate:AzureKeyVault:BaseUrl");
                        var keyVaultSecretName = Configuration.GetValue<string>("ActiveLogin:BankId:ClientCertificate:AzureKeyVault:SecretName");

                        using (var keyVaultCertificateClient = new AzureKeyVaultCertificateClient(azureAdClientId, azureAdClientSecret))
                        {
                            return keyVaultCertificateClient.GetX509Certificate2Async(keyVaultBaseUrl, keyVaultSecretName).GetAwaiter().GetResult();
                        }
                    })
                    .AddBankIdRootCaCertificate(() =>
                    {
                        var certificateFilePath = Path.Combine(_environment.ContentRootPath, Configuration.GetValue<string>("ActiveLogin:BankId:CaCertificate:FilePath"));
                        return new X509Certificate2(certificateFilePath);
                    })
                    .AddBankIdEnvironmentConfiguration(configuration =>
                    {
                        if (Configuration.GetValue("ActiveLogin:BankId:UseTestApiEndpoint", false))
                        {
                            configuration.ApiBaseUrl = BankIdUrls.TestApiBaseUrl;
                        }
                    });

            // Fake BankID API
            if (Configuration.GetValue("ActiveLogin:BankId:UseFakeApi", false))
            {
                services.AddSingleton<IBankIdApiClient>(x => new FakeBankIdApiClient("Fake", "User"));
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

            // BankID Authentication needs areas to be registered for the UI to work
            app.UseMvc(routes =>
            {
                routes.MapRoute("areas", "{area}/{controller=Home}/{action=Index}/{id?}");
                routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
