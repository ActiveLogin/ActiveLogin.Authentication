using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace IdentityServer.ServerSample
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                   .UseApplicationInsights()
                   .ConfigureLogging(builder =>
                   {
                       builder.AddApplicationInsights(options =>
                       {
                           options.IncludeScopes = true;
                       });
                   })
                   .UseStartup<Startup>();
    }
}
