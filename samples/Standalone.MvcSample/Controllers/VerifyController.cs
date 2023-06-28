using ActiveLogin.Authentication.BankId.Api;
using ActiveLogin.Authentication.BankId.Api.Models;

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

public class VerifyController : Controller
{
    private readonly IBankIdVerifyApiClient _bankIdVerifyApiClient;

    public VerifyController(IBankIdVerifyApiClient bankIdVerifyApiClient)
    {
        _bankIdVerifyApiClient = bankIdVerifyApiClient;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost("/verify/api")]
    public async Task<ActionResult<VerifyResultModel>> Verify([FromBody] VerifyRequestModel model)
    {
        ArgumentNullException.ThrowIfNull(model, nameof(model));
        if (string.IsNullOrEmpty(model.QrCodeContent))
        {
            throw new ArgumentNullException(nameof(model.QrCodeContent));
        }

        var verifyResult = await _bankIdVerifyApiClient.VerifyAsync(model.QrCodeContent);
        var user = verifyResult.User;
        var resultModel = new VerifyResultModel(user.PersonalIdentityNumber, user.GivenName, user.Surname, user.Name, user.Age, verifyResult.Authentication.GetIdentifiedAtDateTime(), verifyResult.Verification.GetVerifiedAtDateTime());

        return resultModel;
    }
}
