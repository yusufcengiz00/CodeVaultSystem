using System.ComponentModel.DataAnnotations;

namespace CodeVaultMVC.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "İsim alanı zorunludur.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email alanı zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçersiz e-posta formatı.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Parola alanı zorunludur.")]
        [DataType(DataType.Password)]
        [StringLength(40, MinimumLength = 8, ErrorMessage = "Parola en az 8 karakter olmalıdır.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Parola tekrarı zorunludur.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Parolalar eşleşmiyor.")]
        public string ConfirmPassword { get; set; }
    }
}