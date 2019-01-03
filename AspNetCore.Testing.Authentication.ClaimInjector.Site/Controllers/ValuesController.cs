using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCore.Testing.Authentication.ClaimInjector.Site.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ValuesController : ControllerBase
    {
        // GET api/values
        [HttpGet("[action]")]
        public ActionResult<string> AllowAuthorized() => "value";

        // GET api/values/5
        [HttpGet("[action]")]
        [AllowAnonymous]
        public ActionResult<string> AllowAnonymous() => "value";

        [HttpGet("[action]")]
        [Authorize(Roles = "Reader")]
        public ActionResult<string> RequireRoleReader() => "value";

        [HttpGet("[action]")]
        public ActionResult<string> ReturnsName() => HttpContext.User.Identity.Name;

        [HttpGet("[action]/{claimType}")]
        public ActionResult<string> ReturnsCustomClaim(string claimType) =>
            HttpContext.User.Claims.FirstOrDefault(x => x.Type == claimType)?.Value;
    }
}
