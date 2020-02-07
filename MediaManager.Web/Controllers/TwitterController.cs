using System.Threading.Tasks;
using Extensions.Hosting.AsyncInitialization;
using MediaManager.Extensions;
using MediaManager.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MediaManager.Web.Controllers
{
    [Authorize]
    public class TwitterController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly TwitterService _service;

        public TwitterController(
            UserManager<ApplicationUser> userManager,
            TwitterService service)
        {
            _userManager = userManager;
            _service = service;
        }

        public async Task<IActionResult> Login(string returnUrl = "/")
        {
            ApplicationUser user = await _userManager.GetUserAsync(User);
            await _service.Login(user);
            
            return Redirect(returnUrl);
        }
    }
}