using System.Collections.Generic;
using System.Threading.Tasks;
using MediaManager.Api;

namespace MediaManager
{
    public interface IUserPostsChecker
    {
        List<IUser> WatchedUsers { get; }

        Task CheckAllUsersAsync();
    }
}