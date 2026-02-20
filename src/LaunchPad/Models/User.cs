using System.ComponentModel.DataAnnotations;

namespace LaunchPad.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Username is required.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9_-]+$", ErrorMessage = "Username can only contain letters, numbers, underscores, and hyphens.")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password hash is required.")]
        public string PasswordHash { get; set; } = string.Empty;

        [Timestamp]
        public byte[] RowVersion { get; set; } = Array.Empty<byte>();
    }
}