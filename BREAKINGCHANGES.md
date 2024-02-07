### Breaking changes in Active Login Authentication

Here we try to make a summary of the major breaking changes in things that you as a consumer of Active Login can use/extend/implement.

___Note:___ We might, and will probably, miss to document some of this - if so - please make a PR to this file and add a note on such breaking change.


## TOC

* [Version 8.0.0](#version-800)
* [Version 7.0.0](#version-700)
* [Version 6.0.3](#version-603)
* [Version 6.0.0](#version-600)
* [Version 5.0.0](#version-500)
* [Version 4.0.0](#version-400)

---


## Version 8.0.0

Breaking changes between version 8.0.0 and 7.0.0

### IBankIdLauncherCustomAppCallback becomes IBankIdLauncherCustomBrowser

We have broadedned the scope of the interface `IBankIdLauncherCustomAppCallback` to `IBankIdLauncherCustomBrowser` to allow for more flexibility in the implementation.

We have renamed these things as a result of that:
* `IBankIdLauncherCustomAppCallback` becomes `IBankIdLauncherCustomBrowser`
* `AddCustomAppCallback` becomes `AddCustomBrowser`
* `AddCustomAppCallbackByUserAgent` becomes `AddCustomBrowserByUserAgent`

### Upgrade to .NET 7

We now require .NET 8 - so this requires you to upgrade your website that uses Active Login.

## Version 7.0.0

Breaking changes between version 7.0.0 and 6.0.3

### Upgrade to .NET 7

We now require .NET 7 - so this requires you to upgrade your website that uses Active Login.

### API changes

The API has changed to reflect the new BankID API (v 6.0). An overview of those changes can be found [in this issue](https://github.com/ActiveLogin/ActiveLogin.Authentication/issues/402).

#### Rename API Wrapper

As we've added support for the new Verify API, we have renamed the existing API into App API. These changes should go unnoticed for most of you if you are using the high level abstractions.

* `IBankIdApiClient` is renamed into `IBankIdAppApiClient`
* `BankIdApiClient` is renamed into `BankIdAppApiClient`
* `BankIdSimulatedApiClient` is renamed into `BankIdSimulatedAppApiClient`
* `BankIdSimulatedApiClient` is renamed into `BankIdSimulatedAppApiClient`
* `BankIdApiClientExtensions` is renamed into `BankIdAppApiClientExtensions`

### Custom options

Some API changes (as described above) propagates to the options you can customize per authentication scheme. The following options have been changed:

- `BankIdAllowBiometric` is removed and replaced by `BankIdRequirePinCode`. This has the opposite meaning as the old option, so be sure to update your code if you have used this.

### IBankIdLauncher.GetLaunchInfo is now async

The method `GetLaunchInfo` on `IBankIdLauncher` is now renamed into `GetLaunchInfoAsync` and made async to allow for dynamic retreival.

## Version 6.0.3

Breaking changes in 6.0.3:

- As stated in #384 we had mixed up the UseClientCertificate() and AddClientCertificate() extension methods. These have now been reversed. This would be a breaking change if you add multiple certificates but should affect the majority of consumers.

## Version 6.0.0

Breaking changes between version 5.0.0 and 6.0.0.

___Note:___ Version is a major release with a large set of refactorings, name changes, and namespace changes. We have not covered it all, but tried to summarize the major ones below. In v6 we introduced Sign and wanted to refactor and rename things to make i consistent and easy to use both/either of them. This will cause some work when upgrading from a previous version.

### Renaming NuGet-packages

As all of the support packages related to Azure etc. now only depend on the Core package, we have renamed a few packages to reflect this:

- `ActiveLogin.Authentication.BankId.AspNetCore.Azure` is now `ActiveLogin.Authentication.BankId.AzureKeyVault`
- `ActiveLogin.Authentication.BankId.AspNetCore.AzureMonitor` is now `ActiveLogin.Authentication.BankId.AzureMonitor`
- `ActiveLogin.Authentication.BankId.AspNetCore.QRCoder` is now `ActiveLogin.Authentication.BankId.QRCoder`
- `ActiveLogin.Authentication.BankId.AspNetCore.UAParser` is now `ActiveLogin.Authentication.BankId.UAParser`

### Auth functionality is now separate from core functionality

This means that you need to register the common configuration first and then the auth specific conf, like this:

```csharp
services
    .AddBankId(bankId =>
    {
        bankId.UseQrCoderQrCodeGenerator();
        bankId.UseUaParserDeviceDetection();
        ...
    });

services.AddAuthentication()
    .AddCookie()
    .AddBankIdAuth(bankId =>
    {
        bankId.AddSameDevice();
        bankId.AddOtherDevice();
    });
```

### Moving extension methods away from Microsoft.Extensions.DependencyInjection

As per the [Microsoft guideline about namespaces](https://docs.microsoft.com/en-us/dotnet/core/extensions/options-library-authors#namespace-guidance) in libraries, we have moved our extension methods away from `Microsoft.Extensions.DependencyInjection` into their "natural" namespaces.

This means that in your `Program.cs` you might need to import these namespaces:

```csharp
using ActiveLogin.Authentication.BankId.AspNetCore.Auth;
using ActiveLogin.Authentication.BankId.AspNetCore.Sign;
using ActiveLogin.Authentication.BankId.AzureKeyVault;
using ActiveLogin.Authentication.BankId.AzureMonitor;
using ActiveLogin.Authentication.BankId.Core;
using ActiveLogin.Authentication.BankId.QrCoder;
using ActiveLogin.Authentication.BankId.UaParser;
```

### Requires static files

The stylesheet and the javascript for the client are now served as *.css and *.js files, so you need to enable static file support.

```csharp
app.UseStaticFiles();
```

### Moving core functionality to separate package

Core functionality that is not tied to web (AspNetCore) is moved into a separate package called `ActiveLogin.Authentication.BankId.Core`.

If you directly use/extend/implement anything related to these areas you might have to update your references:

- `IBankIdEndUserIpResolver`
- `IBankIdLauncher`
- `IBankIdResultStore`
- `IBankIdQrCodeGenerator`
- `IBankIdSupportedDeviceDetector`
- `IBankIdAuthRequestUserDataResolver`
- `IBankIdUserMessageLocalizer`
- Events

### Renaming area and URL:s

The area and URL:s used internally are now renamed:

- `/BankIdAuthentication/BankId/Login` is now `/ActiveLogin/BankId/Auth`
- `/BankIdAuthentication/BankId/Api/*` is now `/ActiveLogin/BankId/Api/*`

### Renaming views

These views have been renamed:

- `Login.cshtml` is now  `Init.cshtml`
- `_Login.cshtml` is now  `_Wrapper.cshtml`
- `_LoginForm.cshtml` is now  `_Form.cshtml`
- `_LoginStatus.cshtml` is now  `_Status.cshtml`
- `_LoginScript.cshtml` is now  `_Script.cshtml`
- `_LoginStyle.cshtml` is now  `_Style.cshtml`
- `_spinner.cshtml` is now  `_Spinner.cshtml`

___Note:___ As the area and controller have been renamed these files should now be placed in `/Areas/ActiveLogin/Views/BankIdUiAuth/` or `/Areas/ActiveLogin/Views/Shared/`.

___Note:___ Both the html, css and javascript have breaking changes. See history for respective file for exact changes.

### Renaming BankIdDefaults

`BankIdDefaults` have been renamed into `BankIdAuthDefaults` and can now be found in the namespace `ActiveLogin.Authentication.BankId.AspNetCore.Auth`.

The default values for `SameDeviceAuthenticationScheme`, `OtherDeviceAuthenticationScheme`, `SameDeviceCallbackPath` and `OtherDeviceCallbackPath` have also changed.

### Removing dependency on HttpContext in ActiveLogin.Authentication.BankId.Core

No classes or interfaces in `ActiveLogin.Authentication.BankId.Core` will have a dependency on ``HttpContext`. `HttpContext` would previously be passed as arguments to some methods defined in our interfaces.
If you need `HttpContext` for your implementation, take a dependency on `IHttpContextAccessor` to achieve the same functionality - this is how we solve it internally as well.

### Name changes for *Login

To reflect that we now support both auth and sign we have renamed a lot of classes called something with `Login`. For example:

- `BankIdLoginResult` is now `BankIdUiResult`
- `BankIdLoginOptions` is now `BankIdUiOptions`

### Renaming metadata for Azure Monitor

These properties have been renamed:

- `AL_BankId_LoginOptions_LaunchType` is now `AL_BankId_Options_LaunchType`
- `AL_BankId_LoginOptions_UseQrCode` is now `AL_BankId_Options_UseQrCode`

### BankIdDynamicEndUserIpResolver removed

BankIdDynamicEndUserIpResolver removed is now removed. It was a shorthand for a scenario that should be very uncommon.
lease use BankIdRemoteIpAddressEndUserIpResolver instead if you need to override end user ip retrieval.

### No manual input of PersonalIdentityNumber

The use of personalIdentityNumber in the auth flow is no longer supported as BankID recommends to only using either animated QR codes or autostarttoken on the same device.

### Multi tenant

If you register multiple client certificates (multi-tenant scenarios) you now need to use the `.AddClientCertificate...()` methods instead of `.UseClientCertificate...()`. `.UseClientCertificate...()` will remove any previously registered certificates.

This applies to:

- `.AddClientCertificate()`
- `.AddClientCertificateFromAzureKeyVault()`

### Application Insights Connection String instead of Instrumentation Key

When using Application Insights [you should nowadays specify a connection string instead of just an instrumentation key](https://docs.microsoft.com/en-us/azure/azure-monitor/app/sdk-connection-string?tabs=net). We've updated our methods that explicitly took an instrumentation key to instead take a connection string. This applies to:

- `AddApplicationInsightsEventListener()`

---


## Version 5.0.0

Breaking changes between version 4.0.0 and 5.0.0

### Upgrade to .NET 6

We now require .NET 6 for the UI parts - so this requires you to upgrade your website that uses Active Login.

### Support BankID animated QR code in UI

When implementing the animated QR code, we have had to update the client side script in `LoginScript.cshtml`. Look at the Git history for this file if you want to see the exact changes. This is only a breaking change if you have overridden this file with your own imlpementation.

### Use DefaultAzureCredentials when accessing KeyVault

If using Azure KeyVault for your certificate, we have made chamges to how you authenticate/access that. See [the documention](https://docs.activelogin.net/articles/bankid.html#keyvault-credentials) for how it should be configured now.

### Drop support for GrandID

When we created Active Login we added support for BankID directly as well as through Svensk E-identitet (GrandID). At the time we had cases where we were using both so it was natural to support them both. As time has gone by we see that basically, all new cases for us use the "native" BankID provider so that's where we have focused our efforts in Active Login.

We have reached the point where it is not possible for us to support the GrandId packages anymore as part of this open-source project. In the end, we have just so much time that we want to spend where we think it benefits the most users, and the download counts on NuGet tell this is the native BankID parts.

See more details in this [announcement](https://github.com/ActiveLogin/ActiveLogin.Authentication/discussions/323).

### Issue correct `exp` claim

We previously issued an incorrect exp claim. This is now issued correctly, but might be a breaking change for you.

### Remove built in support for Birthdate and Gender claim

The Birthdate and Gender claim are not issued by the library, but we have [added docs](https://docs.activelogin.net/articles/bankid.html#example-add-birthdate-and-gender-claims) on how to issue them yourself.


---


## Version 4.0.0

Breaking changes between version 3.0.0 and 4.0.0


### Renaming events

We have renamed events and event ids but the log messages should be the same.

A full list of current event names and ids can be found in [BankIdEventTypes.cs](https://github.com/ActiveLogin/ActiveLogin.Authentication/blob/master/src/ActiveLogin.Authentication.BankId.AspNetCore/Events/Infrastructure/BankIdEventTypes.cs).

The category of events logged to ILogger will now be `BankIdLoggerEventListener`.

### Removed UseClientCertificateResolver

The `UseClientCertificateResolver` extension was removed, but docs was added with sample on how to achieve the same result.

### Refactor IBankIdLauncher

`IBankIdLauncher` was refactored to return an object with launch info, not only launch URL.
