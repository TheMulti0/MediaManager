using System.Collections.Generic;
using System.Threading.Tasks;

namespace MediaManager
{
    public interface ISocialMedia
    {
        Task<User> GetIdentityAsync();

        IAsyncEnumerable<Post> FindPostsAsync(string query);

        IAsyncEnumerable<Post> FindPostsAsync(User author);
        
        IAsyncEnumerable<Post> FindPostsAsync(User author, string query);

        Task<Post> PostAsync(string content);

        Task DeleteAsync(Post post);

        Task LikeAsync(Post post);

        Task UnlikeAsync(Post post);
    }
} 