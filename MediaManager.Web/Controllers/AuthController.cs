using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Mvc;

namespace MediaManager.Web.Controllers
{
    [Route("[controller]")]
    public class AuthController : Controller
    {
        private const string DefaultRedirect = "/";

        [HttpGet("unauthorized")]
        public IActionResult NeedLogin()
        {
            return Unauthorized();
        }

        [HttpGet("login")]
        public IActionResult Login(string provider)
        {
            return Challenge(
                new AuthenticationProperties
                {
                    RedirectUri = DefaultRedirect
                }, provider);
        }

        [HttpGet("logout")]
        public async Task<IActionResult> Logout(string provider)
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return Redirect(DefaultRedirect);
        }
    }
}