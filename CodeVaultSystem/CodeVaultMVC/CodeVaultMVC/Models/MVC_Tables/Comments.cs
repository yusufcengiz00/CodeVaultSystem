using System.ComponentModel.DataAnnotations;

namespace CodeVaultMVC.Models.MVC_Tables
{
    public class Comments
    {
        [Key]
        public int CommentID { get; set; }

        [Required(ErrorMessage = "Yorum içeriği zorunludur.")]
        public string Content { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string UserId { get; set; }

        public int TaskID { get; set; }
    }
}