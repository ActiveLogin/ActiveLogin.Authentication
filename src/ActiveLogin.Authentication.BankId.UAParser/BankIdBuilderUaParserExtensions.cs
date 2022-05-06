using ActiveLogin.Authentication.BankId.Core;
using ActiveLogin.Authentication.BankId.Core.SupportedDevice;
using ActiveLogin.Authentication.BankId.UaParser;

using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection;
public static class BankIdBuilderUaParserExtensions
{
    /// <summary>
    /// Use UAParserDeviceDetector instead of the default device detector BankIdSupportedDeviceDetector.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IBankIdBuilder UseUaParserDeviceDetection(this IBankIdBuilder builder)
    {
        var services = builder.Services;
        var descriptor = new ServiceDescriptor(typeof(IBankIdSupportedDeviceDetectorByUserAgent),typeof(UAParserDeviceDetector), ServiceLifetime.Transient);
        services.Replace(descriptor);

        return builder;
    }
}
