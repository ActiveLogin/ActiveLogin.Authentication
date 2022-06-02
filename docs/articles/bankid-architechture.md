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

  ### Flow

  This is the overall sign flow.

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
