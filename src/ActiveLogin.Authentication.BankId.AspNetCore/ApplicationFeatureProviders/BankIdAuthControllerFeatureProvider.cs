using System.Reflection;

using ActiveLogin.Authentication.BankId.AspNetCore.Areas.ActiveLogin.Controllers;

using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace ActiveLogin.Authentication.BankId.AspNetCore.ApplicationFeatureProviders;

internal class BankIdAuthControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
{
    public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
    {
        if (!feature.Controllers.Contains(typeof(BankIdAuthController).GetTypeInfo()))
        {
            feature.Controllers.Add(typeof(BankIdAuthController).GetTypeInfo());
        }
    }
}
