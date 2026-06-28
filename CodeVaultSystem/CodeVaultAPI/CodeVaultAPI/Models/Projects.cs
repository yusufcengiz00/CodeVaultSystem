using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeVaultAPI.Models
{
    public class Projects
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int ProjeID { get; set; }
        public string ProjectName { get; set; }
        public string Description { get; set; }
        public string ProjectUrl { get; set; }
        public string Status { get; set; }
    }
}
