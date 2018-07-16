using System.IO;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using ActiveLogin.Authentication.BankId.Api;
using ActiveLogin.Authentication.BankId.AspNetCore;
using IdentityServer4;
using IdentityServerSample.Certificates;
using Microsoft.AspNetCore.Authentication;
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

            services.AddMvc(options => { options.RespectBrowserAcceptHeader = true; });

            services.AddIdentityServer()
                    .AddDeveloperSigningCredential()
                    .AddInMemoryIdentityResources(Config.GetIdentityResources())
                    .AddInMemoryClients(Config.GetClients(Configuration.GetSection("ActiveLogin:Clients")));

            var authentication = services.AddAuthentication(IdentityServerConstants.DefaultCookieAuthenticationScheme);
            ConfigureBankId(services, authentication);
        }

        private void ConfigureBankId(IServiceCollection services, AuthenticationBuilder authentication)
        {
            // Fake API
            var bankIdConfig = Configuration.GetSection("ActiveLogin:BankId");
            if (bankIdConfig.GetValue("UseFakeApi", false))
            {
                services.AddSingleton<IBankIdApiClient>(x => new FakeBankIdApiClient("Fake", "User"));
            }

            // Get ca certificate
            var bankIdCaCertificateConfig = bankIdConfig.GetSection("CaCertificate");
            var bankIdCaCertificate = GetCaCertificate(bankIdCaCertificateConfig.GetValue<string>("FilePath"));

            // Get client certificate
            var bankIdClientCertificateConfig = bankIdConfig.GetSection("ClientCertificate");
            var bankIdClientCertficiate = GetClientCertificate(
                bankIdClientCertificateConfig.GetValue<string>("AzureAd:ClientId"),
                bankIdClientCertificateConfig.GetValue<string>("AzureAd:ClientSecret"),
                bankIdClientCertificateConfig.GetValue<string>("AzureKeyVault:BaseUrl"),
                bankIdClientCertificateConfig.GetValue<string>("AzureKeyVault:SecretName")
            );

            // Add BankId
            authentication.AddBankId(bankIdClientCertficiate, handler => ConfigureHttpClientHandler(handler, bankIdCaCertificate));

            // Set BankID endpoint settings
            services.AddHttpClient<IBankIdApiClient, BankIdApiClient>(client =>
            {
                if (bankIdConfig.GetValue("UseTestApiEndpoint", false))
                {
                    client.BaseAddress = BankIdUrls.TestApiBaseUrl;
                }
            });
        }

        private X509Certificate2 GetClientCertificate(string azureAdClientId, string azureAdClientSecret, string keyVaultBaseUrl, string keyVaultSecretName)
        {
            using (var keyVaultCertificateClient = new AzureKeyVaultCertificateClient(azureAdClientId, azureAdClientSecret))
            {
                return keyVaultCertificateClient.GetX509Certificate2Async(keyVaultBaseUrl, keyVaultSecretName).GetAwaiter().GetResult();
            }
        }

        private X509Certificate2 GetCaCertificate(string caCertificateFilePath)
        {
            var certificateFilePath = Path.Combine(_environment.ContentRootPath, caCertificateFilePath);
            return new X509Certificate2(certificateFilePath);
        }

        private void ConfigureHttpClientHandler(HttpClientHandler handler, X509Certificate2 caCertficiate)
        {
            var caValidator = new X509CertificateChainValidator(caCertficiate);
            handler.ServerCertificateCustomValidationCallback = caValidator.Validate;
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
