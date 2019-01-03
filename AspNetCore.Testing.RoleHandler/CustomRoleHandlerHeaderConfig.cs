using System;
using System.Linq;

namespace AspNetCore.Testing.RoleHandler
{
    public class CustomRoleHandlerHeaderConfig
    {
        private string _name;
        private string[] _roles;

        public CustomRoleHandlerHeaderConfig() => Reset();

        public bool AnonymousRequest;
        public string Name 
        { 
            get => _name; 
            set
            {
                _name = value ?? throw new ArgumentNullException(nameof(value));
            }
        }

        public string[] Roles
        {
            get => _roles;
            set
            {
                if (value == null || value.Any(x => x == null))
                {
                    throw new ArgumentNullException();
                }

                _roles = value;
            }
        }

        public void Reset()
        {
            this.AnonymousRequest = false;
            this.Name = "Authenticated User";
            this.Roles = new string[0];
        }
    }
}