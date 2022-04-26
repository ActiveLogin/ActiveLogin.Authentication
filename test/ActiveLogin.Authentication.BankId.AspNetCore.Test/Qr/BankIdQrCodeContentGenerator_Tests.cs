using ActiveLogin.Authentication.BankId.AspNetCore.Qr;

using Xunit;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Test.Qr;

public class BankIdQrCodeContentGenerator_Tests
{

    [Theory]
    [InlineData(0, "67df3917-fa0d-44e5-b327-edcc928297f8", "d28db9a7-4cde-429e-a983-359be676944c", "bankid.67df3917-fa0d-44e5-b327-edcc928297f8.0.dc69358e712458a66a7525beef148ae8526b1c71610eff2c16cdffb4cdac9bf8")]
    [InlineData(1, "67df3917-fa0d-44e5-b327-edcc928297f8", "d28db9a7-4cde-429e-a983-359be676944c", "bankid.67df3917-fa0d-44e5-b327-edcc928297f8.1.949d559bf23403952a94d103e67743126381eda00f0b3cbddbf7c96b1adcbce2")]
    [InlineData(2, "67df3917-fa0d-44e5-b327-edcc928297f8", "d28db9a7-4cde-429e-a983-359be676944c", "bankid.67df3917-fa0d-44e5-b327-edcc928297f8.2.a9e5ec59cb4eee4ef4117150abc58fad7a85439a6a96ccbecc3668b41795b3f3")]
    public void Should_Return_QrData_From_BankId_Examples(int time, string qrStartToken, string qrStartSecret, string expectedQrData)
    {
        var qrContentGenerator = new BankIdQrCodeContentGenerator();
        var qrCodeContent = qrContentGenerator.Generate(qrStartToken, qrStartSecret, time);

        Assert.Equal(expectedQrData, qrCodeContent);
    }
}
