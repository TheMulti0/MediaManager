using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using MediaManager.Api;
using MediaManager.Extensions;

namespace MediaManager
{
    public class UsersManager
    {
        private readonly List<IUser> _watchedUsers;
        private readonly Dictionary<IUser, ISocialMediaProvider> _providers;
        private readonly ConcurrentDictionary<IPost, List<IUser>> _operatedItems;

        public UsersManager(
            List<IUser> watchedUsers,
            Dictionary<IUser, ISocialMediaProvider> providers,
            TimeSpan checkInterval)
        {
            _watchedUsers = watchedUsers;
            _providers = providers;
            _operatedItems = new ConcurrentDictionary<IPost, List<IUser>>();

            Observable
                .Interval(checkInterval)
                .Subscribe(l => OperateOnWatchedUsers());
        }

        private void OperateOnWatchedUsers()
        {
            foreach (IUser watchedUser in _watchedUsers)
            {
                _providers
                    .AsParallel()
                    .ForAll(async pair => await Operate(watchedUser, pair.Key, pair.Value));
            }
        }

        private async Task Operate(
            IUser watchedUser,
            IUser currentUser,
            ISocialMediaProvider provider)
        {
            IAsyncEnumerable<Task> uniqueOperations = MapUnique(
                provider.FindPostsAsync(watchedUser),
                _operatedItems,
                currentUser,
                provider.LikeAsync);

            await foreach (Task operation in uniqueOperations)
            {
                await operation;
            }
        }

        private static IAsyncEnumerable<Task> MapUnique<T, U>(
            IAsyncEnumerable<T> source,
            IDictionary<T, List<U>> duplicateCheckMap,
            U identifier,
            Func<T, Task> operation)
        {
            bool IsUnique(T item)
            {
                if (duplicateCheckMap.ContainsKey(item))
                {
                    return !duplicateCheckMap[item].Contains(identifier);
                }

                duplicateCheckMap.Add(item, new List<U>());
                return true;
            }

            Task Operate(T item)
            {
                Task task = operation(item);
                duplicateCheckMap[item].Add(identifier);

                return task;
            }

            return source
                .Where(IsUnique)
                .Select(Operate);
        }
    }
}