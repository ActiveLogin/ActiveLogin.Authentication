# ActiveLogin.Authentication.BankId.AspNetCore.AzureMonitor

ActiveLogin.Authentication.BankId.AspNetCore.AzureMonitor provides `BankIdApplicationInsightsEventListener` that will listen for all events and write them to Application Insights. 

## Sample usage

Call `builder.AddApplicationInsightsEventListener()` to enable it. Note that you can supply options to enable logging of metadata, such as personal identity number, age and IP.

```csharp
services
    .AddAuthentication()
    .AddBankId(builder =>
    {
        builder
            //...
            .AddApplicationInsightsEventListener();
    });
```

## Full documentation

For full documentation and samples, see the Readme in our [GitHub repo](https://github.com/ActiveLogin/ActiveLogin.Authentication).

## Active Login

Active Login is an Open Source project built on .NET that makes it easy to integrate with leading Swedish authentication services like [BankID](https://www.bankid.com/).

https://www.activelogin.net/

## Active Solution

Active Login is built, maintained and sponsored by Active Solution. Active Solution is located in Stockholm (Sweden) and provides IT consulting with focus on web, cloud and AI.

https://www.activesolution.se/
