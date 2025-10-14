using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using ActiveLogin.Authentication.BankId.Api;
using ActiveLogin.Authentication.BankId.Api.Models;
using ActiveLogin.Authentication.BankId.AspNetCore.Test.Helpers;

using Microsoft.AspNetCore.TestHost;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Test;

public abstract class BankId_Ui_Tests_Base
{
    private const string DefaultStateCookieName = "__ActiveLogin.BankIdUiState";
    private const string DefaultUiOptionsCookieName = "__ActiveLogin.BankId.UiOptions";

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

    protected async Task<HttpResponseMessage> MakeRequestWithRequiredContext(string bankIdType, string path, TestServer server, HttpContent content)
    {
        var client = server.CreateClient();

        // Arrange state cookie
        var stateRequest = server.CreateRequest("/ANYURL");
        var stateResponse = await stateRequest.GetAsync();
        var stateCookies = stateResponse.Headers.GetValues("set-cookie");

        // Arrange csrf info
        var loginRequest = CreateRequestWithFakeStateAndUiOptionsCookie(server, $"/ActiveLogin/BankId/{bankIdType}?returnUrl=%2F");
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

    protected async Task<HttpResponseMessage> GetInitializeResponse(string bankIdType, TestServer server, object initializeRequestBody)
    {
        var initializeRequest = new JsonContent(initializeRequestBody);
        return await MakeRequestWithRequiredContext(bankIdType, $"/ActiveLogin/BankId/{bankIdType}/Api/Initialize", server, initializeRequest);
    }

    protected static RequestBuilder CreateRequestWithFakeStateAndUiOptionsCookie(TestServer server, string path)
    {
        var request = server.CreateRequest(path);
        request.AddHeader("Cookie", $"{DefaultStateCookieName}=TEST");
        request.AddHeader("Cookie", $"{DefaultUiOptionsCookieName}=TEST");
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
}
