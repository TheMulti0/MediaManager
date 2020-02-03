using MediaManager.Api;
using Microsoft.AspNetCore.Identity;

namespace MediaManager.Web.Models
{
    public sealed class ApplicationUser : IdentityUser<long>
    {
        public IUser User { get; }
        
        public long TwitterId { get; }

        public string DisplayName { get; }

        public string Url { get; }

        public ApplicationUser()
        {
        }

        public ApplicationUser(IUser user)
        {
            User = user;
            
            TwitterId = user.Id;
            UserName = user.Name;
            DisplayName = user.DisplayName;
            Url = user.Url;
        }

        public bool Equals(IUser other) => TwitterId == other?.Id;
    }
}