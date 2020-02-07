using MediaManager.Api;

namespace MediaManager.Web.Models
{
    public class WatchedUser : IUser
    {
        public long Id { get; set; } = 0;

        public string Name { get; set; } = "";

        public string DisplayName { get; set; } = "";

        public string Url { get; set; } = "";

        public WatchedUser()
        {
        }

        public WatchedUser(IUser user)
        {
            Id = user.Id;
            Name = user.Name;
            DisplayName = user.DisplayName;
            Url = user.Url;
        }
        
        public bool Equals(IUser other) => Id == other?.Id;
    }
}