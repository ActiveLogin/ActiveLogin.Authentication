using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(IdentityServer.ClientSample.Framework48.Startup))]

namespace IdentityServer.ClientSample.Framework48
{
   public partial class Startup
   {
       public void Configuration(IAppBuilder app)
       {
           ConfigureAuthentication(app);
       }
   }
}
