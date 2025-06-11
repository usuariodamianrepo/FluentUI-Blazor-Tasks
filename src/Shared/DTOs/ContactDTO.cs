
using System.ComponentModel.DataAnnotations;

namespace Shared
{
    public sealed class ContactDTO
    {
        public int Id { get; set; }

        [Required]
        [StringLength(256)]
        public string Email { get; set; } = string.Empty;
        [StringLength(256)]
        public string? Company { get; set; }
        [StringLength(256)]
        public string? LastName { get; set; }
        [StringLength(256)]
        public string? FirstName { get; set; }
        [StringLength(256)]
        public string? Phone { get; set; }

        public string Name => $"{FirstName} {LastName}".Trim();
    }
}