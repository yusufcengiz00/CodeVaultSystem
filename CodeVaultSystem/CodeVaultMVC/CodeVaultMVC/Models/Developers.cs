using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace CodeVaultMVC.Models
{
    public class Developers
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonProperty("developerID")]
        public int DeveloperID { get; set; }

        [JsonProperty("fullName")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email Boş Bırakılamaz!")]
        [JsonProperty("eMail")]
        public string EMail { get; set; }

        [JsonProperty("githubUrl")]
        public string GithubUrl { get; set; }

        [JsonProperty("linkedinUrl")]
        public string LinkedinUrl { get; set; }
    }
}