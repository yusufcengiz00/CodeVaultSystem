using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeVaultAPI.Models
{
    public class Technologies
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int TechnologyID { get; set; }
        public string TechnologyName { get; set; }
        public string Category { get; set; }
        public string Version { get; set; }
    }
}
