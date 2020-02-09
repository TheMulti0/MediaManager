using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediaManager.Api;

namespace MediaManager
{
    public class UserPostsChecker : IUserPostsChecker
    {
        private readonly IPostOperationValidator _validator;
        private readonly IProvidersOperator _operator;

        public List<IUser> WatchedUsers { get; }

        public UserPostsChecker(
            IPostOperationValidator validator,
            IProvidersOperator @operator)
        {
            _validator = validator;
            _operator = @operator;

            WatchedUsers = new List<IUser>();
        }

        public Task CheckAllUsersAsync(DateTime postsSince)
        {
            try
            {
                IEnumerable<Task> tasks = WatchedUsers.Select(user => CheckUser(user, postsSince));
                return Task.WhenAll(tasks);
            }
            catch (Exception)
            {
                return Task.CompletedTask;
            }
        }

        private Task CheckUser(IUser user, DateTime postsSince)
        {
            return _operator
                .OperateOnAllAsync(
                    provider => CheckNewPosts(user, provider, postsSince));
        }

        private async Task CheckNewPosts(
            IUser watchedUser,
            ISocialMediaProvider provider,
            DateTime postsSince)
        {
            IUser currentUser = await provider.GetIdentityAsync();

            var query = new PostsSearchQuery(watchedUser)
            {
                Since = postsSince
            };
            await foreach (IPost post in provider.FindPostsAsync(query))
            {
                await OperateOnPost(provider, post, currentUser);
            }
        }

        private async Task OperateOnPost(ISocialMediaProvider provider, IPost post, IUser currentUser)
        {
            bool hasUserOperatedOnPost = _validator.HasUserOperatedOnPost(post.Id, currentUser.Id);
            if (hasUserOperatedOnPost)
            {
                return;
            }

            await provider.LikeAsync(post);
            _validator.UserOperatedOnPost(post.Id, currentUser.Id);
        }
    }
}