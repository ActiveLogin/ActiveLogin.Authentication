# Getting started with GrandID

## Preparation

1. Read through the [GrandID documentation](https://docs.grandid.com/). This ensures you have a basic understanding of the terminology as well as how the flow and security works.
1. [Get in touch with Svensk E-identitet](https://e-identitet.se/tjanster/inloggningsmetoder/bankid/) to receive keys, you need these:
    * `ApiKey`
    * `BankIdServiceKey` (BankID) *Note:* ActiveLogin is built for the latest version of GrandID where only one key is used. Please get in touch with Svensk E-identitet if you need to upgrade to this new version.
1. Add them to your config, for example:

```json
{
  "ActiveLogin:GrandId:ApiKey": "TODO-ADD-YOUR-VALUE",
  "ActiveLogin:GrandId:BankIdServiceKey": "TODO-ADD-YOUR-VALUE"
}
```

## Environments

### Development environment

For trying out quickly (without the need of keys) you can use an in-memory implementation of the API by using `.UseDevelopmentEnvironment()`. This could also bee good when writing tests.

### Development environment with no config

```c#
services
    .AddAuthentication()
    .AddGrandId(builder =>
    {
        builder
            .UseDevelopmentEnvironment()
            .AddBankIdSameDevice(options => { })
            .AddBankIdOtherDevice(options => { });
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
            .AddBankIdSameDevice(options => { })
            .AddBankIdOtherDevice(options => { });
    });
```

### Production environment

This will use the real REST API for GrandID, connecting to either the Test or Production environment. It requires you to have the API keys described under _Preparation_ above.

```c#
services.AddAuthentication()
        .AddGrandId(builder =>
    {
        builder
            .UseProductionEnvironment(config => {
                config.ApiKey = Configuration.GetValue<string>("ActiveLogin:GrandId:ApiKey");
                ...
			})
            ...
    });
```

### Test environment

These samples uses the production environment, to use the test environment, simply swap `.UseProductionEnvironment()` with `.UseTestEnvironment()`.

```c#
services.AddAuthentication()
        .AddGrandId(builder =>
    {
        builder
            .UseTestEnvironment(config => {
			    config.ApiKey = Configuration.GetValue<string>("ActiveLogin:GrandId:ApiKey");
                ...
			})
            ...
    });
```

## Samples

### Using schemes for same device and other device

* *Same device*: Launches the BankID app on the same device, no need to enter any personal identity number.
* *Other device*: You enter your personal identity number and can manually launch the app on your smartphone.

```c#
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

### Using schemas for choose device

This option will display a UI at GrandID where the user can choose between same or other device.

```c#
services
    .AddAuthentication()
    .AddGrandId(builder =>
    {
        builder
            .UseProductionEnvironment(config => {
				config.ApiKey = Configuration.GetValue<string>("ActiveLogin:GrandId:ApiKey");
				config.BankIdServiceKey = Configuration.GetValue<string>("ActiveLogin:GrandId:BankIdServiceKey");
			})
            .AddBankIdChooseDevice();
    });
```

### Customizing schemas

By default, `Add*Device` will use predefined schemas and display names, but they can be changed.

```c#
services
    .AddAuthentication()
    .AddGrandId(builder =>
    {
        builder
            .UseProductionEnvironment(config => {
				config.ApiKey = Configuration.GetValue<string>("ActiveLogin:GrandId:ApiKey");
				config.BankIdServiceKey = Configuration.GetValue<string>("ActiveLogin:GrandId:BankIdServiceKey");
			})
            .AddBankIdSameDevice("custom-auth-scheme", "Custom display name", options => { ... })
            .AddBankIdOtherDevice(GrandIdAuthenticationDefaults.BankIdOtherDeviceAuthenticationScheme, "Custom display name", options => { ... });
    });
```

### Customizing GrandID

GrandId options allows you to set and override some options such as these.

```c#
.AddBankIdOtherDevice(options =>
{
	// Issue birthdate claim based on data extracted from the personal identity number
	options.IssueBirthdateClaim = true;

	// Issue gender claim based on data extracted from the personal identity number
    options.IssueGenderClaim = true;
});
```

If you want to apply some options for all BankID schemes, you can do so by using `.ConfigureBankId(...)`.

```c#
.ConfigureBankId(options =>
{
    options.IssueBirthdateClaim = true;
    options.IssueGenderClaim = true;
});
```
