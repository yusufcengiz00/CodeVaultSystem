using CodeVaultMVC.Models;

namespace CodeVaultMVC.ViewModels
{
    public class AdminDashboardViewModel
    {
        public List<Projects> Projects { get; set; }
        public List<Technologies> Technologies { get; set; }
        public List<Developers> Developers { get; set; }
    }
}