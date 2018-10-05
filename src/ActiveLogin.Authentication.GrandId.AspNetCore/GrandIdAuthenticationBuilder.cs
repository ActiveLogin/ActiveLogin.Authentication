using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Authentication;
using ActiveLogin.Authentication.GrandId.Api;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace ActiveLogin.Authentication.GrandId.AspNetCore
{
    public class GrandIdAuthenticationBuilder
    {
        public AuthenticationBuilder AuthenticationBuilder { get; }

        private readonly List<Action<HttpClient>> _httpClientConfigurators = new List<Action<HttpClient>>();
        private readonly List<Action<HttpClientHandler>> _httpClientHandlerConfigurators = new List<Action<HttpClientHandler>>();

        public GrandIdAuthenticationBuilder(AuthenticationBuilder authenticationBuilder)
        {
            AuthenticationBuilder = authenticationBuilder;

            AddHttpClient(AuthenticationBuilder.Services, _httpClientConfigurators, _httpClientHandlerConfigurators);

            ConfigureHttpClient(httpClient => httpClient.BaseAddress = GrandIdUrls.ProdApiBaseUrl);
            ConfigureHttpClientHandler(httpClientHandler => httpClientHandler.SslProtocols = SslProtocols.Tls12);
        }

        public void ConfigureHttpClient(Action<HttpClient> configureHttpClient)
        {
            _httpClientConfigurators.Add(configureHttpClient);
        }

        public void ConfigureHttpClientHandler(Action<HttpClientHandler> configureHttpClientHandler)
        {
            _httpClientHandlerConfigurators.Add(configureHttpClientHandler);
        }

        private static void AddHttpClient(IServiceCollection services, List<Action<HttpClient>> httpClientConfigurators, List<Action<HttpClientHandler>> httpClientHandlerConfigurators)
        {
            services.AddHttpClient<IGrandIdApiClient, GrandIdApiClient>(httpClient =>
                {
                    httpClientConfigurators.ForEach(configurator => configurator(httpClient));
                })
                .ConfigurePrimaryHttpMessageHandler(() =>
                {
                    var httpClientHandler = new HttpClientHandler();
                    httpClientHandlerConfigurators.ForEach(configurator => configurator(httpClientHandler));
                    return httpClientHandler;
                });
        }
    }
}