using System.Linq;
using System.Threading.Tasks;
using MediaManager.Api;
using MediaManager.Twitter;
using MediaManager.Web.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace MediaManager.Web.Controllers
{
    [Route("[controller]")]
    public class AuthController : Controller
    {
        private const string DefaultRedirect = "/";

        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole<long>> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly TwitterAppConfiguration _twitterConfiguration;
        private readonly string[] _adminUsers;

        public AuthController(
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole<long>> roleManager,
            TwitterAppConfiguration twitterConfiguration,
            RolesConfiguration roles)
        {
            _signInManager = signInManager;
            _roleManager = roleManager;
            _userManager = signInManager.UserManager;
            _twitterConfiguration = twitterConfiguration;
            _adminUsers = roles.AdminUsers;
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

            ApplicationUser user = await GetTwitterUser(token, secret);
            
            ApplicationUser existingUser = await _userManager.Users
                .FirstOrDefaultAsync(u => u.Id == user.Id);
            
            if (existingUser == null)
            {
                await RegisterUser(user, info, token, secret);

                await SignIn(user, returnUrl);
            }
            else
            {
                await SignIn(existingUser, returnUrl);
            }

            return Redirect(returnUrl);
        }

        private async Task RegisterUser(
            ApplicationUser user,
            ExternalLoginInfo info,
            AuthenticationToken token,
            AuthenticationToken secret)
        {
            await _userManager.RegisterUserAsync(
                user,
                info,
                token,
                secret);

            if (_adminUsers?.Contains(user.UserName) ?? false)
            {
                const string role = "Admin";

                await CreateRole(role);
                await _userManager.AddToRoleAsync(user, role);
            }
        }

        private async Task SignIn(ApplicationUser user, string returnUrl) => await _signInManager.SignInAsync(
            user,
            new AuthenticationProperties
            {
                RedirectUri = returnUrl
            });

        private async Task CreateRole(string name)
        {
            if (!await _roleManager.RoleExistsAsync(name))
            {
                await _roleManager.CreateAsync(new IdentityRole<long>(name));
            }
        }

        private async Task<ApplicationUser> GetTwitterUser(
            AuthenticationToken accessToken,
            AuthenticationToken accessTokenSecret)
        {
            ISocialMediaProvider twitter = GetTwitterProvider(accessToken, accessTokenSecret);

            IUser user = await twitter.GetIdentityAsync();
            return new ApplicationUser(user);
        }

        private ISocialMediaProvider GetTwitterProvider(
            AuthenticationToken accessToken,
            AuthenticationToken accessTokenSecret)
        {
            return new TwitterProvider(
                _twitterConfiguration.ConsumerKey,
                _twitterConfiguration.ConsumerSecret,
                accessToken.Value,
                accessTokenSecret.Value);
        }


        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);

            return Redirect(DefaultRedirect);
        }
    }
}