using System.Reflection;

using ActiveLogin.Authentication.BankId.AspNetCore.Areas.BankIdAuthentication.Controllers;

using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace ActiveLogin.Authentication.BankId.AspNetCore.ApplicationFeatureProviders;

internal class BankIdControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
{
    public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
    {
        if (!feature.Controllers.Contains(typeof(BankIdController).GetTypeInfo()))
        {
            feature.Controllers.Add(typeof(BankIdController).GetTypeInfo());
        }
    }
}
