namespace MediaManager
{
    public class Post
    {
        public string Message { get; }

        public MediaUser Author { get; }
        
        public Post(string message, MediaUser author)
        {
            Message = message;
            Author = author;
        }
    }
}