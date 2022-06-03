# ActiveLogin.Authentication.BankId Architechture

This document is aimed towards you who want to understand how ActiveLogin.Authentication.BankId works internally. This might be relevant if you want to contribute or review our code.

The document isn't complete, but we try to write a note on the most important concepts.

## Auth

The BanKID authentication implementation follows the standardized way of implementing an authentication provider in ASP.NET. As BankID requires us to display UI, we provide a built in one using the technique available throug Razor Class Libraries. Even though this UI is hosted as part of the application, from the authentication point of view, we treat it as a `RemoteAuthenticationProvider` as we need to redirect the user to such this UI and wait for the user to complete the auth flow. During the flow state is protected and stored in the quesy string and in a cookie.

### Auth Flow

- `AuthenticationBuilderBankIdAuthExtensions`
  - `AddBankIdAuth()`
    - Adds default implementations to IoC
    - Adds ASP.NET Core authentication configuration
    - Runs the `IBankIdAuthBuilder` pipeline
- `BankIdAuthBuilderExtensions`
  - `Add*`
    - Allows for adding schemes with relevant configuration for Same or Other device
- `BankIdAuthHandler`
  - Inherits the `RemoteAuthenticationHandler` and therefore implements the `IAuthenticationHandler`
- `BankIdUiAuthController`
  - Handles the UI for auth using MVC
- `BankIdUiAuthApiController`
  - Handles the backend calls from the UI for init/statusd/cancel etc.

  ```mermaid
  graph TD
    App-->A
    A[AccountController.Login] -->B

    B[AccountController.ExternalLogin] -->|Challange|C
    C[BankIdUiAuthController.Init] --> D
    D([BankIdUiAuth/Init.cshtml])
    E[BankIdUiAuthApiController.Initialize]
    F[BankIdUiAuthApiController.QrCode]-->D
    G[BankIdUiAuthApiController.Status]-->D
    D-->E
    D-->F
    D-->G
    D-->H
    H[AccountController.ExternalLoginCallback] -->|HttpContext.AuthenticateAsync|App
  ```


## Sign

When we implemented BankId Auth we simply follow the concept and conventions Microsoft have when implementing a custom authentication provider. Scheme configuration, AuthenticationHandler, Challange, Claims etc.

Sign is not such a common concept the therefore the underlying infrastructure is not there. We've tried to use the same basic concepts, but simplified (as we only need to support our own scenario).

### Sign Flow

- `ServiceCollectionBankIdSignExtensions`
  - `AddBankIdSign()`
    - Adds default implementations to IoC
    - Adds ASP.NET Core configuration
    - Runs the `IBankIdSignBuilder` pipeline
- `BankIdSignBuilder`
  - `AddConfig`
    - Similar to `AddScheme` with the difference that it does not register a callback URL, this is instead specified on the call to `InitiateSign` on `BankIdSignService`
    - Will register the config in the `IBankIdSignConfigurationProvider`
- `IBankIdSignConfigurationProvider`
  - Keeps a list of the registered `BankIdSignConfiguration`
- `BankIdSignService`
  - Allows for inittiating the Sign flow and redirecting the user to the correct page
  - Handles the callback when sign is done
  - Get state from cookie, return to the url specified by consumer
- `BankIdUiSignController`
  - Handles the UI for sign using MVC
- `BankIdUiSignApiController`
  - Handles the backend calls from the UI for init/statusd/cancel etc.

  ```mermaid
  graph TD
    App-->A
    A[SignController.Index] -->B

    B[SignController.Sign] -->|IBankIdSignService.InitiateSignAsync|C
    C[BankIdUiSignController.Init] --> D
    D([BankIdUiSign/Init.cshtml])
    E[BankIdUiSignApiController.Initialize]
    F[BankIdUiSignApiController.QrCode]-->D
    G[BankIdUiSignApiController.Status]-->D
    D-->E
    D-->F
    D-->G
    D-->H
    H[SignController.Callback] -->|IBankIdSignService.GetSignResultAsync|App
  ```
