using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using MediaManager.Api;
using OperationCanceledException = System.OperationCanceledException;

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

        public Task CheckAllUsersAsync()
        {
            IEnumerable<Task> tasks = WatchedUsers.Select(CheckUser);
            return Task.WhenAll(tasks);
        }

        private Task CheckUser(IUser user)
        {
            return _operator
                .OperateOnAllAsync(
                    provider => CheckNewPosts(user, provider));
        }

        private async Task CheckNewPosts(
            IUser watchedUser,
            ISocialMediaProvider provider)
        {
            IUser currentUser = await provider.GetIdentityAsync();

            await foreach (IPost post in provider.FindPostsAsync(watchedUser))
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