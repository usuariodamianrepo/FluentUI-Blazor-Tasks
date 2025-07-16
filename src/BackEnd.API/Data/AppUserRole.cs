using System.ComponentModel.DataAnnotations;
namespace BackEnd.API.Data;

public partial class AppUserRole
{
    [Key]
    public int Id { get; set; }
    [Required(ErrorMessage = "⚠️ Role Id is required")]
    public int RoleId { get; set; }
    [Required(ErrorMessage = "⚠️ User Id is required")]
    public int UserId { get; set; }
}