using AspNetCore.Testing.Authentication.ClaimInjector.Site;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace AspNetCore.Testing.Authentication.ClaimInjector.Test
{
    public class CustomClaimTypesUnitTests : IClassFixture<ClaimInjectorWebApplicationFactory<Startup>>
    {
        private readonly ClaimInjectorWebApplicationFactory<Startup> _factory;
        private const string _nameClaimType = "CustomNameClaimType";
        private const string _roleClaimType = "CustomRoleClaimType";

        public CustomClaimTypesUnitTests(ClaimInjectorWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
            _factory.NameClaimType = _nameClaimType;
            _factory.RoleClaimType = _roleClaimType;
            _factory.RoleConfig.Reset();
        }

        /// <summary>
        /// Make an authenticated call with a custom claim and ensure it's accessible.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task NameIsInHttpContextWhenCustomClaimTypeIsUsedTest()
        {
            var expectedValue = "John Doe";

            _factory.RoleConfig.AddClaim(new Claim(_nameClaimType, expectedValue));

            var client = _factory.CreateClient();

            var response = await client.GetAsync($"api/Values/ReturnsName");

            response.EnsureSuccessStatusCode();

            var actual = await response.Content.ReadAsStringAsync();

            Assert.Equal(expectedValue, actual);
        }

        /// <summary>
        /// Make an authenticated call with a custom claim and ensure it's accessible.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task RoleIsInHttpContextWhenCustomClaimTypeIsUsedTest()
        {
            var expectedValue = "Reader";

            _factory.RoleConfig.AddClaim(new Claim(_roleClaimType, expectedValue));

            var client = _factory.CreateClient();

            var response = await client.GetAsync($"api/Values/ReturnsIsInRole/{expectedValue}");

            response.EnsureSuccessStatusCode();

            var actual = await response.Content.ReadAsAsync<bool>();

            Assert.True(actual);
        }
    }
}
