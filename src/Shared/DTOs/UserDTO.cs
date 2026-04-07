using System.ComponentModel.DataAnnotations;

namespace Shared
{
    public sealed class UserDTO
    {
        [StringLength(128)]
        public string Email { get; set; } = string.Empty;

        [StringLength(60)]
        public string Fullname { get; set; } = string.Empty;
        public int Id { get; set; }
        public bool LockoutEnabled { get; set; }
    }
}