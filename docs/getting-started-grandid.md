# Getting started with GrandID

## Preparation

1. [Get in touch with Svensk E-identitet](https://e-identitet.se/tjanster/inloggningsmetoder/bankid/) to receive keys, you need these:
    * `ApiKey`
    * `SameDeviceServiceKey` (BankID same device)
    * `OtherDeviceServiceKey` (BankID other device)
    * `ChooseDeviceServiceKey` (BankID with device choice)
2. Add them to your config, for example:

```json
{
  "ActiveLogin:GrandId:ApiKey": "TODO-ADD-YOUR-VALUE",
  "ActiveLogin:GrandId:SameDeviceServiceKey": "TODO-ADD-YOUR-VALUE",
  "ActiveLogin:GrandId:OtherDeviceServiceKey": "TODO-ADD-YOUR-VALUE",
  "ActiveLogin:GrandId:ChooseDeviceServiceKey": "TODO-ADD-YOUR-VALUE"
}
```

## Samples

### Development environment

For trying out quickly (without the need of keys) you can use an in-memory implementation of the API by using `.UseDevelopmentEnvironment()`. This could also bee good when writing tests.

### Development environment predefined set of schemas

This is the simplest setup that will use the development environment and add the `SameDevice` and `OtherDevice` schemas.

```c#
services
    .AddAuthentication()
    .AddGrandId();
```

### Development environment with no config

```c#
services
    .AddAuthentication()
    .AddGrandId(builder =>
    {
        builder
            .UseDevelopmentEnvironment()
            .AddSameDevice(options => { })
            .AddOtherDevice(options => { });
    });
```

### Development environment with custom person info

The faked name and personal identity number can also be customized like this.

```c#
services
    .AddAuthentication()
    .AddGrandId(builder =>
    {
        builder
            .UseDevelopmentEnvironment("Alice", "Smith", "199908072391")
            .AddSameDevice(options => { })
            .AddOtherDevice(options => { });
    });
```

## Test or production environment

This will use the real REST API for GrandID, connecting to either the Test or Production environment. It requires you to have the API keys described under _Preparation_ above.

These samples uses the production environment, to use the test environment, simply swap `.UseProductionEnvironment()` with `.UseTestEnvironment()`.

### Using schemes for same device and other device

* *Same device*: Launches the BankID app on the same device, no need to enter any personal identity number.
* *Other device*: You enter your personal identity number and can manually launch the app on your smartphone.

```c#
services
    .AddAuthentication()
    .AddGrandId(builder =>
    {
        builder
            .UseProductionEnvironment(Configuration.GetValue<string>("ActiveLogin:GrandId:ApiKey"))
            .AddSameDevice(options =>
            {
                options.AuthenticateServiceKey = Configuration.GetValue<string>("ActiveLogin:GrandId:SameDeviceServiceKey");
            })
            .AddOtherDevice(options =>
            {
                options.AuthenticateServiceKey = Configuration.GetValue<string>("ActiveLogin:GrandId:OtherDeviceServiceKey");
            });
    });
```

### Using schemas for choose device

This option will display a UI at GranID where the user can choose between same or other device.

```c#
services
    .AddAuthentication()
    .AddGrandId(builder =>
    {
        builder
            .UseProductionEnvironment(Configuration.GetValue<string>("ActiveLogin:GrandId:ApiKey"))
            .AddChooseDevice(options =>
            {
                options.AuthenticateServiceKey = Configuration.GetValue<string>("ActiveLogin:GrandId:ChooseDeviceServiceKey");
            });
    });
```