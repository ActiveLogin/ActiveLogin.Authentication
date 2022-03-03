using System.Security.Claims;
using System.Threading.Tasks;

namespace ActiveLogin.Authentication.BankId.AspNetCore
{
    public interface IBankIdClaimsTransformer
    {
        public Task TransformClaims(BankIdClaimsTransformationContext context);
    }
}
