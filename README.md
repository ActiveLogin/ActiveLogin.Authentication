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

It is expected that you have a basic understanding of how ASP.NET Core, ASP.NET Core MVC and ASP.NET Core Authentication works before getting started.

If this is your first external authentication provider, create or configure your `AccountController.cs` to support showing available AutehticationProviders as well as handling external logins. An example is [provided in IdentityServerSample](https://github.com/ActiveLogin/ActiveLogin.Authentication/blob/master/samples/IdentityServerSample/Controllers/AccountController.cs).

#### 2.1 Try it out with an in memory Development environment

BankID requires you to use a client certificate and trust a specific root CA-certificate. To try out the module without those certificates, you can use an in memory development implementation of the BankID REST API.

The authentication part in your `Startup.cs` should look something like this:

```c#
services.AddAuthentication()
        .AddBankId();

services.AddBankIdDevelopmentEnvironment();
```

#### 2.2 Try it out with Bank ID test environment

To start using a real implementation of BankID, there are a few steps to do. These steps describes the scenario where you utilize Azure for things like secure storage of the certificate in an Azure KeyVault.

1. Start by reading through the _BankID Relying Party Guidelines_ [available for download here](https://www.bankid.com/bankid-i-dina-tjanster/rp-info). This ensures you have a basic understanding of the terminology as well as how the flow and security works.
2. Deploy Azure KeyVault to yur subscription, the ARM-template available in [AzureProvisioningSample](https://github.com/ActiveLogin/ActiveLogin.Authentication/tree/master/samples/AzureProvisioningSample) contains a KeyVault, so preferably you deploy that.
3. Download the _SSL certificate for test (FPTestcert2.pfx)_ [available for download here](https://www.bankid.com/bankid-i-dina-tjanster/rp-info).
4. [Import the certificate to your Azure Key Vault.](https://docs.microsoft.com/en-us/azure/key-vault/certificate-scenarios#import-a-certificate)
5. Add the following to you `Startup.cs`:

```c#
services.AddAuthentication()
        .AddBankId()
            .AddBankIdClientCertificateFromAzureKeyVault(Configuration.GetSection("ActiveLogin:BankId:ClientCertificate"))
            .AddBankIdTestEnvironment();
```

6. Add the following configuration values. The AD client should have access to the KeyVault certificate specified in `AzureKeyVaultSecretIdentifier`.

```json
{
  "ActiveLogin:BankId:ClientCertificate:AzureAdClientId": "TODO-ADD-YOUR-VALUE",
  "ActiveLogin:BankId:ClientCertificate:AzureAdClientSecret": "TODO-ADD-YOUR-VALUE",
  "ActiveLogin:BankId:ClientCertificate:AzureKeyVaultSecretIdentifier": "TODO-ADD-YOUR-VALUE"
}
```

7. The root CA-certificate specified in _BankID Relying Party Guidelines_ needs to be trusted at the computer where the app will run.
8. If running in Azure App Service, where trusting custom certificates is not supported, there are extensions to handle that scenario.
9. Instead of trusting the certificate, place it in your web project.
10. Right after `.AddBankIdClientCertificateFromAzureKeyVault(..)`, add the following line:

```c#
.AddBankIdRootCaCertificate(Path.Combine(_environment.ContentRootPath, Configuration.GetValue<string>("ActiveLogin:BankId:CaCertificate:FilePath")))
```

 11. Add the following configuration values. The `FilePath` should point to the certificate you just added, for example:

```json
{
  "ActiveLogin:BankId:CaCertificate:FilePath": "Certificates\\BankIdRootCertificate-Test.crt"
}
```

12. You should now be ready to start!

#### 2.3 Use production environment

To use BankID production environment, the procedure is the same as for test, but the API url and the certificates are different.

1. Start by [contacting a reseller](https://www.bankid.com/kontakt/foeretag/saeljare) to get your very own client certificate. This will probably take a few business days to get sorted.
2. Repeat the process above by uploading the certifdicate to KeyVault and reconfigure your app to use the production certificate.
3. Change your application to trust the root CA-certificate. for production instead of test, this certificate can also be found in the file _BankID Relying Party Guidelines_ [available for download here](https://www.bankid.com/bankid-i-dina-tjanster/rp-info).
4. The final Startup.cs should look something like this:

```c#
services.AddAuthentication()
    .AddBankId()
        .AddBankIdClientCertificateFromAzureKeyVault(Configuration.GetSection("ActiveLogin:BankId:ClientCertificate"))
        .AddBankIdRootCaCertificate(Path.Combine(_environment.ContentRootPath, Configuration.GetValue<string>("ActiveLogin:BankId:CaCertificate:FilePath")))
```

5. Enjoy BankID in your application :)

### 3. Browse tests and samples

For more usecases, samples and inspiration; feel free to browse our unit tests and samples:

* [IdentityServerSample](https://github.com/ActiveLogin/ActiveLogin.Authentication/tree/master/samples/IdentityServerSample)
* [MvcClientSample](https://github.com/ActiveLogin/ActiveLogin.Authentication/tree/master/samples/MvcClientSample)
* [AzureProvisioningSample](https://github.com/ActiveLogin/ActiveLogin.Authentication/tree/master/samples/AzureProvisioningSample)
* [ActiveLogin.Authentication.BankId.Api.Test](https://github.com/ActiveLogin/ActiveLogin.Authentication/tree/master/test/ActiveLogin.Authentication.BankId.Api.Test)

## FAQ

### Can I try out a live demo of the samples?

Yes! They are available here. Please note that MvcClientSample uses IdentityServerSample as the IdentityProvider, so the MvcClientSample is a good place to start.

* MvcClientSample: [https://al-samples-mvcclient.azurewebsites.net](https://al-samples-mvcclient.azurewebsites.net)
* IdentityServerSample: [https://al-samples-identityserver.azurewebsites.net](https://al-samples-identityserver.azurewebsites.net)

## Can the UI be customized?

Yes! The UI is bundled into the package as a Razor Class Library, a technique that allows to [override the parts you want to customize](https://docs.microsoft.com/en-us/aspnet/core/razor-pages/ui-class?view=aspnetcore-2.1&tabs=visual-studio#override-views-partial-views-and-pages). The Views and Controllers that can be customized can be found in the [GitHub repo](https://github.com/ActiveLogin/ActiveLogin.Authentication/tree/master/src/ActiveLogin.Authentication.BankId.AspNetCore/Areas/BankIdAuthentication). 

## Can the messages be localized?

The messages are already localized to English and Swedish using the official recomended texts. To select what texts are beeing used you can for example use the [localization middleware in ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/localization?view=aspnetcore-2.1#localization-middleware).

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