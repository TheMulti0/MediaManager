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

        public IEnumerable<IUser> WatchedUsers { get; }

        public UserPostsChecker(
            IEnumerable<IUser> watchedUsers,
            IPostOperationValidator validator,
            IProvidersOperator @operator)
        {
            _validator = validator;
            _operator = @operator;

            WatchedUsers = watchedUsers;
        }

        public void CheckAllUsers()
        {
            WatchedUsers.AsParallel().ForAll(CheckUser);
        }

        private void CheckUser(IUser user)
        {
            _operator.OperateOnAll(provider => CheckNewPosts(user, provider));
        }

        private async Task CheckNewPosts(
            IUser watchedUser,
            ISocialMediaProvider provider)
        {
            IUser currentUser = await provider.GetIdentityAsync();
            
            await foreach (IPost post in provider.FindPostsAsync(watchedUser))
            {
                bool hasUserOperatedOnPost = _validator.HasUserOperatedOnPost(post.Id, currentUser.Id);
                if (hasUserOperatedOnPost)
                {
                    continue;
                }
                
                await provider.LikeAsync(post);
                _validator.UserOperatedOnPost(post.Id, currentUser.Id);
            }
        }
    }
}