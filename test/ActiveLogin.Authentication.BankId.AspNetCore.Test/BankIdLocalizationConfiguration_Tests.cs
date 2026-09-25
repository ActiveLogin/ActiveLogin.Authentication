using System.Globalization;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

using Xunit;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Test;

public class BankIdLocalizationConfiguration_Tests
{
    [Fact]
    public void AddBankIdLocalization_Resolves_ActiveLoginResources_Localizer()
    {
        var services = new ServiceCollection();
        services.AddSingleton<Microsoft.Extensions.Logging.ILoggerFactory>(NullLoggerFactory.Instance);

        services.AddBankIdLocalization();

        var provider = services.BuildServiceProvider();

        var localizer = provider.GetRequiredService<IStringLocalizer<ActiveLoginResources>>();

        Assert.NotNull(localizer);

        // Verify the embedded default-culture resource is actually found, not just that a localizer object exists.
        var defaultCultureValue = localizer["Cancel_Button"];
        Assert.False(defaultCultureValue.ResourceNotFound);
        Assert.Equal("Cancel", defaultCultureValue.Value);

        // Verify the embedded Swedish resource is also found.
        var originalCulture = CultureInfo.CurrentUICulture;
        try
        {
            CultureInfo.CurrentUICulture = new CultureInfo("sv");

            var swedishValue = localizer["Cancel_Button"];
            Assert.False(swedishValue.ResourceNotFound);
            Assert.Equal("Avbryt", swedishValue.Value);
        }
        finally
        {
            CultureInfo.CurrentUICulture = originalCulture;
        }
    }

    [Fact]
    public void AddBankIdLocalization_Does_Not_Overwrite_Hosts_Shared_LocalizationOptions()
    {
        const string hostResourcesPath = "HostAppResources";

        var services = new ServiceCollection();
        services.AddSingleton<Microsoft.Extensions.Logging.ILoggerFactory>(NullLoggerFactory.Instance);

        // Simulate a host application configuring its own localization before Active Login is added.
        services.AddLocalization(options => options.ResourcesPath = hostResourcesPath);

        services.AddBankIdLocalization();

        var provider = services.BuildServiceProvider();

        // The host's shared LocalizationOptions must remain unaffected by Active Login's localization setup.
        var localizationOptions = provider.GetRequiredService<IOptions<LocalizationOptions>>();
        Assert.Equal(hostResourcesPath, localizationOptions.Value.ResourcesPath);

        // Active Login's own resources should still resolve correctly, isolated from the host's options.
        var activeLoginLocalizer = provider.GetRequiredService<IStringLocalizer<ActiveLoginResources>>();
        Assert.NotNull(activeLoginLocalizer);
    }

    [Fact]
    public void AddBankIdLocalization_Registered_Before_Hosts_Localization_Does_Not_Overwrite_It()
    {
        const string hostResourcesPath = "HostAppResources";

        var services = new ServiceCollection();
        services.AddSingleton<Microsoft.Extensions.Logging.ILoggerFactory>(NullLoggerFactory.Instance);

        // Simulate Active Login being added before the host configures its own localization.
        services.AddBankIdLocalization();

        services.AddLocalization(options => options.ResourcesPath = hostResourcesPath);

        var provider = services.BuildServiceProvider();

        var localizationOptions = provider.GetRequiredService<IOptions<LocalizationOptions>>();
        Assert.Equal(hostResourcesPath, localizationOptions.Value.ResourcesPath);

        // Active Login's own resources should still resolve correctly, even though the host
        // configured its own (different) ResourcesPath after Active Login was registered.
        var activeLoginLocalizer = provider.GetRequiredService<IStringLocalizer<ActiveLoginResources>>();
        Assert.NotNull(activeLoginLocalizer);
    }
}
