# Getting started with BankID

## Preparation

BankID requires you to use a client certificate and trust a specific root CA-certificate. To try out the module without those certificates.

1. Read through the [BankID Relying Party Guidelines](https://www.bankid.com/bankid-i-dina-tjanster/rp-info). This ensures you have a basic understanding of the terminology as well as how the flow and security works.
1. Download the _SSL certificate for test [FPTestcert2.pfx](https://www.bankid.com/bankid-i-dina-tjanster/rp-info).
1. Contact a [reseller](https://www.bankid.com/kontakt/foeretag/saeljare) to get your very own client certificate for production. This will probably take a few business days to get sorted. Please ask for "Direktupphandlad BankID" as they otherwise might refer you to GrandID.
1. The root CA-certificates specified in _BankID Relying Party Guidelines_ (#7 for Production and #8 for Test environment) needs to be trusted at the computer where the app will run. Save those certificates as `BankIdRootCertificate-Prod.crt` and `BankIdRootCertificate-Test.crt`.
    1. If running in Azure App Service, where trusting custom certificates is not supported, there are extensions to handle that scenario. Instead of trusting the certificate, place it in your web project and make sure `CopyToOutputDirectory` is set to `Always`.
    1. Add the following configuration values. The `FilePath` should point to the certificate you just added, for example:

```json
{
  "ActiveLogin:BankId:CaCertificate:FilePath": "Certificates\\BankIdRootCertificate-[Test or Prod].crt"
}
```

### Storing certificates in Azure

These are only necessary if you plan to store your certificates in Azure KeyVault (recommended) and use the extension for easy integration with BankID.

[![Deploy to Azure](https://azuredeploy.net/deploybutton.png)](https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fgithub.com%2FActiveLogin%2FActiveLogin.Authentication%2Ftree%2Fmaster%2Fsamples%2FAzureProvisioningSample%2FActiveLogin.json)

1. Deploy Azure KeyVault to yur subscription, the ARM-template available in [AzureProvisioningSample](https://github.com/ActiveLogin/ActiveLogin.Authentication/tree/master/samples/AzureProvisioningSample) contains a KeyVault, so preferably you deploy that.
1. [Import the certificates](https://docs.microsoft.com/en-us/azure/key-vault/certificate-scenarios#import-a-certificate) to your Azure Key Vault.
1. Add your KeyVault setup to your config. The AD client should have access to the KeyVault certificate specified in `AzureKeyVaultSecretIdentifier`.

```json
{
  "ActiveLogin:BankId:ClientCertificate:AzureAdClientId": "TODO-ADD-YOUR-VALUE",
  "ActiveLogin:BankId:ClientCertificate:AzureAdClientSecret": "TODO-ADD-YOUR-VALUE",
  "ActiveLogin:BankId:ClientCertificate:AzureKeyVaultSecretIdentifier": "TODO-ADD-YOUR-VALUE"
}
```

## Samples

### Development environment

For trying out quickly (without the need of certificates) you can use an in-memory implementation of the API by using `.UseDevelopmentEnvironment()`. This could also bee good when writing tests.

### Development environment predefined set of schemas

This is the simplest setup that will use the development environment and add the `SameDevice` and `OtherDevice` schemas.

```c#
services
    .AddAuthentication()
    .AddBankId();
```

### Development environment with no config

```c#
services
    .AddAuthentication()
    .AddBankId(builder =>
    {
        builder
            .UseDevelopmentEnvironment()
            .AddSameDevice(options => { })
            .AddOtherDevice(options => { });
    });
```

### Development environment with custom person info

The faked name and personal identity number can also be customized like this.

```c#
services
    .AddAuthentication()
    .AddBankId(builder =>
    {
        builder
            .UseDevelopmentEnvironment("Alice", "Smith", "199908072391")
            .AddSameDevice(options => { })
            .AddOtherDevice(options => { });
    });
```

## Test or production environment

This will use the real REST API for BankID, connecting to either the Test or Production environment. It requires you to have the certificates described under _Preparation_ above.

These samples uses the production environment, to use the test environment, simply swap `.UseProductionEnvironment()` with `.UseTestEnvironment()`.

### Using client certificate from Azure KeyVault

```c#
services.AddAuthentication()
        .AddBankId(builder =>
    {
        builder
            .UseProductionEnvironment()
            .UseClientCertificateFromAzureKeyVault(Configuration.GetSection("ActiveLogin:BankId:ClientCertificate"))
            ...
    });
```

## 1. Get started in development

##### 2.1.2 Try it out with Bank ID test environment

To start using a real implementation of BankID, there are a few steps to do. These steps describes the scenario where you utilize Azure for things like secure storage of the certificate in an Azure KeyVault.


5. Add the following to you `Startup.cs`:

```c#
services.AddAuthentication()
        .AddBankId(builder =>
    {
        builder
            .UseTestEnvironment()
            .UseClientCertificateFromAzureKeyVault(Configuration.GetSection("ActiveLogin:BankId:ClientCertificate"))
            .AddSameDevice(options => { })
            .AddOtherDevice(options => { });
    });
```


10. Right after `.UseClientCertificateFromAzureKeyVault(..)`, add the following line:

```c#
.UseRootCaCertificate(Path.Combine(_environment.ContentRootPath, Configuration.GetValue<string>("ActiveLogin:BankId:CaCertificate:FilePath")))
```

##### 2.1.3 Use production environment


```c#
services.AddAuthentication()
        .AddBankId(builder =>
    {
        builder
            .UseProductionEnvironment()
            .UseClientCertificateFromAzureKeyVault(Configuration.GetSection("ActiveLogin:BankId:ClientCertificate"))
            .UseRootCaCertificate(Path.Combine(_environment.ContentRootPath, Configuration.GetValue<string>("ActiveLogin:BankId:CaCertificate:FilePath")))
            .AddSameDevice(options => { })
            .AddOtherDevice(options => { });
    });
```

5. BankId options allows you to set and override some options such as these:

```c#
.AddOtherDevice(options =>
{
    options.BankIdAllowBiometric = false;
    options.BankIdCertificatePolicies = new List<string> { "1.2.752.78.1.1" };
});
```

6. Enjoy BankID in your application :)

## FAQ

### Can the UI be customized?

Yes! The UI is bundled into the package as a Razor Class Library, a technique that allows to [override the parts you want to customize](https://docs.microsoft.com/en-us/aspnet/core/razor-pages/ui-class?view=aspnetcore-2.1&tabs=visual-studio#override-views-partial-views-and-pages). The Views and Controllers that can be customized can be found in the [GitHub repo](https://github.com/ActiveLogin/ActiveLogin.Authentication/tree/master/src/ActiveLogin.Authentication.BankId.AspNetCore/Areas/BankIdAuthentication). 

### Can the messages be localized?

The messages are already localized to English and Swedish using the official recommended texts. To select what texts that are used you can for example use the [localization middleware in ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/localization?view=aspnetcore-2.1#localization-middleware).