[![License: MIT](https://img.shields.io/badge/License-MIT-orange.svg)](https://opensource.org/licenses/MIT) [![Slack](https://img.shields.io/badge/slack-@ActiveLogin-blue.svg?logo=slack)](https://join.slack.com/t/activelogin/shared_invite/enQtODQ0ODYyMTgxMjg0LWJhODhiZmFmODYyMWMzZWEwMjdmYWU2NGRhZmQ0MTg0MzIwNzA2OTM3NTJjOTk2MmE1MzIwMzkzYjllMjAyNzg) [![Twitter Follow](https://img.shields.io/badge/Twitter-@ActiveLoginSE-blue.svg?logo=twitter)](https://twitter.com/ActiveLoginSE)

ActiveLogin.Authentication enables an application to support Swedish BankID (svenskt BankID) authentication in .NET. Built on NET Standard and packaged as NuGet-packages they are easy to install and use on multiple platforms. Used with Identity Server it can be configured as a provider for Azure AD B2C. Totally free to use! [Commercial support and traning](https://activelogin.net/#support) is available if you need assistance or a quick start. 

## Sample usage

Sample usage of the extension method to use client certificates for BankID from Azure KeyVault.

Adds the `UseClientCertificateFromAzureKeyVault(...)` extension method.

```csharp
services
    .AddAuthentication()
    .AddBankId(builder =>
    {
        builder
            .UseClientCertificateFromAzureKeyVault(Configuration.GetSection("ActiveLogin:BankId:ClientCertificate"));
    });
```

The expected configuration looks like this:

```json
{
    "ActiveLogin:BankId:ClientCertificate" {
        "UseManagedIdentity": true,

        "AzureAdTenantId": "",
        "AzureAdClientId": "",
        "AzureAdClientSecret": "",

        "AzureKeyVaultUri": "TODO-ADD-YOUR-VALUE",
        "AzureKeyVaultSecretKey": "TODO-ADD-YOUR-VALUE"
    }
}
```

## Full documentation

For full documentation and samples, see the Readme in our [GitHub repo](https://github.com/ActiveLogin/ActiveLogin.Authentication).