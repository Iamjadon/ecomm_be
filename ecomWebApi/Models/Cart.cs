using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ecomWebApi.Models
{
    public class Cart
    {
        [Key]
        public int CartId { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; } // Link to the user

        [ForeignKey("Product")]
        public int ProductId { get; set; } // Link to the product

        public int Quantity { get; set; }

        public decimal TotalPrice { get; set; } // Quantity * Product Price

        public DateTime CreatedOn { get; set; } = DateTime.Now;

        public virtual User User { get; set; }
        public virtual Product Product { get; set; }
    }
}
