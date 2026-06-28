using System.ComponentModel.DataAnnotations;

namespace CodeVaultMVC.ViewModels
{
    public class VerifyEmailViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
