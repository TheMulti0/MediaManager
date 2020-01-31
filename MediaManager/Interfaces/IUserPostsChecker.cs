using System.Collections.Generic;
using MediaManager.Api;

namespace MediaManager
{
    public interface IUserPostsChecker
    {
        IEnumerable<IUser> WatchedUsers { get; }

        void CheckAllUsers();
    }
}