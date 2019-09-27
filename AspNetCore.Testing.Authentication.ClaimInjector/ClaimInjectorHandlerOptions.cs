using Microsoft.AspNetCore.Authentication;

namespace AspNetCore.Testing.Authentication.ClaimInjector
{
    internal class ClaimInjectorHandlerOptions : AuthenticationSchemeOptions
    {
        public string RoleClaimType { get; set; }

        public string NameClaimType { get; set; }
    }
}
