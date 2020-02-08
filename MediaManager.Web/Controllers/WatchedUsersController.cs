using System;
using System.Linq;
using System.Threading.Tasks;
using MediaManager.Api;
using MediaManager.Web.Data;
using MediaManager.Web.Models;
using MediaManager.Web.Views.WatchedUsers;
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
        public IActionResult Index(IndexViewModel vm)
        {
            return View(vm);
        }        

        [HttpPost]
        public async Task<IActionResult> Index(string userName)
        {
            IUser user;
            try
            {
                user = await GetUser(userName) ?? throw new NullReferenceException();
            }
            catch (NullReferenceException)
            {
                return SendFailure(userName);
            }

            return await AddUser(user, userName);
        }

        private async Task<IActionResult> AddUser(IUser user, string userName)
        {
            var watchedUsers = _mediaManager.PostsChecker.WatchedUsers;
            if (watchedUsers.Any(u => u.Id == user.Id))
            {
                return SendFailure(userName);
            }

            watchedUsers.Add(user);
            await AddWatchedUserToDb(user);

            return Index(new IndexViewModel
            {
                PreviousSucceeded = true,
                UserName = userName
            });
        }

        private IActionResult SendFailure(string userName)
        {
            return Index(new IndexViewModel()
            {
                PreviousSucceeded = false,
                UserName = userName
            });
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

            if (db.WatchedUsers != null)
            {
                await db.WatchedUsers.AddAsync(new WatchedUser(user));
            }

            await db.SaveChangesAsync();
        }
    }
}