using ActiveLogin.Authentication.BankId.AspNetCore.Payment;
using ActiveLogin.Authentication.BankId.Core.Payment;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Standalone.MvcSample.Models;

namespace Standalone.MvcSample.Controllers;

//
// DISCLAIMER
//
// These are samples on how to use Active Login in different situations
// and might not represent optimal way of setting up
// ASP.NET MVC or other components.
//
// Please see this as inspiration, not a complete template.
//

[AllowAnonymous]
public class PaymentController : Controller
{
    private readonly IBankIdPaymentConfigurationProvider _bankIdPaymentConfigurationProvider;
    private readonly IBankIdPaymentService _bankIdPaymentService;

    public PaymentController(IBankIdPaymentConfigurationProvider bankIdPaymentConfigurationProvider, IBankIdPaymentService bankIdPaymentService)
    {
        _bankIdPaymentConfigurationProvider = bankIdPaymentConfigurationProvider;
        _bankIdPaymentService = bankIdPaymentService;
    }

    public async Task<IActionResult> Index()
    {
        var configurations = await _bankIdPaymentConfigurationProvider.GetAllConfigurationsAsync();
        var providers = configurations
            .Where(x => x.DisplayName != null)
            .Select(x => new ExternalProvider(x.DisplayName ?? x.Key, x.Key));
        var viewModel = new BankIdViewModel(providers, $"{Url.Action(nameof(Index))}");

        return View(viewModel);
    }

    [AllowAnonymous]
    [HttpPost("Payment")]
    public IActionResult Payment([FromQuery] string provider, [FromForm] PaymentRequestModel model)
    {
        ArgumentNullException.ThrowIfNull(model, nameof(model));

        var recipientName = "Demo Merchant Name";
        var props = new BankIdPaymentProperties(TransactionType.card, recipientName) { };

        var returnPath = $"{Url.Action(nameof(Callback))}?provider={provider}";
        return this.BankIdInitiatePayment(props, returnPath, provider);
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Callback(string provider)
    {
        var result = await _bankIdPaymentService.GetPaymentResultAsync(provider);
        if (result?.Succeeded != true || result.BankIdCompletionData == null)
        {
            throw new Exception("Payment error");
        }

        return View("Result", new PaymentResultViewModel(
            result.BankIdCompletionData.User.PersonalIdentityNumber,
            result.BankIdCompletionData.User.Name,
            result.BankIdCompletionData.Device.IpAddress,
            result.Properties.Items["transactionType"] ?? string.Empty,
            result.Properties.Items["recipientName"] ?? string.Empty
            )
        );
    }
}
