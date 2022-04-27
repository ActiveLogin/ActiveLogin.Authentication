### Breaking changes in v6.0.0
This document will contain all the breaking changes introduced with v6

## Interfaces that are no longer public

## Refactorings
- supplied implementations of IBankIdSupportedDeviceDetector.Detect no longer takes a userAgent string but tries to resolve the agent using an injected IHttpContextAccessor
- BankIdDynamicEndUserIpResolver removed, please use BankIdRemoteIpAddressEndUserIpResolver instead
- the use of personalIdentityNumber in the auth flow is no longer supported.