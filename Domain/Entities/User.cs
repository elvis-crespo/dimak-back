using System.ComponentModel.DataAnnotations;

namespace dimax_front.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }

        [Required] 
        [StringLength(50)] 
        [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Username can only contain alphanumeric characters and underscores.")]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [StringLength(50)]
        public string Role { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        [RegularExpression(@"^[^@\s]+@[a-zA-Z0-9]+\.[a-zA-Z]{2,}$", ErrorMessage = "El email debe ser válido y terminar con un dominio como gmail.com, hotmail.com, etc.")]
        public string Email { get; set; } = string.Empty;
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public string? Token { get; set; }
        public DateTime? TokenExpiryTime { get; set; }
    }
}
