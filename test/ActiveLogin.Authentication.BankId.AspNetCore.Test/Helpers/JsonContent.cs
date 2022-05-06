using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Test.Helpers;

internal class JsonContent : StringContent
{
    public JsonContent(object obj) :
        base(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json")
    { }
}