using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace CodeVaultMVC.Models
{
    public class Technologies
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonProperty("technologyID")]
        public int TechnologyID { get; set; }

        [JsonProperty("technologyName")]
        public string TechnologyName { get; set; }

        [JsonProperty("category")]
        public string Category { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }
    }
}