using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace AspNetCore.Testing.Authentication.ClaimInjector
{
    internal class ClaimInjectorHandler : AuthenticationHandler<ClaimInjectorHandlerOptions>
    {
        internal const string AuthenticationScheme = "Bypass";

        /// <summary>
        /// This must be public for the middleware to be creatable by asp.net core
        /// </summary>
        /// <param name="options"></param>
        /// <param name="logger"></param>
        /// <param name="encoder"></param>
        /// <param name="clock"></param>
        public ClaimInjectorHandler(IOptionsMonitor<ClaimInjectorHandlerOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                return Task.FromResult(AuthenticateResult.Fail("No authentication header"));
            }

            string base64Json = Request.Headers["Authorization"].ToString().Replace(AuthenticationScheme, "").Trim();
            string json = Encoding.UTF8.GetString(Convert.FromBase64String(base64Json));

            var claimInjectorHandlerHeaderConfig = JsonConvert.DeserializeObject<ClaimInjectorHandlerHeaderConfig>(json);

            var identity = new ClaimsIdentity(claimInjectorHandlerHeaderConfig.Claims, AuthenticationScheme,
                Options.NameClaimType, Options.RoleClaimType);
            return Task.FromResult(
                AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(identity), AuthenticationScheme)));
        }
    }
}