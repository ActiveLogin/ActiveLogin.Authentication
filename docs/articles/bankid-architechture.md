# ActiveLogin.Authentication.BankId Architechture

This document is aimed towards you who want to understand how ActiveLogin.Authentication.BankId works internally. This might be relevant if you want to contribute or review our code.
The document isn't complete, but we try to write a note on the most important concepts.

## Sign

When we implemented BankId Auth we simply follow the concept and conventions Microsoft have when implementing a custom authentication provider. Scheme configuraiton, AuthenticationHandler, Challange, Claims etc etc.
Sign is not such a common concept the therefore the underlying infrastructure is not there. We've tried to use the same basic concepts, but simplified (as we only need to support our own scenario) and these are the components involved to make sign work.

- `ServiceCollectionBankIdSignExtensions`
  - `AddBankIdSign()`
    - Adds default implementations to IoC
    - Adds ASP.NET Core configuration
    - Runs the IBankIdSignBuilder pipeline
- `BankIdSignBuilder`
  - `AddConfig`
    - Similar to `AddScheme` with the difference that it takes the callback path directly
    - Will register the config in the `IBankIdSignConfigurationProvider`
- `IBankIdSignConfigurationProvider`
  - Keeps a list of the registered `BankIdSignConfiguration`

- 
- `BankIdSignMiddleware`
  - Plugs into the ASP.NET pipeline and detects if it should handle the request on sign callback
  - Calls the `IBankIdSignCallbackHandler` with the configkey and options for that config
- `BankIdSignCallbackHandler`
  - Handles the callback when sign is done
  - Get state from cookie, return to the url specified by consumer
- 
