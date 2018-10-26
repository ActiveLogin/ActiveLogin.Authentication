using System;
using ActiveLogin.Identity.Swedish;

namespace ActiveLogin.Authentication.Common.Serialization
{
    public static class JwtSerializer
    {
        /// <summary>
        /// Specified in: http://openid.net/specs/openid-connect-core-1_0.html#rfc.section.5.1
        /// </summary>
        /// <param name="gender"></param>
        /// <returns></returns>
        public static string GetGender(Gender gender)
        {
            switch (gender)
            {
                case Gender.Female:
                    return "female";
                case Gender.Male:
                    return "male";
            }

            return string.Empty;
        }

        /// <summary>
        /// Specified in: http://openid.net/specs/openid-connect-core-1_0.html#rfc.section.5.1
        /// </summary>
        /// <param name="birthdate"></param>
        /// <returns></returns>
        public static string GetBirthdate(DateTime birthdate)
        {
            return birthdate.Date.ToString("yyyy-MM-dd");
        }

        public static string GetExpires(DateTimeOffset expiresUtc)
        {
            return expiresUtc.Date.ToString("yyyy-MM-dd");
        }
    }
}
