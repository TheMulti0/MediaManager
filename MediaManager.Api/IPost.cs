using System;

namespace MediaManager.Api
{
    public interface IPost : IEquatable<IPost>
    {
        long Id { get; }

        string Message { get; }

        IUser Author { get; }

        DateTime CreatedAt { get; }

        string Url { get; }
    }
}