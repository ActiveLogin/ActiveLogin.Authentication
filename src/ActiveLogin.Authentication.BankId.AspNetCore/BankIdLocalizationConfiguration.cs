using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ActiveLogin.Authentication.BankId.AspNetCore;

internal static class BankIdLocalizationConfiguration
{
    /// <summary>
    /// Registers localization for Active Login's own UI resources.
    /// </summary>
    /// <remarks>
    /// This intentionally avoids calling <c>services.AddLocalization(options => options.ResourcesPath = ...)</c>.
    /// <see cref="LocalizationOptions"/> is a single, application-wide option shared by every
    /// <see cref="IStringLocalizer{T}"/> resolved through the default <see cref="IStringLocalizerFactory"/>.
    /// Configuring it here would override (or be overridden by) the host application's own
    /// localization configuration, breaking the host's translations. Instead, a dedicated
    /// <see cref="IStringLocalizerFactory"/>/<see cref="IStringLocalizer{T}"/> is registered
    /// specifically for <see cref="ActiveLoginResources"/>, isolated from the host's settings.
    /// </remarks>
    public static void AddBankIdLocalization(this IServiceCollection services)
    {
        services.AddSingleton<IStringLocalizer<ActiveLoginResources>>(sp =>
        {
            var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
            var options = Options.Create(new LocalizationOptions
            {
                ResourcesPath = BankIdConstants.LocalizationResourcesPath
            });
            var factory = new ResourceManagerStringLocalizerFactory(options, loggerFactory);

            return new StringLocalizer<ActiveLoginResources>(factory);
        });
    }
}
