# ActiveLogin.Authentication.BankId.AzureKeyVault

ActiveLogin.Authentication enables an application to support Swedish BankID (svenskt BankID) authentication in .NET.
Built on NET Standard and packaged as NuGet-packages they are easy to install and use on multiple platforms.

Free to use, [commercial support and training](https://activelogin.net/#support) is available if you need assistance or a quick start. 

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

## Full documentation

For full documentation and samples, see the Readme in our [GitHub repo](https://github.com/ActiveLogin/ActiveLogin.Authentication).

## Active Login

Active Login is an Open Source project built on .NET that makes it easy to integrate with leading Swedish authentication services like [BankID](https://www.bankid.com/).

https://www.activelogin.net/

## Active Solution

Active Login is built, maintained and sponsored by Active Solution. Active Solution is located in Stockholm (Sweden) and provides IT consulting with focus on web, cloud and AI.

https://www.activesolution.se/
