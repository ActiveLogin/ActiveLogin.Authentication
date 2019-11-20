# ActiveLogin.Authentication

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT) [![Build Status](https://dev.azure.com/activesolution/ActiveLogin/_apis/build/status/ActiveLogin.Authentication?branchName=master)](https://dev.azure.com/activesolution/ActiveLogin/_build/latest?definitionId=192&branchName=master)

ActiveLogin.Authentication enables an application to support Swedish BankID's (svenskt BankIDs) authentication workflow in .NET. Built on NET Standard and packaged as NuGet-packages they are easy to install and use on multiple platforms.

## Features

- :id: Supports BankID both natively and through GrandID
- :penguin: Cross platform: Targets .NET Standard 2.0 and .NET Core 3.1
- :lock: GDPR Compliant
- :cloud: Great support for Microsoft Azure
- :earth_americas: Multi language support with English and Swedish out of the box
- :wrench: Customizable UI
- :diamond_shape_with_a_dot_inside: Can be used as a [Custom Identity Proivder for Azure AD B2C](#can-i-use-active-login-to-get-support-for-bankid-or-grandid-in-azure-ad-active-directory-b2c)

## Continuous integration & Packages overview

| Project | Description | NuGet |
| ------- | ----------- | ----- |
| [ActiveLogin.Authentication.BankId.Api](https://github.com/ActiveLogin/ActiveLogin.Authentication/tree/master/src/ActiveLogin.Authentication.BankId.Api) | API client for Swedish BankID's REST API. | [![NuGet](https://img.shields.io/nuget/v/ActiveLogin.Authentication.BankId.Api.svg)](https://www.nuget.org/packages/ActiveLogin.Authentication.BankId.Api/) |
| [ActiveLogin.Authentication.BankId.AspNetCore](https://github.com/ActiveLogin/ActiveLogin.Authentication/tree/master/src/ActiveLogin.Authentication.BankId.AspNetCore) | ASP.NET Core authentication module for Swedish BankID. | [![NuGet](https://img.shields.io/nuget/v/ActiveLogin.Authentication.BankId.AspNetCore.svg)](https://www.nuget.org/packages/ActiveLogin.Authentication.BankId.AspNetCore/) |
| [ActiveLogin.Authentication.BankId.AspNetCore.Azure](https://github.com/ActiveLogin/ActiveLogin.Authentication/tree/master/src/ActiveLogin.Authentication.BankId.AspNetCore.Azure) | Azure integrations for ActiveLogin.Authentication.BankId.AspNetCore. | [![NuGet](https://img.shields.io/nuget/v/ActiveLogin.Authentication.BankId.AspNetCore.Azure.svg)](https://www.nuget.org/packages/ActiveLogin.Authentication.BankId.AspNetCore.Azure/) |
| [ActiveLogin.Authentication.BankId.AspNetCore.QRCoder](https://github.com/ActiveLogin/ActiveLogin.Authentication/tree/master/src/ActiveLogin.Authentication.BankId.AspNetCore.QRCoder) | QR code generation using QRCoder for ActiveLogin.Authentication.BankId.AspNetCore. | [![NuGet](https://img.shields.io/nuget/v/ActiveLogin.Authentication.BankId.AspNetCore.QRCoder.svg)](https://www.nuget.org/packages/ActiveLogin.Authentication.BankId.AspNetCore.QRCoder/) |
| [ActiveLogin.Authentication.GrandId.Api](https://github.com/ActiveLogin/ActiveLogin.Authentication/tree/master/src/ActiveLogin.Authentication.GrandId.Api) | API client for GrandID (Svensk E-identitet) REST API. | [![NuGet](https://img.shields.io/nuget/v/ActiveLogin.Authentication.GrandId.Api.svg)](https://www.nuget.org/packages/ActiveLogin.Authentication.GrandId.Api/) |
| [ActiveLogin.Authentication.GrandId.AspNetCore](https://github.com/ActiveLogin/ActiveLogin.Authentication/tree/master/src/ActiveLogin.Authentication.GrandId.AspNetCore) | ASP.NET Core authentication module for GrandID (Svensk E-identitet). | [![NuGet](https://img.shields.io/nuget/v/ActiveLogin.Authentication.GrandId.AspNetCore.svg)](https://www.nuget.org/packages/ActiveLogin.Authentication.GrandId.AspNetCore/) |

CI-builds from master of all packages are available in [our Azure DevOps Artifacts feed](https://dev.azure.com/activesolution/ActiveLogin/_packaging?_a=feed&feed=ActiveLogin-CI).

## Getting started

First of all, you need to decide if you want to use [native BankID](https://www.bankid.com/bankid-i-dina-tjanster/sa-kommer-du-igang) or BankID through [GrandID (Svensk E-identitet)](https://e-identitet.se/tjanster/inloggningsmetoder/bankid/).

* _Native BankID_ gives you full flexibility, including custom UI but requires issuing a certificate through a bank and usually takes some time to sort out.
* _GrandID (Svensk E-identitet)_ uses a predefined UI and does not support all functionalities of the BankID API, but is really easy to get started with and does not require any certificates.

_Screenshots on how the default UI for Native BankID looks._

![Active Login Screenshots](docs/images/activelogin-screenshots.png)

### 1. Install the NuGet package

ActiveLogin.Authentication is distributed as [packages on NuGet](https://www.nuget.org/profiles/ActiveLogin), install using the tool of your choice, for example _dotnet cli_.

#### BankID

```powershell
dotnet add package ActiveLogin.Authentication.BankId.AspNetCore
```

#### GrandID

```powershell
dotnet add package ActiveLogin.Authentication.GrandId.AspNetCore
```

### 2. Prepare your project

It is expected that you have a basic understanding of how [ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/), [ASP.NET Core MVC](https://docs.microsoft.com/en-us/aspnet/core/mvc/overview) and [ASP.NET Core Authentication](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/identity) works before getting started.

The authentication modules for BankID and GrandID are registered in `ConfigureServices( ... )` in your `Startup.cs`. Depending on your setup, you will probably have to configure challenge and callbacks in `AccountController.cs` or similar.

### 3. Get started in development

Both BankID and GrandID requires you to receive either certificates or API-keys, but to get started and try it out the experience there is a simulated environment options available that uses an in-memory implementation. Great for development and testing.

#### BankID

```c#
services
    .AddAuthentication()
    .AddBankId(builder =>
    {
        builder
            .UseSimulatedEnvironment()
            .AddSameDevice()
            .AddOtherDevice();
    });
```

#### GrandID

```c#
services
    .AddAuthentication()
    .AddGrandId(builder =>
    {
        builder
            .UseSimulatedEnvironment()
            .AddBankIdSameDevice(options => { })
            .AddBankIdOtherDevice(options => { });
    });
```

### 4. Use test or production environments

To authenticate using a real BankID you need to receive a certificate or API-keys, depending on what solution you choose. The details are described in these documents:

* [Getting started with BankID in Test and Production](docs/getting-started-bankid.md)
* [Getting started with GrandID in Test and Production](docs/getting-started-grandid-bankid.md)

Samples on how to use them in production are:

#### [BankID](docs/getting-started-bankid.md)

```c#
services
    .AddAuthentication()
    .AddBankId(builder =>
    {
        builder
            .UseProductionEnvironment()
            .UseClientCertificateFromAzureKeyVault(Configuration.GetSection("ActiveLogin:BankId:ClientCertificate"))
            .UseRootCaCertificate(Path.Combine(_environment.ContentRootPath, Configuration.GetValue<string>("ActiveLogin:BankId:CaCertificate:FilePath")))
            .AddSameDevice()
            .AddOtherDevice();
    });
```

#### [GrandID](docs/getting-started-grandid-bankid.md)

```c#
services
    .AddAuthentication()
    .AddGrandId(builder =>
    {
        builder
            .UseProductionEnvironment(config => {
	        config.ApiKey = Configuration.GetValue<string>("ActiveLogin:GrandId:ApiKey");
	        config.BankIdServiceKey = Configuration.GetValue<string>("ActiveLogin:GrandId:BankIdServiceKey");
            })
            .AddBankIdSameDevice()
            .AddBankIdOtherDevice();
    });
```

## Browse tests and samples

For more use cases, samples and inspiration; feel free to browse our unit tests and samples:

* [IdentityServer.ServerSample](https://github.com/ActiveLogin/ActiveLogin.Authentication/tree/master/samples/IdentityServer.ServerSample)
* [IdentityServer.ClientSample](https://github.com/ActiveLogin/ActiveLogin.Authentication/tree/master/samples/IdentityServer.ClientSample)
* [Standalone.MvcSample](https://github.com/ActiveLogin/ActiveLogin.Authentication/tree/master/samples/Standalone.MvcSample)
* [AzureProvisioningSample](https://github.com/ActiveLogin/ActiveLogin.Authentication/tree/master/samples/AzureProvisioningSample)
* [ActiveLogin.Authentication.BankId.Api.Test](https://github.com/ActiveLogin/ActiveLogin.Authentication/tree/master/test/ActiveLogin.Authentication.BankId.Api.Test)

## Overriding partial views

ActiveLogin comes with predefined views that you can use, but maybe you'd rather use your own views to customize layout or behavior.
In your web project, create the following folder:
```Areas/BankIdAuthentication/Views/BankId```
In this folder, you can then create any of the partials and MVC will then discover your partials and use any of them before ours. It's still possible to call our partials if you still want to use them.

* `_Login.cshtml`
* `_LoginForm.cshtml`
* `_LoginScript.cshtml`
* `_LoginStatus.cshtml`

See [the MVC sample](https://github.com/ActiveLogin/ActiveLogin.Authentication/tree/master/samples/Standalone.MvcSample) to see this in action, as demonstrated [here](https://github.com/ActiveLogin/ActiveLogin.Authentication/tree/master/samples/Standalone.MvcSample/Areas/BankIdAuthentication/Views/BankId/_Login.cshtml).

## FAQ

### What versions of .NET is supported?

The API-wrappers (*.Api) target .NET Standard 2.0, so they can be used from .NET Core >= 2.0 and .NET Framework >= 4.6.1, [see full reference here](https://docs.microsoft.com/en-us/dotnet/standard/net-standard#net-implementation-support). The packages that target .NET Standard are strong named as they can be used from .NET Framework where strong naming can be relevant.

The authentication modules (*.AspNetCore), and related packages, depend on ASP.NET Core MVC 3.1 and therefore require .NET Core 3.1.

Our samples target .NET Core 3.1.

### How do I run the samples?

The samples are configured to run in simulated mode (no BankID certificates or GrandID keys required) by default. The _IdentityServer.ClientSample_ is using the _IdentityServer.ServerSample_ as its identity provider. So to run the _IdentityServer.ClientSample_, the _IdentityServer.ServerSample_ needs to be running first.

The easiest way to try the sample out is to:

1. Configure the solution to use _Multiple startup projects_, and set it to start both _IdentityServer.ServerSample_ and _IdentityServer.ClientSample_
1. Press F5

There is also a standalone sample called _Standalone.MvcSample_ which uses only AspNetCore MVC with minimum of code.

### Can I try out a live demo of the samples?

Yes! They are available here. Please note that IdentityServer.ClientSample uses IdentityServer.ServerSample as the IdentityProvider, so the IdentityServer.ClientSample is a good place to start.

* IdentityServer.ClientSample: [https://al-samples-mvcclient.azurewebsites.net](https://al-samples-mvcclient.azurewebsites.net)
* IdentityServer.ServerSample: [https://al-samples-identityserver.azurewebsites.net](https://al-samples-identityserver.azurewebsites.net)

### Can I use Active Login to get support for BankID or GrandID in Azure AD (Active Directory) B2C?

Yes you can! Azure AD B2C supports using cusotm identity providers that supports [Open ID Connect](https://docs.microsoft.com/sv-se/azure/active-directory-b2c/active-directory-b2c-reference-oidc). If you deploy Active Login as part of Identity Server (see our samples) you can configure your Azure AD B2C to federate to that instance and by doing so get BankID and/or GrandID support.

### Can I prepopulate the personal identity number for the user?

Yes you can! If you provide an authentication property item named `swedishPersonalIdentityNumber` (available as constants `BankIdAuthenticationConstants.AuthenticationPropertyItemSwedishPersonalIdentityNumber` or `GrandIdAuthenticationConstants.AuthenticationPropertyItemSwedishPersonalIdentityNumber`) that value will be used and sent to BankID/GrandID.

_Example usage:_

```csharp
public IActionResult ExternalLogin(string provider, string returnUrl, string personalIdentityNumber)
{
    var props = new AuthenticationProperties
    {
        RedirectUri = Url.Action(nameof(ExternalLoginCallback)),
        Items =
        {
            {"returnUrl", returnUrl},
            {"scheme", provider},
            { BankIdAuthenticationConstants.AuthenticationPropertyItemSwedishPersonalIdentityNumber, personalIdentityNumber }
        }
    };

    return Challenge(props, provider);
}
```
### Do I need to use your ASP.NET Core Auth provider, or can just use the API?

No, it's optional :) We have seperated the API-wrappers for both BankID and GrandID into two separate packages so that you can use them in other scenarios we have not covered. The look like this and are both well documented using XML-comments.

#### ActiveLogin.Authentication.BankId.Api

```csharp
public class BankIdApiClient : IBankIdApiClient
{
    public Task<AuthResponse> AuthAsync(AuthRequest request) { ... }
    public Task<SignResponse> SignAsync(SignRequest request) { ... }
    public Task<CollectResponse> CollectAsync(CollectRequest request) { ... }
    public Task<CancelResponse> CancelAsync(CancelRequest request) { ... }
}
```

#### ActiveLogin.Authentication.GrandId.Api

```csharp
public class GrandIdApiClient : IGrandIdApiClient
{
    public async Task<BankIdFederatedLoginResponse> BankIdFederatedLoginAsync(BankIdFederatedLoginRequest request) { ... }
    public async Task<BankIdGetSessionResponse> BankIdGetSessionAsync(BankIdGetSessionRequest request) { ... }
    public async Task<LogoutResponse> LogoutAsync(LogoutRequest request) { ... }
}
```

### Do Active Login Issue any cookies?

Yes, the `*.AspNetCore` packages will issue cookies to make the auth flow work.

The cookies are called:

- BankId: `__ActiveLogin.BankIdState`
- GrandId: `__ActiveLogin.GrandIdState`

The cookies are there to store state during the auth process, as the user will/might be redirected during the flow. The cookies are session based only and will be deleted once the auth process is finished and/or when the user closes the browser.

Because they are strictly related to temp storage during auth, you should not have to inform the user about these specific cookies (according to the [EU "cookie law"](https://www.cookielaw.org/the-cookie-law/)).

With the current implementaiton (following the convention from Microsoft ASP.NET Core) the usage of cookies is not optional.

A more technical deep dive of the cookies can be found in [this issue](https://github.com/ActiveLogin/ActiveLogin.Authentication/issues/156).

### Why are the names sometimes capitalized?

It seems that the name for some persons are returned in all capitalized letters (like `ALICE SMITH`), the data is probably stored that way at BankID.
We have choosen not to normalize the capitalization of the names as it´s hard or impossible to do so in a general way.

---

## Active Login

_Integrating your systems with market leading authentication services._

Active Login is an Open Source project built on .NET Core that makes it easy to integrate with leading Swedish authentication services like [BankID](https://www.bankid.com/).

It also provide examples of how to use it with the popular OpenID Connect & OAuth 2.0 Framework [IdentityServer](https://identityserver.io/) and provides a template for hosting the solution in Microsoft Azure.
In addition, Active Login also contain convenient modules that help you work with and handle validation of Swedish Personal Identity Number (svenskt personnummer).

### Contribute

We are very open to community contributions to Active Login. You'll need a basic understanding of Git and GitHub to get started. The easiest way to contribute is to open an issue and start a discussion. If you make code changes, submit a pull request with the changes and a description. Don’t forget to always provide tests that cover the code changes.

### License & acknowledgements

Active Login is licensed under the very permissive [MIT license](https://opensource.org/licenses/MIT) for you to be able to use it in commercial or non-commercial applications without many restrictions.

Active Login is built on or uses the following great open source products:

* [.NET Core](https://github.com/dotnet/core)
* [ASP.NET Core](https://github.com/aspnet/Home)
* [IdentityServer](https://github.com/IdentityServer/)
* [XUnit](https://github.com/xunit/xunit)
* [QRCoder](https://github.com/codebude/QRCoder)
* [Bootstrap](https://github.com/twbs/bootstrap)
* [Loading.io](https://loading.io/)

### Sponsors

Active Solution is the main sponsor of Active Login.

![Active Solution](https://activese-assets-prod.azureedge.net/graphics/activesolution-logo.svg)

_We deliver tomorrow's cloud solutions, today. Our costumers choose us because we are engaged, flexible and efficient. We attract the brightest talent and are one of Microsoft's most valued partners._

[https://www.activesolution.se/](https://www.activesolution.se/)

### Support & Training

If you need help with implementing Active Login, there are commercial support & training options available.
See [ActiveLogin.net](https://activelogin.net#support) for more details.