using System.Threading.Tasks;
using MediaManager.Api;
using MediaManager.Web.Data;
using MediaManager.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace MediaManager.Web.Controllers
{
    public class WatchedUsersController : Controller
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly TwitterService _twitter;
        private readonly IMediaManager _mediaManager;

        public WatchedUsersController(
            IServiceScopeFactory scopeFactory,
            UserManager<ApplicationUser> userManager,
            TwitterService twitter, 
            IMediaManager mediaManager)
        {
            _scopeFactory = scopeFactory;
            _userManager = userManager;
            _twitter = twitter;
            _mediaManager = mediaManager;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Post(string userName)
        {
            var user = await GetUser(userName);
            if (user == null)
            {
                return Redirect("");
            }

            _mediaManager.PostsChecker.WatchedUsers.Add(user);
            await AddWatchedUserToDb(user);

            return RedirectToAction(nameof(Index));
        }

        private async Task<IUser> GetUser(string userName)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var provider = await _twitter.UserToTwitter(currentUser);

            return await provider.GetUserAsync(userName);
        }

        private async Task AddWatchedUserToDb(IUser user)
        {
            using IServiceScope scope = _scopeFactory.CreateScope();
            ApplicationDbContext db = scope.GetDatabase();

            await db.WatchedUsers.AddAsync(new WatchedUser(user));
            await db.SaveChangesAsync();
        }
    }
}