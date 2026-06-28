using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeVaultMVC.Models.MVC_Tables
{
    public class Tasks
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TaskID { get; set; }

        [Required(ErrorMessage = "Görev adı zorunludur.")]
        public string TaskName { get; set; }

        public string Description { get; set; }

        public string Status { get; set; } = "Bekliyor";

        public string UserId { get; set; }

        public int ProjectID { get; set; }
        public int DeveloperID { get; set; }
        public int TechnologyID { get; set; }
    }
}