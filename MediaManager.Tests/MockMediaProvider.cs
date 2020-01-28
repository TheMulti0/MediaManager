using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using MediaManager.Api;

namespace MediaManager.Tests
{
    public class MockMediaProvider : ISocialMediaProvider
    {
        private readonly Subject<string> _onNewPost = new Subject<string>();
        public IObservable<string> OnNewPost => _onNewPost;
        
        private readonly Subject<IPost> _onPostDelete = new Subject<IPost>();
        public IObservable<IPost> OnPostDelete => _onPostDelete;
        
        private readonly Subject<IPost> _onPostLike = new Subject<IPost>();
        public IObservable<IPost> OnPostLike => _onPostLike;
        
        private readonly Subject<IPost> _onPostUnlike = new Subject<IPost>();
        public IObservable<IPost> OnPostUnlike => _onPostUnlike;

        private readonly int _postBatchCount;
        
        private int _postIndex = 0;

        public MockMediaProvider(int postBatchCount)
        {
            _postBatchCount = postBatchCount;
        }
        
        public Task<IUser> GetIdentityAsync() 
            => Task.FromResult<IUser>(new MockUser());

        public Task<IUser> GetUserAsync(long userId) 
            => Task.FromResult<IUser>(new MockUser { Id = userId });

        public Task<IUser> GetUserAsync(string name) 
            => Task.FromResult<IUser>(new MockUser { Name = name });

        public Task<IPost> FindPostAsync(long postId) 
            => Task.FromResult<IPost>(new MockPost { Id = postId });

        public IAsyncEnumerable<IPost> FindPostsAsync(string query)
        {
            return AsyncEnumerable
                .Range(0, _postBatchCount)
                .Select(_ => new MockPost { Id = _postIndex++ } );
        }

        public IAsyncEnumerable<IPost> FindPostsAsync(IUser author)
        {
            return AsyncEnumerable
                .Range(0, _postBatchCount)
                .Select(_ => new MockPost { Id = _postIndex++, Author = author});
        }

        public IAsyncEnumerable<IPost> FindPostsAsync(IUser author, string query)
        {
            return AsyncEnumerable
                .Range(0, _postBatchCount)
                .Select(_ => new MockPost { Id = _postIndex++, Author = author });
        }

        public Task<IPost> PostAsync(string description)
        {
            _onNewPost.OnNext(description);
            return Task.FromResult<IPost>(new MockPost());
        }

        public Task DeleteAsync(IPost post)
        {
            _onPostDelete.OnNext(post);
            return Task.CompletedTask;
        }

        public Task LikeAsync(IPost post)
        {
            _onPostLike.OnNext(post);
            return Task.CompletedTask;
        }

        public Task UnlikeAsync(IPost post)
        {
            _onPostUnlike.OnNext(post);
            return Task.CompletedTask;
        }
    }
}