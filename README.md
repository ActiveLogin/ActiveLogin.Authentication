# ActiveLogin.Authentication

ActiveLogin.Authentication enables an application to support Swedish BankID's (svenskt BankIDs) authentication workflow in .NET. Built on NET Standard and packaged as NuGet-packages they are easy to install and use on multiple platforms.

## Continuous integration & Packages overview

[![Build status](https://dev.azure.com/activesolution/ActiveLogin/_apis/build/status/ActiveLogin.Authentication)](https://dev.azure.com/activesolution/ActiveLogin/_build/latest?definitionId=155)

| Project | Description | NuGet |
| ------- | ----------- | ----- |
| [ActiveLogin.Authentication.BankId.Api](https://github.com/ActiveLogin/ActiveLogin.Authentication/tree/master/src/ActiveLogin.Authentication.BankId.Api) | API client for Swedish BankID's REST API | [![NuGet](https://img.shields.io/nuget/v/ActiveLogin.Authentication.BankId.Api.svg)](https://www.nuget.org/packages/ActiveLogin.Authentication.BankId.Api/) |
| [ActiveLogin.Authentication.BankId.AspNetCore](https://github.com/ActiveLogin/ActiveLogin.Authentication/tree/master/src/ActiveLogin.Authentication.BankId.AspNetCore) | ASP.NET Core authentication module for Swedish BankID | [![NuGet](https://img.shields.io/nuget/v/ActiveLogin.Authentication.BankId.AspNetCore.svg)](https://www.nuget.org/packages/ActiveLogin.Authentication.BankId.AspNetCore/) |
| [ActiveLogin.Authentication.BankId.AspNetCore.Azure](https://github.com/ActiveLogin/ActiveLogin.Authentication/tree/master/src/ActiveLogin.Authentication.BankId.AspNetCore.Azure) | Azure integrations for ActiveLogin.Authentication.BankId.AspNetCore | [![NuGet](https://img.shields.io/nuget/v/ActiveLogin.Authentication.BankId.AspNetCore.Azure.svg)](https://www.nuget.org/packages/ActiveLogin.Authentication.BankId.AspNetCore.Azure/) |
| [ActiveLogin.Authentication.GrandId.Api](https://github.com/ActiveLogin/ActiveLogin.Authentication/tree/master/src/ActiveLogin.Authentication.GrandId.Api) | API client for GrandID (Svensk E-identitet) REST API | [![NuGet](https://img.shields.io/nuget/v/ActiveLogin.Authentication.GrandId.Api.svg)](https://www.nuget.org/packages/ActiveLogin.Authentication.GrandId.Api/) |
| [ActiveLogin.Authentication.GrandId.AspNetCore](https://github.com/ActiveLogin/ActiveLogin.Authentication/tree/master/src/ActiveLogin.Authentication.GrandId.AspNetCore) | ASP.NET Core authentication module for GrandID (Svensk E-identitet) | [![NuGet](https://img.shields.io/nuget/v/ActiveLogin.Authentication.GrandId.AspNetCore.svg)](https://www.nuget.org/packages/ActiveLogin.Authentication.GrandId.AspNetCore/) |
| [ActiveLogin.Authentication.Common](https://github.com/ActiveLogin/ActiveLogin.Authentication/tree/master/src/ActiveLogin.Authentication.Common) | Handles common tasks in ActiveLogin.Authentication | [![NuGet](https://img.shields.io/nuget/v/ActiveLogin.Authentication.Common.svg)](https://www.nuget.org/packages/ActiveLogin.Authentication.Common/) |

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

There are providers for both [native BankID](https://www.bankid.com/bankid-i-dina-tjanster/sa-kommer-du-igang) as well as through [GrandID (Svensk E-identitet)](https://e-identitet.se/tjanster/inloggningsmetoder/bankid/), examples of them both will follow.

#### 2.1 Using native BankID

##### 2.1.1 Try it out with an in memory Development environment

BankID requires you to use a client certificate and trust a specific root CA-certificate. To try out the module without those certificates, you can use an in memory development implementation of the BankID REST API.

The authentication part in your `Startup.cs` should look something like this:

```c#
services.AddAuthentication()
        .AddBankId(builder =>
    {
        builder
            .UseDevelopmentEnvironment()
            .AddSameDevice(options => { })
            .AddOtherDevice(options => { });
    });
```

##### 2.1.2 Try it out with Bank ID test environment

To start using a real implementation of BankID, there are a few steps to do. These steps describes the scenario where you utilize Azure for things like secure storage of the certificate in an Azure KeyVault.

1. Start by reading through the _BankID Relying Party Guidelines_ [available for download here](https://www.bankid.com/bankid-i-dina-tjanster/rp-info). This ensures you have a basic understanding of the terminology as well as how the flow and security works.
2. Deploy Azure KeyVault to yur subscription, the ARM-template available in [AzureProvisioningSample](https://github.com/ActiveLogin/ActiveLogin.Authentication/tree/master/samples/AzureProvisioningSample) contains a KeyVault, so preferably you deploy that.

[![Deploy to Azure](https://azuredeploy.net/deploybutton.png)](https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fgithub.com%2FActiveLogin%2FActiveLogin.Authentication%2Ftree%2Fmaster%2Fsamples%2FAzureProvisioningSample%2FActiveLogin.json)

3. Download the _SSL certificate for test (FPTestcert2.pfx)_ [available for download here](https://www.bankid.com/bankid-i-dina-tjanster/rp-info).
4. [Import the certificate to your Azure Key Vault.](https://docs.microsoft.com/en-us/azure/key-vault/certificate-scenarios#import-a-certificate)
5. Add the following to you `Startup.cs`:

```c#
services.AddAuthentication()
        .AddBankId(builder =>
    {
        builder
            .UseTestEnvironment()
            .UseClientCertificateFromAzureKeyVault(Configuration.GetSection("ActiveLogin:BankId:ClientCertificate"))
            .AddSameDevice(options => { })
            .AddOtherDevice(options => { });
    });
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
9. Instead of trusting the certificate, place it in your web project and make sure `CopyToOutputDirectory` is set to `Always`:
```xml
<Content Include="Certificates\BankIdRootCertificate-Test.crt">	
  <CopyToOutputDirectory>Always</CopyToOutputDirectory>	
</Content>
<Content Include="Certificates\BankIdRootCertificate-Prod.crt">	
  <CopyToOutputDirectory>Always</CopyToOutputDirectory>	
</Content>
```
10. Right after `.UseClientCertificateFromAzureKeyVault(..)`, add the following line:

```c#
.UseRootCaCertificate(Path.Combine(_environment.ContentRootPath, Configuration.GetValue<string>("ActiveLogin:BankId:CaCertificate:FilePath")))
```

 11. Add the following configuration values. The `FilePath` should point to the certificate you just added, for example:

```json
{
  "ActiveLogin:BankId:CaCertificate:FilePath": "Certificates\\BankIdRootCertificate-Test.crt"
}
```

12. You should now be ready to start!

##### 2.1.3 Use production environment

To use BankID production environment, the procedure is the same as for test, but the API url and the certificates are different.

1. Start by [contacting a reseller](https://www.bankid.com/kontakt/foeretag/saeljare) to get your very own client certificate. This will probably take a few business days to get sorted.
2. Repeat the process above by uploading the certifdicate to KeyVault and reconfigure your app to use the production certificate.
3. Change your application to trust the root CA-certificate. for production instead of test, this certificate can also be found in the file _BankID Relying Party Guidelines_ [available for download here](https://www.bankid.com/bankid-i-dina-tjanster/rp-info).
4. The final Startup.cs should look something like this:

```c#
services.AddAuthentication()
        .AddBankId(builder =>
    {
        builder
            .UseProductionEnvironment()
            .UseClientCertificateFromAzureKeyVault(Configuration.GetSection("ActiveLogin:BankId:ClientCertificate"))
            .UseRootCaCertificate(Path.Combine(_environment.ContentRootPath, Configuration.GetValue<string>("ActiveLogin:BankId:CaCertificate:FilePath")))
            .AddSameDevice(options => { })
            .AddOtherDevice(options => { });
    });
```

5. BankId options allows you to set and override some options such as these:

```c#
.AddOtherDevice(options =>
{
    options.BankIdAllowBiometric = false;
    options.BankIdCertificatePolicies = new List<string> { "1.2.752.78.1.1" };
});
```

6. Enjoy BankID in your application :)

#### 2.2 Using BankID through GrandID (Svensk E-identitet)

##### 2.2.1 Try it out with an in memory Development environment

GrandID requires you to have an API-key. To try out the module without that key, you can use an in memory development implementation of the GrandID REST API.

The authentication part in your `Startup.cs` should look something like this:

```c#
services.AddAuthentication()
        .AddGrandId(builder =>
        {
            builder
                .UseDevelopmentEnvironment()
                .AddSameDevice(options => { })
                .AddOtherDevice(options => { });
        });
```

##### 2.1.2 Try it out with Bank ID test or prod environment

To start using a real implementation of BankID through GrandID, there are a few steps to do.

1. [Get in touch with Svensk E-identitet](https://e-identitet.se/tjanster/inloggningsmetoder/bankid/) to recevie test and/or production keys.
2. Add the following to you `Startup.cs`:

```c#
services.AddAuthentication()
    .AddGrandId(builder =>
    {
        builder
			//.UseTestEnvironment(Configuration.GetValue<string>("ActiveLogin:GrandId:ApiKey"))
			.UseProductionEnvironment(Configuration.GetValue<string>("ActiveLogin:GrandId:ApiKey"))
            .AddSameDevice(options =>
            {
                options.AuthenticateServiceKey = Configuration.GetValue<string>("ActiveLogin:GrandId:SameDeviceServiceKey");
            })
            .AddOtherDevice(options =>
            {
                options.AuthenticateServiceKey = Configuration.GetValue<string>("ActiveLogin:GrandId:OtherDeviceServiceKey");
            });
    });
```

3. Add the following configuration values.

```json
{
  "ActiveLogin:GrandId:ApiKey": "TODO-ADD-YOUR-VALUE",
  "ActiveLogin:GrandId:SameDeviceServiceKey": "TODO-ADD-YOUR-VALUE",
  "ActiveLogin:GrandId:OtherDeviceServiceKey": "TODO-ADD-YOUR-VALUE",
  "ActiveLogin:GrandId:ChooseDeviceServiceKey": "TODO-ADD-YOUR-VALUE"
}
```

4. You should now be ready to start!
5. Optionally, you can let GrandID display the UI for choosing BankID, then use this instead:

```c#
services.AddAuthentication()
    .AddGrandId(builder =>
    {
        builder
			.UseProductionEnvironment(Configuration.GetValue<string>("ActiveLogin:GrandId:ApiKey"))
			.AddChooseDevice(options =>
            {
                options.AuthenticateServiceKey = Configuration.GetValue<string>("ActiveLogin:GrandId:ChooseDeviceServiceKey");
            });
    });
```

### 3. Browse tests and samples

For more usecases, samples and inspiration; feel free to browse our unit tests and samples:

* [IdentityServerSample](https://github.com/ActiveLogin/ActiveLogin.Authentication/tree/master/samples/IdentityServerSample)
* [MvcClientSample](https://github.com/ActiveLogin/ActiveLogin.Authentication/tree/master/samples/MvcClientSample)
* [AzureProvisioningSample](https://github.com/ActiveLogin/ActiveLogin.Authentication/tree/master/samples/AzureProvisioningSample)
* [ActiveLogin.Authentication.BankId.Api.Test](https://github.com/ActiveLogin/ActiveLogin.Authentication/tree/master/test/ActiveLogin.Authentication.BankId.Api.Test)

#### 4. Running the MVC Client sample

The samples are configured to run in development mode (no BankID certificates required) by default. The _MVC Client sample_ is using the _Identity Server Sample_ as its identity provider. So to run the _MVC Client_, the _Identity Server Sample_ needs to be running first.

The easiest way to try the sample out is to:

1. Configure the solution to use _Multiple startup projects_, and set it to start both _IdentityServerSample_ and _MvcClientSample_
2. Press F5

## FAQ

### Can I try out a live demo of the samples?

Yes! They are available here. Please note that MvcClientSample uses IdentityServerSample as the IdentityProvider, so the MvcClientSample is a good place to start.

* MvcClientSample: [https://al-samples-mvcclient.azurewebsites.net](https://al-samples-mvcclient.azurewebsites.net)
* IdentityServerSample: [https://al-samples-identityserver.azurewebsites.net](https://al-samples-identityserver.azurewebsites.net)

### Can the UI be customized?

Yes! The UI is bundled into the package as a Razor Class Library, a technique that allows to [override the parts you want to customize](https://docs.microsoft.com/en-us/aspnet/core/razor-pages/ui-class?view=aspnetcore-2.1&tabs=visual-studio#override-views-partial-views-and-pages). The Views and Controllers that can be customized can be found in the [GitHub repo](https://github.com/ActiveLogin/ActiveLogin.Authentication/tree/master/src/ActiveLogin.Authentication.BankId.AspNetCore/Areas/BankIdAuthentication). 

### Can the messages be localized?

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
