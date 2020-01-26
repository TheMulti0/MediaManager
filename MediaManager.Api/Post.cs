using System;

namespace MediaManager.Api
{
    public class Post : IEquatable<Post>
    {
        public long Id { get; }

        public string Message { get; }

        public User Author { get; }

        public DateTime CreatedAt { get; }

        public string Url { get; }

        public Post(
            long id,
            string message,
            User author,
            DateTime createdAt,
            string url)
        {
            Id = id;
            Message = message;
            Author = author;
            CreatedAt = createdAt;
            Url = url;
        }

        public bool Equals(Post? other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return Id == other.Id;
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != this.GetType())
            {
                return false;
            }
            return Equals((Post) obj);
        }

        public override int GetHashCode() => Id.GetHashCode();

        public static bool operator ==(Post? left, Post? right) => Equals(left, right);

        public static bool operator !=(Post? left, Post? right) => !Equals(left, right);
    }
}