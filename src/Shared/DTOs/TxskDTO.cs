using System.ComponentModel.DataAnnotations;

namespace Shared
{
    public partial class TxskDTO
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(256)]
        public string? Title { get; set; }

        public int? ContactId { get; set; }
        public string? ContactName { get; set; }

        [Required(ErrorMessage = "The Due Date field is required")]
        public DateTime? DueDate { get; set; }
        
        [Required(ErrorMessage = "The Txsk Type field is required")]
        public int TxskTypeId { get; set; }
        public string? TxskTypeName { get; set; }

        [Required(ErrorMessage = "The Status field is required")]
        public int TxskStatusId { get; set; }
        public string? TxskStatusName { get; set; }
    }
}
