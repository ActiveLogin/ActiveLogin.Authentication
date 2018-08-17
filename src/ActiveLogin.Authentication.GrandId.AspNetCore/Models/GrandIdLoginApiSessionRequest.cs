using ActiveLogin.Authentication.GrandId.Api.Models;
using ActiveLogin.Identity.Swedish.AspNetCore.Validation;

namespace ActiveLogin.Authentication.GrandId.AspNetCore.Models
{
    public class GrandIdLoginApiSessionRequest
    {
        public string SessionId { get; set; }

        public DeviceOption DeviceOption { get; set; } = DeviceOption.ChooseDevice;
    }
}