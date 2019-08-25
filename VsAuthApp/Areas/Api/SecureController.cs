using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace VsAuthApp.Api
{
    [Route("api/secure")]
    [ApiController]
    [Authorize]
    public class SecureController : ControllerBase
    {
        public SecureController()
        {
            // lol
        }

        [HttpGet]
        public ActionResult<string> GetSecureValue()
        {
            return "Super Secure Value";
        }
    }
}