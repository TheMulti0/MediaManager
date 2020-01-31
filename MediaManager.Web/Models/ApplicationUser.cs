using MediaManager.Api;
using Microsoft.AspNetCore.Identity;

namespace MediaManager.Web.Models
{
    public sealed class ApplicationUser : IdentityUser<long>
    {
        public long TwitterId { get; set; }

        public string DisplayName { get; set; }

        public string Url { get; set; }

        public ApplicationUser()
        {
        }

        public ApplicationUser(IUser user)
        {
            TwitterId = user.Id;
            UserName = user.Name;
            DisplayName = user.DisplayName;
            Url = user.Url;
        }

        public bool Equals(IUser other) => TwitterId == other?.Id;
    }
}