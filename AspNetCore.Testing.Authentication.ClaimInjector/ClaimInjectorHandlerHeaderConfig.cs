using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Newtonsoft.Json;

namespace AspNetCore.Testing.Authentication.ClaimInjector
{
    /// <summary>
    /// This is the per call configuration for the Authorization shim. It allows for customization of
    /// all of the claims presented to the server. It's accessed via: <see cref="ClaimInjectorWebApplicationFactory{T}.RoleConfig"/>
    /// Helper functions exist for <see cref="Name"/> and <see cref="Roles"/>, but all claims can be added
    /// via <see cref="AddClaim(System.String, System.String)"/>.
    /// </summary>
    public class ClaimInjectorHandlerHeaderConfig
    {
        [JsonProperty]
        private Dictionary<string, List<string>> _claims = new Dictionary<string, List<string>>();

        [JsonConstructor]
        internal ClaimInjectorHandlerHeaderConfig() => Reset();

        /// <summary>
        /// True makes the client generate an anonymous request, false causes a client
        /// with all claims added to be generated.
        /// </summary>
        [JsonProperty]
        public bool AnonymousRequest { get; set; }

        /// <summary>
        /// Sets the name claim.
        /// </summary>
        public string Name 
        { 
            set
            {
                if (value == null) throw new ArgumentNullException(nameof(value));

                _claims.Remove(ClaimTypes.Name);

                _claims.Add(ClaimTypes.Name, new List<string> {value});
            }
        }

        /// <summary>
        /// Clears existing roles and sets the current roles to the array passed in.
        /// </summary>
        public string[] Roles
        {
            set
            {
                if (value == null || value.Any(x => x == null)) {
                    throw new ArgumentNullException(nameof(value));
                }

                _claims.Remove(ClaimTypes.Role);

                foreach(var role in value) {
                    AddClaim(ClaimTypes.Role, role);
                }
            }
        }

        /// <summary>
        /// Adds a claim to the collection of claims.
        /// </summary>
        /// <param name="claimType">The type of claim. <see cref="ClaimTypes"/> for standard values.</param>
        /// <param name="value">The value of the claim.</param>
        public void AddClaim(string claimType, string value)
        {
            if (!this._claims.TryGetValue(claimType, out var values))
            {
                values = new List<string>();
                this._claims.Add(claimType, values);
            }

            values.Add(value);
        }

        /// <summary>
        /// Adds a claim to the collection of claims.
        /// </summary>
        /// <param name="claim">The claim to add.</param>
        public void AddClaim(Claim claim) => AddClaim(claim.Type, claim.Value);

        internal IEnumerable<Claim> Claims => _claims.SelectMany(x => x.Value.Select(y => new Claim(x.Key, y)));

        /// <summary>
        /// Resets the claim collection to it's default state.
        /// </summary>
        public void Reset()
        {
            this._claims.Clear();
            this.AnonymousRequest = false;
            this.Name = "Authenticated User";
            this.Roles = new string[0];
        }
    }
}