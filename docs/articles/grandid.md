# ActiveLogin.Authentication.GrandId

___DISCLAIMER:___ The packages in ActiveLogin for Svensk E-identitet / GrandID are not beeing worked on anymore, and are therefore not supported. See this [announcement](https://github.com/ActiveLogin/ActiveLogin.Authentication/discussions/323) for more info. The documentation below is provided as is and reflects the state for the 4.0.0 release of the GrandId packages.

## Table of contents

* [NuGet Packages](#nuget-packages)
  + [ActiveLogin.Authentication.GrandId.*](#activeloginauthenticationgrandid)
* [Getting started](#getting-started)
  + [1. Preparation](#1-preparation)
  + [2. Install the NuGet package](#2-install-the-nuget-package)
  + [3. Prepare your project](#3-prepare-your-project)
  + [4. Get started in development](#4-get-started-in-development)
  + [5. Use test or production environments](#5-use-test-or-production-environments)
* [Samples](#samples)
  + [Simulated environment](#simulated-environment)
  + [Simulated environment with no config](#simulated-environment-with-no-config)
  + [Simulated environment with custom person info](#simulated-environment-with-custom-person-info)
  + [Production environment](#production-environment)
  + [Test environment](#test-environment)
  + [Using schemes for same device and other device](#using-schemes-for-same-device-and-other-device)
  + [Using schemas for choose device](#using-schemas-for-choose-device)
  + [Customizing schemas](#customizing-schemas)
  + [Customizing GrandID](#customizing-grandid)
  + [Api only](#api-only)

## NuGet Packages

### ActiveLogin.Authentication.GrandId.*

Packages for GrandID (Svensk E-identitet).

| Project | Description | NuGet | Downloads |
| ------- | ----------- | ----- | --------- |
| [GrandId.Api](https://github.com/ActiveLogin/ActiveLogin.Authentication/tree/main/src/ActiveLogin.Authentication.GrandId.Api) | API client for the GrandID (Svensk E-identitet) REST API. | [![NuGet](https://img.shields.io/nuget/vpre/ActiveLogin.Authentication.GrandId.Api.svg)](https://www.nuget.org/packages/ActiveLogin.Authentication.GrandId.Api/) | [![NuGet (Pre)](https://img.shields.io/nuget/dt/ActiveLogin.Authentication.GrandId.Api.svg)](https://www.nuget.org/packages/ActiveLogin.Authentication.GrandId.Api/) |
| [GrandId.AspNetCore](https://github.com/ActiveLogin/ActiveLogin.Authentication/tree/main/src/ActiveLogin.Authentication.GrandId.AspNetCore) | ASP.NET authentication module for GrandID (Svensk E-identitet). | [![NuGet](https://img.shields.io/nuget/vpre/ActiveLogin.Authentication.GrandId.AspNetCore.svg)](https://www.nuget.org/packages/ActiveLogin.Authentication.GrandId.AspNetCore/) | [![NuGet (Pre)](https://img.shields.io/nuget/dt/ActiveLogin.Authentication.GrandId.AspNetCore.svg)](https://www.nuget.org/packages/ActiveLogin.Authentication.GrandId.AspNetCore/) |

## Getting started

GrandID (Svensk E-identitet) uses a predefined UI and does not support all functionalities of the BankID API, but is really easy to get started with and does not require any certificates.

### 1. Preparation

Active Login is designed to make it very easy to get started with GrandID (Svensk E-identitet), but in the end you are resonsible for making sure that you are complient with the technical guidelines and/or legal agreements.

Therefore, before you start using Active Login, please read the documentation relevant to your needs. This will also make sure you understand the concepts better.

1. Read through the [GrandID documentation](https://docs.grandid.com/). This ensures you have a basic understanding of the terminology as well as how the flow and security works.
1. [Get in touch with Svensk E-identitet](https://e-identitet.se/tjanster/inloggningsmetoder/bankid/) to receive keys, you need these:
    * `ApiKey`
    * `BankIdServiceKey` (BankID) *Note:* ActiveLogin is built for the (at the time of writing this) latest version of GrandID where only one key is used. Please get in touch with Svensk E-identitet if you need to upgrade to this new version.
1. Add them to your config, for example:

```json
{
  "ActiveLogin:GrandId:ApiKey": "TODO-ADD-YOUR-VALUE",
  "ActiveLogin:GrandId:BankIdServiceKey": "TODO-ADD-YOUR-VALUE"
}
```

### 2. Install the NuGet package

ActiveLogin.Authentication is distributed as [packages on NuGet](https://www.nuget.org/profiles/ActiveLogin), install using the tool of your choice, for example _dotnet cli_.

```console
dotnet add package ActiveLogin.Authentication.GrandId.AspNetCore
```

### 3. Prepare your project

It is expected that you have a basic understanding of how [ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/), [ASP.NET Core MVC](https://docs.microsoft.com/en-us/aspnet/core/mvc/overview) and [ASP.NET Core Authentication](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/identity) works before getting started.

Also, you are expected to have read up on the [latest information from BankID](https://www.bankid.com/utvecklare/guider). Active Login will help you to implement BankID according to guidelines, but in the end, it's your responsiblity to follow the BankID agreement.

The authentication modules for GrandID are registered in your `Program.cs`. Depending on your setup, you will probably have to configure challenge and callbacks in `AccountController.cs` or similar.

### 4. Get started in development

GrandID requires you to receive API-keys, but to get started and try it out the experience there is a simulated environment options available that uses an in-memory implementation. Great for development and testing.

```csharp
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

___Note:___ Regarding ASP.NET Routes. Also make sure that you map the controller route in ASP.NET Endpoint routing, like this:

```csharp
app.MapDefaultControllerRoute();
```

### 5. Use test or production environments

To authenticate using a real BankID you need to use the API-keys:

```csharp
services
    .AddAuthentication()
    .AddGrandId(builder =>
    {
        builder
            .UseProductionEnvironment(config => {
                config.ApiKey = configuration.GetValue<string>("ActiveLogin:GrandId:ApiKey");
                config.BankIdServiceKey = configuration.GetValue<string>("ActiveLogin:GrandId:BankIdServiceKey");
            })
            .AddBankIdSameDevice()
            .AddBankIdOtherDevice();
    });
```

## Samples

### Simulated environment

For trying out quickly (without the need of keys) you can use an in-memory implementation of the API by using `.UseSimulatedEnvironment()`. This could also be good when writing tests.

### Simulated environment with no config

```csharp
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

### Simulated environment with custom person info

The faked name and personal identity number can also be customized like this.

```csharp
services
    .AddAuthentication()
    .AddGrandId(builder =>
    {
        builder
            .UseSimulatedEnvironment("Alice", "Smith", "199908072391")
            .AddBankIdSameDevice(options => { })
            .AddBankIdOtherDevice(options => { });
    });
```

### Production environment

This will use the real REST API for GrandID, connecting to either the Test or Production environment. It requires you to have the API keys described under _Preparation_ above.

```csharp
services.AddAuthentication()
        .AddGrandId(builder =>
    {
        builder
            .UseProductionEnvironment(config => {
                config.ApiKey = configuration.GetValue<string>("ActiveLogin:GrandId:ApiKey");
            })
            ...
    });
```

### Test environment

These samples uses the production environment, to use the test environment, simply swap `.UseProductionEnvironment()` with `.UseTestEnvironment()`.

```csharp
services.AddAuthentication()
        .AddGrandId(builder =>
    {
        builder
            .UseTestEnvironment(config => {
                config.ApiKey = configuration.GetValue<string>("ActiveLogin:GrandId:ApiKey");
		...
            })
            ...
    });
```

### Using schemes for same device and other device

* *Same device*: Launches the BankID app on the same device, no need to enter any personal identity number.
* *Other device*: You enter your personal identity number and can manually launch the app on your smartphone.

```csharp
services
    .AddAuthentication()
    .AddGrandId(builder =>
    {
        builder
            .UseProductionEnvironment(config => {
                config.ApiKey = configuration.GetValue<string>("ActiveLogin:GrandId:ApiKey");
                config.BankIdServiceKey = configuration.GetValue<string>("ActiveLogin:GrandId:BankIdServiceKey");
	    })
            .AddBankIdSameDevice()
            .AddBankIdOtherDevice();
    });
```

### Using schemas for choose device

This option will display a UI at GrandID where the user can choose between same or other device.

```csharp
services
    .AddAuthentication()
    .AddGrandId(builder =>
    {
        builder
            .UseProductionEnvironment(config => {
                config.ApiKey = configuration.GetValue<string>("ActiveLogin:GrandId:ApiKey");
                config.BankIdServiceKey = configuration.GetValue<string>("ActiveLogin:GrandId:BankIdServiceKey");
	    })
            .AddBankIdChooseDevice();
    });
```

### Customizing schemas

By default, `Add*Device` will use predefined schemas and display names, but they can be changed.

```csharp
services
    .AddAuthentication()
    .AddGrandId(builder =>
    {
        builder
            .UseProductionEnvironment(config => {
                config.ApiKey = configuration.GetValue<string>("ActiveLogin:GrandId:ApiKey");
                config.BankIdServiceKey = configuration.GetValue<string>("ActiveLogin:GrandId:BankIdServiceKey");
	    })
            .AddBankIdSameDevice("custom-auth-scheme", "Custom display name", options => { ... })
            .AddBankIdOtherDevice(GrandIdDefaults.BankIdOtherDeviceAuthenticationScheme, "Custom display name", options => { ... });
    });
```

### Customizing GrandID

GrandId options allows you to set and override some options such as these.

```csharp
.AddBankIdOtherDevice(options =>
{
	// Issue birthdate claim based on data extracted from the personal identity number
	options.IssueBirthdateClaim = true;

	// Issue gender claim based on data extracted from the personal identity number
	options.IssueGenderClaim = true;
});
```

If you want to apply some options for all BankID schemes, you can do so by using `.ConfigureBankId(...)`.

```csharp
.ConfigureBankId(options =>
{
    options.IssueBirthdateClaim = true;
    options.IssueGenderClaim = true;
});
```

### Api only

We have seperated the API-wrapper into a separate package so that you can use it in other scenarios we have not covered. They look like this and are both well documented using XML-comments.

The constructor for the ApiClient takes an `HttpClient` and you need to configure that `HttpClient` with a `BaseAddress`, `Tls12`, client certificates etc. depending on your needs.

```csharp
public class GrandIdApiClient : IGrandIdApiClient
{
    public async Task<BankIdFederatedLoginResponse> BankIdFederatedLoginAsync(BankIdFederatedLoginRequest request) { ... }
    public async Task<BankIdGetSessionResponse> BankIdGetSessionAsync(BankIdGetSessionRequest request) { ... }
    public async Task<LogoutResponse> LogoutAsync(LogoutRequest request) { ... }
}
```
