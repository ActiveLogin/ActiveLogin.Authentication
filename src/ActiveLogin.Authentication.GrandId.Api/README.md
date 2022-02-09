# ActiveLogin.Authentication.GrandId.Api

ActiveLogin.Authentication enables an application to support Swedish BankID (svenskt BankID) authentication in .NET.
Built on NET Standard and packaged as NuGet-packages they are easy to install and use on multiple platforms.

Free to use, [commercial support and training](https://activelogin.net/#support) is available if you need assistance or a quick start. 

## Sample usage

The constructor for the api client takes an `HttpClient` and you need to configure that `HttpClient` with a `BaseAddress` etc. depending on your needs.

The client have these public methods.

```csharp
public class GrandIdApiClient : IGrandIdApiClient
{
    public async Task<BankIdFederatedLoginResponse> BankIdFederatedLoginAsync(BankIdFederatedLoginRequest request) { ... }
    public async Task<BankIdGetSessionResponse> BankIdGetSessionAsync(BankIdGetSessionRequest request) { ... }
    public async Task<LogoutResponse> LogoutAsync(LogoutRequest request) { ... }
}
```

## Full documentation

For full documentation and samples, see the Readme in our [GitHub repo](https://github.com/ActiveLogin/ActiveLogin.Authentication).

## Active Login

Active Login is an Open Source project built on .NET that makes it easy to integrate with leading Swedish authentication services like [BankID](https://www.bankid.com/).

https://www.activelogin.net/

## Active Solution

Active Login is built, maintained and sponsored by Active Solution. Active Solution is located in Stockholm (Sweden) and provides IT consulting with focus on web, cloud and AI.

https://www.activesolution.se/
