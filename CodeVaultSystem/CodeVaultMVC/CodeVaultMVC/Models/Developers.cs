using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json; // Bunu eklemeyi unutma

namespace CodeVaultMVC.Models
{
    public class Developers
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonProperty("developerID")] // JSON'daki "developerID" ile eşleşir
        public int DeveloperID { get; set; }

        [JsonProperty("fullName")] // JSON'daki "fullName" ile eşleşir
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email Boş Bırakılamaz!")]
        [JsonProperty("eMail")] // JSON'daki "eMail" ile eşleşir
        public string EMail { get; set; }

        [JsonProperty("githubUrl")]
        public string GithubUrl { get; set; }

        [JsonProperty("linkedinUrl")]
        public string LinkedinUrl { get; set; }
    }
}