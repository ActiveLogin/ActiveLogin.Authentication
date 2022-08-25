using System.Text;
using System.Text.Encodings.Web;

namespace ActiveLogin.Authentication.BankId.Core.Helpers;

public static class QueryStringGenerator
{
    public static string ToQueryString(IEnumerable<KeyValuePair<string, string>> parameters)
    {
        var builder = new StringBuilder();
        var first = true;
        foreach (var parameter in parameters)
        {
            builder.Append(first ? '?' : '&');
            first = false;
            builder.Append(UrlEncoder.Default.Encode(parameter.Key));
            builder.Append('=');
            builder.Append(UrlEncoder.Default.Encode(parameter.Value));
        }

        return builder.ToString();
    }
}
