using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Extensions.Hosting.AsyncInitialization;
using MediaManager.Api;
using MediaManager.Extensions;
using MediaManager.Twitter;
using MediaManager.Web.Data;
using MediaManager.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.DependencyInjection;
using Tweetinvi;

namespace MediaManager.Web
{
    public class TwitterService : IAsyncInitializer
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly TwitterAppConfiguration _configuration;
        private readonly IMediaManager _mediaManager;
        
        public TwitterService(
            IServiceScopeFactory scopeFactory,
            TwitterAppConfiguration configuration,
            IMediaManager mediaManager)
        {
            _scopeFactory = scopeFactory;
            _configuration = configuration;
            _mediaManager = mediaManager;
        }
        
        public async Task InitializeAsync()
        {
            using (IServiceScope scope = _scopeFactory.CreateScope())
            {
                ApplicationDbContext db = GetDatabase(scope);

                await db.Database.EnsureCreatedAsync();
                
                await db.Users
                    .ToAsyncEnumerable()
                    .ForEachAwaitAsync(Login);
            }
            
            _mediaManager.Validator
                .OnUserOperatedOnPost
                .SubscribeAsync(OnUserOperatedOnPost);
            
            _mediaManager.BeginUserPostWatch();
        }

        private async Task OnUserOperatedOnPost((long postId, long userId) ids)
        {
            (long postId, long userId) = ids;
            
            using IServiceScope scope = _scopeFactory.CreateScope();
            ApplicationDbContext db = GetDatabase(scope);

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
            ConcurrentBag<ISocialMediaProvider> providers = _mediaManager.Operator.Providers;
            if (!providers.Contains(provider))
            {
                providers.Add(provider);
            }
        }

        private async Task<ISocialMediaProvider> UserToTwitter(ApplicationUser applicationUser)
        {
            using IServiceScope scope = _scopeFactory.CreateScope();

            IAsyncEnumerable<IdentityUserToken<long>> asyncTokens = GetDatabase(scope).UserTokens.ToAsyncEnumerable();

            IdentityUserToken<long> token = await FindTokenAsync(asyncTokens, applicationUser, "access_token");
            IdentityUserToken<long> secret = await FindTokenAsync(asyncTokens, applicationUser, "access_token_secret");

            return new TwitterProvider(
                _configuration.ConsumerKey,
                _configuration.ConsumerSecret,
                token.Value,
                secret.Value);
        }

        private static ApplicationDbContext GetDatabase(IServiceScope scope) => scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

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