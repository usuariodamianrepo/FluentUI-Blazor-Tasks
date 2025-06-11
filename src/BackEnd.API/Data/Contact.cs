using System.ComponentModel.DataAnnotations;

namespace BackEnd.API.Data;

public partial class Contact
{
    [Required]
    public int Id { get; set; }

    [Required]
    [MaxLength(120)]
    public string Email { get; set; } = null!;
    [MaxLength(60)]
    public string? Company { get; set; }
    [Required]
    [MaxLength(60)]
    public string LastName { get; set; } = null!;
    [Required]
    [MaxLength(60)]
    public string FirstName { get; set; } = null!;
    [MaxLength(60)]
    public string? Phone { get; set; }
    
    public string Name => $"{FirstName} {LastName}".Trim();

    public virtual ICollection<Txsk> Txsks { get; set; } = new List<Txsk>();
}
