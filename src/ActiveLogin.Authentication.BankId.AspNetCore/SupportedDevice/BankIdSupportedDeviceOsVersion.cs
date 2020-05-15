using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ActiveLogin.Authentication.BankId.AspNetCore.SupportedDevice
{
    public class BankIdSupportedDeviceOsVersion
    {
        public BankIdSupportedDeviceOsVersion(int majorVersion = 0, int minorVersion = 0, int patch = 0)
        {
            MajorVersion = majorVersion;
            MinorVersion = minorVersion;
            Patch = patch;
        }

        public int MajorVersion { get; } = 0;
        public int MinorVersion { get; } = 0;
        public int Patch { get;} = 0;
    }
}
