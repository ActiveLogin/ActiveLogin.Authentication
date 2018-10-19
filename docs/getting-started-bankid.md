# Getting started with GrandID

## 1. Get started in development

BankID requires you to use a client certificate and trust a specific root CA-certificate. To try out the module without those certificates, you can use an in memory development implementation of the BankID REST API.

The authentication part in your `Startup.cs` should look like this:

```c#
services.AddAuthentication()
        .AddBankId(builder =>
    {
        builder
            .UseDevelopmentEnvironment()
            .AddSameDevice(options => { })
            .AddOtherDevice(options => { });
    });
```

##### 2.1.2 Try it out with Bank ID test environment

To start using a real implementation of BankID, there are a few steps to do. These steps describes the scenario where you utilize Azure for things like secure storage of the certificate in an Azure KeyVault.

1. Start by reading through the _BankID Relying Party Guidelines_ [available for download here](https://www.bankid.com/bankid-i-dina-tjanster/rp-info). This ensures you have a basic understanding of the terminology as well as how the flow and security works.
2. Deploy Azure KeyVault to yur subscription, the ARM-template available in [AzureProvisioningSample](https://github.com/ActiveLogin/ActiveLogin.Authentication/tree/master/samples/AzureProvisioningSample) contains a KeyVault, so preferably you deploy that.

[![Deploy to Azure](https://azuredeploy.net/deploybutton.png)](https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fgithub.com%2FActiveLogin%2FActiveLogin.Authentication%2Ftree%2Fmaster%2Fsamples%2FAzureProvisioningSample%2FActiveLogin.json)

3. Download the _SSL certificate for test (FPTestcert2.pfx)_ [available for download here](https://www.bankid.com/bankid-i-dina-tjanster/rp-info).
4. [Import the certificate to your Azure Key Vault.](https://docs.microsoft.com/en-us/azure/key-vault/certificate-scenarios#import-a-certificate)
5. Add the following to you `Startup.cs`:

```c#
services.AddAuthentication()
        .AddBankId(builder =>
    {
        builder
            .UseTestEnvironment()
            .UseClientCertificateFromAzureKeyVault(Configuration.GetSection("ActiveLogin:BankId:ClientCertificate"))
            .AddSameDevice(options => { })
            .AddOtherDevice(options => { });
    });
```

6. Add the following configuration values. The AD client should have access to the KeyVault certificate specified in `AzureKeyVaultSecretIdentifier`.

```json
{
  "ActiveLogin:BankId:ClientCertificate:AzureAdClientId": "TODO-ADD-YOUR-VALUE",
  "ActiveLogin:BankId:ClientCertificate:AzureAdClientSecret": "TODO-ADD-YOUR-VALUE",
  "ActiveLogin:BankId:ClientCertificate:AzureKeyVaultSecretIdentifier": "TODO-ADD-YOUR-VALUE"
}
```

7. The root CA-certificate specified in _BankID Relying Party Guidelines_ needs to be trusted at the computer where the app will run.
8. If running in Azure App Service, where trusting custom certificates is not supported, there are extensions to handle that scenario.
9. Instead of trusting the certificate, place it in your web project and make sure `CopyToOutputDirectory` is set to `Always`:

```xml
<Content Include="Certificates\BankIdRootCertificate-Test.crt">	
  <CopyToOutputDirectory>Always</CopyToOutputDirectory>	
</Content>
<Content Include="Certificates\BankIdRootCertificate-Prod.crt">	
  <CopyToOutputDirectory>Always</CopyToOutputDirectory>	
</Content>
```

10. Right after `.UseClientCertificateFromAzureKeyVault(..)`, add the following line:

```c#
.UseRootCaCertificate(Path.Combine(_environment.ContentRootPath, Configuration.GetValue<string>("ActiveLogin:BankId:CaCertificate:FilePath")))
```

 11. Add the following configuration values. The `FilePath` should point to the certificate you just added, for example:

```json
{
  "ActiveLogin:BankId:CaCertificate:FilePath": "Certificates\\BankIdRootCertificate-Test.crt"
}
```

12. You should now be ready to start!

##### 2.1.3 Use production environment

To use BankID production environment, the procedure is the same as for test, but the API url and the certificates are different.

1. Start by [contacting a reseller](https://www.bankid.com/kontakt/foeretag/saeljare) to get your very own client certificate. This will probably take a few business days to get sorted.
2. Repeat the process above by uploading the certificate to KeyVault and reconfigure your app to use the production certificate.
3. Change your application to trust the root CA-certificate. for production instead of test, this certificate can also be found in the file _BankID Relying Party Guidelines_ [available for download here](https://www.bankid.com/bankid-i-dina-tjanster/rp-info).
4. The final Startup.cs should look something like this:

```c#
services.AddAuthentication()
        .AddBankId(builder =>
    {
        builder
            .UseProductionEnvironment()
            .UseClientCertificateFromAzureKeyVault(Configuration.GetSection("ActiveLogin:BankId:ClientCertificate"))
            .UseRootCaCertificate(Path.Combine(_environment.ContentRootPath, Configuration.GetValue<string>("ActiveLogin:BankId:CaCertificate:FilePath")))
            .AddSameDevice(options => { })
            .AddOtherDevice(options => { });
    });
```

5. BankId options allows you to set and override some options such as these:

```c#
.AddOtherDevice(options =>
{
    options.BankIdAllowBiometric = false;
    options.BankIdCertificatePolicies = new List<string> { "1.2.752.78.1.1" };
});
```

6. Enjoy BankID in your application :)

## FAQ

### Can the UI be customized?

Yes! The UI is bundled into the package as a Razor Class Library, a technique that allows to [override the parts you want to customize](https://docs.microsoft.com/en-us/aspnet/core/razor-pages/ui-class?view=aspnetcore-2.1&tabs=visual-studio#override-views-partial-views-and-pages). The Views and Controllers that can be customized can be found in the [GitHub repo](https://github.com/ActiveLogin/ActiveLogin.Authentication/tree/master/src/ActiveLogin.Authentication.BankId.AspNetCore/Areas/BankIdAuthentication). 

### Can the messages be localized?

The messages are already localized to English and Swedish using the official recommended texts. To select what texts that are used you can for example use the [localization middleware in ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/localization?view=aspnetcore-2.1#localization-middleware).