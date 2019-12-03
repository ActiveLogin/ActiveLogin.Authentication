using System;

namespace ActiveLogin.Authentication.BankId.AspNetCore
{
    public class IncompatibleSerializationVersion : Exception
    {
        public IncompatibleSerializationVersion(string type)
        : base($"Can't deserialize {type} because it was serialized with another version.")
        {

        }
    }
}