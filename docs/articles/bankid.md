# ActiveLogin.Authentication.BankId

ActiveLogin.Authentication enables an application to support Swedish BankID (svenskt BankID) authentication and signing in .NET.

The most common scenbario is to use Active Login for BankID auth/login, so most of the concepts will be described from that perspective. We've designed sign to follow the same patterns and amke sure we can share things like certificate handling etc.

## Table of contents

* [Getting started](#getting-started)
  + [1. Preparation](#1-preparation)
  + [2. Read the documentation](#2-read-the-documentation)
  + [3. Install the NuGet package](#3-install-the-nuget-package)
  + [3. Prepare your project](#3-prepare-your-project)
  + [4. Get started in development](#4-get-started-in-development)
  + [5. Use test or production environments](#5-use-test-or-production-environments)
  + [6. Monitoring](#6-monitoring)
* [Environments](#environments)
  + [Simulated environment](#simulated-environment)
  + [Simulated environment with no config](#simulated-environment-with-no-config)
  + [Simulated environment with custom person info](#simulated-environment-with-custom-person-info)
  + [Test environment](#test-environment)
  + [Production environment](#production-environment)
  + [Full sample for production](#full-sample-for-production)
* [Sign](#sign)
* [Basic configuration samples](#basic-configuration-samples)
  + [Using client certificate from Azure KeyVault](#using-client-certificate-from-azure-keyvault)
  + [Using client certificate from custom source](#using-client-certificate-from-custom-source)
  + [Adding schemas](#adding-schemas)
  + [Customizing schemas](#customizing-schemas)
  + [Custom schema](#custom-schema)
  + [Customizing BankID options](#customizing-bankid-options)
* [Concepts](#concepts)
  + [Storing certificates in Azure](#storing-certificates-in-azure)
  + [Claims Issuing](#claims-issuing)
  + [BankID Certificate Policies](#bankid-certificate-policies)
  + [Return URL for cancellation](#return-url-for-cancellation)
  + [Handle missing or invalid state cookie](#handle-missing-or-invalid-state-cookie)
  + [Multi tenant scenario](#multi-tenant-scenario)
  + [Customize the UI](#customize-the-ui)
  + [Event listeners](#event-listeners)
  + [Store data on auth completion](#store-data-on-auth-completion)
  + [Resolve the end user ip](#resolve-the-end-user-ip)
  + [Resolve user data on Auth request](#resolve-user-data-on-auth-request)
  + [Custom QR code generation](#custom-qr-code-generation)
  + [Custom browser detection and launch info](#custom-browser-detection-and-launch-info)
  + [Verify digital ID card](#verify-digital-id-card)
  + [Use api wrapper only](#use-api-wrapper-only)
  + [Running on Linux](#running-on-linux)
  + [Localization](#localization)
  + [Names of the person might be capitalized](#names-of-the-person-might-be-capitalized)
  + [Cookies issued](#cookies-issued)
  + [Browser support](#browser-support)


---


## Getting started

### 1. Preparation

#### Certificates

BankID requires you to use a client certificate and trust a specific root CA-certificate.

Read through the [BankID Relying Party Guidelines](https://www.bankid.com/utvecklare/guider). This ensures you have a basic understanding of the terminology as well as how the flow and security works.

_For test:_ We have (with the permission from BankID) embedded the SSL certificate ([FPTestcert3_20200618.pfx](https://www.bankid.com/utvecklare/guider)) in the library.
_For production:_ Contact a [reseller](https://www.bankid.com/foretag/anslut-foeretag) to get your very own client certificate for production. This will probably take a few business days to get sorted. Please ask for "Direktupphandlad BankID" as they otherwise might refer you to a broker/partner. If you haven't decided on using BankID, but want to try it out anyway there are test- and simulation possibilities. See Environments below.

The root CA-certificates specified in _BankID Relying Party Guidelines_ (#7 for Production and #8 for Test environment) needs to be trusted at the computer where the app will run. We have (with the permission from BankID) embedded these in the library for easy verification.

### 2. Read the documentation

It is expected that you have a basic understanding of how [ASP.NET](https://docs.microsoft.com/en-us/aspnet/core/), [ASP.NET MVC](https://docs.microsoft.com/en-us/aspnet/core/mvc/overview) and [ASP.NET Authentication](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/identity) works before getting started.


Active Login is designed to make it very easy to get started with BankID, but in the end you are responsible for making sure that you are complient with the technical guidelines and/or legal agreements.

Therefore, before you start using Active Login, please read the documentation relevant to your needs. This will also make sure you understand the concepts better.

- [BankID (Swedish)](https://www.bankid.com/utvecklare/guider)
- [BankID (English)](https://www.bankid.com/en/utvecklare/guider)


### 3. Install the NuGet package

ActiveLogin.Authentication is distributed as [packages on NuGet](https://www.nuget.org/profiles/ActiveLogin), install using the tool of your choice, for example _dotnet cli_.

```console
dotnet add package ActiveLogin.Authentication.BankId.AspNetCore
```


### 3. Prepare your project

The authentication modules for BankID is registered in your `Program.cs`. Depending on your setup, you will probably have to configure challenge and callbacks in `AccountController.cs` or similar.

For the UI to work, it expects there to be a `_Layout.cshtml` available so that it can render within at `@RenderBody()`.

The BankID packages have UI is not dependent on any UI library, but the samples uses [Bootstrap](https://getbootstrap.com/), please make sure these styles are available in the `_Layout.cshtml`.

Our Samples might give you an inspiration on how to do all these.

### 4. Get started in development

BankID requires you to sign an agreement and receive a certificate used to identity you as a company. To get started and try it out the experience there is a simulated environment options available that uses an in-memory implementation. Great for development and testing.

```csharp
services
    .AddBankId(bankId =>
    {
        bankId
            .AddDebugEventListener()
            .UseSimulatedEnvironment();
    });

services
    .AddAuthentication()
    .AddBankIdAuth(bankId =>
    {
        bankId
            .AddSameDevice();
    });
```


### 5. Use test or production environments

To authenticate using a real BankID you need to receive the certificate. See details under Preperation above.

Samples on how to use them in production are:

```csharp
services
    .AddBankId(bankId =>
    {
        bankId
            .AddApplicationInsightsEventListener(options =>
            {
                options.LogUserPersonalIdentityNumberHints = true;
            })
            .UseProductionEnvironment()
            .UseClientCertificateFromAzureKeyVault(configuration.GetSection("ActiveLogin:BankId:ClientCertificate"))
            .AddSameDevice()
            .AddOtherDevice()
            .UseQrCoderQrCodeGenerator()
            .UseUaParserDeviceDetection();
    });

services
    .AddAuthentication()
    .AddBankIdAuth(bankId =>
    {
        bankId
            .UseProductionEnvironment();
    });
```

___Note:___ `.AddApplicationInsightsEventListener()` requires the [ActiveLogin.Authentication.BankId.AzureMonitor](https://www.nuget.org/packages/ActiveLogin.Authentication.BankId.AzureMonitor/) package.

___Note:___ `.UseQrCoderQrCodeGenerator()` requires the [ActiveLogin.Authentication.BankId.QRCoder](https://www.nuget.org/packages/ActiveLogin.Authentication.BankId.QRCoder/) package.

___Note:___ `.UseUaParserDeviceDetection()` requires the [ActiveLogin.Authentication.BankId.UAParser](https://www.nuget.org/packages/ActiveLogin.Authentication.BankId.UAParser/) package.


### 6. Monitoring

Active Login provides a structured way of generating and logging events. These coould be monitored to get statistics and health status of your BankID login method.

Read more on the topic in [Active Login Monitor](monitor.md).

![Active Login Monitor](https://alresourcesprod.blob.core.windows.net/docsassets/active-login-monitor-screenshot_1.png)


---


## Environments


### Simulated environment

For trying out quickly (without the need of certificates) you can use an in-memory implementation of the API by using `.UseSimulatedEnvironment()`. This could also be good when writing tests.


### Simulated environment with no config

```csharp
services
    .AddBankId(bankId =>
    {
        bankId.UseSimulatedEnvironment();
    });
```


### Simulated environment with custom person info

The faked name and personal identity number can also be customized like this.

```csharp
services
    .AddBankId(bankId =>
    {
        bankId.UseSimulatedEnvironment("Alice", "Smith", "199908072391")
    });
```


### Test environment

This will use the real REST API for BankID, connecting to the Test environment.

It will automatically register both the root and client certificate, even though this behaviour can be disabled. A scenario might be that you want to use the same flow for both test and prod and therefore make sure that fetching the certificate from KeyVault works by trying that out for test.

```csharp
services
    .AddBankId(bankId =>
    {
        bankId.UseTestEnvironment();
    });
```

Disable adding the certificates:

```csharp
services
    .AddBankId(bankId =>
    {
        bankId.UseTestEnvironment(useBankIdRootCertificate: false, useBankIdClientCertificate: false);
    });
```


### Production environment

This will use the real REST API for BankID, connecting to the Production environment. It requires you to have the client certificates described under _Preparation_ above.

```csharp
services
    .AddBankId(bankId =>
    {
        bankId.UseProductionEnvironment();
    });
```

Disable adding the root certificates:

```csharp
services
    .AddBankId(bankId =>
    {
        bankId.UseProductionEnvironment(useBankIdRootCertificate: false);
    });
```

### Full sample for production

Finally, a full sample on how to use BankID in production with client certificate from Azure KeyVault and trusting a custom root certificate.

```csharp
services
    .AddBankId(bankId =>
    {
        .UseProductionEnvironment()
        .UseClientCertificateFromAzureKeyVault(configuration.GetSection("ActiveLogin:BankId:ClientCertificate"))
        .UseQrCoderQrCodeGenerator()
        .UseUaParserDeviceDetection();
    });

services
    .AddAuthentication()
    .AddBankIdAuth(bankId =>
    {
        bankId
            .AddSameDevice()
            .AddOtherDevice();
    });
```

---

# Sign

Sign works very similar to auth, but can't utilize the "built in" support for schemes etc. So there are some differences.

At first, you need to register both the common BankID logic (environment, cert etc) as well as the sign specific configration (devices).

```csharp
// Add Active Login - BankID
services
    .AddBankId(bankId =>
    {
        bankId.AddDebugEventListener();
        bankId.UseQrCoderQrCodeGenerator();
        bankId.UseUaParserDeviceDetection();
        bankId.UseSimulatedEnvironment();
    });

// Add Active Login - Sign
services
    .AddBankIdSign(bankId =>
    {
        bankId.AddSameDevice(BankIdSignDefaults.SameDeviceConfigKey, "BankID (SameDevice)", options => { });
        bankId.AddOtherDevice(BankIdSignDefaults.OtherDeviceConfigKey, "BankID (OtherDevice)", options => { });
    });
```

Once that is done you will be able to use these services in your application, for example in your controller:

* `IBankIdSignConfigurationProvider` : List the registered configuraitons (SameDevice / Other Device)
* `IBankIdSignService` : Initiate and resulve the result of sign flow

Here is a minimal sample. See `Standalone.MvcSample` for more details.

```csharp
[AllowAnonymous]
public class SignController : Controller
{
    private readonly IBankIdSignConfigurationProvider _bankIdSignConfigurationProvider;
    private readonly IBankIdSignService _bankIdSignService;

    public SignController(IBankIdSignConfigurationProvider bankIdSignConfigurationProvider, IBankIdSignService bankIdSignService)
    {
        _bankIdSignConfigurationProvider = bankIdSignConfigurationProvider;
        _bankIdSignService = bankIdSignService;
    }

    public async Task<IActionResult> Index()
    {
        var configurations = await _bankIdSignConfigurationProvider.GetAllConfigurationsAsync();
        var providers = configurations
            .Where(x => x.DisplayName != null)
            .Select(x => new ExternalProvider(x.DisplayName ?? x.Key, x.Key));
        var viewModel = new BankIdViewModel(providers, "~/");

        return View(viewModel);
    }

    public IActionResult Sign(string provider)
    {
        var props = new BankIdSignProperties("The info displayed for the user") // The user visible data
        {
            UserNonVisibleData = new byte[1024], // Whataver data you want to sign
            UserVisibleDataFormat = BankIdUserVisibleDataFormats.SimpleMarkdownV1, // The format of the user visible data, use empty or the markwodn constant
            Items =
            {
                {"returnUrl", "~/"},
                {"scheme", provider}
            }
        };
        var returnPath = $"{Url.Action(nameof(Callback))}?provider={provider}";
        return this.BankIdInitiateSign(props, returnPath, provider);
    }

    [HttpPost]
    public async Task<IActionResult> Callback(string provider)
    {
        var result = await _bankIdSignService.GetSignResultAsync(provider);
        if (result?.Succeeded != true)
        {
            throw new Exception("Sign error");
        }

        // Parse these to store the signed values
        var ocspResponse = result.BankIdCompletionData?.OcspResponse;
        var signature = result.BankIdCompletionData?.Signature;

        return Redirect(result.Properties?.Items["returnUrl"] ?? "~/");
    }
}
```

---


## Basic configuration samples

### Using client certificate from Azure KeyVault

```csharp
services.AddBankId(bankId =>
    {
        bankId
            .UseProductionEnvironment()
            .UseClientCertificateFromAzureKeyVault(configuration.GetSection("ActiveLogin:BankId:ClientCertificate"))
            ...
    });
```


### Using client certificate from custom source

```csharp
services.AddBankId(bankId =>
    {
        bankId
            .UseProductionEnvironment()
            .UseClientCertificate(() => new X509Certificate2( ... ))
            ...
    });
```

### Adding schemas

* *Same device*: Launches the BankID app on the same device, no need to enter any personal identity number.
* *Other device*: The user manually launches the app the smartphone and scans the QR code.

```csharp
services
    .AddAuthentication()
    .AddBankIdAuth(bankId =>
    {
        bankId
            .AddSameDevice()
            .AddOtherDevice();
    });
```


### Customizing schemas

By default, `Add*Device` will use predefined schemas and display names, but they can be changed.

```csharp
services
    .AddAuthentication()
    .AddBankIdAuth(bankId =>
    {
        bankId
            .AddSameDevice("custom-auth-scheme", "Custom display name", options => { ... })
            .AddOtherDevice(BankIdDefaults.OtherDeviceAuthenticationScheme, "Custom display name", options => { ... });
    });
```

### Customizing BankID options

BankId options allows you to set and override some options such as these.

```csharp
.AddOtherDevice(options =>
{
    // If the client needs to provide MRTD (Machine readable travel document) information to complete the order.
    // Only Swedish passports and national ID cards are supported.
    options.BankIdRequireMrtd = true;

    // Users are required to sign the transaction with their PIN code, even if they have biometrics activated.
    options.BankIdRequirePinCode = true;

    // Limit possible login methods to, for example, only allow BankID on smartcard.
    options.BankIdCertificatePolicies = BankIdCertificatePolicies.GetPoliciesForProductionEnvironment(...);
});
```

If you want to apply some options for all BankID schemes, you can do so by using `.Configure(...)`.

```csharp
.Configure(options =>
{
    options.BankIdCertificatePolicies = BankIdCertificatePolicies.GetPoliciesForProductionEnvironment(...);
});
```


---


## Concepts

### Storing certificates in Azure

These are only necessary if you plan to store your certificates in Azure KeyVault (recommended) and use the extension for easy integration with BankID.

[![Deploy to Azure](https://azuredeploy.net/deploybutton.png)](https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2FActiveLogin%2FActiveLogin.Authentication%2Fmain%2Fsamples%2FAzureProvisioningSample%2FActiveLogin.json)

1. Deploy Azure KeyVault to your subscription. The ARM-template available in [AzureProvisioningSample](https://github.com/ActiveLogin/ActiveLogin.Authentication/tree/main/samples/AzureProvisioningSample)  contains configuration that creates a KeyVault and enables [Managed Service Identity](https://azure.microsoft.com/en-us/resources/samples/app-service-msi-keyvault-dotnet/) for the App Service.
1. [Import the certificates](https://docs.microsoft.com/en-us/azure/key-vault/certificate-scenarios#import-a-certificate) to your Azure Key Vault.
1. Add the following to your config, the secret identifier and auth settings.

```json
{
    "ActiveLogin:BankId:ClientCertificate": {
        "AzureKeyVaultUri": "TODO-ADD-YOUR-VALUE",
        "AzureKeyVaultSecretName": "TODO-ADD-YOUR-VALUE"
    }
}
```

#### Certificates are secrets

When configuring the AzureKeyVaultSecretName, the name is retrieved from the _Certificates_ rather than _Secrets_ in the Azure Portal. It is called a _secret_ in the API since this is how Azure Key Vault exposes certificates with private keys.

You can read more about the reasoning behind this [in this blog post](https://azidentity.azurewebsites.net/post/2018/07/03/azure-key-vault-certificates-are-secrets) or in the very extensive [official documentation](https://docs.microsoft.com/en-gb/azure/key-vault/about-keys-secrets-and-certificates#BKMK_CompositionOfCertificate).

#### KeyVault credentials

By default, the `DefaultAzureCredential` will be used as credentials. For info on how to use that, see [Microsoft docs](https://docs.microsoft.com/en-us/dotnet/api/azure.identity.defaultazurecredential). The minimal configuration then looks like this:

```json
{
    "ActiveLogin:BankId:ClientCertificate" {
        "AzureKeyVaultUri": "TODO-ADD-YOUR-VALUE",
        "AzureKeyVaultSecretName": "TODO-ADD-YOUR-VALUE"
    }
}
```

You can override the specific managed identity client id to use:

```json
{
    "ActiveLogin:BankId:ClientCertificate" {
        "AzureKeyVaultUri": "TODO-ADD-YOUR-VALUE",
        "AzureKeyVaultSecretName": "TODO-ADD-YOUR-VALUE",

        "AzureManagedIdentityClientId": ""
    }
}
```

You can also override to use client credentials:

```json
{
    "ActiveLogin:BankId:ClientCertificate" {
        "AzureKeyVaultUri": "TODO-ADD-YOUR-VALUE",
        "AzureKeyVaultSecretName": "TODO-ADD-YOUR-VALUE",

        "AzureAdTenantId": "",
        "AzureAdClientId": "",
        "AzureAdClientSecret": ""
    }
}
```

They will be evaluated in the order:

1. `ClientSecretCredential` with `AzureAdTenantId` + `AzureAdClientId` + `AzureAdClientSecret` (if specified)
2. `DefaultAzureCredential` with `AzureManagedIdentityClientId` (if specified)
3. `DefaultAzureCredential`


### Claims Issuing

Active Login aims to issue the most relevant claims that can be extracted from the information provided by BankID. There are scenarios where you might like to change issued claims or add new ones yourself.
We've made the claims issuing pipeline pluggable so you can add your own transformer.

All of the default claims behaviour are implemented in `BankIdDefaultClaimsTransformer` and this is the only transformer added by default.

#### Implementing IBankIdClaimsTransformer

You are also able to create your own transformer by inheriting it from the interface `IBankIdClaimsTransformer`. `BankIdClaimsTransformationContext` will contain the relevant context, and also the already issued list of claims that you can transform.

Once implemented, register your implementation using:

```csharp
services
    .AddAuthentication()
    .AddBankIdAuth(bankId =>
    {
        bankId.AddSameDevice();
        bankId.AddClaimsTransformer<BankIdYourCustomClaimsTransformer>();
    });
```

The claims beeing issued by default have the names/keys specified in the public class `BankIdClaimTypes` so you can refer to them by these constants.


#### Example: Add orderref as txn claim

If the application that uses ActiveLogin BankId needs to keep an audit trail of the sign-in, the _txn_ claim could preferably be used for this.

From [OpenId Connect for Identity Assurance](https://openid.net/specs/openid-connect-4-identity-assurance-1_0.html):
> The txn Claim as defined in [RFC8417] is used in the context of this extension to build audit trails across the parties involved in an OpenID Connect transaction.

```csharp
public class BankIdTxnClaimsTransformer : IBankIdClaimsTransformer
{
    public Task TransformClaims(BankIdClaimsTransformationContext context)
    {
        context.AddClaim("txn", context.BankIdOrderRef);

        return Task.CompletedTask;
    }
}
```

__Note:__ If the _txn_ claim is issued, you are responsible for making sure to keep relevant audit informaiton given that session. See the OpenId Connect spec linked above for more information.


#### Example: Add birthdate and gender claims

It is possible to extract some information from the swedish personal identity number. In previous versions of Active Login this was a built in feature, but is now removed from the default set of claims beeing issued.
If you still are interested in such functionality, you can easily implement the functionality using the code below.

See information on the limitations of hint information in the [ActiveLogin.Identity readme](https://github.com/ActiveLogin/ActiveLogin.Identity#hints).

```csharp
public class BankIdPinHintClaimsTransformer : IBankIdClaimsTransformer
{
    private const string GenderJwtType = "gender";
    private const string BirthdateJwtType = "birthdate";

    public Task TransformClaims(BankIdClaimsTransformationContext context)
    {
        var personalIdentityNumber = PersonalIdentityNumber.Parse(context.PersonalIdentityNumber);

        // Add gender from gender hint
        // See https://github.com/ActiveLogin/ActiveLogin.Identity#hints for limitations
        var jwtGender = GetJwtGender(personalIdentityNumber.GetGenderHint());
        if (!string.IsNullOrEmpty(jwtGender))
        {
            context.AddClaim(GenderJwtType, jwtGender);
        }

        // Add birthdate from birthdate hint
        // See https://github.com/ActiveLogin/ActiveLogin.Identity#hints for limitations
        var jwtBirthdate = GetJwtBirthdate(personalIdentityNumber.GetDateOfBirthHint());
        context.AddClaim(BirthdateJwtType, jwtBirthdate);

        return Task.CompletedTask;
    }

    private static string GetJwtGender(Gender gender)
    {
        // Specified in: http://openid.net/specs/openid-connect-core-1_0.html#rfc.section.5.1
        return gender switch
        {
            Gender.Female => "female",
            Gender.Male => "male",

            _ => string.Empty,
        };
    }

    private static string GetJwtBirthdate(DateTime birthdate)
    {
        // Specified in: http://openid.net/specs/openid-connect-core-1_0.html#rfc.section.5.1
        return birthdate.Date.ToString("yyyy-MM-dd");
    }
}
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
	var policies = new[] {
            BankIdCertificatePolicy.BankIdOnFile,
            BankIdCertificatePolicy.MobileBankId
        };
	if(isProductionEnvironment) {
		options.BankIdCertificatePolicies = BankIdCertificatePolicies.GetPoliciesForProductionEnvironment(policies);
	} else {
		options.BankIdCertificatePolicies = BankIdCertificatePolicies.GetPoliciesForTestEnvironment(policies);
	}
});
```


### Return URL for cancellation

If a user cancels the login, the user will be redirected to the `cancelReturnUrl`.

The defaults for cancellation are as follows:

* Same Device Scheme returns to scheme selection
* Other Device Scheme returns to scheme selection when using QR codes

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


### Handle missing or invalid state cookie

If the user navigates directly to the BankdID status page (*/ActiveLogin/BankId/Auth*) the state cookie (*__ActiveLogin.BankIdState*) will be missing. If that happens, the flow will fail. By default, the user will be redirected back to the `cancelReturnUrl`, see [Setting the return URL for cancellation](#return-url-for-cancellation).

This behaviour can be overriden by implementing `IBankIdInvalidStateHandler` and adding that to the IOC-container.

A simple sample of such handler is:

```csharp
public class SampleInvalidStateHandler : IBankIdInvalidStateHandler
{
    public Task HandleAsync(HttpContext httpContext, BankIdInvalidStateContext invalidStateContext)
    {
        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

        return Task.CompletedTask;
    }
}
```


### Multi tenant scenario

With the current architecture of Active Login all services are registered "globally" and you can't call `.AddBankId()` more than once.
To run Active Login in a multi tenant scenario, where different customers should use different certificates, you could register multiple certificates and on runtime select the correct one per request. To register multiple certificates you need to use the `.AddClientCertificate...()` instead of `.UseClientCertificate...()` as the `.Use...()` version will overwrite any existing certificates registered with the http client handler.

With our current solution, this requires you to disable pooling of the `SocketsHttpHandler` so we've decided not to ship that code in the NuGet-package, but below you'll find a sample on how it could be configured. We hope to redesign this in the future.

___Note:___ The code below is a sample and because it disables `PooledConnection` it might (and will) have performance implications.

```csharp
internal static class BankIdBuilderExtensions
{
    public static IBankIdBuilder UseClientCertificateResolver(this IBankIdBuilder builder, Func<ServiceProvider, X509CertificateCollection, string, X509Certificate> configureClientCertificateResolver)
    {
        builder.ConfigureHttpClientHandler((serviceProvider, httpClientHandler) =>
        {
            httpClientHandler.PooledConnectionLifetime = TimeSpan.Zero;
            httpClientHandler.SslOptions.LocalCertificateSelectionCallback =
                (sender, host, certificates, certificate, issuers) => configureClientCertificateResolver(serviceProvider, certificates, host);
        });

        return builder;
    }
}

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        // ...
        services
            .AddBankId(bankId =>
            {
                bankId
                    .AddClientCertificateFromAzureKeyVault(configuration.GetSection("ActiveLogin:BankId:ClientCertificate1"))
                    .AddClientCertificateFromAzureKeyVault(configuration.GetSection("ActiveLogin:BankId:ClientCertificate2"))
                    .AddClientCertificateFromAzureKeyVault(configuration.GetSection("ActiveLogin:BankId:ClientCertificate3"))
                    .UseClientCertificateResolver((serviceCollection, certificates, hostname) =>
                    {
                        // Apply logic here to select the correct certificate
                        return certificates[0];
                    });

                // ...
            }
    }
}
```


### Customize the UI

Active Login comes with predefined views that you can use, but maybe you'd rather use your own views to customize layout or behavior.

The UI is bundled into the package as a Razor Class Library, a technique that allows to [override the parts you want to customize](https://docs.microsoft.com/en-us/aspnet/core/razor-pages/ui-class?view=aspnetcore-2.1&tabs=visual-studio#override-views-partial-views-and-pages). The Views and Controllers that can be customized can be found in the [GitHub repo](https://github.com/ActiveLogin/ActiveLogin.Authentication/tree/main/src/ActiveLogin.Authentication.BankId.AspNetCore/Areas/ActiveLogin).

To override the default UI your web project, create the following folder:
`Areas/ActiveLogin/Views/Shared`

In this folder, you can then create any of the partials and MVC will then discover your partials and use any of them before ours. It's still possible to call our partials if you still want to use them.

- `Init.cshtml`
- `_Wrapper.cshtml`
- `_Form.cshtml`
- `_Status.cshtml`
- `_Script.cshtml`
- `_Style.cshtml`
- `_Spinner.cshtml`

If you want, you can override the UI for Auth and Sign with different templates. Do so by placing the files in one of these folders:

* `Areas/ActiveLogin/Views/BankIdUiAuth`
* `Areas/ActiveLogin/Views/BankIdUiSign`

See [the MVC sample](https://github.com/ActiveLogin/ActiveLogin.Authentication/tree/main/samples/Standalone.MvcSample) to see this in action, as demonstrated [here](https://github.com/ActiveLogin/ActiveLogin.Authentication/tree/main/samples/Standalone.MvcSample/Areas/ActiveLogin/Views/BankIdUiAuth/_Wrapper.cshtml).


### Event listeners

During the login flow, quite a lot of things are happening and using our event listeners you can listen and act on those events. By implementing and regestering `IBankIdEventListener` you will be notified when an event occurs. A common scenario is logging. Multiple event listeners can be registered.

`BankIdEvent` is the base class for all events which all events will inherit from. Each event might (and in most cases will) have unique properties relevant for that specific event.

#### Event types

At the moment, we trigger the events listed below. They all have unique event properties relevant to the event type.

- AspNet
    - `BankIdAspNetChallengeSuccessEvent`
    - `BankIdAspNetAuthenticateSuccessEvent`
    - `BankIdAspNetAuthenticateFailureEvent`
- Auth
    - `BankIdAuthSuccessEvent`
    - `BankIdAuthErrorEvent`
- Sign
    - `BankIdSignSuccessEvent`
    - `BankIdSignErrorEvent`
- Collect
    - `BankIdCollectPendingEvent`
    - `BankIdCollectCompletedEvent`
    - `BankIdCollectFailureEvent`
    - `BankIdCollectErrorEvent`
- Cancel
    - `BankIdCancelSuccessEvent`
    - `BankIdCancelErrorEvent`

#### Sample implementation

```csharp
public class BankIdSampleEventListener : IBankIdEventListener
{
    public Task HandleAsync(BankIdEvent bankIdEvent)
    {
        Console.WriteLine($"{bankIdEvent.EventTypeName}: {bankIdEvent.EventSeverity}");
        return Task.CompletedTask;
    }
}
```

```csharp
services
    .AddBankId(bankId =>
    {
        bankId.AddEventListener<BankIdSampleEventListener>();
    });
```

#### Built in event listeners

##### BankIdDebugEventListener

`BankIdDebugEventListener` will listen for all events and write them as serialized JSON to the debug log using `ILogger.LogDebug(...)`.
Call `bankId.AddDebugEventListener()` to enable it. Good to have for local development to see all details about what is happening.

```csharp
services
    .AddBankId(bankId =>
    {
        bankId.AddDebugEventListener();
    });
```

##### BankIdApplicationInsightsEventListener

`BankIdApplicationInsightsEventListener` will listen for all events and write them to Application Insights.

Call `bankId.AddApplicationInsightsEventListener()` to enable it. Note that you can supply options to enable logging of metadata, such as personal identity number, age and IP.

___Note:___ This event listener is available is available through a separate package called `ActiveLogin.Authentication.BankId.AzureMonitor`.

```csharp
services
    .AddBankId(bankId =>
    {
        bankId.AddApplicationInsightsEventListener();
    });
```

By default it will use whatever InstrumentationKey is registered with the application. There are overloads available so you can customize this:

```csharp
services
    .AddBankId(bankId =>
    {
        bankId.AddApplicationInsightsEventListener("CUSTOM_CONNECTION_STRING");
    });
```

You can also customize what kind of data should be logged together with the Application Insight events. For example:


```csharp
services
    .AddBankId(bankId =>
    {
        bankId.AddApplicationInsightsEventListener(options =>
            {
                options.LogUserPersonalIdentityNumber = false;
                options.LogUserPersonalIdentityNumberHints = true;

                options.LogUserNames = false;

                options.LogDeviceIpAddress = false;
                options.LogDeviceUniqueHardwareId = true;
                options.LogUserBankIdIssueDate = true;

                // And more...
            });
    });
```


##### BankIdLoggerEventListener

`BankIdLoggerEventListener` will listen for all events and write them with a descriptive text to the log using `ILogger.Log(...)`.
This listener is registered by default on startup, se info below if you want to clear the default listeners.


#### Default registered event listeners

By default, two event listeners will be enabled:
- `BankIdLoggerEventListener` (Log all events to `ILogger`)
- `BankIdResultStoreEventListener` (Map the completion event for `IBankIdResultStore`, see info below under __Store data on auth completion__.)

If you want to remove those implementations, remove any class implementing `IBankIdEventListener` from the ASP.NET Core services in your `Startup.cs`:

```csharp
services.RemoveAll(typeof(IBankIdEventListener));
```


### Store data on auth completion

When the login flow is completed and the collect request to BankID returns data, any class implementing `IBankIdResultStore` registered in the DI will be called.
There is a shorthand method (`AddResultStore`) on the BankIdBuilder to register the implementation.

___Note:___ `IBankIdResultStore` is just a shorthand for the `BankIdCollectCompletedEvent` as described above.

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
    .AddBankId(bankId =>
    {
        bankId.AddResultStore<BankIdResultSampleLoggerStore>();
    });
```

The default implementation will log all data to the tracelog. If you want to remove that implementation, remove any class implementing `IBankIdResultStore` from the ASP.NET Core services in your `Startup.cs`:

```csharp
services.RemoveAll(typeof(IBankIdResultStore));
```


### Resolve the end user ip

In some scenarios, like running behind a proxy, you might want to resolve the end user IP yourself and override the default implementaion.

Either register a class implementing `IBankIdEndUserIpResolver`:

```csharp
services.AddTransient<IBankIdEndUserIpResolver, EndUserIpResolver>();
```

### Resolve user data on Auth request

BankID allows you to display a text during authentication to describe the intent. Active Login allows you to set these parameters when authenticating:

<img src="https://alresourcesprod.blob.core.windows.net/docsassets/active-login-bankid-uservisibledata-screenshot_1.jpg" width="250" alt="User visible data" />

* `UserVisibleData`
* `UserNonVisibleData`
* `UserVisibleDataFormat`

These can either be set as static data during startup in `Program.cs` or dynamically by overiding the interface `IBankIdAuthRequestUserDataResolver`.

Sample of static text without formatting:

```csharp
bankId.UseAuthRequestUserData(authUserData =>
{
    authUserData.UserVisibleData = "Login to your account at Active Login";
});
```

Sample of static text with formatting:

```csharp
bankId.UseAuthRequestUserData(authUserData =>
{
    var message = new StringBuilder();
    message.AppendLine("# Active Login");
    message.AppendLine();
    message.AppendLine("Welcome to the *Active Login* demo.");

    authUserData.UserVisibleData = message.ToString();
    authUserData.UserVisibleDataFormat = BankIdUserVisibleDataFormats.SimpleMarkdownV1;
});
```

For more advanced scenarios, you can generate the user data dynamically by implementing `IBankIdAuthRequestUserDataResolver`:

```csharp
public class BankIdAuthRequestDynamicUserDataResolver : IBankIdAuthRequestUserDataResolver
{
    public Task<BankIdAuthUserData> GetUserDataAsync(BankIdAuthRequestContext authRequestContext, HttpContext httpContext)
    {
        return Task.FromResult(new BankIdAuthUserData()
        {
            UserVisibleData = "*Time:* " + DateTime.Now.ToLongTimeString(),
            UserVisibleDataFormat = BankIdUserVisibleDataFormats.SimpleMarkdownV1
        });;
    }
}
```

```csharp
services.AddTransient<IBankIdAuthRequestUserDataResolver, BankIdAuthRequestDynamicUserDataResolver>();
```

___Note:___ For sign, user data is mandatory, and therefore part of the initiate flow.


### Custom QR code generation

By default the `ActiveLogin.Authentication.BankId.QRCoder` package is needed to generate QR codes using the `UseQrCoderQrCodeGenerator` extension method.

If you wish to provide your own implementation of QR code generation simply implement the `IBankIdQrCodeGenerator` interface and add your implementation as a service.

```csharp
services.AddTransient<IBankIdQrCodeGenerator, CustomQrCodeGenerator>();
```

### Custom browser detection and launch info

The functionality provided tries to detect the device by looking at the user agent. We need to know what device is used to launch the BankId app and this differs from iOS/Android/PC/Mac.

By implementing `IBankIdLauncher` you can customize exactly how to launch the app. It is very rare that you need to change this, but could be relevant if you use Active Login for authenticating a user in a native mobile app.

```csharp
services.AddTransient<IBankIdLauncher, CustomBankIdLauncher>();
```

By implementing `IBankIdSupportedDeviceDetector` you can override how the client device is detected. This info will be used by the default BankIdLauncher.

```csharp
services.AddTransient<IBankIdSupportedDeviceDetector, CustomBankIdSupportedDeviceDetector>();
```


#### Use UAParserDeviceDetector for device and browser detection

In Active Login device and browser detection is required for example to determine which URL to use to launch the BankID app, according to the BankID Relaying party Guidelines. This logic is primarily encapsulated into `IBankIdSupportedDeviceDetector`.

The default implementation provided in `ActiveLogin.Authentication.BankId.AspNetCore` is limited to supports the ~top 5 most common browsers on both iOS and Android. But since an incorrect browser detection can lead to an incorrect launch URL and result in a broken user flow, `UAParserDeviceDetector` in the `ActiveLogin.Authentication.BankId.UAParser` package should be used to support additional browsers. It has a dependency on package [uap-csharp](https://github.com/ua-parser/uap-csharp) for improved user agent parsing.

#### Shorthand for only overriding config for custom browsers

If you want to support your custom app, or a third party app (like the built in browsers in Instagram, Facebook etc.) we've made it simple to support those scenarios by allowing you to specify a custom browser config.

The most common scenario is that you will set the schema for the app as return URL if you detect a specific User Agent, so for that scenario we've made an extension method.

Note: The return url will onlt by applied on iOS, as Android will return the user to the app automatically.

In the sample below we add support for Instagram and Facebook:

```csharp
services
    .AddBankId(bankId =>
    {
        // ...

        bankId.AddCustomBrowserByUserAgent(userAgent => userAgent.Contains("Instagram"), "instagram://");
        bankId.AddCustomBrowserByUserAgent(userAgent => userAgent.Contains("FBAN") || userAgent.Contains("FBAV"), "fb://");

        // ...
    });
```

If you need, you can also specify the reload behaviour on the custom browser:

```csharp
services
    .AddBankId(bankId =>
    {
        // ...

        bankId.AddCustomBrowserByUserAgent(userAgent => userAgent.Contains("Instagram"), new BankIdLauncherUserAgentCustomBrowser("instagram://", BrowserReloadBehaviourOnReturnFromBankIdApp.Never));

        // ...
    });
```

If you need to do something custom, you can implement `IBankIdLauncherCustomBrowser`:

```csharp
services
    .AddBankId(bankId =>
    {
        // ...

        bankId.AddCustomBrowser<BankIdFacebookAppBrowserConfig>();

        // ...
    });
```

```csharp
public class BankIdFacebookAppBrowserConfig : IBankIdLauncherCustomAppCallback
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public BankIdFacebookAppCallback(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Task<bool> IsApplicable(BankIdLauncherCustomAppCallbackContext context)
    {
        var userAgent = _httpContextAccessor.HttpContext?.Request.Headers.UserAgent.FirstOrDefault();
        if (string.IsNullOrWhiteSpace(userAgent))
        {
            return Task.FromResult(false);
        }

        var isFacebook = userAgent.Contains("FBAN") || userAgent.Contains("FBAV");
        return Task.FromResult(isFacebook);
    }

    public Task<string> GetCustomAppReturnUrl(BankIdLauncherCustomAppCallbackContext context)
    {
        return Task.FromResult(
            new BankIdLauncherCustomAppCallbackResult("fb://", BrowserReloadBehaviourOnReturnFromBankIdApp.Never, BrowserMightRequireUserInteractionToLaunch.Default)
        );
    }
}

```

### Verify digital ID card

To use the API for "Verify digital ID card" you first need to register the BankID services, select an environment etc.

```csharp
services
    .AddBankId(bankId =>
    {
        bankId
            .AddDebugEventListener()
            .UseTestEnvironment();
    });
```

Then you can use the Verify API from, for example, an MVC Controller. The API allows you to send in the content of the QR-code and responds with the verification details.

In the example below the client (HTML/JS for example) have already decoded the QR-code.

```csharp
public class VerifyRequestModel
{
    public string QrCodeContent { get; set; } = string.Empty;
}

public class VerifyController : Controller
{
    private readonly IBankIdVerifyApiClient _bankIdVerifyApiClient;

    public VerifyController(IBankIdVerifyApiClient bankIdVerifyApiClient)
    {
        _bankIdVerifyApiClient = bankIdVerifyApiClient;
    }

    [HttpPost("/verify/api")]
    public async Task<ActionResult<string>> Verify([FromBody] VerifyRequestModel model)
    {
        // Minimalistic sample implementation

        ArgumentNullException.ThrowIfNull(model, nameof(model));
        if (string.IsNullOrEmpty(model.QrCodeContent))
        {
            throw new ArgumentNullException(nameof(model.QrCodeContent));
        }

        var verifyResult = await _bankIdVerifyApiClient.VerifyAsync(model.QrCodeContent);
        return verifyResult.User.PersonalIdentityNumber;
    }
}
```

### Use api wrapper only

We have seperated the API-wrapper for BankID into a separate package so that you can use it in other scenarios we have not covered. They look like this and are both well documented using XML-comments.

The constructor for these ApiClients takes an `HttpClient` and you need to configure that `HttpClient` with a `BaseAddress`, `Tls12`, client certificates etc. depending on your needs.

For easy use the APIs you register the BankID services, select an environment etc. and then the APIs are ready to be injected using IoC.



```csharp
services
    .AddBankId(bankId =>
    {
        bankId
            .AddDebugEventListener()
            .UseTestEnvironment();
    });
```

___Note:___ The `BankIdApiClient` class below is available in the `ActiveLogin.Authentication.BankId.Api` package.

*App API:*
```csharp
public class BankIdAppApiClient : IBankIdAppApiClient
{
    public Task<AuthResponse> AuthAsync(AuthRequest request) { ... }
    public Task<SignResponse> SignAsync(SignRequest request) { ... }
    public Task<CollectResponse> CollectAsync(CollectRequest request) { ... }
    public Task<CancelResponse> CancelAsync(CancelRequest request) { ... }
}
```

*Verify API:*
```csharp
public class BankIdVerifyApiClient : IBankIdVerifyApiClient
{
    public Task<VerifyResponse> VerifyAsync(VerifyRequest request) { ... }
}
```


### Localization

The messages are already localized to English and Swedish using the official recommended texts. To select what language that is used you can for example use the [localization middleware in ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/localization#localization-middleware).

The user messages that will be displayed are provided through the implementation of `IBankIdUserMessageLocalizer`, so by overriding this you can customize the messages.

```csharp
services.AddTransient<IBankIdUserMessageLocalizer, CustomBankIdUserMessageLocalizer>();
```


### Names of the person might be capitalized

The names comes from the bank that the end user has, and some banks (due to legacy) stores all of the names in all caps (like `ALICE SMITH`).

We have choosen not to normalize the capitalization of the names as it´s hard or impossible to do so in a general way.


### Cookies issued

The `*.AspNetCore` package will issue a cookie to make the auth flow work

The cookie is called: `__ActiveLogin.BankIdUiState`

The cookie is there to store state during the auth process, as the user will/might be redirected during the flow. The cookie is session based only and will be deleted once the auth process is finished and/or when the user closes the browser.

Because it is strictly related to temp storage during auth, you should not have to inform the user about these specific cookies (according to the [EU "cookie law"](https://www.cookielaw.org/the-cookie-law/)).

With the current implementaiton (following the convention from Microsoft ASP.NET) the usage of cookies is not optional.

A more technical deep dive of the cookies can be found in [this issue](https://github.com/ActiveLogin/ActiveLogin.Authentication/issues/156).


### Browser support

We aim at supporting the latest version of all major browsers both on desktop and on mobile.

All browsers on mobile are supported to show the UI, but the redirect flow have been tested and verified on these:
- iOS
    - Safari
    - Chrome
    - Edge
    - Firefox
    - Opera Touch
- Android
    - Chrome
    - Firefox
    - Edge
    - Samsung Internet
    - Opera Mini

___Note:___ Brave on iOS/Android identifies as Safari or Chrome for privacy reasons and will get wrong configuration, so the redirect flow will fail.

___Note:___ If you aim to support IE11 a polyfill for some JavaScript features we are using is needed.

* [Fetch](https://developer.mozilla.org/en-US/docs/Web/API/Fetch_API): https://github.com/github/fetch
