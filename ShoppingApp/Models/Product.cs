using System.ComponentModel.DataAnnotations;

namespace ShoppingApp.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        [Display(Name="Nazwa produktu")]
        [StringLength(30)]
        public string Name { get; set; }

        [Required]
        [Display(Name="Wartość produktu")]
        [Range(0,100)]
        public decimal Value { get; set; }
    }
}
