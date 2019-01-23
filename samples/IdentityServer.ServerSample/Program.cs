using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace IdentityServer.ServerSample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                   .UseApplicationInsights()
                   .UseStartup<Startup>();
    }
}
