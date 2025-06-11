using System.ComponentModel.DataAnnotations;

namespace BackEnd.API.Data;

public partial class TxskType
{
    public int Id { get; set; }
    [Required]
    [MaxLength(60)]
    public string? Name { get; set; }

    public virtual ICollection<Txsk> Txsks { get; set; } = new List<Txsk>();
}
