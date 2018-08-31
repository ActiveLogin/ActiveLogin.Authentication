using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Authentication;
using ActiveLogin.Authentication.GrandId.Api;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ActiveLogin.Authentication.GrandId.AspNetCore
{
    public class GrandIdAuthenticationBuilder
    {
        public AuthenticationBuilder AuthenticationBuilder { get; }
        public string Name { get; }

        private readonly List<Action<HttpClient>> _httpClientConfigurators = new List<Action<HttpClient>>();
        private readonly List<Action<HttpClientHandler>> _httpClientHandlerConfigurators = new List<Action<HttpClientHandler>>();

        public IServiceCollection Services;

        public GrandIdAuthenticationBuilder(AuthenticationBuilder authenticationBuilder, string name)
        {
            AuthenticationBuilder = authenticationBuilder;
            Name = name;

            Services = AuthenticationBuilder.Services;

            AddGrandIdHttpClient(Services, Name, _httpClientConfigurators, _httpClientHandlerConfigurators);

            ConfigureGrandIdHttpClient(httpClient => httpClient.BaseAddress = GrandIdUrls.ProdApiBaseUrl);
            ConfigureGrandIdHttpClientHandler(httpClientHandler => httpClientHandler.SslProtocols = SslProtocols.Tls12);
        }

        public void ConfigureGrandIdHttpClient(Action<HttpClient> configureHttpClient)
        {
            AuthenticationBuilder.Services.TryAddTransient<GrandIdApiClient>();
            _httpClientConfigurators.Add(configureHttpClient);
        }

        public void ConfigureGrandIdHttpClientHandler(Action<HttpClientHandler> configureHttpClientHandler)
        {
            AuthenticationBuilder.Services.TryAddTransient<GrandIdApiClient>();
            _httpClientHandlerConfigurators.Add(configureHttpClientHandler);
        }
            
        private static void AddGrandIdHttpClient(IServiceCollection services, string name, List<Action<HttpClient>> httpClientConfigurators, List<Action<HttpClientHandler>> httpClientHandlerConfigurators)
        {
            services.AddHttpClient<IGrandIdApiClient, GrandIdApiClient>(name, httpClient =>
                {
                    httpClientConfigurators.ForEach(configurator => configurator(httpClient));
                })
                .ConfigurePrimaryHttpMessageHandler(() =>
                {
                    var httpClientHandler = new HttpClientHandler();
                    httpClientHandlerConfigurators.ForEach(configurator => configurator(httpClientHandler));
                    return httpClientHandler;
                });

            services.TryAddTransient<IGrandIdApiClient, GrandIdApiClient>();
        }
    }
}