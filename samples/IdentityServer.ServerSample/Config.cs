using System.Collections.Generic;
using ActiveLogin.Authentication.BankId.AspNetCore;
using IdentityServer4;
using IdentityServer4.Models;
using Microsoft.Extensions.Configuration;

namespace IdentityServer.ServerSample
{
    public static class Config
    {
        private const string PersonaIidentityNumberScopeName = "personalidentitynumber";

        public static IEnumerable<Client> GetClients(IConfiguration clientsConfiguration)
        {
            return new List<Client>
            {
                new Client
                {
                    ClientId = "mvc",
                    ClientName = "MVC Client",
                    AllowedGrantTypes = GrantTypes.Hybrid,
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    AlwaysIncludeUserClaimsInIdToken = true,

                    RedirectUris = { clientsConfiguration["MvcClient:RedirectUri"] },
                    PostLogoutRedirectUris = { clientsConfiguration["MvcClient:PostLogoutRedirectUri"] },

                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        PersonaIidentityNumberScopeName
                    },

                    RequireConsent = false          
                }
            };
        }

        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResource(PersonaIidentityNumberScopeName, new List<string>
                {
                    BankIdClaimTypes.SwedishPersonalIdentityNumber
                })
            };
        }
    }
}
