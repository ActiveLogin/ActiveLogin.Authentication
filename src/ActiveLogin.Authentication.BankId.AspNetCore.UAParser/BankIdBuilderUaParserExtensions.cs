using ActiveLogin.Authentication.BankId.AspNetCore;
using ActiveLogin.Authentication.BankId.AspNetCore.SupportedDevice;
using ActiveLogin.Authentication.BankId.AspNetCore.UAParser;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class BankIdBuilderUaParserExtensions
    {
        /// <summary>
        /// Use UAParserDeviceDetector instead of the default device detector BankIdSupportedDeviceDetector.
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IBankIdBuilder UseUaParserDeviceDetection(this IBankIdBuilder builder)
        {
            var services = builder.AuthenticationBuilder.Services;
            services.TryAddTransient<IBankIdSupportedDeviceDetector, UAParserDeviceDetector>();

            return builder;
        }
    }
}
