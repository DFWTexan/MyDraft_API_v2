using System.ComponentModel.DataAnnotations;

namespace JWTAuthentication.NET6._0.Auth
{
    public class ConfrimEmailModel
    {
        [Required(ErrorMessage = "Token is required")]
        public string? Token { get; set; }
    }
}
