using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using ActiveLogin.Authentication.BankId.Api;
using ActiveLogin.Authentication.BankId.Api.Models;
using ActiveLogin.Authentication.BankId.AspNetCore.Test.Helpers;
using ActiveLogin.Authentication.BankId.Core;
using ActiveLogin.Authentication.BankId.Core.CertificatePolicies;
using ActiveLogin.Authentication.BankId.AspNetCore.DataProtection;
using ActiveLogin.Authentication.BankId.AspNetCore.Models;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Test;

using Cookie = (string Key, string Value);

public abstract class BankId_Ui_Tests_Base
{
    protected const string AuthStateKeyCookieName = "__ActiveLogin.BankIdAuthStateKey";
    protected const string SignStateKeyCookieName = "__ActiveLogin.BankIdUiSignStateKey";
    protected const string PaymentStateKeyCookieName = "__ActiveLogin.BankIdUiPaymentStateKey";


    protected class TestBankIdAppApi(
        IBankIdAppApiClient bankIdAppApiClient
    ) : IBankIdAppApiClient
    {
        public bool CancelAsyncIsCalled { get; private set; }

        public Task<AuthResponse> AuthAsync(AuthRequest request) => bankIdAppApiClient.AuthAsync(request);
        public Task<SignResponse> SignAsync(SignRequest request) => bankIdAppApiClient.SignAsync(request);
        public Task<PaymentResponse> PaymentAsync(PaymentRequest request) => bankIdAppApiClient.PaymentAsync(request);
        public Task<PhoneAuthResponse> PhoneAuthAsync(PhoneAuthRequest request) => bankIdAppApiClient.PhoneAuthAsync(request);
        public Task<PhoneSignResponse> PhoneSignAsync(PhoneSignRequest request) => bankIdAppApiClient.PhoneSignAsync(request);
        public Task<CollectResponse> CollectAsync(CollectRequest request) => bankIdAppApiClient.CollectAsync(request);
        public Task<CancelResponse> CancelAsync(CancelRequest request)
        {
            CancelAsyncIsCalled = true;
            return bankIdAppApiClient.CancelAsync(request);
        }
    }

    protected async Task<HttpResponseMessage> MakeRequestWithRequiredContext(string bankIdType, string path, TestServer server, HttpContent content, params Cookie[] cookies)
    {
        var client = server.CreateClient();

        // Arrange state cookie
        var stateRequest = server.CreateRequest("/ANYURL");
        var stateResponse = await stateRequest.GetAsync();
        var stateCookies = stateResponse.Headers.TryGetValues("set-cookie", out var stateCookieValues)
            ? stateCookieValues
            : [];

        // Arrange csrf info

        var loginRequestPath = $"/{BankIdConstants.Routes.ActiveLoginAreaName}/{BankIdConstants.Routes.BankIdPathName}/{bankIdType}?returnUrl=%2F&uiOptions=X&orderRef=Y";
        var loginRequest = CreateRequestWithCookies(server, loginRequestPath, cookies);
        var loginResponse = await loginRequest.GetAsync();
        var loginCookies = loginResponse.Headers.TryGetValues("set-cookie", out var cookieValues) ? cookieValues : new string[0];
        var loginContent = await loginResponse.Content.ReadAsStringAsync();
        var csrfToken = GetRequestVerificationToken(loginContent);

        // Arrange acting request
        var allCookies = new List<string>(stateCookies);
        allCookies.AddRange(loginCookies);

        // Add the original cookies that were passed to the method
        foreach (var (key, value) in cookies)
        {
            allCookies.Add($"{key}={value}");
        }

        allCookies = allCookies.Distinct().ToList();

        var request = content;
        request.Headers.Add("Cookie", allCookies);
        request.Headers.Add("RequestVerificationToken", csrfToken);

        return await client.PostAsync(path, request);
    }

    protected async Task<HttpResponseMessage> GetInitializeResponse(string bankIdType, TestServer server, object initializeRequestBody, params (string key, string value)[] cookies)
    {
        var initializeRequest = new JsonContent(initializeRequestBody);

        var path = string.Format("/{0}/{1}/{2}/{3}/{4}",
            BankIdConstants.Routes.ActiveLoginAreaName,
            BankIdConstants.Routes.BankIdPathName,
            bankIdType,
            BankIdConstants.Routes.BankIdApiControllerPath,
            BankIdConstants.Routes.BankIdApiInitializeActionName);
        return await MakeRequestWithRequiredContext(bankIdType, path, server, initializeRequest, cookies);
    }

