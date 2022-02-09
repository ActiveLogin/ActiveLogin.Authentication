# ActiveLogin.Authentication.BankId.AspNetCore

ActiveLogin.Authentication enables an application to support Swedish BankID (svenskt BankID) authentication in .NET.
Built on NET Standard and packaged as NuGet-packages they are easy to install and use on multiple platforms.

Free to use, [commercial support and training](https://activelogin.net/#support) is available if you need assistance or a quick start. 

## Sample usage

Sample usage of the ASP.NET authentication module.

### Development

```csharp
services
    .AddAuthentication()
    .AddBankId(builder =>
    {
        builder
            .AddDebugEventListener();
            .UseSimulatedEnvironment()
            .AddSameDevice();
    });
```

### Production

```csharp
services
    .AddAuthentication()
    .AddBankId(builder =>
    {
        builder
            .AddApplicationInsightsEventListener(options =>
            {
                options.LogUserPersonalIdentityNumberHints = true;
                options.LogCertificateDates = true;
            })
            .UseProductionEnvironment()
            .UseClientCertificateFromAzureKeyVault(Configuration.GetSection("ActiveLogin:BankId:ClientCertificate"))
            .UseRootCaCertificate(Path.Combine(_environment.ContentRootPath, Configuration.GetValue<string>("ActiveLogin:BankId:CaCertificate:FilePath")))
            .AddSameDevice()
            .AddOtherDevice()
            .UseQrCoderQrCodeGenerator();
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
