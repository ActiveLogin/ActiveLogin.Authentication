﻿using System;
using ActiveLogin.Authentication.BankId.Api;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ActiveLogin.Authentication.BankId.AspNetCore
{
    public static class BankIdAuthenticationBuilderEnvironmentExtensions
    {
        public static IBankIdAuthenticationBuilder UseEnvironment(this IBankIdAuthenticationBuilder builder, Action<BankIdEnvironmentConfiguration> configureBankIdEnvironment)
        {
            var configuration = new BankIdEnvironmentConfiguration();
            configureBankIdEnvironment(configuration);
            builder.ConfigureBankIdHttpClient(httpClient =>
            {
                httpClient.BaseAddress = configuration.ApiBaseUrl;
            });

            builder.AddBankIdApiClient();

            return builder;
        }

        public static IBankIdAuthenticationBuilder UseTestEnvironment(this IBankIdAuthenticationBuilder builder)
        {
            builder.UseEnvironment(configuration =>
            {
                configuration.ApiBaseUrl = BankIdUrls.TestApiBaseUrl;
            });

            return builder;
        }

        public static IBankIdAuthenticationBuilder UseProductionEnvironment(this IBankIdAuthenticationBuilder builder)
        {
            builder.UseEnvironment(configuration =>
            {
                configuration.ApiBaseUrl = BankIdUrls.ProdApiBaseUrl;
            });

            return builder;
        }

        public static IBankIdAuthenticationBuilder UseDevelopmentEnvironment(this IBankIdAuthenticationBuilder builder)
        {
            builder.AuthenticationBuilder.Services.AddSingleton<IBankIdApiClient>(x => new BankIdDevelopmentApiClient());

            return builder;
        }

        public static IBankIdAuthenticationBuilder UseDevelopmentEnvironment(this IBankIdAuthenticationBuilder builder, string givenName, string surname)
        {
            builder.AuthenticationBuilder.Services.AddSingleton<IBankIdApiClient>(x => new BankIdDevelopmentApiClient(givenName, surname));

            return builder;
        }

        private static IBankIdAuthenticationBuilder AddBankIdApiClient(this IBankIdAuthenticationBuilder builder)
        {
            builder.AuthenticationBuilder.Services.TryAddTransient<IBankIdApiClient, BankIdApiClient>();

            return builder;
        }
    }
}