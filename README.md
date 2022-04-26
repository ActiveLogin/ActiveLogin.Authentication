# ActiveLogin.Authentication

[![License: MIT](https://img.shields.io/badge/License-MIT-orange.svg)](https://opensource.org/licenses/MIT)
[![Build Status](https://github.com/ActiveLogin/ActiveLogin.Authentication/actions/workflows/build.yml/badge.svg)](https://github.com/ActiveLogin/ActiveLogin.Authentication/actions/workflows/build.yml)
[![Live demo](https://img.shields.io/static/v1?label=Demo&message=Live%20demo&color=008bc3)](https://demo.activelogin.net/)
[![Docs](https://img.shields.io/static/v1?label=Docs&message=Documentation&color=008bc3)](https://docs.activelogin.net/)
[![Discussion](https://img.shields.io/github/discussions/ActiveLogin/ActiveLogin.Authentication)](https://github.com/ActiveLogin/ActiveLogin.Authentication/discussions)
[![Twitter Follow](https://img.shields.io/badge/Twitter-@ActiveLoginSE-blue.svg?logo=twitter)](https://twitter.com/ActiveLoginSE)


ActiveLogin.Authentication enables an application to support Swedish BankID (svenskt BankID) authentication in .NET. Built on NET Standard and packaged as NuGet-packages they are easy to install and use on multiple platforms.

Free to use, [commercial support and training](#support--training) is available if you need assistance or a quick start.


## Features

- :id: Supports BankID Auth and Sign
- :penguin: Cross platform: Targets .NET Standard 2.0 and .NET 6
- :five: Built on V5.1 (the latest) BankID JSON API
- :white_square_button: Supports BankID animated QR code (Safe start)
- :lock: GDPR: Security by design
- :cloud: Designed with Microsoft Azure in mind (KeyVault, Monitor, Application Insights, AD B2C etc.)
- :earth_americas: Multi language support with English and Swedish out of the box
- :wrench: Customizable UI
- :diamond_shape_with_a_dot_inside: Can be used as a [Custom Identity Provider for Azure AD B2C](#how-do-i-use-active-login-to-get-support-for-bankid-in-azure-ad-active-directory-b2c)


## Screenshots

_Screenshots on how the default UI for Native BankID looks on different devices._

![Active Login Screenshots](https://alresourcesprod.blob.core.windows.net/docsassets/active-login-screenshots.png)

_Screenshot on monitoring dashboard._

![Active Login Monitor](https://alresourcesprod.blob.core.windows.net/docsassets/active-login-monitor-screenshot_1.png)


## Table of contents

___Note:___ This Readme reflects the state of our main branch and the code documented here might not be released as packages on NuGet.org yet. For early access, see our [CI builds](#projects--packages-overview).

* [Features](#features)
* [Screenshots](#screenshots)
* [Projects & Packages overview](#projects--packages-overview)
* [Usage & Docs](#usage--docs)
* [Samples](#samples)
* [Tests](#tests)
* [FAQ](#faq)
  + [What version of .NET is supported?](#what-version-of-net-is-supported)
  + [How do I build the solution locally?](#how-do-i-build-the-solution-locally)
  + [How do I run the samples locally?](#how-do-i-run-the-samples-locally)
  + [How do I use Active Login to get support for BankID in Azure AD (Active Directory) B2C?](#how-do-i-use-active-login-to-get-support-for-bankid-in-azure-ad-active-directory-b2c)
  + [Where did the packages for GrandId / Svensk E-identitet go?](#where-did-the-packages-for-grandid--svensk-e-identitet-go)
* [Active Login](#active-login)
  + [Security](#security)
  + [Contribute](#contribute)
  + [Stay updated and join the discussion](#stay-updated-and-join-the-discussion)
  + [License](#license)
  + [Acknowledgements](#acknowledgements)
  + [Sponsors](#sponsors)
  + [Support & Training](#support--training)


## Projects & Packages overview

CI-builds from main of all packages are available in our [GitHub Packages feed](https://github.com/orgs/ActiveLogin/packages).

| Project | Description | NuGet | Downloads |
| ------- | ----------- | ----  | --------- |
| [BankId.Api](https://github.com/ActiveLogin/ActiveLogin.Authentication/tree/main/src/ActiveLogin.Authentication.BankId.Api) | API client for the Swedish BankID REST API. | [![NuGet](https://img.shields.io/nuget/vpre/ActiveLogin.Authentication.BankId.Api.svg)](https://www.nuget.org/packages/ActiveLogin.Authentication.BankId.Api/) | [![NuGet (Pre)](https://img.shields.io/nuget/dt/ActiveLogin.Authentication.BankId.Api.svg)](https://www.nuget.org/packages/ActiveLogin.Authentication.BankId.Api/) |
| [BankId.AspNetCore](https://github.com/ActiveLogin/ActiveLogin.Authentication/tree/main/src/ActiveLogin.Authentication.BankId.AspNetCore) | ASP.NET authentication module for Swedish BankID. | [![NuGet](https://img.shields.io/nuget/vpre/ActiveLogin.Authentication.BankId.AspNetCore.svg)](https://www.nuget.org/packages/ActiveLogin.Authentication.BankId.AspNetCore/) | [![NuGet (Pre)](https://img.shields.io/nuget/dt/ActiveLogin.Authentication.BankId.AspNetCore.svg)](https://www.nuget.org/packages/ActiveLogin.Authentication.BankId.AspNetCore/) |
| [BankId.AspNetCore.Azure](https://github.com/ActiveLogin/ActiveLogin.Authentication/tree/main/src/ActiveLogin.Authentication.BankId.AspNetCore.Azure) | Azure KeyVault integrations for the AspNetCore package. | [![NuGet](https://img.shields.io/nuget/vpre/ActiveLogin.Authentication.BankId.AspNetCore.Azure.svg)](https://www.nuget.org/packages/ActiveLogin.Authentication.BankId.AspNetCore.Azure/) | [![NuGet (Pre)](https://img.shields.io/nuget/dt/ActiveLogin.Authentication.BankId.AspNetCore.Azure.svg)](https://www.nuget.org/packages/ActiveLogin.Authentication.BankId.AspNetCore.Azure/) |
| [BankId.AspNetCore.AzureMonitor](https://github.com/ActiveLogin/ActiveLogin.Authentication/tree/main/src/ActiveLogin.Authentication.BankId.AspNetCore.AzureMonitor) | Azure Monitor (Application Insights) integrations for the AspNetCore package. | [![NuGet](https://img.shields.io/nuget/vpre/ActiveLogin.Authentication.BankId.AspNetCore.AzureMonitor.svg)](https://www.nuget.org/packages/ActiveLogin.Authentication.BankId.AspNetCore.AzureMonitor/) | [![NuGet (Pre)](https://img.shields.io/nuget/dt/ActiveLogin.Authentication.BankId.AspNetCore.AzureMonitor.svg)](https://www.nuget.org/packages/ActiveLogin.Authentication.BankId.AspNetCore.AzureMonitor/) |
| [BankId.AspNetCore.QRCoder](https://github.com/ActiveLogin/ActiveLogin.Authentication/tree/main/src/ActiveLogin.Authentication.BankId.AspNetCore.QRCoder) | QR code generation using QRCoder the AspNetCore package. | [![NuGet](https://img.shields.io/nuget/vpre/ActiveLogin.Authentication.BankId.AspNetCore.QRCoder.svg)](https://www.nuget.org/packages/ActiveLogin.Authentication.BankId.AspNetCore.QRCoder/) | [![NuGet (Pre)](https://img.shields.io/nuget/dt/ActiveLogin.Authentication.BankId.AspNetCore.QRCoder.svg)](https://www.nuget.org/packages/ActiveLogin.Authentication.BankId.AspNetCore.QRCoder/) |
| [BankId.AspNetCore.UAParser](https://github.com/ActiveLogin/ActiveLogin.Authentication/tree/main/src/ActiveLogin.Authentication.BankId.AspNetCore.UACoder) | Device and browser detection using UAParser. | [![NuGet](https://img.shields.io/nuget/vpre/ActiveLogin.Authentication.BankId.AspNetCore.UAParser.svg)](https://www.nuget.org/packages/ActiveLogin.Authentication.BankId.AspNetCore.UAParser/) | [![NuGet (Pre)](https://img.shields.io/nuget/dt/ActiveLogin.Authentication.BankId.AspNetCore.UAParser.svg)](https://www.nuget.org/packages/ActiveLogin.Authentication.BankId.AspNetCore.UAParser/) |


## Usage & Docs

Full documentation with step by step instructions, samples, customization and details on how to configure the options is available here:

[Documentation for ActiveLogin.Authentication.BankID](docs/articles/bankid.md)

Active Login is designed to make it easy to get started with BankID in .NET. The most basic setup looks like this:

```csharp
services
    .AddAuthentication()
    .AddBankId(builder =>
    {
        builder
            .UseSimulatedEnvironment()
            .AddSameDevice();
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
| [Standalone.MvcSample](https://github.com/ActiveLogin/ActiveLogin.Authentication/tree/main/samples/Standalone.MvcSample) | ASP.NET MVC with Active Login as auth provider for BankID. |
| [AzureProvisioningSample](https://github.com/ActiveLogin/ActiveLogin.Authentication/tree/main/samples/AzureProvisioningSample) | ARM template with Azure KeyVault, Azure App Service, Azure Monitor / Application Insights etc. |

_Please note that IdentityServer.ClientSample uses IdentityServer.ServerSample as the IdentityProvider, so the IdentityServer.ClientSample is a good place to start._

A live demo is available at [https://demo.activelogin.net/](https://demo.activelogin.net/).

## Tests

* [BankId.Api.Test](https://github.com/ActiveLogin/ActiveLogin.Authentication/tree/main/test/ActiveLogin.Authentication.BankId.Api.Test)
* [BankId.AspNetCore.Test](https://github.com/ActiveLogin/ActiveLogin.Authentication/tree/main/test/ActiveLogin.Authentication.BankId.AspNetCore.Test)
* [BankId.AspNetCore.Azure.Test](https://github.com/ActiveLogin/ActiveLogin.Authentication/tree/main/test/ActiveLogin.Authentication.BankId.AspNetCore.Azure.Test)


---


## FAQ

Here is a summary of common, general technical questions.

The [docs for ActiveLogin.Authentication.BankId](docs/articles/bankid.md) contains an FAQ specific to BankID.

For commercial / business related questions, see the [FAQ at ActiveLogin.net](https://activelogin.net/#faq).


### What version of .NET is supported?

The API-wrapper (ActiveLogin.Authentication.BankId.Api) target .NET Standard 2.0, so it can be used from .NET >= 5.0, .NET Core >= 2.0 and .NET Framework >= 4.6.1, [see full reference here](https://docs.microsoft.com/en-us/dotnet/standard/net-standard#net-implementation-support). The package that target .NET Standard is strong named as it can be used from .NET Framework where strong naming can be relevant.

The authentication module (*.AspNetCore), and related packages, depend on ASP.NET 6 and therefore require .NET 6.

Our samples target .NET 6 and follow the conventions used there.


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


### How do I run the samples locally?

The samples are configured to run in simulated mode (no BankID certificate required) by default. The _Standalone.MvcSample_ is using the the most basic sample and is a good start.

To run the sample: Navigate to `/Samples/Standalone.MvcSample/` and run:

```console
dotnet run
```


### How do I use Active Login to get support for BankID in Azure AD (Active Directory) B2C?

Azure AD B2C supports using custom identity providers that supports [Open ID Connect](https://docs.microsoft.com/sv-se/azure/active-directory-b2c/active-directory-b2c-reference-oidc). If you deploy Active Login as part of IdentityServer (see our samples) you can configure your Azure AD B2C to federate to that instance and by doing so get BankID support.

![Active Login with Azure AD B2C](https://alresourcesprod.blob.core.windows.net/docsassets/active-login-setup-azuread-b2c.png)


### Where did the packages for GrandId / Svensk E-identitet go?

The packages in ActiveLogin for Svensk E-identitet / GrandID are not beeing worked on anymore, and are therefore not supported as part of this open source project.

The [documentation available](docs/articles/grandid.md) is provided as is and reflects the state for the 4.0.0 release of the GrandId packages.



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
