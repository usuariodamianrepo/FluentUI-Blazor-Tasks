using System.ComponentModel.DataAnnotations;
namespace BackEnd.API.Data;

public partial class AppRole
{
    [Key]
    public int Id { get; set; }
    [StringLength(20)]
    public string? Name { get; set; }
}