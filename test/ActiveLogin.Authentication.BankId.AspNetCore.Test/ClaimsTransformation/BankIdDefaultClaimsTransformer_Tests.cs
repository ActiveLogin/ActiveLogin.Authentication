using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using ActiveLogin.Authentication.BankId.AspNetCore.Auth;
using ActiveLogin.Authentication.BankId.AspNetCore.ClaimsTransformation;

using Microsoft.AspNetCore.Authentication;
using Moq;
using Xunit;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Test.ClaimsTransformation;

public class BankIdDefaultClaimsTransformer_Tests
{
    [Fact]
    public async Task Should_Add_PIN_As_Sub_Claim()
    {
        // Arrange
        var bankIdOptions = new BankIdAuthOptions();
        var context = new BankIdClaimsTransformationContext(bankIdOptions, "", "381123-9106", "", "", "");

        // Act
        var claims = await TransformClaims(context);

        // Assert
        AssertClaim(claims, "sub", "193811239106");
    }

    [Fact]
    public async Task Should_Add_Names_As_Name_Claims()
    {
        // Arrange
        var bankIdOptions = new BankIdAuthOptions();
        var context = new BankIdClaimsTransformationContext(bankIdOptions, "", "381123-9106", "NAME", "GIVEN_NAME", "FAMILY_NAME");

        // Act
        var claims = await TransformClaims(context);

        // Assert
        AssertClaim(claims, "name", "NAME");
        AssertClaim(claims, "given_name", "GIVEN_NAME");
        AssertClaim(claims, "family_name", "FAMILY_NAME");
    }

    [Fact]
    public async Task Should_Add_PIN_As_swedish_personal_identity_number_Claim()
    {
        // Arrange
        var bankIdOptions = new BankIdAuthOptions();
        var context = new BankIdClaimsTransformationContext(bankIdOptions, "", "381123-9106", "", "", "");

        // Act
        var claims = await TransformClaims(context);

        // Assert
        AssertClaim(claims, "swedish_personal_identity_number", "381123-9106");
    }

    [Fact]
    public async Task Should_Add_TokenExpiresIn_As_exp_Claim()
    {
        // Arrange
        var bankIdOptions = new BankIdAuthOptions();
        bankIdOptions.TokenExpiresIn = TimeSpan.FromHours(2);

        var context = new BankIdClaimsTransformationContext(bankIdOptions, "", "381123-9106", "", "", "");
        var systemClockMock = new Mock<ISystemClock>();
        var dateTime = new DateTime(2022, 03, 11, 05, 30, 30, DateTimeKind.Utc);
        systemClockMock.Setup(x => x.UtcNow).Returns(dateTime);
        var claimsTransformer = new BankIdDefaultClaimsTransformer(systemClockMock.Object);

        // Act
        await claimsTransformer.TransformClaims(context);
        var claims = context.Claims;

        // Assert
        AssertClaim(claims, "exp", "1646983830");
    }

    [Fact]
    public async Task Should_Add_AuthenticationMethod_As_amr_Claim()
    {
        // Arrange
        var bankIdOptions = new BankIdAuthOptions
        {
            IssueAuthenticationMethodClaim = true,
            AuthenticationMethodName = "AUTH_METHOD"
        };

        var context = new BankIdClaimsTransformationContext(bankIdOptions, "", "381123-9106", "", "", "");

        // Act
        var claims = await TransformClaims(context);

        // Assert
        AssertClaim(claims, "amr", "AUTH_METHOD");
    }

    [Fact]
    public async Task Should_Not_Add_AuthenticationMethod_As_amr_Claim_When_Disabled()
    {
        // Arrange
        var bankIdOptions = new BankIdAuthOptions
        {
            IssueAuthenticationMethodClaim = false,
            AuthenticationMethodName = "AUTH_METHOD"
        };

        var context = new BankIdClaimsTransformationContext(bankIdOptions, "", "381123-9106", "", "", "");

        // Act
        var claims = await TransformClaims(context);

        // Assert
        AssertNoClaim(claims, "amr");
    }

    [Fact]
    public async Task Should_Add_IdentityProvider_As_idp_Claim()
    {
        // Arrange
        var bankIdOptions = new BankIdAuthOptions
        {
            IssueIdentityProviderClaim = true,
            IdentityProviderName = "IDENTITY_PROVIDER"
        };

        var context = new BankIdClaimsTransformationContext(bankIdOptions, "", "381123-9106", "", "", "");

        // Act
        var claims = await TransformClaims(context);

        // Assert
        AssertClaim(claims, "idp", "IDENTITY_PROVIDER");
    }

    [Fact]
    public async Task Should_Not_Should_Add_IdentityProvider_As_idp_Claim_When_Disabled()
    {
        // Arrange
        var bankIdOptions = new BankIdAuthOptions
        {
            IssueIdentityProviderClaim = false,
            IdentityProviderName = "IDENTITY_PROVIDER"
        };

        var context = new BankIdClaimsTransformationContext(bankIdOptions, "", "381123-9106", "", "", "");

        // Act
        var claims = await TransformClaims(context);

        // Assert
        AssertNoClaim(claims, "idp");
    }

    private async Task<List<Claim>> TransformClaims(BankIdClaimsTransformationContext context)
    {
        var claimsTransformer = new BankIdDefaultClaimsTransformer(new SystemClock());

        await claimsTransformer.TransformClaims(context);
        return context.Claims;
    }

    private void AssertClaim(List<Claim> claims, string type, string value)
    {
        Assert.NotEmpty(claims.Where(x => x.Type == type && x.Value == value));
    }

    private void AssertNoClaim(List<Claim> claims, string type)
    {
        Assert.Empty(claims.Where(x => x.Type == type));
    }
}
