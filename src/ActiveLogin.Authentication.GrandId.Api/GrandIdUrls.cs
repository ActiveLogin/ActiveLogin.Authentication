using System;

namespace ActiveLogin.Authentication.GrandId.Api;

/// <summary>
/// URLs for GrandID REST API.
/// </summary>
public static class GrandIdUrls
{
    /// <summary>
    /// Base url for production API.
    /// </summary>
    public static readonly Uri ProductionApiBaseUrl = new Uri("https://client.grandid.com/json1.1/");

    /// <summary>
    /// Base url for swedish production API.
    /// </summary>
    public static readonly Uri SwedishProductionApiBaseUrl = new Uri("https://client.e-identitet.se/json1.1/");

    /// <summary>
    /// Base url for test API.
    /// </summary>
    public static readonly Uri TestApiBaseUrl = new Uri("https://client.test.e-identitet.se/json1.1/");
}
