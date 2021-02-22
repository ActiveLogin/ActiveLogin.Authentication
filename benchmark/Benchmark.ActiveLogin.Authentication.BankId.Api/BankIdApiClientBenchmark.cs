using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ActiveLogin.Authentication.BankId.Api.Models;
using BenchmarkDotNet.Attributes;
using Moq;
using Moq.Protected;

namespace ActiveLogin.Authentication.BankId.Api
{
    public class BankIdApiClientBenchmark
    {
        private readonly BankIdApiClient _bankIdApiClient;
        private readonly Mock<HttpMessageHandler> _handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        private readonly AuthRequest _authRequest = new AuthRequest("127.0.0.1", "191212121212", new Requirement());
        private readonly SignRequest _signRequest = new SignRequest("127.0.0.1", "test");
        private readonly CollectRequest _collectRequest = new CollectRequest("abc123");
        private readonly CancelRequest _cancelRequest = new CancelRequest("abc123");

        public BankIdApiClientBenchmark()
        {
            var httpClient = new HttpClient(_handlerMock.Object)
            {
                BaseAddress = new Uri("http://test.com/"),
            };

            _bankIdApiClient = new BankIdApiClient(httpClient);
        }

        [Benchmark]
        public async Task AuthAsync()
        {
            ConfigureHandlerMock("{ \"orderRef\": \"abc123\", \"autoStartToken\": \"def456\" }");

            await _bankIdApiClient.AuthAsync(_authRequest);
        }

        [Benchmark]
        public async Task SignAsync()
        {
            ConfigureHandlerMock("{ \"orderRef\": \"abc123\", \"autoStartToken\": \"def456\" }");

            await _bankIdApiClient.SignAsync(_signRequest);
        }

        [Benchmark]
        public async Task CollectAsync()
        {
            ConfigureHandlerMock("{ \"orderRef\":\"abc123\" }");

            await _bankIdApiClient.CollectAsync(_collectRequest);
        }

        [Benchmark]
        public async Task CancelAsync()
        {
            ConfigureHandlerMock("{ }");

            await _bankIdApiClient.CancelAsync(_cancelRequest);
        }

        private void ConfigureHandlerMock(string returnJsonContent)
        {
            _handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(returnJsonContent, Encoding.Default, "application/json"),
                })
                .Verifiable();
        }
    }
}
