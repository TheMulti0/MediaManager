using Tweetinvi.Models;

namespace MediaManager
{
    public static class TwitterExtensions
    {
        public static User ToMediaUser(this IUser user) =>
            new User(
                user.Id,
                user.ScreenName,
                user.Name,
                user.Url);

        public static Post ToPost(this ITweet tweet) =>
            new Post(
                tweet.Id,
                tweet.Text,
                tweet.CreatedBy.ToMediaUser(),
                tweet.CreatedAt,
                tweet.Url);
    }
}