    protected static RequestBuilder CreateRequestWithCookies(
        TestServer server,
        string path,
        params Cookie[] cookies
    )
    {
        var request = server.CreateRequest(path);
        if (cookies.Length > 0)
        {
            var cookieHeader = string.Join("; ", cookies.Select(c => $"{c.Key}={c.Value}"));
            request.AddHeader("Cookie", cookieHeader);
        }
        return request;
    }

    protected static string GetRequestVerificationToken(string html)
    {
        return GetInlineJsonValue(html, "antiXsrfRequestToken");
    }

    protected static string GetInlineJsonValue(string html, string key)
    {
        var match = Regex.Match(html, @"""" + key + @"""[:]\s""(.*)""");
        return match.Success
            ? match.Groups[1].Value
            : string.Empty;
    }

    /// <summary>
    /// Creates both cookies needed for the cookie-based state storage:
    /// 1. The state key cookie (e.g., PaymentStateKeyCookieName) containing the StateKey
    /// 2. The actual state cookie (named with the StateKey) containing the serialized state
    /// </summary>
    protected static Cookie[] CreateStateCookies<T>(
        string stateKeyCookieName,
        StateKey stateKey,
        T state,
        IServiceProvider serviceProvider
    ) where T : Models.BankIdUiState
    {
        // Create the state protector to serialize the state data
        var stateProtector = serviceProvider.GetRequiredService<IBankIdDataStateProtector<T>>();
        var serializedState = stateProtector.Protect(state);

        return
        [
            (stateKeyCookieName, stateKey.Key),                                 // Cookie 1: state key cookie
            (CookieStateStorage.StateCookieName(stateKey), serializedState)    // Cookie 2: state data cookie

        ];
    }

    /// <summary>
    /// Creates a BankID UI request with proper state and UI options setup.
    /// This helper reduces boilerplate by handling the common pattern of:
    /// 1. Creating UI options
    /// 2. Creating both required state cookies (state key + state data)
    /// 3. Protecting UI options
    /// 4. Building the URL with proper parameters
    /// </summary>
    protected static async Task<HttpResponseMessage> CreateBankIdUiRequest<T>(
        TestServer server,
        string path,
        string stateKeyCookieName,
        T state,
        BankIdUiOptions uiOptions = null,
        string returnUrl = "/",
        string orderRef = "Y"
    ) where T : Models.BankIdUiState
    {
        uiOptions ??= new BankIdUiOptions(
            [],
            true, false, false, false,
            returnUrl,
            stateKeyCookieName,
            CardReader.class1
        );

        var uiOptionsProtector = server.Services.GetRequiredService<IBankIdDataStateProtector<BankIdUiOptions>>();
        var protectedUiOptions = uiOptionsProtector.Protect(uiOptions);

        var encodedReturnUrl = Uri.EscapeDataString(returnUrl);
        var encodedUiOptions = Uri.EscapeDataString(protectedUiOptions);
        var fullUrl = $"{path}?returnUrl={encodedReturnUrl}&uiOptions={encodedUiOptions}&orderRef={orderRef}";

        var stateKey = StateKey.New();
        var stateCookies = CreateStateCookies(stateKeyCookieName, stateKey, state, server.Services);
        var request = CreateRequestWithCookies(server, fullUrl, stateCookies);
        return await request.GetAsync();
    }

    /// <summary>
    /// Simplified helper for creating BankID UI options with commonly used parameters
    /// </summary>
    protected static BankIdUiOptions CreateUiOptions(
        string stateKeyCookieName,
        string cancelReturnUrl = "/",
        bool sameDevice = true,
        bool requirePinCode = false,
        bool requireMrtd = false,
        bool returnRisk = false,
        CardReader cardReader = CardReader.class1,
        List<BankIdCertificatePolicy> certificatePolicies = null
    )
    {
        return new BankIdUiOptions(
            certificatePolicies ?? [],
            sameDevice,
            requirePinCode,
            requireMrtd,
            returnRisk,
            cancelReturnUrl,
            stateKeyCookieName,
            cardReader
        );
    }
}
