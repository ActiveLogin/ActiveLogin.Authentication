# Getting started with BankID

## Dependencies

The BankID packages has UI that uses classes from [Bootstrap 4](https://getbootstrap.com/), please make sure these styles are available on the page for the expected UI.

## Preparation

BankID requires you to use a client certificate and trust a specific root CA-certificate.

1. Read through the [BankID Relying Party Guidelines](https://www.bankid.com/bankid-i-dina-tjanster/rp-info). This ensures you have a basic understanding of the terminology as well as how the flow and security works.
1. Download the SSL certificate for test ([FPTestcert2.pfx](https://www.bankid.com/bankid-i-dina-tjanster/rp-info)).
1. Contact a [reseller](https://www.bankid.com/kontakt/foeretag/saeljare) to get your very own client certificate for production. This will probably take a few business days to get sorted. Please ask for "Direktupphandlad BankID" as they otherwise might refer you to GrandID.
1. The root CA-certificates specified in _BankID Relying Party Guidelines_ (#7 for Production and #8 for Test environment) needs to be trusted at the computer where the app will run. Save those certificates as `BankIdRootCertificate-Prod.crt` and `BankIdRootCertificate-Test.crt`.
    1. If running in Azure App Service, where trusting custom certificates is not supported, there are extensions to handle that scenario. Instead of trusting the certificate, place it in your web project and make sure `CopyToOutputDirectory` is set to `Always`.
    1. Add the following configuration values. The `FilePath` should point to the certificate you just added, for example:

```json
{
    "ActiveLogin:BankId:CaCertificate:FilePath": "Certificates\\BankIdRootCertificate-[Test or Prod].crt"
}
```

__Note:__ When using MacOS, path strings use ```'/'``` for subfolders: ```"Certificates/BankIdRootCertificate-[Test or Prod].crt"```

### Storing certificates in Azure

These are only necessary if you plan to store your certificates in Azure KeyVault (recommended) and use the extension for easy integration with BankID.

[![Deploy to Azure](https://azuredeploy.net/deploybutton.png)](https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2FActiveLogin%2FActiveLogin.Authentication%2Fmaster%2Fsamples%2FAzureProvisioningSample%2FActiveLogin.json)

1. Deploy Azure KeyVault to your subscription. The ARM-template available in [AzureProvisioningSample](https://github.com/ActiveLogin/ActiveLogin.Authentication/tree/master/samples/AzureProvisioningSample)  contains configuration that creates a KeyVault and enables [Managed Service Identity](https://azure.microsoft.com/en-us/resources/samples/app-service-msi-keyvault-dotnet/) for the App Service.
1. [Import the certificates](https://docs.microsoft.com/en-us/azure/key-vault/certificate-scenarios#import-a-certificate) to your Azure Key Vault.
1. Add the following to your config, the secret identifier and auth settings.

```json
{
    "ActiveLogin:BankId:ClientCertificate": {
        "UseManagedIdentity": true,

        "AzureAdTenantId": "",
        "AzureAdClientId": "",
        "AzureAdClientSecret": "",

        "AzureKeyVaultUri": "TODO-ADD-YOUR-VALUE",
        "AzureKeyVaultSecretName": "TODO-ADD-YOUR-VALUE"
    }
}
```

#### Certificates are secrets

When configuring the AzureKeyVaultSecretName, the name is retrieved from the _Certificates_ rather than _Secrets_ in the Azure Portal. It is called a _secret_ in the API since this is how Azure Key Vault exposes certificates with private keys.

You can read more about the reasoning behind this [in this blog post](https://azidentity.azurewebsites.net/post/2018/07/03/azure-key-vault-certificates-are-secrets) or in the very extensive [official documentation](https://docs.microsoft.com/en-gb/azure/key-vault/about-keys-secrets-and-certificates#BKMK_CompositionOfCertificate).

#### Authentication using Managed Identity

If possible, you should use [Azure AD Managed Identity](https://docs.microsoft.com/en-us/azure/active-directory/managed-identities-azure-resources/overview) to connect to Azure KeyVault. To use Managed Identity, set `UseManagedIdentity` to `true`.

#### Authentication using Client Credentials

If Managed Identity can't be used, you can authenticate to Azure Key Vault using Client Credentials. Then set `UseManagedIdentity` to `false` and instead set values for `AzureAdTenantId`, `AzureAdClientId` and `AzureAdClientSecret`.

## Environments

### Simulated environment

For trying out quickly (without the need of certificates) you can use an in-memory implementation of the API by using `.UseSimulatedEnvironment()`. This could also be good when writing tests.

### Simulated environment with no config

```csharp
services
    .AddAuthentication()
    .AddBankId(builder =>
    {
        builder
            .UseSimulatedEnvironment()
            .AddSameDevice()
            .AddOtherDevice()
            .UseQrCoderQrCodeGenerator();
    })
```

### Development environment with custom person info

The faked name and personal identity number can also be customized like this.

```csharp
services
    .AddAuthentication()
    .AddBankId(builder =>
    {
        builder
            .UseSimulatedEnvironment("Alice", "Smith", "199908072391")
            .AddSameDevice()
            .AddOtherDevice()
            .UseQrCoderQrCodeGenerator();
    });
```

### Production environment

This will use the real REST API for BankID, connecting to either the Test or Production environment. It requires you to have the certificates described under _Preparation_ above.

```csharp
services.AddAuthentication()
        .AddBankId(builder =>
    {
        builder
            .UseProductionEnvironment()
            ...
    });
```

### Test

These samples uses the production environment, to use the test environment, simply swap `.UseProductionEnvironment()` with `.UseTestEnvironment()`. You will also have to use a different client and root certificate, see info under _Preparation_ above.

```csharp
services.AddAuthentication()
        .AddBankId(builder =>
    {
        builder
            .UseTestEnvironment()
            ...
    });
```

## Samples

### Using client certificate from Azure KeyVault

```csharp
services.AddAuthentication()
        .AddBankId(builder =>
    {
        builder
            .UseProductionEnvironment()
            .UseClientCertificateFromAzureKeyVault(Configuration.GetSection("ActiveLogin:BankId:ClientCertificate"))
            ...
    });
```

### Using client certificate from custom source

```csharp
services.AddAuthentication()
        .AddBankId(builder =>
    {
        builder
            .UseProductionEnvironment()
            .UseClientCertificate(() => new X509Certificate2( ... ))
            ...
    });
```

### Using root ca certificate

BankID uses a self signed root ca certificate that you need to trust. This is not possible in all scenarios, like in Azure App Service. To solve this there is an extension available to trust a custom root certificate using code. It can be used like this.

```csharp
services.AddAuthentication()
        .AddBankId(builder =>
    {
        builder
            .UseProductionEnvironment()
            .UseRootCaCertificate(Path.Combine(_environment.ContentRootPath, Configuration.GetValue<string>("ActiveLogin:BankId:CaCertificate:FilePath")))
            ...
    });
```

### Adding schemas

* *Same device*: Launches the BankID app on the same device, no need to enter any personal identity number.
* *Other device*: You enter your personal identity number and can manually launch the app on your smartphone.

```csharp
services
    .AddAuthentication()
    .AddBankId(builder =>
    {
        builder
            .UseProductionEnvironment()
            ...
            .AddSameDevice()
            .AddOtherDevice();
    });
```

### Customizing schemas

By default, `Add*Device` will use predefined schemas and display names, but they can be changed.

```csharp
services
    .AddAuthentication()
    .AddBankId(builder =>
    {
        builder
            .UseProductionEnvironment()
            ...
            .AddSameDevice("custom-auth-scheme", "Custom display name", options => { ... })
            .AddOtherDevice(BankIdDefaults.OtherDeviceAuthenticationScheme, "Custom display name", options => { ... });
    });
```

### Custom schema

If you want to roll your own, complete custom config, that can be done using `.AddCustom()`.

```csharp
services
    .AddAuthentication()
    .AddBankId(builder =>
    {
        builder
            ...
            .AddCustom(options => {
                options.BankIdAutoLaunch = true;
                options.BankIdAllowChangingPersonalIdentityNumber = false;
            });
    });
```

### Customizing BankID

BankId options allows you to set and override some options such as these.

```csharp
.AddOtherDevice(options =>
{
    // If the user can use biometric identification such as fingerprint or face recognition
    options.BankIdAllowBiometric = false;

    // Limit possible login methods to, for example, only allow BankID on smartcard.
    options.BankIdCertificatePolicies = BankIdCertificatePolicies.GetPoliciesForProductionEnvironment(...);

    // Issue birthdate claim based on data extracted from the personal identity number
    options.IssueBirthdateClaim = true;

    // Issue gender claim based on data extracted from the personal identity number
    options.IssueGenderClaim = true;

    // Turn off qr code and use personal identity number instead
    options.BankIdUseQrCode = false;
});
```

If you want to apply some options for all BankID schemes, you can do so by using `.Configure(...)`.

```csharp
.Configure(options =>
{
    options.IssueBirthdateClaim = true;
    options.IssueGenderClaim = true;
});
```

### Setting the return URL for cancellation

The defaults for cancellation are as follows:

* Same Device Scheme returns to scheme selection
* Other Device Scheme returns to scheme selection when using QR codes
* Other Device Scheme returns to PIN input when using PIN input instead of QR codes

It is possible to override the default navigation when cancelling an authentication request. The URL used for navigation is set through the `cancelReturnUrl` item in the `AuthenticationProperties` passed in the authentication challenge.

```csharp
var props = new AuthenticationProperties
{
    RedirectUri = Url.Action(nameof(ExternalLoginCallback)),
    Items =
    {
        { "returnUrl", "~/" },
        { "cancelReturnUrl", "~/some-custom-cancellation-url" },
        { "scheme", provider }
    }
};

return Challenge(props, provider);
```

### Custom QR code generation

By default the `ActiveLogin.Authentication.BankId.AspNetCore.Qr` package is needed to generate QR codes using the `UseQrCoderQrCodeGenerator` extension method.

If you wish to provide your own implementation of QR code generation simply implement the `IBankIdQrCodeGenerator` interface and add your implementation as a service.

```csharp
services.AddTransient<IBankIdQrCodeGenerator, CustomQrCodeGenerator>();
```

### BankID Certificate Policies

BankId options allows you to set a list of certificate policies and there is a class available to help you out with this.

```csharp
.AddOtherDevice(options =>
{
	options.BankIdCertificatePolicies = BankIdCertificatePolicies.GetPoliciesForProductionEnvironment(BankIdCertificatePolicy.BankIdOnFile, BankIdCertificatePolicy.MobileBankId);
});
```

Because the policies have different values for test and production environment, you need to use either `.GetPoliciesForProductionEnvironment()` or `.GetPoliciesForTestEnvironment()` depending on what environment you are using.

Example:

```csharp
.AddOtherDevice(options =>
{
	var policies = new[] { BankIdCertificatePolicy.BankIdOnFile, BankIdCertificatePolicy.MobileBankId };
	if(isProductionEnvironment) {
		options.BankIdCertificatePolicies = BankIdCertificatePolicies.GetPoliciesForProductionEnvironment(policies);
	} else {
		options.BankIdCertificatePolicies = BankIdCertificatePolicies.GetPoliciesForTestEnvironment(policies);
	}
});
```

### Store data on auth completion

When the login flow is completed and the collect request to BankID returns data, any class implementing `IBankIdResultStore` registered in the DI will be called.
There is a shorthand method (`AddResultStore`) on the BankIdBuilder to register the implementation.

*Sample implementation:*

```csharp
public class BankIdResultSampleLoggerStore : IBankIdResultStore
{
    private readonly EventId _eventId = new EventId(101, "StoreCollectCompletedCompletionData");
    private readonly ILogger<BankIdResultTraceLoggerStore> _logger;

    public BankIdResultSampleLoggerStore(ILogger<BankIdResultTraceLoggerStore> logger)
    {
        _logger = logger;
    }

    public Task StoreCollectCompletedCompletionData(string orderRef, CompletionData completionData)
    {
        _logger.LogTrace(_eventId, "Storing completion data for OrderRef '{OrderRef}' (UserPersonalIdentityNumber: '{UserPersonalIdentityNumber}')", orderRef, completionData.User.PersonalIdentityNumber);

        return Task.CompletedTask;
    }
}

services
    .AddAuthentication()
    .AddBankId(builder =>
    {
        builder
            //...
            .AddResultStore<BankIdResultSampleLoggerStore>();
    });
```

The default implementation will log all data to the tracelog. If you want to remove that implementation, remove any class implementing `IBankIdResultStore` from the ASP.NET Core services in your `Startup.cs`:

```csharp
services.RemoveAll(typeof(IBankIdResultStore));
```

### Resolve the end user ip

In some scenarios, like running behind a proxy, you might want to resolve the end user IP yourself and override the default implementaion.

Either register a class implementing `IEndUserIpResolver`:

```csharp
builder.UseEndUserIpResolver<EndUserIpResolver>();
```

Or use the shorthand version:

```csharp
builder.UseEndUserIpResolver(httpContext =>
{
    return httpContext.Connection.RemoteIpAddress.ToString();
});
```

### Full sample for production

Finally, a full sample on how to use BankID in production with client certificate from Azure KeyVault and trusting a custom root certificate.

```csharp
services
    .AddAuthentication()
    .AddBankId(builder =>
    {
        builder
            .UseProductionEnvironment()
            .UseClientCertificateFromAzureKeyVault(Configuration.GetSection("ActiveLogin:BankId:ClientCertificate"))
            .UseRootCaCertificate(Path.Combine(_environment.ContentRootPath, Configuration.GetValue<string>("ActiveLogin:BankId:CaCertificate:FilePath")))
            .AddSameDevice(options => { })
            .AddOtherDevice(options => { })
            .UseQrCoderQrCodeGenerator();
    });
```

## FAQ

### How do I customize the UI?

The UI is bundled into the package as a Razor Class Library, a technique that allows to [override the parts you want to customize](https://docs.microsoft.com/en-us/aspnet/core/razor-pages/ui-class?view=aspnetcore-2.1&tabs=visual-studio#override-views-partial-views-and-pages). The Views and Controllers that can be customized can be found in the [GitHub repo](https://github.com/ActiveLogin/ActiveLogin.Authentication/tree/master/src/ActiveLogin.Authentication.BankId.AspNetCore/Areas/BankIdAuthentication).

### Can the messages be localized?

The messages are already localized to English and Swedish using the official recommended texts. To select what texts that are used you can for example use the [localization middleware in ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/localization?view=aspnetcore-2.1#localization-middleware).
