using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace MediaManager
{
    public class PostOperationValidator : IPostOperationValidator
    {
        private readonly ConcurrentDictionary<long, List<long>> _operatedPosts;

        public PostOperationValidator()
        {
            _operatedPosts = new ConcurrentDictionary<long, List<long>>();
        }

        public bool HasUserOperatedOnPost(long postId, long userId)
        {
            return _operatedPosts.ContainsKey(postId) && _operatedPosts[postId].Any(uid => uid == userId);
        }

        public void UserOperatedOnPost(long postId, long userId)
        {
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