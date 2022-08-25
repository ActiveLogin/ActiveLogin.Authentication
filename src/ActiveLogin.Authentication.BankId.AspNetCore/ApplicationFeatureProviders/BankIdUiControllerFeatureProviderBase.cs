using System.Reflection;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace ActiveLogin.Authentication.BankId.AspNetCore.ApplicationFeatureProviders;

internal abstract class BankIdUiControllerFeatureProviderBase<TController> : IApplicationFeatureProvider<ControllerFeature> where TController : ControllerBase
{
    public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
    {
        if (!feature.Controllers.Contains(typeof(TController).GetTypeInfo()))
        {
            feature.Controllers.Add(typeof(TController).GetTypeInfo());
        }
    }
}
