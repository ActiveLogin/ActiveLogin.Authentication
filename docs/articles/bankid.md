# ActiveLogin.Authentication.BankId

ActiveLogin.Authentication enables an application to support Swedish BankID (svenskt BankID) authentication in .NET.

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
* [Basic configuration samples](#basic-configuration-samples)
  + [Using client certificate from Azure KeyVault](#using-client-certificate-from-azure-keyvault)
  + [Using client certificate from custom source](#using-client-certificate-from-custom-source)
  + [Using root CA certificate](#using-root-ca-certificate)
  + [Adding schemas](#adding-schemas)
  + [Customizing schemas](#customizing-schemas)
  + [Custom schema](#custom-schema)
  + [Customizing BankID options](#customizing-bankid-options)
* [Concepts](#concepts)
  + [Storing certificates in Azure](#storing-certificates-in-azure)
  + [BankId claim types](#bankid-claim-types)
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

1. Read through the [BankID Relying Party Guidelines](https://www.bankid.com/utvecklare/guider). This ensures you have a basic understanding of the terminology as well as how the flow and security works.
1. Download the SSL certificate for test ([FPTestcert2.pfx](https://www.bankid.com/utvecklare/guider)).
1. Contact a [reseller](https://www.bankid.com/foretag/anslut-foeretag) to get your very own client certificate for production. This will probably take a few business days to get sorted. Please ask for "Direktupphandlad BankID" as they otherwise might refer you to a broker/partner. If you haven't decided on using BankID, but want to try it out anyway there are test- and simulation possibilities. See Environments below.
1. The root CA-certificates specified in _BankID Relying Party Guidelines_ (#7 for Production and #8 for Test environment) needs to be trusted at the computer where the app will run. Save those certificates as `BankIdRootCertificate-Prod.crt` and `BankIdRootCertificate-Test.crt`.
    1. If running in Azure App Service, where trusting custom certificates [is not supported](https://azure.github.io/AppService/2021/06/22/Root-CA-on-App-Service-Guide.html), there are extensions available in our packages to handle that scenario. See documentation below. Instead of trusting the certificate, place it in your web project and make sure `CopyToOutputDirectory` is set to `Always`.
    1. Add the following configuration values. The `FilePath` should point to the certificate you just added, for example:

```json
{
    "ActiveLogin:BankId:CaCertificate:FilePath": "Certificates\\BankIdRootCertificate-[Test or Prod].crt"
}
```

___Note:___ When using MacOS or Linux, path strings use ```'/'``` for subfolders: ```"Certificates/BankIdRootCertificate-[Test or Prod].crt"```


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

The BankID packages have UI that uses classes from [Bootstrap 4](https://getbootstrap.com/), please make sure these styles are available in the `_Layout.cshtml`.

Our Samples might give you an inspiration on how to do all these.

### 4. Get started in development

BankID requires you to sign an agreement and receive a certificate used to identity you as a company. To get started and try it out the experience there is a simulated environment options available that uses an in-memory implementation. Great for development and testing.

```csharp
services
    .AddAuthentication()
    .AddBankId(builder =>
    {
        builder
            .AddDebugEventListener();
            .UseSimulatedEnvironment()
            .AddSameDevice();
    });
```


### 5. Use test or production environments

To authenticate using a real BankID you need to receive the certificate. See details under Preperation above.

Samples on how to use them in production are:

```csharp
services
    .AddAuthentication()
    .AddBankId(builder =>
    {
        builder
            .AddApplicationInsightsEventListener(options =>
            {
                options.LogUserPersonalIdentityNumberHints = true;
                options.LogCertificateDates = true;
            })
            .UseProductionEnvironment()
            .UseClientCertificateFromAzureKeyVault(configuration.GetSection("ActiveLogin:BankId:ClientCertificate"))
            .UseRootCaCertificate(Path.Combine(environment.ContentRootPath, configuration.GetValue<string>("ActiveLogin:BankId:CaCertificate:FilePath")))
            .AddSameDevice()
            .AddOtherDevice()
            .UseQrCoderQrCodeGenerator()
            .UseUaParserDeviceDetection();
    });
```

___Note:___ `.AddApplicationInsightsEventListener()` requires the [ActiveLogin.Authentication.BankId.AspNetCore.AzureMonitor](https://www.nuget.org/packages/ActiveLogin.Authentication.BankId.AspNetCore.AzureMonitor/) package.

___Note:___ `.UseQrCoderQrCodeGenerator()` requires the [ActiveLogin.Authentication.BankId.AspNetCore.QRCoder](https://www.nuget.org/packages/ActiveLogin.Authentication.BankId.AspNetCore.QRCoder/) package.

___Note:___ `.UseUaParserDeviceDetection()` requires the [ActiveLogin.Authentication.BankId.AspNetCore.UAParser](https://www.nuget.org/packages/ActiveLogin.Authentication.BankId.AspNetCore.UAParser/) package.


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
    .AddAuthentication()
    .AddBankId(builder =>
    {
        builder
            .UseSimulatedEnvironment()
            .AddSameDevice()
            ...
    })
```


### Simulated environment with custom person info

The faked name and personal identity number can also be customized like this.

```csharp
services
    .AddAuthentication()
    .AddBankId(builder =>
    {
        builder
            .UseSimulatedEnvironment("Alice", "Smith", "199908072391")
            .AddSameDevice()
            ...
    });
```


### Test environment

This will use the real REST API for BankID, connecting to the Test environment. It requires you to have the certificates described under _Preparation_ above.

```csharp
services.AddAuthentication()
        .AddBankId(builder =>
    {
        builder
            .UseTestEnvironment()
            .AddSameDevice()
            ...
    });
```


### Production environment

This will use the real REST API for BankID, connecting to the Production environment. It requires you to have the certificates described under _Preparation_ above.

```csharp
services.AddAuthentication()
        .AddBankId(builder =>
    {
        builder
            .UseProductionEnvironment()
            .AddSameDevice()
            ...
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
            .UseClientCertificateFromAzureKeyVault(configuration.GetSection("ActiveLogin:BankId:ClientCertificate"))
            .UseRootCaCertificate(Path.Combine(environment.ContentRootPath, configuration.GetValue<string>("ActiveLogin:BankId:CaCertificate:FilePath")))
            .AddSameDevice(options => { })
            .AddOtherDevice(options => { })
            .UseQrCoderQrCodeGenerator()
            .UseUaParserDeviceDetection();
    });
```

---


## Basic configuration samples

### Using client certificate from Azure KeyVault

```csharp
services.AddAuthentication()
        .AddBankId(builder =>
    {
        builder
            .UseProductionEnvironment()
            .UseClientCertificateFromAzureKeyVault(configuration.GetSection("ActiveLogin:BankId:ClientCertificate"))
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


### Using root CA certificate

BankID uses a self signed root ca certificate that you need to trust. This is not possible in all scenarios, like in [Azure App Service](https://azure.github.io/AppService/2021/06/22/Root-CA-on-App-Service-Guide.html). To solve this there is an extension available to trust a custom root certificate using code. It can be used like this.

```csharp
services.AddAuthentication()
        .AddBankId(builder =>
    {
        builder
            .UseProductionEnvironment()
            .UseRootCaCertificate(Path.Combine(environment.ContentRootPath, configuration.GetValue<string>("ActiveLogin:BankId:CaCertificate:FilePath")))
            ...
    });
```


### Adding schemas

* *Same device*: Launches the BankID app on the same device, no need to enter any personal identity number.
* *Other device*: The user manually launches the app the smartphone and scans the QR code.

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

If you want to roll your own, complete custom config, that can be done using `.AddCustom()`. This is not recomended and in most scenarios not needed.

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


### Customizing BankID options

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

### BankId claim types

The claims beeing issued have the names/keys specified in `BankIdClaimTypes`.


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


### Handle missing or invalid state cookie

If the user navigates directly to the BankdID status page (*/BankIdAuthentication/Login*) the state cookie (*__ActiveLogin.BankIdState*) will be missing. If that happens, the flow will fail. By default, the user will be redirected back to the `cancelReturnUrl`, see [Setting the return URL for cancellation](#return-url-for-cancellation).

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
To run Active Login in a multi tenant scenario, where different customers should use different certificates, you could register multiple certificates and on runtime select the correct one per request.
With our current solution, this requires you to disable pooling of the `SocketsHttpHandler` so we've decided not to ship that code in the NuGet-package, but below you'll find a sample on how it could be configured. We hope to redesign this in the future.

___Note:___ The code below is a sample and because it disables `PooledConnection` it might (and will) have performance implications.

```csharp
internal static class BankIdBuilderExtensions
{
    public static IBankIdBuilder UseClientCertificateResolver(this IBankIdBuilder builder, Func<ServiceProvider, X509CertificateCollection, string, X509Certificate> configureClientCertificateResolver)
    {
        builder.ConfigureHttpClientHandler(httpClientHandler =>
        {
            var services = builder.AuthenticationBuilder.Services;
            var serviceProvider = services.BuildServiceProvider();

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
        services.AddAuthentication()
            .AddBankId(builder =>
            {
                builder
                    .UseClientCertificateFromAzureKeyVault(configuration.GetSection("ActiveLogin:BankId:ClientCertificate1"))
                    .UseClientCertificateFromAzureKeyVault(configuration.GetSection("ActiveLogin:BankId:ClientCertificate2"))
                    .UseClientCertificateFromAzureKeyVault(configuration.GetSection("ActiveLogin:BankId:ClientCertificate3"))
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

The UI is bundled into the package as a Razor Class Library, a technique that allows to [override the parts you want to customize](https://docs.microsoft.com/en-us/aspnet/core/razor-pages/ui-class?view=aspnetcore-2.1&tabs=visual-studio#override-views-partial-views-and-pages). The Views and Controllers that can be customized can be found in the [GitHub repo](https://github.com/ActiveLogin/ActiveLogin.Authentication/tree/main/src/ActiveLogin.Authentication.BankId.AspNetCore/Areas/BankIdAuthentication).

To override the default UI your web project, create the following folder:
```Areas/BankIdAuthentication/Views/BankId```

In this folder, you can then create any of the partials and MVC will then discover your partials and use any of them before ours. It's still possible to call our partials if you still want to use them.

* `_Login.cshtml`
* `_LoginForm.cshtml`
* `_LoginScript.cshtml`
* `_LoginStatus.cshtml`
* `_LoginStyle.cshtml`

See [the MVC sample](https://github.com/ActiveLogin/ActiveLogin.Authentication/tree/main/samples/Standalone.MvcSample) to see this in action, as demonstrated [here](https://github.com/ActiveLogin/ActiveLogin.Authentication/tree/main/samples/Standalone.MvcSample/Areas/BankIdAuthentication/Views/BankId/_Login.cshtml).


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
    .AddAuthentication()
    .AddBankId(builder =>
    {
        builder
            ...
            .AddEventListener<BankIdSampleEventListener>();
    });
```

#### Built in event listeners

##### BankIdDebugEventListener

`BankIdDebugEventListener` will listen for all events and write them as serialized JSON to the debug log using `ILogger.LogDebug(...)`.
Call `builder.AddDebugEventListener()` to enable it. Good to have for local development to see all details about what is happening.

```csharp
services
    .AddAuthentication()
    .AddBankId(builder =>
    {
        builder
            ...
            .AddDebugEventListener();
    });
```

##### BankIdApplicationInsightsEventListener

`BankIdApplicationInsightsEventListener` will listen for all events and write them to Application Insights.

Call `builder.AddApplicationInsightsEventListener()` to enable it. Note that you can supply options to enable logging of metadata, such as personal identity number, age and IP.

___Note:___ This event listener is available is available through a separate package called `ActiveLogin.Authentication.BankId.AspNetCore.AzureMonitor`.

```csharp
services
    .AddAuthentication()
    .AddBankId(builder =>
    {
        builder
            ...
            .AddApplicationInsightsEventListener();
    });
```

By default it will use whatever InstrumentationKey is registered with the application. There are overloads available so you can customize this:

```csharp
services
    .AddAuthentication()
    .AddBankId(builder =>
    {
        builder
            ...
            .AddApplicationInsightsEventListener("CUSTOM_KEY");
    });
```

You can also customize what kind of data should be logged together with the Application Insight events. For example:


```csharp
services
    .AddAuthentication()
    .AddBankId(builder =>
    {
        builder
            ...
            .AddApplicationInsightsEventListener(options =>
            {
                options.LogUserPersonalIdentityNumber = false;
                options.LogUserPersonalIdentityNumberHints = true;

                options.LogUserNames = false;

                options.LogDeviceIpAddress = false;
                options.LogCertificateDates = true;

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

### Resolve user data on Auth request

BankID allows you to display a text during authentication to describe the intent. Active Login allows you to set these parameters when authenticating:

<img src="https://alresourcesprod.blob.core.windows.net/docsassets/active-login-bankid-uservisibledata-screenshot_1.jpg" width="250" alt="User visible data" />

* `UserVisibleData`
* `UserNonVisibleData`
* `UserVisibleDataFormat`

These can either be set as static data during startup in `Program.cs` or dynamically by overiding the interface `IBankIdAuthRequestUserDataResolver`.

Sample of static text without formatting:

```csharp
builder.UseAuthRequestUserData(authUserData =>
{
    authUserData.UserVisibleData = "Login to your account at Active Login";
});
```

Sample of static text with formatting:

```csharp
builder.UseAuthRequestUserData(authUserData =>
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

### Custom QR code generation

By default the `ActiveLogin.Authentication.BankId.AspNetCore.Qr` package is needed to generate QR codes using the `UseQrCoderQrCodeGenerator` extension method.

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

The default implementation provided in `ActiveLogin.Authentication.BankId.AspNetCore` is limited to supports the ~top 5 most common browsers on both iOS and Android. But since an incorrect browser detection can lead to an incorrect launch URL and result in a broken user flow, `UAParserDeviceDetector` in the `ActiveLogin.Authentication.BankId.AspNetCore.UAParser` package should be used to support additional browsers. It has a dependency on package [uap-csharp](https://github.com/ua-parser/uap-csharp) for improved user agent parsing.


### Use api wrapper only

We have seperated the API-wrapper for BankID into a separate package so that you can use it in other scenarios we have not covered. They look like this and are both well documented using XML-comments.

The constructor for these ApiClients takes an `HttpClient` and you need to configure that `HttpClient` with a `BaseAddress`, `Tls12`, client certificates etc. depending on your needs.

___Note:___ The `BankIdApiClient` class below is available in the `ActiveLogin.Authentication.BankId.Api` package.

```csharp
public class BankIdApiClient : IBankIdApiClient
{
    public Task<AuthResponse> AuthAsync(AuthRequest request) { ... }
    public Task<SignResponse> SignAsync(SignRequest request) { ... }
    public Task<CollectResponse> CollectAsync(CollectRequest request) { ... }
    public Task<CancelResponse> CancelAsync(CancelRequest request) { ... }
}
```

___Note:___ The class `BankIdUserVisibleDataFormats` contains constants for valid values of `userVisibleDataFormat`, example:

```csharp
    BankIdUserVisibleDataFormats.SimpleMarkdownV1
```


### Running on Linux

#### Certificate handling on Linux

`X509Certificate2` can not be handled in the same way when running in Linux as on Windows. The certificate is Base64 encoded and must be decoded before creating the `X509Certificate2` instance. Below is an example for the BankId root certificate:

Copy the content between Begin certificate and End certificate, and paste it into a resource string in a Resource.resx file.

With the certificate in the reosurce, this code can be used to create the `X509Certificate2` instance. Note the second line that decodes the Base64 string.

```csharp
var rootCertEncoded = CertificateResources.BankIdRootTestCertificate;
var rootCertBytes = Convert.FromBase64String(rootCertEncoded);

return new X509Certificate2(rootCertBytes, string.Empty, X509KeyStorageFlags.MachineKeySet);
```

### QRCode generation on Linux

The `ActiveLogin.Authentication.BankId.AspNetCore.QRCoder` package has a dependency on package [libgdiplus](https://github.com/mono/libgdiplus) on Linux.

If you are using Active Login with BankID QR-Codes on either WSL (Windows Subsystem for Linux) or in a Linux Docker Container your OS must have this package installed.

Add [libgdiplus](https://github.com/mono/libgdiplus) to your Dockerfile using apt-get.
```dockerfile
FROM mcr.microsoft.com/dotnet/core/aspnet:6.0 AS base
RUN apt-get update && apt-get -y install libgdiplus libc6-dev
...
```


### Localization

The messages are already localized to English and Swedish using the official recommended texts. To select what language that is used you can for example use the [localization middleware in ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/localization#localization-middleware).

The user messages that will be displayed are provided through the implementation of `IBankIdUserMessageLocalizer`, so by overriding this you can customize the messages.

```csharp
services.AddTransient<IBankIdUserMessageLocalizer, CustomBankIdUserMessageLocalizer>();
```

The defualt implementation (`BankIdUserMessageStringLocalizer`) uses `Microsoft.Extensions.Localization.IStringLocalizer` and therefore chooses the texts in the `*.resx` files.


### Names of the person might be capitalized

The names comes from the bank that the end user has, and some banks (due to legacy) stores all of the names in all caps (like `ALICE SMITH`).

We have choosen not to normalize the capitalization of the names as it´s hard or impossible to do so in a general way.


### Cookies issued

The `*.AspNetCore` package will issue a cookie to make the auth flow work

The cookie is called: `__ActiveLogin.BankIdState`

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
