using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediaManager.Api;

namespace MediaManager
{
    public interface IPostsChecker
    {
        List<IUser> WatchedUsers { get; }

        Task CheckAllUsersAsync(DateTime postsSince);
    }
}