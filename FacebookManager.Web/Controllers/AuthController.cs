using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Mvc;

namespace FacebookManager.Web.Controllers
{
    [Route("[controller]")]
    public class AuthController : Controller
    {
        [HttpGet("unauthorized")]
        public IActionResult NeedLogin()
        {
            return Unauthorized();
        }

        [HttpGet("login")]
        public IActionResult Login(string redirectUri = "/")
        {
            return Challenge(
                new AuthenticationProperties
                {
                    RedirectUri = redirectUri
                },
                FacebookDefaults.AuthenticationScheme);
        }
    }
}