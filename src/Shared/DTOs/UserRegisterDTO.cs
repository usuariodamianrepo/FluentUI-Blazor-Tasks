using System.ComponentModel.DataAnnotations;

namespace Shared
{
    public class UserRegisterDTO
    {
        [Required]
        [MinLength(5)]
        [MaxLength(100)]
        public string? Fullname { get; set; }

        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        //[Compare(nameof(Password))]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Required]
        //[Compare(nameof(Password))]
        [DataType(DataType.Password)]
        public string? ConfirmPassword { get; set; }
    }
}
