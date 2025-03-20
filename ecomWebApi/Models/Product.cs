using System.ComponentModel.DataAnnotations;

namespace ecomWebApi.Models
{
    public class Product
    {
        public int ProductId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; } // Original Price

        [Range(0.01, double.MaxValue)]
        public decimal? SalePrice { get; set; } // Discounted Price (Nullable)

        [Required]
        [Range(0, int.MaxValue)]
        public int Stock { get; set; } // Available stock

        public bool Published { get; set; }
        public string ImgUrl { get; set; }
    }
}
