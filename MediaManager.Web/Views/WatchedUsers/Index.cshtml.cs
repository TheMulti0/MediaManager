using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MediaManager.Web.Views.WatchedUsers
{
    public class IndexViewModel
    {
        public bool? PreviousSucceeded { get; set; }

        public string Message { get; set; }
 
        [BindProperty]
        public string UserName { get; set; }
    }
}