using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public class TwitterController : Controller
    {
        private readonly TwitterAppConfiguration _twitterConfiguration;
        private readonly ApplicationDbContext _database;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly UserWatcher _userWatcher;
        
        public TwitterController(
            TwitterAppConfiguration twitterConfiguration, 
            ApplicationDbContext database,
            UserManager<ApplicationUser> userManager)
        {
            _twitterConfiguration = twitterConfiguration;
            _database = database;
            _userManager = userManager;
            _userWatcher = new UserWatcher(new List<IUser>(), TimeSpan.FromSeconds(5));
        }

        public async Task Login()
        {
            ApplicationUser applicationUser = await _userManager.GetUserAsync(User);
            (IUser user, ISocialMediaProvider provider) = await UserToTwitter(applicationUser);
            
            _userWatcher.Add(user, provider);
        }

        private async Task<(IUser user, ISocialMediaProvider provider)> UserToTwitter(ApplicationUser applicationUser)
        {
            IAsyncEnumerable<IdentityUserToken<long>> asyncTokens = _database.UserTokens.ToAsyncEnumerable();

            IdentityUserToken<long> token = await FindTokenAsync(asyncTokens, applicationUser, "access_token");
            IdentityUserToken<long> secret = await FindTokenAsync(asyncTokens, applicationUser, "access_token_secret");

            ISocialMediaProvider twitter = new TwitterProvider(
                _twitterConfiguration.ConsumerKey,
                _twitterConfiguration.ConsumerSecret,
                token.Value,
                secret.Value);

            IUser user = await twitter.GetIdentityAsync();

            return (user, twitter);
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