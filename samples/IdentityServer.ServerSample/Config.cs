using System.Collections.Generic;
using ActiveLogin.Authentication.BankId.AspNetCore;
using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using Microsoft.Extensions.Configuration;

namespace IdentityServer.ServerSample
{
    public static class Config
    {
        private const string PersonalIdentityNumberScopeName = "personalidentitynumber";

        public static IEnumerable<Client> GetClients(IConfiguration clientsConfiguration)
        {
            return new List<Client>
            {
                new Client
                {
                    ClientName = "Active Login - IdentityServer - MvcClientSample",
                    ClientId = clientsConfiguration["MvcClient:ClientId"],
                    ClientSecrets =
                    {
                        new Secret(clientsConfiguration["MvcClient:ClientSecret"].Sha256())
                    },

                    AllowedGrantTypes = GrantTypes.Hybrid,

                    RedirectUris = { clientsConfiguration["MvcClient:RedirectUri"] },
                    PostLogoutRedirectUris = { clientsConfiguration["MvcClient:PostLogoutRedirectUri"] },

                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        PersonalIdentityNumberScopeName
                    },

                    RequireConsent = false,

                    // We can only obtain the info from BankID on sign in,
                    //  so as long as we don't persist it in a local database
                    //  we have to pass them on right away in the id token
                    AlwaysIncludeUserClaimsInIdToken = true,
                }
            };
        }

        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResource(PersonalIdentityNumberScopeName, new List<string>
                {
                    BankIdClaimTypes.SwedishPersonalIdentityNumber
                })
            };
        }
    }
}
