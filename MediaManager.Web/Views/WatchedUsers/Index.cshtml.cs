using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MediaManager.Web.Views.WatchedUsers
{
    public class IndexViewModel
    {
        public bool? PreviousSucceeded { get; set; }
        
        public string UserName { get; set; }
    }
}