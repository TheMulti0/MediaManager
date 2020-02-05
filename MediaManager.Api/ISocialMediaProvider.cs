using System.Collections.Generic;
using System.Threading.Tasks;

namespace MediaManager.Api
{
    public interface ISocialMediaProvider
    {
        Task<IUser> GetIdentityAsync();

        Task<IUser> GetUserAsync(long userId);

        Task<IUser> GetUserAsync(string name);

        Task<IPost> FindPostAsync(long postId);

        IAsyncEnumerable<IPost> FindPostsAsync(PostsSearchQuery query);
        
        Task<IPost> PostAsync(string description);

        Task DeleteAsync(IPost post);

        Task LikeAsync(IPost post);

        Task UnlikeAsync(IPost post);
    }
} 