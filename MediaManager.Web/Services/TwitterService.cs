using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Extensions.Hosting.AsyncInitialization;
using MediaManager.Api;
using MediaManager.Twitter;
using MediaManager.Web.Data;
using MediaManager.Web.Data.Entities;
using MediaManager.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace MediaManager.Web.Services
{
    public class TwitterService : IAsyncInitializer
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly TwitterAppConfiguration _configuration;
        
        private readonly IPostsWatcher _postsWatcher;
        private readonly IPostsChecker _postsChecker;
        private readonly IProvidersOperator _providersOperator;
        private readonly IPostOperationValidator _postOperationValidator;

        public TwitterService(
            IServiceScopeFactory scopeFactory,
            TwitterAppConfiguration configuration,
            
            IPostsWatcher postsWatcher,
            IPostsChecker postsChecker,
            IProvidersOperator providersOperator,
            IPostOperationValidator postOperationValidator)
        {
            _scopeFactory = scopeFactory;
            _configuration = configuration;
            
            _postsWatcher = postsWatcher;
            _postsChecker = postsChecker;
            _providersOperator = providersOperator;
            _postOperationValidator = postOperationValidator;
        }
        
        public async Task InitializeAsync()
        {
            using (IServiceScope scope = _scopeFactory.CreateScope())
            {
                ApplicationDbContext db = scope.GetDatabase();

                await db.Database.EnsureCreatedAsync();
                
                await db.Users
                    .ToAsyncEnumerable()
                    .ForEachAwaitAsync(Login);

                if (db.OperatedPosts != null)
                {
                    await db.OperatedPosts
                        .ToAsyncEnumerable()
                        .ForEachAsync(
                            post => _postOperationValidator.UserOperatedOnPost(post.PostId, post.UserId));
                }

                if (db.WatchedUsers != null)
                {
                    var watchedUsers = await db.WatchedUsers.ToListAsync();
                    _postsChecker
                        .WatchedUsers
                        .AddRange(watchedUsers);
                }
            }
            
            _postOperationValidator
                .OnUserOperatedOnPost
                .SubscribeAsync(OnUserOperatedOnPost);
            
            _postsWatcher.StartWatch();
        }

        private async Task OnUserOperatedOnPost((long postId, long userId) ids)
        {
            (long postId, long userId) = ids;
            
            using IServiceScope scope = _scopeFactory.CreateScope();
            ApplicationDbContext db = scope.GetDatabase();

            await db.OperatedPosts
                .AddAsync(
                    new OperatedPost
                    {
                        PostId = postId,
                        UserId = userId
                    });

            await db.SaveChangesAsync();
        }

        public async Task Login(ApplicationUser user)
        {
            try
            {
                ISocialMediaProvider provider = await UserToTwitter(user);
                AddProvider(provider);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private void AddProvider(ISocialMediaProvider provider)
        {
            ConcurrentBag<ISocialMediaProvider> providers = _providersOperator.Providers;
            if (!providers.Contains(provider))
            {
                providers.Add(provider);
            }
        }

        public async Task<ISocialMediaProvider> UserToTwitter(ApplicationUser applicationUser)
        {
            using IServiceScope scope = _scopeFactory.CreateScope();

            IAsyncEnumerable<IdentityUserToken<long>> asyncTokens = scope.GetDatabase().UserTokens.ToAsyncEnumerable();

            IdentityUserToken<long> token = await FindTokenAsync(asyncTokens, applicationUser, "access_token");
            IdentityUserToken<long> secret = await FindTokenAsync(asyncTokens, applicationUser, "access_token_secret");

            return new TwitterProvider(
                _configuration.ConsumerKey,
                _configuration.ConsumerSecret,
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