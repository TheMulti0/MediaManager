using System.ComponentModel.DataAnnotations;

namespace MediaManager.Web.Data.Entities
{
    public class OperatedPost
    {
        [Key]
        public long PostId { get; set; }

        public long UserId { get; set; }
    }
}