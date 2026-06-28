using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeVaultAPI.Models
{
    public class Developers
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int DeveloperID { get; set; }
        public string FullName { get; set; }
        [Required(ErrorMessage ="Email Boş Bırakılamaz!")]
        public string EMail { get; set; }
        public string GithubUrl { get; set; }
        public string LinkedinUrl { get; set; }
    }
}
