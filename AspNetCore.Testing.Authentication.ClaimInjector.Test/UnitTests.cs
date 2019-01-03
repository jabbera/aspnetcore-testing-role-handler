using System;
using System.Net;
using System.Threading.Tasks;
using AspNetCore.Testing.Authentication.ClaimInjector;
using AspNetCore.Testing.Authentication.ClaimInjector.Site;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.IdentityModel.Tokens;
using Xunit;

namespace AspNetCore.Testing.Authentication.ClaimInjector.Test
{
    public class UnitTests : IClassFixture<ClaimInjectorWebApplicationFactory<Startup>>
    {
        private readonly ClaimInjectorWebApplicationFactory<Startup> _factory;

        public UnitTests(ClaimInjectorWebApplicationFactory<Startup> factory)
        {
            this._factory = factory;
            this._factory.RoleConfig.Reset();
        }

        [Fact]
        public async Task DefaultConfigIsAuthorizedTest()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync("api/Values/AllowAuthorized");

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task NoAuthorizationHeaderReturnsUnauthorizedTest()
        {
            this._factory.RoleConfig.AnonymousRequest = true;

            var client = _factory.CreateClient();

            var response = await client.GetAsync("api/Values/AllowAuthorized");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task CreationOfAnonymousClientsWorksTest()
        {
            this._factory.RoleConfig.AnonymousRequest = true;

            var client = _factory.CreateDefaultClient();

            var response = await client.GetAsync("api/Values/AllowAnonymous");

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task RoleCustomizationWorksTest()
        {
            this._factory.RoleConfig.Roles = new[] {"Reader"};

            var client = _factory.CreateClient();

            var response = await client.GetAsync("api/Values/RequireRoleReader");

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task NameIsInHttpContextTest()
        {
            string expectedName = "Hello World";

            this._factory.RoleConfig.Name = expectedName;

            var client = _factory.CreateClient();

            var response = await client.GetAsync("api/Values/ReturnsName");

            response.EnsureSuccessStatusCode();

            string actual = await response.Content.ReadAsStringAsync();

            Assert.Equal(expectedName, actual);
        }

        [Fact]
        public async Task CustomClaimsWorkTest()
        {
            string expectedValue = "Hello World";
            string expectedClaimType = "CustomClaimType";

            this._factory.RoleConfig.AddClaim(expectedClaimType, expectedValue);

            var client = _factory.CreateClient();

            var response = await client.GetAsync($"api/Values/ReturnsCustomClaim/{expectedClaimType}");

            response.EnsureSuccessStatusCode();

            string actual = await response.Content.ReadAsStringAsync();

            Assert.Equal(expectedValue, actual);
        }

        [Fact]
        public void NullNameThrowsTest() => Assert.Throws<ArgumentNullException>(() => _factory.RoleConfig.Name = null);
        
        [Fact]
        public void NullRolesThrowsTest() => Assert.Throws<ArgumentNullException>(() => _factory.RoleConfig.Roles = null);

        [Fact]
        public void NullRoleInCollectionThrowsTest() => Assert.Throws<ArgumentNullException>(() => _factory.RoleConfig.Roles = new string[] { null });
    }
}
