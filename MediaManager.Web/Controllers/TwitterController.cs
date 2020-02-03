using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Extensions.Hosting.AsyncInitialization;
using MediaManager.Api;
using MediaManager.Twitter;
using MediaManager.Web.Data;
using MediaManager.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MediaManager.Web.Controllers
{
    [Authorize]
    public class TwitterController : Controller, IAsyncInitializer
    {
        private readonly TwitterAppConfiguration _twitterConfiguration;
        private readonly ApplicationDbContext _database;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMediaManager _mediaManager;
        
        public TwitterController(
            TwitterAppConfiguration twitterConfiguration, 
            ApplicationDbContext database,
            UserManager<ApplicationUser> userManager,
            IMediaManager mediaManager)
        {
            _twitterConfiguration = twitterConfiguration;
            _database = database;
            _userManager = userManager;
            _mediaManager = mediaManager;
        }
        
        public async Task InitializeAsync()
        {
            await _database.Database.EnsureCreatedAsync();
            await _database.Users.ForEachAwaitAsync(Login);
        }

        public async Task<IActionResult> Login(string returnUrl = "/")
        {
            ApplicationUser applicationUser = await _userManager.GetUserAsync(User);
            await Login(applicationUser);
            
            _mediaManager.BeginUserPostWatch(TimeSpan.FromSeconds(2));

            return Redirect(returnUrl);
        }

        private async Task Login(ApplicationUser applicationUser)
        {
            ISocialMediaProvider provider = await UserToTwitter(applicationUser);
            ConcurrentBag<ISocialMediaProvider> providers = _mediaManager.Operator.Providers;
            if (!providers.Contains(provider))
            {
                providers.Add(provider);
            }
        }

        private async Task<ISocialMediaProvider> UserToTwitter(ApplicationUser applicationUser)
        {
            IAsyncEnumerable<IdentityUserToken<long>> asyncTokens = _database.UserTokens.ToAsyncEnumerable();

            IdentityUserToken<long> token = await FindTokenAsync(asyncTokens, applicationUser, "access_token");
            IdentityUserToken<long> secret = await FindTokenAsync(asyncTokens, applicationUser, "access_token_secret");

            return new TwitterProvider(
                _twitterConfiguration.ConsumerKey,
                _twitterConfiguration.ConsumerSecret,
                token.Value,
                secret.Value);
        }

        private static ValueTask<IdentityUserToken<long>> FindTokenAsync(
            IAsyncEnumerable<IdentityUserToken<long>> tokens,
            ApplicationUser user,
            string name)
        {
            return tokens
                .FirstOrDefaultAsync(token => 
                                         token.UserId == user.Id &&
                                         token.Name == name);
        }

    }
}