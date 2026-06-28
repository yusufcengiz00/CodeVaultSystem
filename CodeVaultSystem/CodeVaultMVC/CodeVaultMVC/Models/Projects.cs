using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json; // Bunu eklediğinden emin ol

namespace CodeVaultMVC.Models
{
    public class Projects
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonProperty("projeID")] // API'den gelen 'projeID'yi karşılar
        public int ProjeID { get; set; }

        [JsonProperty("projectName")] // API'den gelen 'projectName'i karşılar
        public string ProjectName { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("projectUrl")]
        public string ProjectUrl { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }
    }
}