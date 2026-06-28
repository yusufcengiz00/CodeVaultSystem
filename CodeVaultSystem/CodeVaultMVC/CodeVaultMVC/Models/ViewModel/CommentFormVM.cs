using CodeVaultMVC.Models.MVC_Tables;

namespace CodeVaultMVC.ViewModels
{
    public class CommentFormVM
    {
        public Comments Comment { get; set; } = new Comments();
        public List<Tasks> Tasks { get; set; } = new();
    }
}