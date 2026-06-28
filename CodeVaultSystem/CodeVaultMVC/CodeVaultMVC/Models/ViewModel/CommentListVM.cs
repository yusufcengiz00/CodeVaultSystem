
using System.ComponentModel.DataAnnotations;

namespace CodeVaultMVC.ViewModels
{
    public class CommentListVM
    {
        public int CommentID { get; set; }

        public string Content { get; set; }

        public string UserName { get; set; }

        public string TaskName { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
