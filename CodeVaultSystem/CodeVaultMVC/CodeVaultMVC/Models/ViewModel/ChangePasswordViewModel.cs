using System.ComponentModel.DataAnnotations;

namespace CodeVaultMVC.ViewModels
{
    public class ChangePasswordViewModel
    {
        [Required]
        public string Email { get; set; } // Kullanıcıyı tanımak için

        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }
    }
}
