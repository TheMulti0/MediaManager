using System;

namespace MediaManager.Api
{
    public class PostsSearchQuery
    {
        public string? Query { get; }

        public IUser? Author { get; }

        public int MaximumResults { get; set; } = 40;

        public DateTime? Since { get; set; } = null;

        public PostsSearchQuery(string query)
        {
            Query = query;
        }

        public PostsSearchQuery(IUser author)
        {
            Author = author;
        }

        public PostsSearchQuery(string query, IUser author)
        {
            Query = query;
            Author = author;
        }
    }
}