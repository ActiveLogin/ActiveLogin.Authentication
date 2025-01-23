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

namespace ActiveLogin.Authentication.BankId.AspNetCore.Test;

public abstract class BankId_Ui_Tests_Base
{
    protected const string AuthStateKeyCookieName = "__ActiveLogin.BankIdAuthStateKey";
    protected const string SignStateKeyCookieName = "__ActiveLogin.BankIdUiSignStateKey";


    protected class TestBankIdAppApi(IBankIdAppApiClient bankIdAppApiClient) : IBankIdAppApiClient
    {
        public bool CancelAsyncIsCalled { get; private set; }

        public Task<AuthResponse> AuthAsync(AuthRequest request)
        {
            return bankIdAppApiClient.AuthAsync(request);
        }

        public Task<SignResponse> SignAsync(SignRequest request)
        {
            return bankIdAppApiClient.SignAsync(request);
        }

        public Task<PhoneAuthResponse> PhoneAuthAsync(PhoneAuthRequest request)
        {
            return bankIdAppApiClient.PhoneAuthAsync(request);
        }

        public Task<PhoneSignResponse> PhoneSignAsync(PhoneSignRequest request)
        {
            return bankIdAppApiClient.PhoneSignAsync(request);
        }

        public Task<CollectResponse> CollectAsync(CollectRequest request)
        {
            return bankIdAppApiClient.CollectAsync(request);
        }

        public Task<CancelResponse> CancelAsync(CancelRequest request)
        {
            CancelAsyncIsCalled = true;
            return bankIdAppApiClient.CancelAsync(request);
        }
    }

    protected async Task<HttpResponseMessage> GetInitializeResponse(string bankIdType, TestServer server, object initializeRequestBody, params (string key, string value)[] cookies)
    {
        var initializeRequest = new JsonContent(initializeRequestBody);
        return await MakeRequestWithRequiredContext(bankIdType, $"/ActiveLogin/BankId/{bankIdType}/Api/Initialize", server, initializeRequest, cookies);
    }

    protected async Task<HttpResponseMessage> MakeRequestWithRequiredContext(
        string bankIdType,
        string path,
        TestServer server,
        HttpContent content,
        params (string key, string value)[] cookies
    ){
        var client = server.CreateClient();

        // Arrange state cookie
        var stateRequest = server.CreateRequest("/ANYURL");
        var stateResponse = await stateRequest.GetAsync();
        var stateCookies = stateResponse.Headers.GetValues("set-cookie");

        // Arrange csrf info
        var loginRequest = CreateRequestWithCookies(server, $"/ActiveLogin/BankId/{bankIdType}?returnUrl=%2F&uiOptions=X&orderRef=Y", cookies);
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

    protected async Task<(IStateStorage<T>, StateKey)> SetupStateStorage<T>(T state)
        where T : Models.BankIdUiState
    {
        var stateStorage = new InMemoryStateStorage<T>();
        var stateKey = await stateStorage.WriteAsync(state);
        return (stateStorage, stateKey);
    }
}
