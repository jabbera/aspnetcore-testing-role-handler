using System;
using System.Net;
using System.Security.Claims;
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

        /// <summary>
        /// Make an authenticated call to an authenticated endpoint. <see cref="Site.Controllers.ValuesController.AllowAuthorized"/>
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task DefaultConfigIsAuthorizedTest()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync("api/Values/AllowAuthorized");

            response.EnsureSuccessStatusCode();
        }

        /// <summary>
        /// Make an unauthenticated call to and authenticated endpoint. <see cref="Site.Controllers.ValuesController.AllowAuthorized"/>
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task NoAuthorizationHeaderReturnsUnauthorizedTest()
        {
            this._factory.RoleConfig.AnonymousRequest = true;

            var client = _factory.CreateClient();

            var response = await client.GetAsync("api/Values/AllowAuthorized");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        /// <summary>
        /// Make an unauthenticated call to an unauthenticated endpoint. <see cref="Site.Controllers.ValuesController.AllowAnonymous"/>
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task CreationOfAnonymousClientsWorksTest()
        {
            this._factory.RoleConfig.AnonymousRequest = true;

            var client = _factory.CreateDefaultClient();

            var response = await client.GetAsync("api/Values/AllowAnonymous");

            response.EnsureSuccessStatusCode();
        }

        /// <summary>
        /// Make an authenticated call with a role to an authenticated endpoint requiring the role. <see cref="Site.Controllers.ValuesController.RequireRoleReader"/>
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task RoleCustomizationWorksTest()
        {
            this._factory.RoleConfig.Roles = new[] {"Reader"};

            var client = _factory.CreateClient();

            var response = await client.GetAsync("api/Values/RequireRoleReader");

            response.EnsureSuccessStatusCode();
        }

        /// <summary>
        /// Make an authenticated call without required role to an authenticated endpoint
        /// requiring the role. <see cref="Site.Controllers.ValuesController.RequireRoleReader"/>
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task RoleCustomizationWorkDeniedTest()
        {
            this._factory.RoleConfig.Roles = new[] { "Writer" };

            var client = _factory.CreateClient();

            var response = await client.GetAsync("api/Values/RequireRoleReader");

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }


        /// <summary>
        /// Make an authenticated call with a custom name and ensure that name is accessible. 
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Make an authenticated call with a custom claim and ensure it's accessible.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task CustomClaimsWorkTest()
        {
            string expectedValue = "Hello World";
            string expectedClaimType = "CustomClaimType";

            this._factory.RoleConfig.AddClaim(new Claim(expectedClaimType, expectedValue));

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
