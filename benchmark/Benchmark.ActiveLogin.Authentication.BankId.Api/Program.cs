using BenchmarkDotNet.Running;

namespace ActiveLogin.Authentication.BankId.Api
{
    static class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<BankIdApiClientBenchmark>();
        }
    }
}
