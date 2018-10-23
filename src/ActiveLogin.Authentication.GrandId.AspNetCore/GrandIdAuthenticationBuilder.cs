using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Authentication;
using ActiveLogin.Authentication.GrandId.Api;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace ActiveLogin.Authentication.GrandId.AspNetCore
{
    public class GrandIdAuthenticationBuilder : IGrandIdAuthenticationBuilder
    {
        public AuthenticationBuilder AuthenticationBuilder { get; }

        private readonly List<Action<HttpClient>> _httpClientConfigurators = new List<Action<HttpClient>>();
        private readonly List<Action<HttpClientHandler>> _httpClientHandlerConfigurators = new List<Action<HttpClientHandler>>();

        public GrandIdAuthenticationBuilder(AuthenticationBuilder authenticationBuilder)
        {
            AuthenticationBuilder = authenticationBuilder;

            ConfigureHttpClient(httpClient => httpClient.BaseAddress = GrandIdUrls.ProductionApiBaseUrl);
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

        public void EnableHttpClient()
        {
            AuthenticationBuilder.Services.AddHttpClient<IGrandIdApiClient, GrandIdApiClient>(httpClient =>
                {
                    _httpClientConfigurators.ForEach(configurator => configurator(httpClient));
                })
                .ConfigurePrimaryHttpMessageHandler(() =>
                {
                    var httpClientHandler = new HttpClientHandler();
                    _httpClientHandlerConfigurators.ForEach(configurator => configurator(httpClientHandler));
                    return httpClientHandler;
                });
        }
    }
}