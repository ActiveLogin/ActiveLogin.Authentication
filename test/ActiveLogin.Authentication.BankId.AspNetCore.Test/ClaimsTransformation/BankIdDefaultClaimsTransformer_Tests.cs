using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authentication;

using Xunit;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Test.ClaimsTransformation
{
    public class BankIdDefaultClaimsTransformer_Tests
    {
        [Fact]
        public async Task Should_Add_PIN_As_Sub_Claim()
        {
            // Arrange
            var bankIdOptions = new BankIdOptions();
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
            var bankIdOptions = new BankIdOptions();
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
            var bankIdOptions = new BankIdOptions();
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
            var bankIdOptions = new BankIdOptions();
            var date = DateTime.Now.ToShortDateString();

            var context = new BankIdClaimsTransformationContext(bankIdOptions, "", "381123-9106", "", "", "");

            // Act
            var claims = await TransformClaims(context);

            // Assert
            AssertClaim(claims, "exp", date);
        }

        [Fact]
        public async Task Should_Add_Authentication_Method_As_amr_Claim()
        {
            // Arrange
            var bankIdOptions = new BankIdOptions();

            var context = new BankIdClaimsTransformationContext(bankIdOptions, "", "381123-9106", "", "", "");

            // Act
            var claims = await TransformClaims(context);

            // Assert
            AssertClaim(claims, "amr", bankIdOptions.AuthenticationMethodName);
        }

        [Fact]
        public async Task Should_Add_Birth_Date_As_birthdate_Claim()
        {
            // Arrange
            var bankIdOptions = new BankIdOptions()
            {
                IssueBirthdateClaim = true
            };

            var context = new BankIdClaimsTransformationContext(bankIdOptions, "", "381123-9106", "", "", "");

            // Act
            var claims = await TransformClaims(context);

            // Assert
            AssertClaim(claims, "birthdate", "1938-11-23");
        }

        [Fact]
        public async Task Should_Add_Gender_As_gender_Claim()
        {
            // Arrange
            var bankIdOptions = new BankIdOptions()
            {
                IssueGenderClaim = true
            };

            var context = new BankIdClaimsTransformationContext(bankIdOptions, "", "350824-9079", "", "", "");

            // Act
            var claims = await TransformClaims(context);

            // Assert
            AssertClaim(claims, "gender", GenderFromPIN(context));
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

        private string GenderFromPIN(BankIdClaimsTransformationContext context)
        {
            int secondLastNumber = int.Parse(context.PersonalIdentityNumber.Substring(context.PersonalIdentityNumber.Length - 2, 1));
            if (secondLastNumber % 2 == 0)
                return "female";
            else
                return "male";
        }
    }
}
