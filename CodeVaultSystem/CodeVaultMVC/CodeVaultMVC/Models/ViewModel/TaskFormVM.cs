using CodeVaultMVC.Models;
using CodeVaultMVC.Models.MVC_Tables;

namespace CodeVaultMVC.ViewModels
{
    public class TaskFormVM
    {
        public Tasks Task { get; set; } = new Tasks();
        public List<Projects> Projects { get; set; } = new();
        public List<Developers> Developers { get; set; } = new();
        public List<Technologies> Technologies { get; set; } = new();
    }
}