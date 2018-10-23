using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Authentication;
using ActiveLogin.Authentication.BankId.Api;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ActiveLogin.Authentication.BankId.AspNetCore
{
    public class BankIdAuthenticationBuilder : IBankIdAuthenticationBuilder
    {
        public AuthenticationBuilder AuthenticationBuilder { get; }

        private readonly List<Action<HttpClient>> _httpClientConfigurators = new List<Action<HttpClient>>();
        private readonly List<Action<HttpClientHandler>> _httpClientHandlerConfigurators = new List<Action<HttpClientHandler>>();

        public BankIdAuthenticationBuilder(AuthenticationBuilder authenticationBuilder)
        {
            AuthenticationBuilder = authenticationBuilder;

            var services = AuthenticationBuilder.Services;

            AddBankIdHttpClient(services, _httpClientConfigurators, _httpClientHandlerConfigurators);

            ConfigureBankIdHttpClient(httpClient => httpClient.BaseAddress = BankIdUrls.ProdApiBaseUrl);
            ConfigureBankIdHttpClientHandler(httpClientHandler => httpClientHandler.SslProtocols = SslProtocols.Tls12);
        }

        public void ConfigureBankIdHttpClient(Action<HttpClient> configureHttpClient)
        {
            _httpClientConfigurators.Add(configureHttpClient);
        }

        public void ConfigureBankIdHttpClientHandler(Action<HttpClientHandler> configureHttpClientHandler)
        {
            _httpClientHandlerConfigurators.Add(configureHttpClientHandler);
        }
            
        private static void AddBankIdHttpClient(IServiceCollection services, List<Action<HttpClient>> httpClientConfigurators, List<Action<HttpClientHandler>> httpClientHandlerConfigurators)
        {
            services.AddHttpClient<IBankIdApiClient, BankIdApiClient>(httpClient =>
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