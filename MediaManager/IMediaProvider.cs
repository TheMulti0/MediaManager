using System.Collections.Generic;
using System.Threading.Tasks;

namespace MediaManager
{
    public interface IMediaProvider
    {
        Task<MediaUser> GetIdentityAsync();

        IAsyncEnumerable<Post> FindPostsAsync(string query);

        IAsyncEnumerable<Post> FindPostsAsync(MediaUser author);
        

        IAsyncEnumerable<Post> FindPostsAsync(MediaUser author, string query);
    }
} 