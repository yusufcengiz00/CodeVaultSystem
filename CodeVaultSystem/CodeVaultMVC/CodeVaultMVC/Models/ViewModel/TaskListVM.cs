
namespace CodeVaultMVC.ViewModels
{
    public class TaskListVM
    {
        public int TaskID { get; set; }
        public string TaskName { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }


        public string ProjectName { get; set; }
        public string DeveloperName { get; set; }
        public string TechnologyName { get; set; }
    }
}