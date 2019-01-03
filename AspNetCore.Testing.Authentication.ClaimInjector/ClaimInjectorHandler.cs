using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace AspNetCore.Testing.Authentication.ClaimInjector
{
    public class ClaimInjectorHandler : AuthenticationHandler<ClaimInjectorHandlerOptions>
    {
        public const string AuthenticationScheme = "Bypass";

        public ClaimInjectorHandler(IOptionsMonitor<ClaimInjectorHandlerOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) 
            : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!this.Request.Headers.ContainsKey("Authorization"))
            {
                return Task.FromResult(AuthenticateResult.Fail("No authentication header"));
            }

            string base64Json = this.Request.Headers["Authorization"].ToString().Replace(AuthenticationScheme, "").Trim();
            string json = Encoding.UTF8.GetString(Convert.FromBase64String(base64Json));

            var claimInjectorHandlerHeaderConfig = JsonConvert.DeserializeObject<ClaimInjectorHandlerHeaderConfig>(json);

            ClaimsIdentity identity = new ClaimsIdentity(claimInjectorHandlerHeaderConfig.Claims, AuthenticationScheme);
            return Task.FromResult(
                AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(identity), AuthenticationScheme)));
        }
    }
}