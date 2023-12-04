using System.ComponentModel.DataAnnotations;

namespace JWTAuthentication.NET6._0.Auth
{
    public class ResetPssWrdModel
    {
        [Required(ErrorMessage = "Code is required")]
        public int? Code { get; set; }
        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string? Email { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string? NewPassword { get; set; }
    }
}
