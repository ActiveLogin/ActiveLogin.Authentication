﻿using System.Threading.Tasks;
using ActiveLogin.Authentication.GrandId.Api.Models;

namespace ActiveLogin.Authentication.GrandId.Api
{
    /// <summary>
    /// Extensions to enable easier access to common api scenarios.
    /// </summary>
    public static class GrandIdApiClientExtensions
    {
        public static Task<AuthResponse> AuthAsync(this IGrandIdApiClient apiClient, string apiKey, string authenticateServiceKey, string callbackUrl)
        {
            return apiClient.AuthAsync(new AuthRequest(apiKey, authenticateServiceKey, callbackUrl));
        }

        public static Task<SessionStateResponse> GetSessionAsync(this IGrandIdApiClient apiClient, string apiKey, string authenticateServiceKey, string sessionId)
        {
            return apiClient.GetSessionAsync(new SessionStateRequest(apiKey, authenticateServiceKey, sessionId));
        }
    }
}
