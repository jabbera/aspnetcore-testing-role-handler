using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Newtonsoft.Json;

namespace AspNetCore.Testing.Authentication.ClaimInjector
{
    public class ClaimInjectorHandlerHeaderConfig
    {
        [JsonProperty] private Dictionary<string, List<string>> _claims = new Dictionary<string, List<string>>();

        public ClaimInjectorHandlerHeaderConfig() => Reset();

        [JsonProperty]
        public bool AnonymousRequest { get; set; }

        public string Name 
        { 
            set
            {
                if (value == null) throw new ArgumentNullException(nameof(value));

                _claims.Remove(ClaimTypes.Name);

                _claims.Add(ClaimTypes.Name, new List<string> {value});
            }
        }

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

        public void AddClaim(string claimType, string value)
        {
            if (!this._claims.TryGetValue(claimType, out var values))
            {
                values = new List<string>();
                this._claims.Add(claimType, values);
            }

            values.Add(value);
        }

        public void AddClaim(Claim claim) => AddClaim(claim.Type, claim.Value);

        internal IEnumerable<Claim> Claims => _claims.SelectMany(x => x.Value.Select(y => new Claim(x.Key, y)));

        public void Reset()
        {
            this._claims.Clear();
            this.AnonymousRequest = false;
            this.Name = "Authenticated User";
            this.Roles = new string[0];
        }
    }
}