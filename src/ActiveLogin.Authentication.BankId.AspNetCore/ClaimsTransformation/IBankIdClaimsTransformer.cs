using System.Threading.Tasks;

namespace ActiveLogin.Authentication.BankId.AspNetCore.ClaimsTransformation;

public interface IBankIdClaimsTransformer
{
    public Task TransformClaims(BankIdClaimsTransformationContext context);
}
