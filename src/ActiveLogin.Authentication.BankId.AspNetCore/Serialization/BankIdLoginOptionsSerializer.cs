using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ActiveLogin.Authentication.BankId.AspNetCore.Models;
using ActiveLogin.Identity.Swedish;
using Microsoft.AspNetCore.Authentication;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Serialization
{
    internal class BankIdLoginOptionsSerializer : IDataSerializer<BankIdLoginOptions>
    {
        private const int FormatVersion = 1;
        private const char CertificatePoliciesSeparator = ';';

        public byte[] Serialize(BankIdLoginOptions model)
        {
            using (var memory = new MemoryStream())
            {
                using (var writer = new BinaryWriter(memory))
                {
                    writer.Write(FormatVersion);

                    writer.Write(string.Join(CertificatePoliciesSeparator.ToString(), model.CertificatePolicies ?? new List<string>()));
                    writer.Write(model.PersonalIdentityNumber?.To12DigitString() ?? string.Empty);
                    writer.Write(model.AllowChangingPersonalIdentityNumber);
                    writer.Write(model.AutoLaunch);
                    writer.Write(model.AllowBiometric);

                    writer.Flush();
                    return memory.ToArray();
                }
            }
        }

        public BankIdLoginOptions Deserialize(byte[] data)
        {
            using (var memory = new MemoryStream(data))
            {
                using (var reader = new BinaryReader(memory))
                {
                    if (reader.ReadInt32() != FormatVersion)
                    {
                        return null;
                    }

                    var certificatePolicies = reader.ReadString()
                        .Split(new[] { CertificatePoliciesSeparator }, StringSplitOptions.RemoveEmptyEntries)
                        .ToList();

                    string personalIdentityNumberString = reader.ReadString();
                    SwedishPersonalIdentityNumber personalIdentityNumber = string.IsNullOrEmpty(personalIdentityNumberString)
                            ? null
                            : SwedishPersonalIdentityNumber.Parse(personalIdentityNumberString);

                    bool allowChangingPersonalIdentityNumber = reader.ReadBoolean();
                    bool autoLaunch = reader.ReadBoolean();
                    bool allowBiometric = reader.ReadBoolean();

                    return new BankIdLoginOptions(
                        certificatePolicies,
                        personalIdentityNumber,
                        allowChangingPersonalIdentityNumber,
                        autoLaunch,
                        allowBiometric
                    );
                }
            }
        }
    }
}
