# ActiveLogin.Authentication

ActiveLogin.Authentication enables an application to support Swedish BankID's (svenskt BankIDs) authentication workflow in .NET. Built on NET Standard and packaged as NuGet-packages they are easy to install and use on multiple platforms.

## Features

- :id: Supports BankID both natively and through GrandID
- :penguin: Cross platform: Targets .NET Standard 2.0 and .NET Framework 4.6.1
- :heavy_check_mark: Strong named
- :lock: GDPR Compliant
- :cloud: Great support for Microsoft Azure
- :earth_americas: Multi language support with English and Swedish out of the box
- :wrench: Customizable UI

## Continuous integration & Packages overview

[![Build status](https://dev.azure.com/activesolution/ActiveLogin/_apis/build/status/ActiveLogin.Authentication)](https://dev.azure.com/activesolution/ActiveLogin/_build/latest?definitionId=155)

| Project | Description | NuGet |
| ------- | ----------- | ----- |
| [ActiveLogin.Authentication.BankId.Api](https://github.com/ActiveLogin/ActiveLogin.Authentication/tree/master/src/ActiveLogin.Authentication.BankId.Api) | API client for Swedish BankID's REST API. | [![NuGet](https://img.shields.io/nuget/v/ActiveLogin.Authentication.BankId.Api.svg)](https://www.nuget.org/packages/ActiveLogin.Authentication.BankId.Api/) |
| [ActiveLogin.Authentication.BankId.AspNetCore](https://github.com/ActiveLogin/ActiveLogin.Authentication/tree/master/src/ActiveLogin.Authentication.BankId.AspNetCore) | ASP.NET Core authentication module for Swedish BankID. | [![NuGet](https://img.shields.io/nuget/v/ActiveLogin.Authentication.BankId.AspNetCore.svg)](https://www.nuget.org/packages/ActiveLogin.Authentication.BankId.AspNetCore/) |
| [ActiveLogin.Authentication.BankId.AspNetCore.Azure](https://github.com/ActiveLogin/ActiveLogin.Authentication/tree/master/src/ActiveLogin.Authentication.BankId.AspNetCore.Azure) | Azure integrations for ActiveLogin.Authentication.BankId.AspNetCore. | [![NuGet](https://img.shields.io/nuget/v/ActiveLogin.Authentication.BankId.AspNetCore.Azure.svg)](https://www.nuget.org/packages/ActiveLogin.Authentication.BankId.AspNetCore.Azure/) |
| [ActiveLogin.Authentication.GrandId.Api](https://github.com/ActiveLogin/ActiveLogin.Authentication/tree/master/src/ActiveLogin.Authentication.GrandId.Api) | API client for GrandID (Svensk E-identitet) REST API. | [![NuGet](https://img.shields.io/nuget/v/ActiveLogin.Authentication.GrandId.Api.svg)](https://www.nuget.org/packages/ActiveLogin.Authentication.GrandId.Api/) |
| [ActiveLogin.Authentication.GrandId.AspNetCore](https://github.com/ActiveLogin/ActiveLogin.Authentication/tree/master/src/ActiveLogin.Authentication.GrandId.AspNetCore) | ASP.NET Core authentication module for GrandID (Svensk E-identitet). | [![NuGet](https://img.shields.io/nuget/v/ActiveLogin.Authentication.GrandId.AspNetCore.svg)](https://www.nuget.org/packages/ActiveLogin.Authentication.GrandId.AspNetCore/) |

## Getting started

First of all, you need to decide if you want to use [native BankID](https://www.bankid.com/bankid-i-dina-tjanster/sa-kommer-du-igang) or BankID through [GrandID (Svensk E-identitet)](https://e-identitet.se/tjanster/inloggningsmetoder/bankid/).

* _Native BankID_ gives you full flexibility, including custom UI but requires issuing a certificate through a bank and usually takes some time to sort out.
* _GrandID (Svensk E-identitet)_ uses a predefined UI and does not support all functionalities of the BankID API, but is really easy to get started with and does not require any certificates.

_Screenshots on how the default UI for Native BankID looks._

![Active Login Screenshots](docs/images/activelogin-screenshots.png)

## Configure as service

### Build the IdP

    docker build . -f IdentityServer.ServerSample.Dockerfile -t active-login-idp:latest

### Build your own IdP

TODO

### Use together with Ory Hydra / OpenId Connect Provider

TODO

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

## FAQ

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

### Why are the names sometimes capitalized?

It seems that the name for some persons are returned in all capitalized letters (like `ALICE SMITH`), the data is probably stored that way at BankID.
We have choosen not to normalize the capitalization of the names as it´s hard or impossible to do so in a general way.

## Active Login

_Integrating your systems with market leading authentication services._

Active Login is an Open Source project built on .NET Standard that makes it easy to integrate with leading Swedish authentication services like [BankID](https://www.bankid.com/).

It also provide examples of how to use it with the popular OpenID Connect & OAuth 2.0 Framework [IdentityServer](https://identityserver.io/) and provides a template for hosting the solution in Microsoft Azure.
In addition, Active Login also contain convenient modules that help you work with and handle validation of Swedish Personal Identity Number (svenskt personnummer).

### Contribute

We are very open to community contributions to Active Login. You'll need a basic understanding of Git and GitHub to get started. The easiest way to contribute is to open an issue and start a discussion. If you make code changes, submit a pull request with the changes and a description. Don’t forget to always provide tests that cover the code changes.

### License & acknowledgements

Active Login is licensed under the very permissive [MIT license](https://opensource.org/licenses/MIT) for you to be able to use it in commercial or non-commercial applications without many restrictions.

Active Login is built on or uses the following great open source products:

* [.NET Standard](https://github.com/dotnet/standard)
* [ASP.NET Core](https://github.com/aspnet/Home)
* [XUnit](https://github.com/xunit/xunit)
* [IdentityServer](https://github.com/IdentityServer/)
* [Bootstrap](https://github.com/twbs/bootstrap)
* [Font Awesome](https://github.com/FortAwesome/Font-Awesome)
