using System.ComponentModel.DataAnnotations;
namespace BackEnd.API.Data;
public partial class AppUser
{
    [Key] 
    public int Id { get; set; }
    [StringLength(60)]
    public string? FullName { get; set; }
    [StringLength(128)]
    public string? Email { get; set; }
    public string? Password { get; set; }
    [Required(ErrorMessage = "⚠️ Lockout Enabled is required")]
    public bool LockoutEnabled { get; set; }
}