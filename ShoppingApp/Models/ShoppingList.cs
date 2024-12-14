using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShoppingApp.Models
{
    public class ShoppingList
    {
        [Key]
        public int Id { get; set; }


        [Display(Name="Produkt")]
        public int ProductId { get; set; }


        [Display(Name = "Produkt")]
        public virtual Product? Product { get; set; }


        public string? UserID { get; set; }
        public IdentityUser? User {  get; set; }
    }
}
