using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace AspNetCore.Testing.Authentication.ClaimInjector
{
    /// <summary>
    /// A derived class that instruments the WebHost with the claim injector and the client
    /// with the appropriate Authorization header per the configured: <see cref="ClaimInjectorHandlerHeaderConfig"/>
    /// via: <see cref="RoleConfig"/>. If you derived off this class to further customized testing
    /// please be sure to call <c>base.<see cref="ConfigureClient"/></c> and <c>base.<see cref="ConfigureWebHost"/></c>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ClaimInjectorWebApplicationFactory<T> : WebApplicationFactory<T> where T : class
    {
        /// <summary>
        /// The main customization point of the claims.
        /// </summary>
        public ClaimInjectorHandlerHeaderConfig RoleConfig { get; } = new ClaimInjectorHandlerHeaderConfig();

        protected override void ConfigureClient(HttpClient client)
        {
            base.ConfigureClient(client);

            if (RoleConfig.AnonymousRequest)
            {
                return;
            }

            string json = JsonConvert.SerializeObject(RoleConfig);

            string base64Json = Convert.ToBase64String(Encoding.UTF8.GetBytes(json));

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bypass", base64Json);
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);

            builder.ConfigureTestServices(services =>
            {
                services.AddAuthentication(x =>
                    {
                        x.DefaultScheme = ClaimInjectorHandler.AuthenticationScheme;
                        x.DefaultAuthenticateScheme = ClaimInjectorHandler.AuthenticationScheme;
                        x.DefaultChallengeScheme = ClaimInjectorHandler.AuthenticationScheme;
                    })
                    .AddScheme<ClaimInjectorHandlerOptions, ClaimInjectorHandler>(ClaimInjectorHandler.AuthenticationScheme,
                        x => { });
            });
        }
    }
}
