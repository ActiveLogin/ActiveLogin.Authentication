# ActiveLogin.Authentication

ActiveLogin.Authentication enables an application to support Swedish BankID's (svenskt BankIDs) authentication workflow in .NET. Built on NET Standard and packaged as NuGet-packages they are easy to install and use on multiple platforms.

## Continuous integration & Packages overview

| Project | Description | NuGet | Build (VSTS) |
| ------- | ----------- | ----- | ------------ |
| [ActiveLogin.Authentication.BankId.Api](https://github.com/ActiveLogin/ActiveLogin.Authentication/tree/master/src/ActiveLogin.Authentication.BankId.Api) | API client for Swedish BankID's REST API | [![NuGet](https://img.shields.io/nuget/v/ActiveLogin.Authentication.BankId.Api.svg)](https://www.nuget.org/packages/ActiveLogin.Authentication.BankId.Api/) | [![VSTS Build](https://activesolution.visualstudio.com/_apis/public/build/definitions/131274d9-4174-4035-a0e3-f6e5e9444d9f/155/badge)](https://activesolution.visualstudio.com/ActiveLogin/_build/index?definitionId=155) |
| [ActiveLogin.Authentication.BankId.AspNetCore](https://github.com/ActiveLogin/ActiveLogin.Authentication/tree/master/src/ActiveLogin.Authentication.BankId.AspNetCore) | ASP.NET Core middleware for Swedish BankID | [![NuGet](https://img.shields.io/nuget/v/ActiveLogin.Authentication.BankId.AspNetCore.svg)](https://www.nuget.org/packages/ActiveLogin.Authentication.BankId.AspNetCore/) | [![VSTS Build](https://activesolution.visualstudio.com/_apis/public/build/definitions/131274d9-4174-4035-a0e3-f6e5e9444d9f/155/badge)](https://activesolution.visualstudio.com/ActiveLogin/_build/index?definitionId=155) |
| [ActiveLogin.Authentication.BankId.AspNetCore.Azure](https://github.com/ActiveLogin/ActiveLogin.Authentication/tree/master/src/ActiveLogin.Authentication.BankId.AspNetCore.Azure) | Azure integrations for ActiveLogin.Authentication.BankId.AspNetCore | [![NuGet](https://img.shields.io/nuget/v/ActiveLogin.Authentication.BankId.AspNetCore.Azure.svg)](https://www.nuget.org/packages/ActiveLogin.Authentication.BankId.AspNetCore.Azure/) | [![VSTS Build](https://activesolution.visualstudio.com/_apis/public/build/definitions/131274d9-4174-4035-a0e3-f6e5e9444d9f/155/badge)](https://activesolution.visualstudio.com/ActiveLogin/_build/index?definitionId=155) |
| [ActiveLogin.Authentication.Common](https://github.com/ActiveLogin/ActiveLogin.Authentication/tree/master/src/ActiveLogin.Authentication.Common) | Handles common tasks in ActiveLogin.Authentication | [![NuGet](https://img.shields.io/nuget/v/ActiveLogin.Authentication.Common.svg)](https://www.nuget.org/packages/ActiveLogin.Authentication.Common/) | [![VSTS Build](https://activesolution.visualstudio.com/_apis/public/build/definitions/131274d9-4174-4035-a0e3-f6e5e9444d9f/155/badge)](https://activesolution.visualstudio.com/ActiveLogin/_build/index?definitionId=155) |

## Getting started

### 1. Install the NuGet package

ActiveLogin.Authentication is distributed as [packages on NuGet](https://www.nuget.org/profiles/ActiveLogin), install using the tool of your choice.

For example using .NET CLI:
```powershell
dotnet add package ActiveLogin.Authentication.BankId.AspNetCore
```

### 2. Use the classes in your project
TODO

### 3. Browse tests and samples

For more usecases, samples and inspiration; feel free to browse our unit tests and samples:

* [IdentityServerSample](https://github.com/ActiveLogin/ActiveLogin.Authentication/tree/master/samples/IdentityServerSample)
* [MvcClientSample](https://github.com/ActiveLogin/ActiveLogin.Authentication/tree/master/samples/MvcClientSample)
* [AzureProvisioningSample](https://github.com/ActiveLogin/ActiveLogin.Authentication/tree/master/samples/AzureProvisioningSample)
* [ActiveLogin.Authentication.BankId.Api.Test](https://github.com/ActiveLogin/ActiveLogin.Authentication/tree/master/test/ActiveLogin.Authentication.BankId.Api.Test)

## FAQ

### Can I try out a live demo of the samples?

Yes! They are available here:
* MvcClientSample: https://al-samples-mvcclient.azurewebsites.net
* IdentityServerSample: https://al-samples-identityserver.azurewebsites.net

## ActiveLogin

_Integrating your systems with market leading authentication services._

ActiveLogin is an Open Source project built on .NET Standard that makes it easy to integrate with leading Swedish authentication services like [BankID](https://www.bankid.com/).

It also provide examples of how to use it with the popular OpenID Connect & OAuth 2.0 Framework [IdentityServer](https://identityserver.io/) and provides a template for hosting the solution in Microsoft Azure.
In addition, ActiveLogin also contain convenient modules that help you work with and handle validation of Swedish Personal Identity Number (svenskt personnummer).

### Contribute

We are very open to community contributions to ActiveLogin. You'll need a basic understanding of Git and GitHub to get started. The easiest way to contribute is to open an issue and start a discussion. If you make code changes, submit a pull request with the changes and a description. Don’t forget to always provide tests that cover the code changes. 

### License & acknowledgements

ActiveLogin is licensed under the very permissive [MIT license](https://opensource.org/licenses/MIT) for you to be able to use it in commercial or non-commercial applications without many restrictions.

ActiveLogin is built on or uses the following great open source products:

* [.NET Standard](https://github.com/dotnet/standard)
* [ASP.NET Core](https://github.com/aspnet/Home)
* [XUnit](https://github.com/xunit/xunit)
* [IdentityServer](https://github.com/IdentityServer/)
* [Bootstrap](https://github.com/twbs/bootstrap)
* [Font Awesome](https://github.com/FortAwesome/Font-Awesome)