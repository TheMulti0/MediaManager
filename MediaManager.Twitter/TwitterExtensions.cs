using MediaManager.Api;
using Tweetinvi.Models;
using User = MediaManager.Api.User;

namespace MediaManager.Twitter
{
    public static class TwitterExtensions
    {
        public static User ToUser(this IUser user) =>
            new User(
                user.Id,
                user.ScreenName,
                user.Name,
                user.Url);

        public static Post ToPost(this ITweet tweet) =>
            new Post(
                tweet.Id,
                tweet.Text,
                tweet.CreatedBy.ToUser(),
                tweet.CreatedAt,
                tweet.Url);
    }
}