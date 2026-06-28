using CodeVaultMVC.Models.MVC_Tables;

namespace CodeVaultMVC.Models.ViewModel
{
    public class UserDashboardViewModel
    {
        public List<Tasks> MyTasks { get; set; }
        public List<Comments> MyComments { get; set; }
    }
}
