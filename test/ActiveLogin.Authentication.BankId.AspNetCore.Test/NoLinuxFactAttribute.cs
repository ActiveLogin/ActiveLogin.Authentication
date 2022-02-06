using System.Runtime.InteropServices;
using Xunit;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Test
{
    public sealed class NoLinuxFactAttribute : FactAttribute
    {
        public NoLinuxFactAttribute(string linuxSkipReason)
        {
            //if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            //{
            //    Skip = linuxSkipReason;
            //}
        }
    }
}
