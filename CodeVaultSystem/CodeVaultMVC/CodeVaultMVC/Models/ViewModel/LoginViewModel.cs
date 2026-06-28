using System.ComponentModel.DataAnnotations;

namespace CodeVaultMVC.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email alanı zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçersiz e-posta formatı.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Parola alanı zorunludur.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Beni Hatırla")]
        public bool RememberMe { get; set; }
    }
}