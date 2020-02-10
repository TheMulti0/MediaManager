using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediaManager.Api;
using MediaManager.Web.Data;
using MediaManager.Web.Data.Entities;
using MediaManager.Web.Services;
using MediaManager.Web.Views.WatchedUsers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace MediaManager.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class WatchedUsersController : Controller
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly TwitterService _twitter;
        
        private readonly IPostsWatcher _postsWatcher;
        private readonly IPostsChecker _postsChecker;

        public WatchedUsersController(
            IServiceScopeFactory scopeFactory,
            UserManager<ApplicationUser> userManager,
            TwitterService twitter,
            
            IPostsWatcher postsWatcher,
            IPostsChecker postsChecker)
        {
            _scopeFactory = scopeFactory;
            _userManager = userManager;
            _twitter = twitter;

            _postsWatcher = postsWatcher;
            _postsChecker = postsChecker;
        }

        [HttpGet]
        public IActionResult Index(IndexViewModel vm)
        {
            return View(vm);
        }
        
        [HttpPost]
        public async Task<IActionResult> Index(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                return SendFailure("Please enter a username!");
            }
            
            IUser user;
            try
            {
                user = await GetUser(userName) ?? throw new NullReferenceException();
            }
            catch (NullReferenceException)
            {
                return SendFailure($"Cannot find Twitter username '@{userName}'");
            }

            return await AddUser(user);
        }

        [HttpPost]
        public async Task<IActionResult> Remove(string userName, string returnUrl)
        {
            List<IUser> watchedUsers = _postsChecker.WatchedUsers;
            
            IUser user = watchedUsers.FirstOrDefault(u => u.Name == userName);
            if (user != null)
            {
                watchedUsers.Remove(user);
                
                await RemoveWatchedUserFromDb(user);
            }

            return Redirect(returnUrl);
        }

        [HttpGet]
        public IActionResult CheckManually(string returnUrl)
        {
            _postsWatcher.StartWatch();

            return Redirect(returnUrl);
        }

        private async Task<IActionResult> AddUser(IUser user)
        {
            List<IUser> watchedUsers = _postsChecker.WatchedUsers;
            if (watchedUsers.Any(u => u.Id == user.Id))
            {
                return SendFailure($"User '@{user.Name}' is already being watched!");
            }

            watchedUsers.Add(user);
            await AddWatchedUserToDb(user);

            return Index(new IndexViewModel
            {
                PreviousSucceeded = true,
                Message = "Success!"
            });
        }

        private IActionResult SendFailure(string message)
        {
            return Index(new IndexViewModel()
            {
                PreviousSucceeded = false,
                Message = message
            });
        }

        private async Task<IUser> GetUser(string userName)
        {
            ApplicationUser currentUser = await _userManager.GetUserAsync(User);
            ISocialMediaProvider provider = await _twitter.UserToTwitter(currentUser);

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
        
        private async Task RemoveWatchedUserFromDb(IUser user)
        {
            using IServiceScope scope = _scopeFactory.CreateScope();
            ApplicationDbContext db = scope.GetDatabase();

            if (db.WatchedUsers != null)
            {
                WatchedUser watchedUser = await db.WatchedUsers.FirstOrDefaultAsync(u => u.Name == user.Name);
                
                db.WatchedUsers.Remove(watchedUser);
            }

            await db.SaveChangesAsync();
        }
    }
}