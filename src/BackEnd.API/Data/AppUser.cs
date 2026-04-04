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
    [Required(ErrorMessage = "⚠️ Lockout Enabled is required")] // Suggestion Tool : https://docs.fluentvalidation.net/en/latest/
                                                                // the Advantage over DataAnnotations is to not polute the Domain or Data Model   
                                                                // example : https://medium.com/@jenilsojitra/comparing-data-annotations-and-fluent-validation-in-net-core-99fb66d84f99
    public bool LockoutEnabled { get; set; }
}