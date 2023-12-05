using System.ComponentModel.DataAnnotations;

namespace JWTAuthentication.NET6._0.Auth
{
    public class ConfirmEmailModel
    {
        [Required(ErrorMessage = "Token is required")]
        public string? token { get; set; }
        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string? email { get; set; }
    }
}
