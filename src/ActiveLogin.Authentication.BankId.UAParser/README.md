# ActiveLogin.Authentication.BankId.UAParser

ActiveLogin.Authentication enables an application to support Swedish BankID (svenskt BankID) authentication in .NET.
Built on NET Standard and packaged as NuGet-packages they are easy to install and use on multiple platforms.

Free to use, [commercial support and training](https://activelogin.net/#support) is available if you need assistance or a quick start. 

## Sample usage

Sample usage of the extension method to use [ua_parser C# Library](https://github.com/ua-parser/uap-csharp) for device detection for BankID.

Adds the `UseUaParserDeviceDetection()` extension method.

```csharp
services
    .AddAuthentication()
    .AddBankId(bankId =>
    {
        bankId.UseUaParserDeviceDetection();
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
