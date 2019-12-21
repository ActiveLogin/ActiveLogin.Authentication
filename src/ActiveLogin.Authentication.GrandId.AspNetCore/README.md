# ActiveLogin.Authentication.GrandId.AspNetCore

ActiveLogin.Authentication enables an application to support Swedish BankID (svenskt BankID) authentication in .NET. Built on NET Standard and packaged as NuGet-packages they are easy to install and use on multiple platforms. Used with Identity Server it can be configured as a provider for Azure AD B2C. Free to use, [commercial support and traning](https://activelogin.net/#support) is available if you need assistance or a quick start. 

## Sample usage

Sample usage of the ASP.NET Core authentication module.

### Development

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

### Production

```csharp
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

## Full documentation

For full documentation and samples, see the Readme in our [GitHub repo](https://github.com/ActiveLogin/ActiveLogin.Authentication).

## Active Login

Active Login is an Open Source project built on .NET Core that makes it easy to integrate with leading Swedish authentication services like [BankID](https://www.bankid.com/).

https://www.activelogin.net/

## Active Solution

Active Login is built, maintained and sponsored by Active Solution. Active Solution is located in Stockholm (Sweden) and provides IT consulting with focus on web, cloud and AI.

https://www.activesolution.se/
