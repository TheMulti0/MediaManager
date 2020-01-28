using System;
using MediaManager.Api;
using Tweetinvi.Models;
using IUser = MediaManager.Api.IUser;

namespace MediaManager.Twitter
{
    public class TwitterPost : IPost
    {
        public long Id { get; }
        
        public string Message { get; }
        
        public IUser Author { get; }
        
        public DateTime CreatedAt { get; }
        
        public string Url { get; }

        public TwitterPost(ITweet tweet)
        {
            Id = tweet.Id;
            Message = tweet.Text;
            Author = new TwitterUser(tweet.CreatedBy);
            CreatedAt = tweet.CreatedAt;
            Url = tweet.Url;
        }

        public bool Equals(IPost other) => Id == other?.Id;

    }
}