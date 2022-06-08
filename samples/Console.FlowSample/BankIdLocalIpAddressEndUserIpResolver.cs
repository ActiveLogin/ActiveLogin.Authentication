using System.Net;
using System.Net.Sockets;

using ActiveLogin.Authentication.BankId.Core.UserContext;

public class BankIdLocalIpAddressEndUserIpResolver : IBankIdEndUserIpResolver
{
    public string GetEndUserIp()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }

        return "127.0. 0.1";
    }
}
