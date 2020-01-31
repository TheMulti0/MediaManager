using System.Linq;
using System.Threading.Tasks;
using MediaManager.Api;
using MediaManager.Twitter;
using MediaManager.Web.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MediaManager.Web.Controllers
{
    [Route("[controller]")]
    public class AuthController : Controller
    {
        private const string DefaultRedirect = "/";
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly TwitterAppConfiguration _twitterConfiguration;

        public AuthController(
            SignInManager<ApplicationUser> signInManager,
            TwitterAppConfiguration twitterConfiguration)
        {
            _signInManager = signInManager;
            _userManager = signInManager.UserManager;
            _twitterConfiguration = twitterConfiguration;
        }

        [HttpGet("unauthorized")]
        public IActionResult NeedLogin()
        {
            return Unauthorized();
        }

        [HttpGet("login")]
        public IActionResult Login(
            string provider,
            string returnUrl = DefaultRedirect)
        {
            string redirectUrl = Url.Action(
                nameof(ExternalLoginCallback),
                "auth",
                new { Provider = provider, ReturnUrl = returnUrl });
            
            AuthenticationProperties properties = _signInManager
                .ConfigureExternalAuthenticationProperties(provider, redirectUrl);

            return new ChallengeResult(provider, properties);
        }

        [HttpGet("externalLoginCallback")]
        public async Task<IActionResult> ExternalLoginCallback(
            string provider,
            string returnUrl = DefaultRedirect)
        {  
            ExternalLoginInfo info = await _signInManager.GetExternalLoginInfoAsync(provider, provider);
            (var token, var secret) = info.AuthenticationTokens.ExtractTokens();

            ApplicationUser appUser = await GetTwitterUser(token, secret);
            
            bool userExists = await _userManager.Users
                .AsAsyncEnumerable()
                .AnyAsync(user => appUser.TwitterId == user.TwitterId);
            
            if (!userExists)
            {
                await _userManager.RegisterUserAsync(
                    appUser,
                    info,
                    token,
                    secret);
            }
            
            await _signInManager.SignInAsync(
                appUser,
                new AuthenticationProperties
                {
                    RedirectUri = returnUrl
                });
            
            return Redirect(returnUrl);
        }

        private async Task<ApplicationUser> GetTwitterUser(
            AuthenticationToken accessToken,
            AuthenticationToken accessTokenSecret)
        {
            ISocialMediaProvider twitter = GetTwitterProvider(accessToken, accessTokenSecret);

            IUser user = await twitter.GetIdentityAsync();
            return new ApplicationUser(user);
        }

        private ISocialMediaProvider GetTwitterProvider(AuthenticationToken accessToken, AuthenticationToken accessTokenSecret)
        {
            ISocialMediaProvider twitter = new TwitterProvider(
                _twitterConfiguration.ConsumerKey,
                _twitterConfiguration.ConsumerSecret,
                accessToken.Value,
                accessTokenSecret.Value);
            return twitter;
        }


        [HttpGet("logout")]
        public async Task<IActionResult> Logout(string provider)
        {
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);

            return Redirect(DefaultRedirect);
        }
    }
}