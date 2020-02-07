using MediaManager.Web.Data;
using Microsoft.Extensions.DependencyInjection;

namespace MediaManager.Web
{
    public static class ScopeFactoryExtensions
    {
        public static ApplicationDbContext GetDatabase(this IServiceScope scope) 
            => scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    }
}
