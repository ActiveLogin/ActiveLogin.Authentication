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

using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Test;

public abstract class BankId_Ui_Tests_Base
{
    protected const string AuthStateKeyCookieName = "__ActiveLogin.BankIdAuthStateKey";
    protected const string SignStateKeyCookieName = "__ActiveLogin.BankIdUiSignStateKey";
    protected const string PaymentStateKeyCookieName = "__ActiveLogin.BankIdUiPaymentStateKey";


    protected class TestBankIdAppApi : IBankIdAppApiClient
    {
        private readonly IBankIdAppApiClient _bankIdAppApiClient;

        public bool CancelAsyncIsCalled { get; private set; }

        public TestBankIdAppApi(IBankIdAppApiClient bankIdAppApiClient)
        {
            _bankIdAppApiClient = bankIdAppApiClient;
        }

        public Task<AuthResponse> AuthAsync(AuthRequest request)
        {
            return _bankIdAppApiClient.AuthAsync(request);
        }

        public Task<SignResponse> SignAsync(SignRequest request)
        {
            return _bankIdAppApiClient.SignAsync(request);
        }

        public Task<PaymentResponse> PaymentAsync(PaymentRequest request)
        {
            return _bankIdAppApiClient.PaymentAsync(request);
        }

        public Task<PhoneAuthResponse> PhoneAuthAsync(PhoneAuthRequest request)
        {
            return _bankIdAppApiClient.PhoneAuthAsync(request);
        }

        public Task<PhoneSignResponse> PhoneSignAsync(PhoneSignRequest request)
        {
            return _bankIdAppApiClient.PhoneSignAsync(request);
        }

        public Task<CollectResponse> CollectAsync(CollectRequest request)
        {
            return _bankIdAppApiClient.CollectAsync(request);
        }

        public Task<CancelResponse> CancelAsync(CancelRequest request)
        {
            CancelAsyncIsCalled = true;
            return _bankIdAppApiClient.CancelAsync(request);
        }
    }

    protected async Task<HttpResponseMessage> MakeRequestWithRequiredContext(string bankIdType, string path, TestServer server, HttpContent content, params (string key, string value)[] cookies)
    {
        var client = server.CreateClient();

        // Arrange state cookie
        var stateRequest = server.CreateRequest("/ANYURL");
        var stateResponse = await stateRequest.GetAsync();
        var stateCookies = stateResponse.Headers.GetValues("set-cookie");

        // Arrange csrf info

        var loginRequestPath = $"/{BankIdConstants.Routes.ActiveLoginAreaName}/{BankIdConstants.Routes.BankIdPathName}/{bankIdType}?returnUrl=%2F&uiOptions=X&orderRef=Y";
        var loginRequest = CreateRequestWithCookies(server, loginRequestPath, cookies);
        var loginResponse = await loginRequest.GetAsync();
        var loginCookies = loginResponse.Headers.GetValues("set-cookie");
        var loginContent = await loginResponse.Content.ReadAsStringAsync();
        var csrfToken = GetRequestVerificationToken(loginContent);

        // Arrange acting request
        var allCookies = new List<string>(stateCookies);
        allCookies.AddRange(loginCookies);
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

    protected static RequestBuilder CreateRequestWithCookies(TestServer server, string path, params (string key, string value)[] cookies)
    {
        var request = server.CreateRequest(path);
        foreach (var (key, value) in cookies)
        {
            request.AddHeader("Cookie", $"{key}={value}");
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
        if (match.Success)
        {
            return match.Groups[1].Value;
        }

        return string.Empty;
    }

    protected async Task<(IStateStorage, StateKey)> SetupStateStorage<T>(T state)
        where T : Models.BankIdUiState
    {
        var options = Options.Create(new MemoryCacheOptions());
        var memoryCache = new MemoryCache(options);
        var stateStorage = new InMemoryStateStorage(memoryCache, TimeSpan.FromMinutes(1));
        var stateKey = await stateStorage.SetAsync(state);
        return (stateStorage, stateKey);
    }
}
