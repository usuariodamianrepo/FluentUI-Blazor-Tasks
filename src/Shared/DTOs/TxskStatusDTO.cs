
using System.ComponentModel.DataAnnotations;

namespace Shared
{
    public partial class TxskStatusDTO
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(60)]
        public string? Name { get; set; }
    }
}