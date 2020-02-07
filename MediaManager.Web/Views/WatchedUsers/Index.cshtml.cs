using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MediaManager.Web.Views.WatchedUsers
{
    [BindProperties]
    public class IndexModel : PageModel
    {
        public string UserName { get; set; }

        public void OnPost()
        {
            var userName = Request.Form["UserName"];
        }
    }
}