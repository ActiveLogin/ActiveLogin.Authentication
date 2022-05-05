using System.Text;
using System.Text.Encodings.Web;

namespace ActiveLogin.Authentication.BankId.Core;

public class QueryBuilder
{
    private readonly List<KeyValuePair<string, string>> _params;

    public QueryBuilder(IEnumerable<KeyValuePair<string, string>> parameters)
    {
        _params = new List<KeyValuePair<string, string>>(parameters);
    }

    public override string ToString()
    {
        var builder = new StringBuilder();
        var first = true;
        _params.ForEach(pair =>
        {
            builder.Append(first ? '?' : '&');
            first = false;
            builder.Append(UrlEncoder.Default.Encode(pair.Key));
            builder.Append('=');
            builder.Append(UrlEncoder.Default.Encode(pair.Value));
        });

        return builder.ToString();
    }
}
