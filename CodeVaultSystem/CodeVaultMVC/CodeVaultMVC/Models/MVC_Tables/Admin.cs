namespace CodeVaultMVC.Models.MVC_Tables
{
    public class Admin
    {
        public int AdminID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; } // Şimdilik plain text, sonra hash'leyeceğiz
        public string Email { get; set; }
    }
}
