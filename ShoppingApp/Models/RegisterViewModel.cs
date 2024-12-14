using System.ComponentModel.DataAnnotations;

namespace ShoppingApp.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Rola użytkownika")]
        public string Role { get; set; } = "Child"; // Domyślna rola
    }
}
