using Tweetinvi.Models;

namespace MediaManager.Twitter
{
    public static class TwitterExtensions
    {
        public static MediaUser ToMediaUser(this IUser user) =>
            new MediaUser(
                user.Id,
                user.ScreenName,
                user.Name);
    }
}