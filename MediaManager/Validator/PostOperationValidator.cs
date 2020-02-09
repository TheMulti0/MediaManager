using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;

namespace MediaManager
{
    public class PostOperationValidator : IPostOperationValidator
    {
        private readonly ConcurrentDictionary<long, List<long>> _operatedPosts;
        private readonly Subject<(long postId, long userId)> _onUserOperatedOnPost;

        public IObservable<(long postId, long userId)> OnUserOperatedOnPost => _onUserOperatedOnPost;

        public PostOperationValidator()
        {
            _operatedPosts = new ConcurrentDictionary<long, List<long>>();
            _onUserOperatedOnPost = new Subject<(long postId, long userId)>();
        }


        public bool HasUserOperatedOnPost(long postId, long userId)
        {
            return _operatedPosts.ContainsKey(postId) && _operatedPosts[postId].Any(uid => uid == userId);
        }

        public void UserOperatedOnPost(long postId, long userId)
        {
            _onUserOperatedOnPost.OnNext((postId, userId));
            
            if (_operatedPosts.ContainsKey(postId))
            {
                _operatedPosts[postId].Add(userId);
            }
            else
            {
                AddPostAndUser(postId, userId);
            }
        }

        private void AddPostAndUser(long postId, long userId)
        {
            bool tryAdd = _operatedPosts.TryAdd(
                postId,
                new List<long>
                {
                    userId
                });

            if (!tryAdd)
            {
                throw new InvalidOperationException($"Cannot add user #{userId} to post #{postId}");
            }
        }
    }
}