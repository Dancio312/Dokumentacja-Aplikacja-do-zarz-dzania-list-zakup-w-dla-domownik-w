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
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        [Range(0.01, 100.99, ErrorMessage = "Wartość musi być w zakresie od 0,01 do 100,99.")]
        public decimal Value { get; set; }
    }
}
