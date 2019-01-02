namespace AspNetCore.Testing.RoleHandler
{
    public class CustomRoleHandlerHeaderConfig
    {
        public CustomRoleHandlerHeaderConfig() => Reset();

        public bool AnonymousRequest;
        public string Name { get; set; }
        public string[] Roles { get; set; }

        public void Reset()
        {
            this.AnonymousRequest = false;
            this.Name = "Authenticated User";
            this.Roles = new string[0];
        }
    }
}