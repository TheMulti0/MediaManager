using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace MediaManager.Web
{
    public static class AuthenticationExtensions
    {
        public static async Task CreateTicketAsync<T>(ResultContext<T> context) 
            where T : AuthenticationSchemeOptions
        {
            var claim = new Claim("Tokens", "");
            foreach (KeyValuePair<string, string> token in context.Properties.Items)
            {
                claim.Properties.Add(token);
            }
            var identity = context.Principal?.Identity as ClaimsIdentity;
            identity?.AddClaim(claim);

            await context.HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                context.Principal);
        }
    }
}