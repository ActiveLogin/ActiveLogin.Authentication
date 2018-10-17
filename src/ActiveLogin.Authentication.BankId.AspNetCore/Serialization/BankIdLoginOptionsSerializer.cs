using System.IO;
using ActiveLogin.Authentication.BankId.AspNetCore.Models;
using ActiveLogin.Identity.Swedish;
using Microsoft.AspNetCore.Authentication;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Serialization
{
    internal class BankIdLoginOptionsSerializer : IDataSerializer<BankIdLoginOptions>
    {
        private const int FormatVersion = 1;

        public byte[] Serialize(BankIdLoginOptions model)
        {
            using (var memory = new MemoryStream())
            {
                using (var writer = new BinaryWriter(memory))
                {
                    writer.Write(FormatVersion);

                    writer.Write(model.CertificatePolicies);
                    writer.Write(model.PersonalIdentityNumber?.ToLongString() ?? string.Empty);
                    writer.Write(model.AllowChangingPersonalIdentityNumber);
                    writer.Write(model.AutoLaunch);

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

                    var certificatePolicies = reader.ReadString();
                    var personalIdentityNumberString = reader.ReadString();
                    var personalIdentityNumber = string.IsNullOrEmpty(personalIdentityNumberString) ? null : SwedishPersonalIdentityNumber.Parse(personalIdentityNumberString);
                    var allowChangingPersonalIdentityNumber = reader.ReadBoolean();
                    var autoLaunch = reader.ReadBoolean();

                    return new BankIdLoginOptions(
                        certificatePolicies,
                        personalIdentityNumber,
                        allowChangingPersonalIdentityNumber,
                        autoLaunch
                    );
                }
            }
        }
    }
}