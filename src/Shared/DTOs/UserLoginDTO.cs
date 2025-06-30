using System.ComponentModel.DataAnnotations;

namespace Shared
{
    public class UserLoginDTO
    {
        [Required]
        public string Username { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
