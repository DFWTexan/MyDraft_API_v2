using System.ComponentModel.DataAnnotations;

namespace JWTAuthentication.NET6._0.Auth
{
    public class CodeRequest
    {
        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string? Email { get; set; }
    }
}
