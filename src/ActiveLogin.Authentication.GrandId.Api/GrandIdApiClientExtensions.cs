﻿using ActiveLogin.Authentication.GrandId.Api.Models;
using System.Threading.Tasks;

namespace ActiveLogin.Authentication.GrandId.Api
{
    /// <summary>
    /// Extensions to enable easier access to common api scenarios.
    /// </summary>
    public static class GrandIdApiClientExtensions
    {
        public static Task<AuthResponse> FederatedLoginAsync(this IGrandIdApiClient apiClient, string authenticateServiceKey, string callbackUrl)
        {
            return apiClient.FederatedLoginAsync(new AuthRequest(authenticateServiceKey, callbackUrl));
        }

        public static Task<AuthResponse> FederatedLoginAsync(this IGrandIdApiClient apiClient, string authenticateServiceKey, string callbackUrl, string personalIdentityNumber)
        {
            return apiClient.FederatedLoginAsync(new AuthRequest(authenticateServiceKey, callbackUrl, personalIdentityNumber));
        }

        public static Task<SessionStateResponse> GetSessionAsync(this IGrandIdApiClient apiClient, string authenticateServiceKey, string sessionId)
        {
            return apiClient.GetSessionAsync(new SessionStateRequest(authenticateServiceKey, sessionId));
        }
    }
}
