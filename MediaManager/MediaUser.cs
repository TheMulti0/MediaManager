using System;

namespace MediaManager
{
    public class MediaUser
    {
        public long Id { get; }
        
        public string Name { get; }

        public string DisplayName { get; }

        public MediaUser(long id, string name, string displayName)
        {
            Name = name;
            DisplayName = displayName;
            Id = id;
        }

        public static bool operator ==(MediaUser? left, MediaUser? right)
        {
            return left?.Id == right?.Id;
        }

        public static bool operator !=(MediaUser? left, MediaUser? right) => left?.Id != right?.Id;
    }
}