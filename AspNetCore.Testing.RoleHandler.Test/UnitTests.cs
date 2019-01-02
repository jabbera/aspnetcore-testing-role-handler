using System;
using System.Net;
using System.Threading.Tasks;
using AspNetCore.Testing.RoleHandler;
using AspNetCore.Testing.Site;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.IdentityModel.Tokens;
using Xunit;

namespace role_handler_test
{
    public class UnitTests : IClassFixture<CustomRoleWebApplicationFactory<Startup>>
    {
        private readonly CustomRoleWebApplicationFactory<Startup> _factory;

        public UnitTests(CustomRoleWebApplicationFactory<Startup> factory)
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
    }
}
