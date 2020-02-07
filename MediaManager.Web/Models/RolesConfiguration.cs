using Microsoft.Extensions.Configuration;

namespace MediaManager.Web.Models
{
    public class RolesConfiguration
    {
        public string[] AdminUsers { get; set; }

        public RolesConfiguration(IConfiguration configuration)
        {
            AdminUsers = configuration.GetSection("Roles:AdminUsers").Get<string[]>();
        }
    }
}