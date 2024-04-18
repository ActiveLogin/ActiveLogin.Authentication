# ActiveLogin.Authentication

[![License: MIT](https://img.shields.io/badge/License-MIT-orange.svg)](https://opensource.org/licenses/MIT)
[![Build Status](https://github.com/ActiveLogin/ActiveLogin.Authentication/actions/workflows/build.yml/badge.svg)](https://github.com/ActiveLogin/ActiveLogin.Authentication/actions/workflows/build.yml)
[![Live demo](https://img.shields.io/static/v1?label=Demo&message=Live%20demo&color=008bc3)](https://demo.activelogin.net/)
[![Docs](https://img.shields.io/static/v1?label=Docs&message=Documentation&color=008bc3)](https://docs.activelogin.net/)
[![Discussion](https://img.shields.io/github/discussions/ActiveLogin/ActiveLogin.Authentication)](https://github.com/ActiveLogin/ActiveLogin.Authentication/discussions)
[![Twitter Follow](https://img.shields.io/badge/Twitter-@ActiveLoginSE-blue.svg?logo=twitter)](https://twitter.com/ActiveLoginSE)


ActiveLogin.Authentication enables an application to support Swedish BankID (svenskt BankID) authentication in .NET.

[Active Login is licensed](LICENSE.md) is provided under the very permissive [MIT license](https://opensource.org/licenses/MIT) for you to be able to use it in commercial or non-commercial applications without many restrictions.
Active Login is provided "as is", without any warrany of any kind. If you need support, [commercial support and training](#support--training) is available.

Active Login is not a product created by BankID. It is an unofficial project that was developed by Active Solution. All trademarks are the property of their respective owners.

## Features

- :id: Supports BankID Auth (API, Flow and UI)
- :pencil: Supports BankID Sign (API, Flow and UI)
- :phone::id: Supports BankID Phone Auth (API)
- :phone::pencil: Supports BankID Phone Sign (API)
- :relaxed: Supports BankID Verify digital ID card (API)
- :penguin: Cross platform: Targets .NET Standard 2.0 and .NET 8
- :six: Built on V6.0 (the latest) BankID JSON API
- :checkered_flag: Supports BankID animated QR code (Secure start)
- :cloud: Designed with Microsoft Azure in mind (KeyVault, Monitor, Application Insights, AD B2C etc.)
- :earth_americas: Multi language support with English and Swedish out of the box
- :wrench: Customizable and extensible
- :diamond_shape_with_a_dot_inside: Can be used as a [Custom Identity Provider for Azure AD B2C](#how-do-i-use-active-login-to-get-support-for-bankid-in-azure-ad-active-directory-b2c)


## Screenshots

_Screenshots on how the default UI for Native BankID looks on different devices._

![Active Login Screenshots](https://alresourcesprod.blob.core.windows.net/docsassets/active-login-screenshots.png)

_Screenshot on monitoring dashboard._

![Active Login Monitor](https://alresourcesprod.blob.core.windows.net/docsassets/active-login-monitor-screenshot_1.png)


## Table of contents

___Note:___ This Readme reflects the state of our main branch and the code documented here might not be released as packages on NuGet.org yet. For early access, see our [CI builds](#projects--packages-overview).

- [ActiveLogin.Authentication](#activeloginauthentication)
  - [Features](#features)
  - [Screenshots](#screenshots)
  - [Table of contents](#table-of-contents)
  - [Projects \& Packages overview](#projects--packages-overview)
  - [Usage \& Docs](#usage--docs)
  - [Samples](#samples)
  - [Tests](#tests)
  - [FAQ](#faq)
    - [What version of .NET is supported?](#what-version-of-net-is-supported)
    - [How do I build the solution locally?](#how-do-i-build-the-solution-locally)
      - [Devcontainer and GitHub Codespaces](#devcontainer-and-github-codespaces)
    - [How do I run the samples locally?](#how-do-i-run-the-samples-locally)
    - [How do I use Active Login to get support for BankID in Azure AD (Active Directory) B2C?](#how-do-i-use-active-login-to-get-support-for-bankid-in-azure-ad-active-directory-b2c)
  - [Active Login](#active-login)
    - [Security](#security)
    - [Contribute](#contribute)
      - [Contributors](#contributors)
    - [Stay updated and join the discussion](#stay-updated-and-join-the-discussion)
    - [License](#license)
    - [Acknowledgements](#acknowledgements)
    - [Sponsors](#sponsors)
    - [Support \& Training](#support--training)


## Projects & Packages overview

CI-builds from main of all packages are available in our [GitHub Packages feed](https://github.com/orgs/ActiveLogin/packages).

| Project | Description | NuGet | Downloads |
| ------- | ----------- | ----  | --------- |
| [BankId.Api](https://github.com/ActiveLogin/ActiveLogin.Authentication/tree/main/src/ActiveLogin.Authentication.BankId.Api) | API client for the Swedish BankID REST API. | [![NuGet](https://img.shields.io/nuget/vpre/ActiveLogin.Authentication.BankId.Api.svg)](https://www.nuget.org/packages/ActiveLogin.Authentication.BankId.Api/) | [![NuGet (Pre)](https://img.shields.io/nuget/dt/ActiveLogin.Authentication.BankId.Api.svg)](https://www.nuget.org/packages/ActiveLogin.Authentication.BankId.Api/) |
| [BankId.Core](https://github.com/ActiveLogin/ActiveLogin.Authentication/tree/main/src/ActiveLogin.Authentication.BankId.Core) | Core functionality for the BankID flow. | [![NuGet](https://img.shields.io/nuget/vpre/ActiveLogin.Authentication.BankId.Core.svg)](https://www.nuget.org/packages/ActiveLogin.Authentication.BankId.Core/) | [![NuGet (Pre)](https://img.shields.io/nuget/dt/ActiveLogin.Authentication.BankId.Core.svg)](https://www.nuget.org/packages/ActiveLogin.Authentication.BankId.Core/) |
| [BankId.AspNetCore](https://github.com/ActiveLogin/ActiveLogin.Authentication/tree/main/src/ActiveLogin.Authentication.BankId.AspNetCore) | ASP.NET authentication module for Swedish BankID. | [![NuGet](https://img.shields.io/nuget/vpre/ActiveLogin.Authentication.BankId.AspNetCore.svg)](https://www.nuget.org/packages/ActiveLogin.Authentication.BankId.AspNetCore/) | [![NuGet (Pre)](https://img.shields.io/nuget/dt/ActiveLogin.Authentication.BankId.AspNetCore.svg)](https://www.nuget.org/packages/ActiveLogin.Authentication.BankId.AspNetCore/) |
| [BankId.AzureKeyVault](https://github.com/ActiveLogin/ActiveLogin.Authentication/tree/main/src/ActiveLogin.Authentication.BankId.AzureKeyVault) | Azure KeyVault integrations for the AspNetCore package. | [![NuGet](https://img.shields.io/nuget/vpre/ActiveLogin.Authentication.BankId.AzureKeyVault.svg)](https://www.nuget.org/packages/ActiveLogin.Authentication.BankId.AzureKeyVault/) | [![NuGet (Pre)](https://img.shields.io/nuget/dt/ActiveLogin.Authentication.BankId.AzureKeyVault.svg)](https://www.nuget.org/packages/ActiveLogin.Authentication.BankId.AzureKeyVault/) |
| [BankId.AzureMonitor](https://github.com/ActiveLogin/ActiveLogin.Authentication/tree/main/src/ActiveLogin.Authentication.BankId.AzureMonitor) | Azure Monitor (Application Insights) integrations for the AspNetCore package. | [![NuGet](https://img.shields.io/nuget/vpre/ActiveLogin.Authentication.BankId.AzureMonitor.svg)](https://www.nuget.org/packages/ActiveLogin.Authentication.BankId.AzureMonitor/) | [![NuGet (Pre)](https://img.shields.io/nuget/dt/ActiveLogin.Authentication.BankId.AzureMonitor.svg)](https://www.nuget.org/packages/ActiveLogin.Authentication.BankId.AzureMonitor/) |
| [BankId.QRCoder](https://github.com/ActiveLogin/ActiveLogin.Authentication/tree/main/src/ActiveLogin.Authentication.BankId.QRCoder) | QR code generation using QRCoder the AspNetCore package. | [![NuGet](https://img.shields.io/nuget/vpre/ActiveLogin.Authentication.BankId.QRCoder.svg)](https://www.nuget.org/packages/ActiveLogin.Authentication.BankId.QRCoder/) | [![NuGet (Pre)](https://img.shields.io/nuget/dt/ActiveLogin.Authentication.BankId.QRCoder.svg)](https://www.nuget.org/packages/ActiveLogin.Authentication.BankId.QRCoder/) |
| [BankId.UAParser](https://github.com/ActiveLogin/ActiveLogin.Authentication/tree/main/src/ActiveLogin.Authentication.BankId.UACoder) | Device and browser detection using UAParser. | [![NuGet](https://img.shields.io/nuget/vpre/ActiveLogin.Authentication.BankId.UAParser.svg)](https://www.nuget.org/packages/ActiveLogin.Authentication.BankId.UAParser/) | [![NuGet (Pre)](https://img.shields.io/nuget/dt/ActiveLogin.Authentication.BankId.UAParser.svg)](https://www.nuget.org/packages/ActiveLogin.Authentication.BankId.UAParser/) |


## Usage & Docs

Full documentation with step by step instructions, samples, customization and details on how to configure the options is available here:

[Documentation for ActiveLogin.Authentication.BankID](docs/articles/bankid.md)

Active Login is designed to make it easy to get started with BankID in .NET. The most basic setup looks like this:

```csharp
// Common
services
    .AddBankId(bankId =>
    {
        bankId.UseTestEnvironment();
    });

// Auth
services
    .AddAuthentication()
    .AddBankIdAuth(bankId =>
    {
        bankId.AddSameDevice();
    });

// Sign
services
    .AddBankIdSign(bankId =>
    {
        bankId.AddSameDevice();
    });
```


---


## Samples

For more use cases, samples and inspiration; feel free to browse our [unit tests](https://github.com/ActiveLogin/ActiveLogin.Authentication/tree/main/test) and [samples](https://github.com/ActiveLogin/ActiveLogin.Authentication/tree/main/samples).

_Note: These are samples on how to use Active Login in different situations and might not represent optimal way of setting up ASP.NET MVC, IdentityServer or other components. Please see them as inspiration._

| Project | Description |
| ------- | ----------- |
| [IdentityServer.ClientSample](https://github.com/ActiveLogin/ActiveLogin.Authentication/tree/main/samples/IdentityServer.ClientSample) | ASP.NET MVC site using the IdentityServer.ServerSample as auth provider. |
| [IdentityServer.ServerSample](https://github.com/ActiveLogin/ActiveLogin.Authentication/tree/main/samples/IdentityServer.ServerSample) | IdentityServer with Active Login as auth provider for BankID. |
| [Standalone.MvcSample](https://github.com/ActiveLogin/ActiveLogin.Authentication/tree/main/samples/Standalone.MvcSample) | ASP.NET MVC with Active Login as auth provider for BankID. Also demo of Sign. |
| [AzureProvisioningSample](https://github.com/ActiveLogin/ActiveLogin.Authentication/tree/main/samples/AzureProvisioningSample) | ARM template with Azure KeyVault, Azure App Service, Azure Monitor / Application Insights etc. |
| [Phone.ConsoleSample](https://github.com/ActiveLogin/ActiveLogin.Authentication/tree/main/samples/Phone.ConsoleSample) | Console application with a simple Phone Auth/Sign example |

_Please note that IdentityServer.ClientSample uses IdentityServer.ServerSample as the IdentityProvider, so the IdentityServer.ClientSample is a good place to start._

A live demo is available at [https://demo.activelogin.net/](https://demo.activelogin.net/).

## Tests

* [BankId.Api.Test](https://github.com/ActiveLogin/ActiveLogin.Authentication/tree/main/test/ActiveLogin.Authentication.BankId.Api.Test)
* [BankId.Core.Test](https://github.com/ActiveLogin/ActiveLogin.Authentication/tree/main/test/ActiveLogin.Authentication.BankId.Core.Test)
* [BankId.AspNetCore.Test](https://github.com/ActiveLogin/ActiveLogin.Authentication/tree/main/test/ActiveLogin.Authentication.BankId.AspNetCore.Test)
* [BankId.UAParser.Test](https://github.com/ActiveLogin/ActiveLogin.Authentication/tree/main/test/ActiveLogin.Authentication.BankId.UAParser.Test)
* [BankId.AzureKeyVault.Test](https://github.com/ActiveLogin/ActiveLogin.Authentication/tree/main/test/ActiveLogin.Authentication.BankId.AzureKeyVault.Test)


---


## FAQ

Here is a summary of common, general technical questions.

The [docs for ActiveLogin.Authentication.BankId](docs/articles/bankid.md) contains an FAQ specific to BankID.

For commercial / business related questions, see the [FAQ at ActiveLogin.net](https://activelogin.net/#faq).


### What version of .NET is supported?

The API-wrapper (ActiveLogin.Authentication.BankId.Api) target .NET Standard 2.0, so it can be used from .NET >= 5.0, .NET Core >= 2.0 and .NET Framework >= 4.6.1, [see full reference here](https://docs.microsoft.com/en-us/dotnet/standard/net-standard#net-implementation-support). The package that target .NET Standard is strong named as it can be used from .NET Framework where strong naming can be relevant.

The authentication module (*.AspNetCore), depend on ASP.NET 8 and therefore requires .NET 8.
The core module (*.Core), and related packages, depend on and requires .NET 8.

Our samples target .NET 8 and follow the conventions used there.


### How do I build the solution locally?

Active Login is built using .NET, make sure you have the relevant version of the SDK and runtime installed.

Run the following command in the root to build all projects:

```console
dotnet build
```

Run the following command in the root to run all tests:

```console
dotnet test
```

#### Devcontainer and GitHub Codespaces

We also support [devcontainer.json](https://code.visualstudio.com/docs/remote/containers#_create-a-devcontainerjson-file) so that you can [build the solution in a container](https://code.visualstudio.com/docs/remote/containers) and use [GitHub Codespaces](https://docs.github.com/en/codespaces/overview).

### How do I run the samples locally?

The samples are configured to run for the test environment (no BankID certificate required as it's bundled with the library) by default.
The _Standalone.MvcSample_ is using the the most basic sample and is a good start.

To run the sample: Navigate to `/Samples/Standalone.MvcSample/` and run:

```console
dotnet run
```


### How do I use Active Login to get support for BankID in Azure AD (Active Directory) B2C?

Azure AD B2C supports using custom identity providers that supports [Open ID Connect](https://docs.microsoft.com/sv-se/azure/active-directory-b2c/active-directory-b2c-reference-oidc). If you deploy Active Login as part of IdentityServer (see our samples) you can configure your Azure AD B2C to federate to that instance and by doing so get BankID support.

![Active Login with Azure AD B2C](https://alresourcesprod.blob.core.windows.net/docsassets/active-login-setup-azuread-b2c.png)


---


## Active Login

Active Login is an Open Source project built on .NET that makes it easy to integrate with leading Swedish authentication services like [BankID](https://www.bankid.com/).


### Security

In our [Security Policy](SECURITY.md) you can read about how to report a vulnerability, how to subscribe to security alerts and what packages we currently support.


### Contribute

We are very open to community contributions to Active Login.
Please see our [contribution guidelines](CONTRIBUTING.md) before getting started.

#### Contributors

Thank you to all who have and are contributing to this project!

![Contributors](https://contrib.rocks/image?repo=ActiveLogin/ActiveLogin.Authentication)


### Stay updated and join the discussion

The three primary ways to interact and stay updated with Active Login are:

- [Watch our GitHub repo](https://github.com/ActiveLogin/ActiveLogin.Authentication/watchers)
- [Interact on GitHub Discussions](https://github.com/ActiveLogin/ActiveLogin.Authentication/discussions)
- [Follow us on Twitter](https://twitter.com/ActiveLoginSE)


### License

[Active Login is licensed](LICENSE.md) under the very permissive [MIT license](https://opensource.org/licenses/MIT) for you to be able to use it in commercial or non-commercial applications without many restrictions.

The BankID certificates from the BankID documentation are released as part of Active Login with the permission from BankID (Finansiell ID-Teknik BID AB).

*All trademarks are the property of their respective owners.*


### Acknowledgements

Active Login is built on or uses the following great open source products:

* [.NET](https://github.com/dotnet/core)
* [ASP.NET](https://github.com/aspnet/Home)
* [XUnit](https://github.com/xunit/xunit)
* [QRCoder](https://github.com/codebude/QRCoder)
* [UAParser](https://github.com/ua-parser/uap-csharp)
* [AngleSharp](https://github.com/AngleSharp/AngleSharp)
* [Moq](https://github.com/moq/moq)
* [Bootstrap](https://github.com/twbs/bootstrap)
* [Loading.io](https://loading.io/)

For samples, these great products are used:

* [Duende IdentityServer](https://duendesoftware.com/products/identityserver)


### Sponsors

Active Solution is the main sponsor of Active Login. Active Solution is located in Sweden and provides IT consulting with focus on web, Azure and AI.


![Active Solution](https://alresourcesprod.blob.core.windows.net/docsassets/activesolution-logo.svg)

_Bright cloud solutions - System development that shines. Together, we create systems that will rocket your business._

And yes, [we are hiring](https://www.activesolution.se/jobb/) :woman_technologist: :)

[https://www.activesolution.se/](https://www.activesolution.se/)


### Support & Training

If you need help with implementing Active Login, there are commercial support & training options available.

We can help you out with:

- Education and training on:
	- Active Login
	- IdentityServer
	- Azure AD B2C
	- Authentication on the .NET platform in general
- Hands on implementing BankID using Active Login
- Implement BankID as a custom Identity Provider for Azure AD B2C
- Continuous support for Active Login

See [ActiveLogin.net](https://activelogin.net#support) for more details on how to get in touch with us :telephone_receiver:.
