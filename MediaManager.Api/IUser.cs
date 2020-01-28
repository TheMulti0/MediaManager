using System;

namespace MediaManager.Api
{
    public interface IUser : IEquatable<IUser>
    {
        long Id { get; }

        string Name { get; }

        string DisplayName { get; }

        string Url { get; }
    }
}