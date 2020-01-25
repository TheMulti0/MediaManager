using System;

namespace MediaManager
{
    public class User : IEquatable<User>
    {
        public long Id { get; }

        public string Name { get; }

        public string DisplayName { get; }

        public string Url { get; }

        public User(
            long id,
            string name,
            string displayName,
            string url)
        {
            Id = id;
            Name = name;
            DisplayName = displayName;
            Url = url;
        }

        public bool Equals(User? other)
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
            return Equals((User) obj);
        }

        public override int GetHashCode() => Id.GetHashCode();

        public static bool operator ==(User? left, User? right) => Equals(left, right);

        public static bool operator !=(User? left, User? right) => !Equals(left, right);
    }
}