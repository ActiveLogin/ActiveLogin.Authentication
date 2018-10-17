using System.IO;
using ActiveLogin.Authentication.BankId.AspNetCore.Areas.BankIdAuthentication.Models;
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

                    return new BankIdLoginOptions
                    {
                        CertificatePolicies = reader.ReadString()
                    };
                }
            }
        }
    }
}